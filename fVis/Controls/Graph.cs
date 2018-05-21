using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using fVis.Extensions;
using Unclassified.TxLib;

namespace fVis.Controls
{
    /// <summary>
    /// Control that shows graph of given mathematical functions
    /// </summary>
    internal class Graph : Control
    {
        public enum RenderModeType
        {
            LowPrecision,
            MediumPrecision,
            HighPrecision,
            ExtremePrecision
        }

        public enum HighlightDifferencesMode
        {
            None,
            ConstantHighlight,
            DynamicHighlight,
            Average
        }

        private struct Difference // 8 + 8 + 8 + 8 = ~32 bytes
        {
            public double X;
            public double MinY;
            public double MaxY;
            public long Index;
        }

        public const double DefaultScaleFactor = 60;
        public const double MinScaleFactor = 0.01;

        private const long MediumPrecisionScale = 500000;
        private const long HighPrecisionScale = 100000000000;
        private const long ExtremePrecisionScale = 200000000000000000;

        private const int SelectionThreshold = 10;
        private const float MouseWheelRatio = 1.4f;

        private const int CachedDifferencesThreshold = 4000000;    // 4M

        private long offsetPxX, offsetPxY;
        private Point mousePosition;
        private Point mousePositionLast;

        private RenderModeType renderMode;
        private double scaleFactor = DefaultScaleFactor;

        private bool isDragging;

        private HighlightDifferencesMode highlightDifferences;

        private Font monospacedFont;

        private Pen penAxis;
        private Pen penSelection;
        private Brush brushSelectionFill;

        private long cachedDifferencesCount;
        private Dictionary<double, Difference> cachedDifferences;

        private IEnumerable<ListView.Item> dataSource;

        /// <summary>
        /// Current location of "x" axis within viewport
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long AxisX
        {
            get
            {
                return offsetPxX + (ClientSize.Width / 2);
            }
            set
            {
                offsetPxX = value - (ClientSize.Width / 2);
                Invalidate();
            }
        }

        /// <summary>
        /// Current location of "y" axis within viewport
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long AxisY
        {
            get
            {
                return offsetPxY + (ClientSize.Height / 2);
            }
            set
            {
                offsetPxY = value - (ClientSize.Height / 2);
                Invalidate();
            }
        }

        /// <summary>
        /// List of mathematical functions
        /// </summary>
        public IEnumerable<ListView.Item> DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource == value)
                    return;

                BindDataSource(value);

                InvalidateCachedDifferences();
                Invalidate();
            }
        }

        /// <summary>
        /// Current mode of differences highlighting
        /// </summary>
        public HighlightDifferencesMode HighlightDifferences
        {
            get { return highlightDifferences; }
            set
            {
                if (highlightDifferences == value)
                    return;

                highlightDifferences = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Current render mode
        /// </summary>
        public RenderModeType RenderMode
        {
            get { return renderMode; }
        }

        /// <summary>
        /// Current scale factor
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double ScaleFactor
        {
            get { return scaleFactor; }
            set
            {
                double factor = Math.Max(value, MinScaleFactor);

                if (scaleFactor == factor)
                    return;

                Size clientSize = ClientSize;
                ZoomToPoint(new Point(clientSize.Width / 2, clientSize.Height / 2), factor);
            }
        }

        /// <summary>
        /// Value of "x" on the left side of the viewport
        /// </summary>
        public double MinVisibleX
        {
            get
            {
                long axisX = offsetPxX + (ClientSize.Width / 2);
                return (0 - axisX) / scaleFactor;
            }
        }

        /// <summary>
        /// Value of "x" on the right side of the viewport
        /// </summary>
        public double MaxVisibleX
        {
            get
            {
                long axisX = offsetPxX + (ClientSize.Width / 2);
                return (ClientSize.Width - axisX) / scaleFactor;
            }
        }

        public event Action ScaleFactorChanged;

        public Graph()
        {
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.Selectable |
                ControlStyles.StandardDoubleClick | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);

            mousePositionLast.X = -1;
            mousePositionLast.Y = -1;

            monospacedFont = new Font("Consolas", 8f);

            penAxis = new Pen(Color.Black);
            penSelection = new Pen(SystemColors.Highlight);
            brushSelectionFill = new SolidBrush(Color.FromArgb(0x44, SystemColors.Highlight));

            cachedDifferences = new Dictionary<double, Difference>();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            Size clientSize = ClientSize;

            // Axis
            long axisX = offsetPxX + (clientSize.Width / 2);
            long axisY = offsetPxY + (clientSize.Height / 2);

            if (axisX >= 0 && axisX < clientSize.Width)
                e.Graphics.DrawLine(penAxis, (int)axisX, 0, (int)axisX, clientSize.Height);

            if (axisY >= 0 && axisY < clientSize.Height)
                e.Graphics.DrawLine(penAxis, 0, (int)axisY, clientSize.Width, (int)axisY);

            // Graph
            if (dataSource != null) {
                if (renderMode == RenderModeType.ExtremePrecision) {
                    RenderGraphExtremePrecision(e, clientSize, axisX, axisY);
                } else {
                    RenderGraph(e, clientSize, axisX, axisY);
                }
            }

            // Selection Rectangle
            if (isDragging) {
                int selectionX = Math.Min(mousePosition.X, mousePositionLast.X);
                int selectionY = Math.Min(mousePosition.Y, mousePositionLast.Y);
                int selectionWidth = Math.Abs(mousePosition.X - mousePositionLast.X);
                int selectionHeight = Math.Abs(mousePosition.Y - mousePositionLast.Y);

                e.Graphics.FillRectangle(brushSelectionFill, selectionX + 1, selectionY + 1, selectionWidth - 1, selectionHeight - 1);
                e.Graphics.DrawRectangle(penSelection, selectionX, selectionY, selectionWidth, selectionHeight);
            }

            OnPaintTextOverlay(e, clientSize, axisX, axisY);

            base.OnPaint(e);
        }

        /// <summary>
        /// Renders text overlay
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        /// <param name="clientSize">Client size</param>
        /// <param name="axisX">Axis X</param>
        /// <param name="axisY">Axis Y</param>
        private void OnPaintTextOverlay(PaintEventArgs e, Size clientSize, long axisX, long axisY)
        {
            const int TextTop = 5;
            const int LineHeight = 13;
            const int LineSeparation = 6;

            double mouseX, mouseY;
            if (renderMode == RenderModeType.ExtremePrecision) {
                mouseX = mousePosition.X.SubtractDivideLossless(axisX, scaleFactor);
                mouseY = -mousePosition.Y.SubtractDivideLossless(axisY, scaleFactor);
            } else {
                mouseX = ((mousePosition.X - axisX) / scaleFactor);
                mouseY = -((mousePosition.Y - axisY) / scaleFactor);
            }

            switch (renderMode) {
                case RenderModeType.LowPrecision:
                    TextRenderer.DrawText(e.Graphics, "X:", monospacedFont, new Point(5, TextTop), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics, mouseX.ToString("+0.00000;-0.00000; 0", CultureInfo.InvariantCulture),
                        monospacedFont, new Point(22, TextTop), ForeColor, TextFormatFlags.Default);

                    TextRenderer.DrawText(e.Graphics, "Y:", monospacedFont, new Point(5, TextTop + LineHeight), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics, mouseY.ToString("+0.00000;-0.00000; 0", CultureInfo.InvariantCulture),
                        monospacedFont, new Point(22, TextTop + LineHeight), ForeColor, TextFormatFlags.Default);
                    break;

                case RenderModeType.MediumPrecision:
                    TextRenderer.DrawText(e.Graphics, "X:", monospacedFont, new Point(5, TextTop), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics,
                        mouseX.ToString("+0.000000######;-0.000000######; 0", CultureInfo.InvariantCulture), monospacedFont,
                        new Point(22, TextTop), ForeColor, TextFormatFlags.Default);

                    TextRenderer.DrawText(e.Graphics, "Y:", monospacedFont, new Point(5, TextTop + LineHeight), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics,
                        mouseY.ToString("+0.000000######;-0.000000######; 0", CultureInfo.InvariantCulture), monospacedFont,
                        new Point(22, TextTop + LineHeight), ForeColor, TextFormatFlags.Default);
                    break;

                case RenderModeType.HighPrecision:
                    TextRenderer.DrawText(e.Graphics, "X:", monospacedFont, new Point(5, TextTop), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics,
                        mouseX.ToString("+0.0000000000##################;-0.0000000000##################; 0",
                            CultureInfo.InvariantCulture), monospacedFont, new Point(22, TextTop), ForeColor,
                        TextFormatFlags.Default);

                    TextRenderer.DrawText(e.Graphics, "Y:", monospacedFont, new Point(5, TextTop + LineHeight), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics,
                        mouseY.ToString("+0.0000000000##################;-0.0000000000##################; 0",
                            CultureInfo.InvariantCulture), monospacedFont, new Point(22, TextTop + LineHeight), ForeColor,
                        TextFormatFlags.Default);
                    break;

                case RenderModeType.ExtremePrecision:
                    TextRenderer.DrawText(e.Graphics, "X:", monospacedFont, new Point(5, TextTop), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics, mouseX.ToBits(), monospacedFont, new Point(22, TextTop), ForeColor,
                        TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics, mouseX.ToExactString(), monospacedFont,
                        new Point(22, TextTop + LineHeight), ForeColor, TextFormatFlags.Default);

                    TextRenderer.DrawText(e.Graphics, "Y:", monospacedFont,
                        new Point(5, TextTop + LineHeight * 2 + LineSeparation), ForeColor, TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics, mouseY.ToBits(), monospacedFont,
                        new Point(22, TextTop + LineHeight * 2 + LineSeparation), ForeColor, TextFormatFlags.Default);
                    TextRenderer.DrawText(e.Graphics, mouseY.ToExactString(), monospacedFont,
                        new Point(22, TextTop + LineHeight * 3 + LineSeparation), ForeColor, TextFormatFlags.Default);

                    if (dataSource != null) {
                        int y = TextTop + LineHeight * 4 + LineSeparation * 3;

                        if (highlightDifferences == HighlightDifferencesMode.None) {
                            foreach (ListView.Item item in dataSource) {
                                if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                                    continue;

                                double resultY = item.NumericValueSource.Evaluate(mouseX);
                                TextRenderer.DrawText(e.Graphics, "■:", monospacedFont, new Point(5, y), item.Color,
                                    TextFormatFlags.Default);
                                TextRenderer.DrawText(e.Graphics, resultY.ToBits(), monospacedFont, new Point(22, y), item.Color,
                                    TextFormatFlags.Default);
                                TextRenderer.DrawText(e.Graphics, resultY.ToExactString(), monospacedFont,
                                    new Point(22, y + LineHeight), item.Color, TextFormatFlags.Default);

                                y += LineHeight * 2 + LineSeparation;
                            }
                        } else {
                            List<string> bitsList = new List<string>();

                            foreach (ListView.Item item in dataSource) {
                                if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                                    continue;

                                double resultY = item.NumericValueSource.Evaluate(mouseX);
                                string bits = resultY.ToBits();

                                TextRenderer.DrawText(e.Graphics, "■:", monospacedFont, new Point(5, y), item.Color,
                                    TextFormatFlags.Default);
                                TextRenderer.DrawText(e.Graphics, bits, monospacedFont, new Point(22, y), item.Color,
                                    TextFormatFlags.Default);
                                TextRenderer.DrawText(e.Graphics, resultY.ToExactString(), monospacedFont,
                                    new Point(22, y + LineHeight), item.Color, TextFormatFlags.Default);

                                y += LineHeight * 2 + LineSeparation;

                                bitsList.Add(bits);
                            }

                            // Find differences in bits
                            const int BitCount = 8 * sizeof(double) + 2;
                            StringBuilder[] bitsDifferences = new StringBuilder[bitsList.Count];

                            for (int i = 0; i < bitsDifferences.Length; i++) {
                                bitsDifferences[i] = new StringBuilder(BitCount);
                            }

                            for (int j = 0; j < BitCount; j++) {
                                bool diff = false;
                                for (int i = 1; i < bitsDifferences.Length; i++) {
                                    if (bitsList[i - 1][j] != bitsList[i][j]) {
                                        diff = true;
                                        break;
                                    }
                                }

                                for (int i = 0; i < bitsDifferences.Length; i++) {
                                    // Append only digits that differ
                                    bitsDifferences[i].Append(diff ? bitsList[i][j] : ' ');
                                }
                            }

                            // Highlight them
                            y = TextTop + LineHeight * 4 + LineSeparation * 3;
                            for (int i = 0; i < bitsDifferences.Length; i++) {
                                TextRenderer.DrawText(e.Graphics, bitsDifferences[i].ToString(), monospacedFont,
                                    new Point(22, y), Color.FromArgb(unchecked((int)0xffe00000)), TextFormatFlags.Default);
                                y += LineHeight * 2 + LineSeparation;
                            }
                        }
                    }
                    break;
            }

            string info;
            if (highlightDifferences != HighlightDifferencesMode.None) {
                info = string.Format(Tx.T("main.cached diffs"), Tx.N(cachedDifferences.Count), Tx.N(CachedDifferencesThreshold));
            } else {
                info = "";
            }
            TextRenderer.DrawText(e.Graphics, info, monospacedFont, new Point(5, clientSize.Height - LineHeight - LineSeparation), ForeColor, TextFormatFlags.Default);
        }

        /// <summary>
        /// Renders graph in basic precision
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        /// <param name="clientSize">Client size</param>
        /// <param name="axisX">Axis X</param>
        /// <param name="axisY">Axis Y</param>
        private unsafe void RenderGraph(PaintEventArgs e, Size clientSize, long axisX, long axisY)
        {
            SmoothingMode oldSmoothingMode = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            for (int j = 1; j < clientSize.Width; j++) {
                long x1i = (j - 1);
                long x2i = j;
                double x1 = (x1i - axisX) / scaleFactor;
                double x2 = (x2i - axisX) / scaleFactor;

                bool isFirst = true;
                double computedMinY = double.MaxValue, computedMaxY = double.MinValue;

                foreach (ListView.Item item in dataSource) {
                    if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                        continue;

                    double y1_ = item.NumericValueSource.Evaluate(x1);
                    double y2_ = item.NumericValueSource.Evaluate(x2);
                    if (double.IsNaN(y1_) || double.IsNaN(y2_) || double.IsInfinity(y1_) || double.IsInfinity(y2_))
                        continue;

                    if (isFirst) {
                        isFirst = false;
                        computedMinY = computedMaxY = y2_;
                    } else {
                        if (y2_ > computedMaxY) {
                            computedMaxY = y2_;
                        } else if (y2_ < computedMinY) {
                            computedMinY = y2_;
                        }
                    }

                    if (highlightDifferences != HighlightDifferencesMode.Average) {
                        float y1i = (float)((-y1_ * scaleFactor) + axisY);
                        float y2i = (float)((-y2_ * scaleFactor) + axisY);
                        if (y1i < -32000 || y1i >= 32000 || y2i < -32000 || y2i >= 32000)
                            continue;

                        try {
                            using (Pen pen = new Pen(item.Color)) {
                                e.Graphics.DrawLine(pen, x1i, y1i, x2i, y2i);
                            }
                        } catch {
                            //Debug.WriteLine("Render overflow detected!");
                            item.CheckState = CheckState.Indeterminate;
                        }
                    }
                }

                if (!isFirst && highlightDifferences != HighlightDifferencesMode.None && computedMinY != computedMaxY) {
                    if (!cachedDifferences.ContainsKey(x2)) {
                        CleanCachedDifferences();

                        // Add new item to cache
                        cachedDifferences[x2] = new Difference {
                            X = x2,
                            MinY = computedMinY,
                            MaxY = computedMaxY,
                            Index = cachedDifferencesCount++
                        };
                    }
                }
            }

            e.Graphics.SmoothingMode = SmoothingMode.None;

            if (highlightDifferences == HighlightDifferencesMode.ConstantHighlight) {
                using (Pen penDifference = new Pen(Color.FromArgb(unchecked((int)0x88ee0000)))) {
                    bool[] viewDifferences = new bool[clientSize.Width];

                    const float distance = 6;
                    foreach (var difference in cachedDifferences) {
                        int viewX = (int)Math.Round((difference.Value.X * scaleFactor) + axisX);
                        if (viewX < 0 || viewX >= clientSize.Width || viewDifferences[viewX]) {
                            continue;
                        }

                        double computedMinY = difference.Value.MinY;
                        double computedMaxY = difference.Value.MaxY;
                        float viewMinY = (float)((-computedMinY * scaleFactor) + axisY);
                        float viewMaxY = (float)((-computedMaxY * scaleFactor) + axisY);
                        if ((viewMinY < -distance && viewMaxY < -distance) ||
                            (viewMinY >= clientSize.Height + distance && viewMaxY >= clientSize.Height + distance)) {
                            continue;
                        }

                        viewDifferences[viewX] = true;
                        e.Graphics.DrawLine(penDifference, viewX, Math.Min(viewMinY + distance, clientSize.Height - 1), viewX, Math.Max(viewMaxY - distance, 0));
                    }
                }
            } else if (highlightDifferences == HighlightDifferencesMode.DynamicHighlight) {
                ulong[] viewDistances = new ulong[clientSize.Width];
                double[] viewMinY = new double[clientSize.Width];
                double[] viewMaxY = new double[clientSize.Width];

                foreach (var difference in cachedDifferences) {
                    int viewX = (int)Math.Round((difference.Value.X * scaleFactor) + axisX);
                    if (viewX < 0 || viewX >= clientSize.Width) {
                        continue;
                    }

                    double computedMinY = difference.Value.MinY;
                    double computedMaxY = difference.Value.MaxY;
                    long minYi = *(long*)&computedMinY;
                    long maxYi = *(long*)&computedMaxY;
                    long distance = Math.Abs(minYi - maxYi);

                    if (viewDistances[viewX] <= 0) {
                        viewMinY[viewX] = computedMinY;
                        viewMaxY[viewX] = computedMaxY;
                    }
                    viewDistances[viewX] += (ulong)distance;
                }

                using (Pen penDifference = new Pen(Color.FromArgb(unchecked((int)0x99ee0000)))) {
                    for (int i = 0; i < viewDistances.Length; i++) {
                        if (viewDistances[i] <= 0) {
                            continue;
                        }

                        float viewMinYi = (float)((-viewMinY[i] * scaleFactor) + axisY);
                        float viewMaxYi = (float)((-viewMaxY[i] * scaleFactor) + axisY);
                        if ((viewMinYi < -2000 && viewMaxYi < -2000) ||
                            (viewMinYi >= clientSize.Height + 2000 && viewMaxYi >= clientSize.Height + 2000)) {
                            continue;
                        }

                        float distance = Math.Max((float)Math.Log(viewDistances[i]), 1);
                        e.Graphics.DrawLine(penDifference, i, Math.Min(viewMinYi + distance, clientSize.Height - 1), i, Math.Max(viewMaxYi - distance, 0));
                    }
                }
            } else if (highlightDifferences == HighlightDifferencesMode.Average) {
                using (Pen penDifference = new Pen(Color.FromArgb(unchecked((int)0xffee0000)))) {
                    bool[] viewDifferences = new bool[clientSize.Width];

                    const float distance = 2;
                    foreach (var difference in cachedDifferences) {
                        int viewX = (int)Math.Round((difference.Value.X * scaleFactor) + axisX);
                        if (viewX < 0 || viewX >= clientSize.Width || viewDifferences[viewX]) {
                            continue;
                        }

                        double computedY = (difference.Value.MinY + difference.Value.MaxY) * 0.5;
                        float viewY = (float)((-computedY * scaleFactor) + axisY);
                        if (viewY < -distance || viewY >= clientSize.Height + distance ) {
                            continue;
                        }

                        viewDifferences[viewX] = true;
                        e.Graphics.DrawLine(penDifference, viewX, viewY - distance, viewX, viewY + distance);
                        e.Graphics.DrawLine(penDifference, viewX - distance, viewY, viewX + distance, viewY);
                    }
                }
            }

            e.Graphics.SmoothingMode = oldSmoothingMode;
        }

        /// <summary>
        /// Renders graph in extreme precision using MPIR
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        /// <param name="clientSize">Client size</param>
        /// <param name="axisX">Axis X</param>
        /// <param name="axisY">Axis Y</param>
        private unsafe void RenderGraphExtremePrecision(PaintEventArgs e, Size clientSize, long axisX, long axisY)
        {
            SmoothingMode oldSmoothingMode = e.Graphics.SmoothingMode;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;

            double xF_ = 0.SubtractDivideLossless(axisX, scaleFactor);
            double xL_ = clientSize.Width.SubtractDivideLossless(axisX, scaleFactor);

            // Swap first and last for negative "x" values
            long xFi = *(long*)&xF_;
            long xLi = *(long*)&xL_;
            if (xFi > xLi) {
                long swap = xLi;
                xLi = xFi;
                xFi = swap;
            }

            // Extend bounds to show last incompete item
            xLi += 1;

            // Adjust line alpha according to current scale factor
            int lineIntensity = Math.Min(Math.Max((int)(clientSize.Width / ((xLi - xFi) * 2)), 0), 50);

            using (Brush brushDifference = new SolidBrush(Color.FromArgb(unchecked((int)0x99ff0000))))
            using (Pen penLine = new Pen(Color.FromArgb(lineIntensity, 0, 0, 0))) {
                // Draw all values in viewport
                long x1 = xFi - 1;
                for (long x2 = xFi; x2 <= xLi; x2++) {
                    double x1_ = *(double*)&x1;
                    double x2_ = *(double*)&x2;

                    int x1i = x1_.MultiplyAddLosslessToInt(scaleFactor, axisX);
                    int x2i = x2_.MultiplyAddLosslessToInt(scaleFactor, axisX);

                    bool isFirst = true;
                    double computedMinY = double.MaxValue, computedMaxY = double.MinValue;
                    int viewMinY = int.MaxValue, viewMaxY = int.MinValue;

                    // Draw line separating "x" values
                    e.Graphics.DrawLine(penLine, x2i, 0, x2i, clientSize.Height);

                    foreach (ListView.Item item in dataSource) {
                        if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                            continue;

                        double y1_ = item.NumericValueSource.Evaluate(x1_);
                        double y2_ = item.NumericValueSource.Evaluate(x2_);
                        if (double.IsNaN(y1_) || double.IsNaN(y2_) || double.IsInfinity(y1_) || double.IsInfinity(y2_))
                            continue;

                        int y1i = y1_.NegMultiplyAddLosslessToInt(scaleFactor, axisY);
                        int y2i = y2_.NegMultiplyAddLosslessToInt(scaleFactor, axisY);

                        if (isFirst) {
                            isFirst = false;
                            computedMinY = computedMaxY = y1_;
                            viewMinY = viewMaxY = y1i;
                        } else {
                            if (y1_ > computedMaxY) {
                                computedMaxY = y1_;
                                viewMaxY = y1i;
                            } else if (y1_ < computedMinY) {
                                computedMinY = y1_;
                                viewMinY = y1i;
                            }
                        }

                        if (y1i < -32000 || y1i >= 32000 || y2i < -32000 || y2i >= 32000)
                            continue;

                        try {
                            using (Pen pen = new Pen(item.Color)) {
                                e.Graphics.DrawLine(pen, x1i, y1i, x2i, y1i);
                                e.Graphics.DrawLine(pen, x2i, y1i, x2i, y2i);
                            }
                        } catch {
                            //Debug.WriteLine("Render overflow detected!");
                            item.CheckState = CheckState.Indeterminate;
                        }
                    }

                    if (!isFirst && highlightDifferences != HighlightDifferencesMode.None && computedMinY != computedMaxY) {
                        int xDiff, wDiff;
                        if (x1i < x2i) {
                            xDiff = x1i;
                            wDiff = x2i - x1i;
                        } else {
                            wDiff = x1i - x2i;
                            xDiff = x1i - wDiff;
                        }

                        int distance;
                        if (highlightDifferences == HighlightDifferencesMode.DynamicHighlight) {
                            long minYi = *(long*)&computedMinY;
                            long maxYi = *(long*)&computedMaxY;
                            distance = (int)Math.Max(Math.Log(Math.Abs(minYi - maxYi)), 1);
                        } else {
                            distance = 5;
                        }

                        viewMaxY = Math.Max(viewMaxY - distance, 0);
                        viewMinY = Math.Min(viewMinY + distance, clientSize.Height);
                        e.Graphics.FillRectangle(brushDifference, xDiff, viewMaxY, wDiff, viewMinY - viewMaxY);

                        if (!cachedDifferences.ContainsKey(x1_)) {
                            CleanCachedDifferences();

                            // Add new item to cache
                            cachedDifferences[x1_] = new Difference {
                                X = x1_,
                                MinY = computedMinY,
                                MaxY = computedMaxY,
                                Index = cachedDifferencesCount++
                            };
                        }
                    }

                    x1 = x2;
                }
            }

            e.Graphics.SmoothingMode = oldSmoothingMode;
        }

        /// <summary>
        /// Clean oldest differences from cache
        /// </summary>
        private void CleanCachedDifferences()
        {
            // Check if the cache is full
            if (cachedDifferences.Count > CachedDifferencesThreshold) {
                const int RemoveCount = CachedDifferencesThreshold / 200;

                // Free 1/200 of the cache
                Difference[] remove = new Difference[RemoveCount];
                int index = 0;
                foreach (var difference in cachedDifferences) {
                    if (index < RemoveCount) {
                        // Fill array with first "n" items
                        remove[index] = difference.Value;

                        if (index == RemoveCount - 1) {
                            // Array filled, sort it by create time
                            for (int k = 1; k < RemoveCount; k++) {
                                if (remove[k - 1].Index < remove[k].Index) {
                                    Difference swap = remove[k];
                                    remove[k] = remove[k - 1];
                                    remove[k - 1] = swap;
                                } else {
                                    break;
                                }
                            }
                        }
                    } else {
                        // Try to add new item to array, if the item was created earlier then items in array
                        if (remove[0].Index > difference.Value.Index) {
                            remove[0] = difference.Value;

                            // Item added to array, sort it again by create time
                            for (int k = 1; k < RemoveCount; k++) {
                                if (remove[k - 1].Index < remove[k].Index) {
                                    Difference swap = remove[k];
                                    remove[k] = remove[k - 1];
                                    remove[k - 1] = swap;
                                } else {
                                    break;
                                }
                            }
                        }
                    }

                    index++;
                }

                // Remove all items in array from cache
                for (int i = 0; i < RemoveCount; i++) {
                    cachedDifferences.Remove(remove[i].X);
                }
            }
        }

        /// <summary>
        /// Clear whole cache
        /// </summary>
        private void InvalidateCachedDifferences()
        {
            cachedDifferencesCount = 0;
            cachedDifferences.Clear();
        }

        protected override unsafe void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            mousePositionLast = e.Location;

            if (e.Button == MouseButtons.Middle) {
                // Zoom to difference under cursor
                long axisX = offsetPxX + (ClientSize.Width / 2);

                long maxDistance = 0;
                double x = 0, y = 0;

                foreach (var difference in cachedDifferences) {
                    int viewX = (int)((difference.Value.X * scaleFactor) + axisX);
                    if (viewX != e.X) {
                        continue;
                    }

                    double computedMinY = difference.Value.MinY;
                    double computedMaxY = difference.Value.MaxY;
                    long minYi = *(long*)&computedMinY;
                    long maxYi = *(long*)&computedMaxY;
                    long distance = Math.Abs(minYi - maxYi);

                    if (maxDistance < distance) {
                        maxDistance = distance;
                        x = difference.Value.X;
                        y = computedMaxY;
                    }
                }

                if (maxDistance > 0) {
                    // Difference found
                    ZoomToValue(x, y);
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isDragging) {
                int selectionWidth = Math.Abs(mousePosition.X - mousePositionLast.X);
                int selectionHeight = Math.Abs(mousePosition.Y - mousePositionLast.Y);
                if (selectionWidth > SelectionThreshold || selectionHeight > SelectionThreshold) {
                    int selectionX = Math.Min(mousePosition.X, mousePositionLast.X);
                    int selectionY = Math.Min(mousePosition.Y, mousePositionLast.Y);

                    ZoomToRectangle(new Rectangle(selectionX, selectionY, selectionWidth, selectionHeight));
                }

                isDragging = false;
            }

            mousePositionLast.X = -1;
            mousePositionLast.Y = -1;

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mousePositionLast.X != -1 && mousePositionLast.Y != -1) {
                if (e.Button == MouseButtons.Left) {
                    offsetPxX += e.X - mousePositionLast.X;
                    offsetPxY += e.Y - mousePositionLast.Y;

                    mousePositionLast = e.Location;
                } else if (e.Button == MouseButtons.Right) {
                    if (!isDragging) {
                        int selectionWidth = Math.Abs(mousePosition.X - mousePositionLast.X);
                        int selectionHeight = Math.Abs(mousePosition.Y - mousePositionLast.Y);
                        if (selectionWidth > SelectionThreshold || selectionHeight > SelectionThreshold) {
                            isDragging = true;
                        }
                    }
                }
            }

            mousePosition = e.Location;
            Invalidate();

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            double newScale;
            if (e.Delta < 0) {
                newScale = (scaleFactor / MouseWheelRatio);
            } else {
                newScale = (scaleFactor * MouseWheelRatio);
            }

            if (newScale >= MinScaleFactor) {
                //ZoomToPoint(e.Location, newScale);

                // Lower mouse sensitivity
                Size clientSize = ClientSize;
                Size clientSizeHalf = new Size(clientSize.Width / 2, clientSize.Height / 2);
                Point point = new Point(clientSizeHalf.Width + (e.X - clientSizeHalf.Width) / 3, clientSizeHalf.Height + (e.Y - clientSizeHalf.Height) / 3);
                ZoomToPoint(point, newScale);
            }

            base.OnMouseWheel(e);
        }

        protected virtual void OnScaleFactorChanged()
        {
            long axisX = offsetPxX + (ClientSize.Width / 2);
            double x1 = (0 - axisX) / scaleFactor;
            double x2 = (2 - axisX) / scaleFactor;
            double x3 = (4 - axisX) / scaleFactor;
            if (x1 == x2 || x2 == x3) {
                renderMode = RenderModeType.ExtremePrecision;
            } else if (scaleFactor < MediumPrecisionScale / Math.Abs(x1)) {
                renderMode = RenderModeType.LowPrecision;
            } else if (scaleFactor < HighPrecisionScale / Math.Abs(x1)) {
                renderMode = RenderModeType.MediumPrecision;
            } else {
                renderMode = RenderModeType.HighPrecision;
            }

            ScaleFactorChanged?.Invoke();

            Invalidate();
        }

        /// <summary>
        /// Zooms to given rectangle in viewport
        /// </summary>
        /// <param name="selectionBounds">Rectangle to zoom to</param>
        public void ZoomToRectangle(Rectangle selectionBounds)
        {
            Size clientSize = ClientSize;
            Point selectionCenter = new Point(selectionBounds.X + selectionBounds.Width / 2,
                selectionBounds.Y + selectionBounds.Height / 2);

            double ratioX = (double)clientSize.Width / (selectionBounds.Width + 2);
            double ratioY = (double)clientSize.Height / (selectionBounds.Height + 2);
            double ratio = Math.Min(ratioX, ratioY);

            scaleFactor *= ratio;

            offsetPxX = (long)((offsetPxX - (selectionCenter.X - (clientSize.Width / 2))) * ratio);
            offsetPxY = (long)((offsetPxY - (selectionCenter.Y - (clientSize.Height / 2))) * ratio);

            OnScaleFactorChanged();
        }

        /// <summary>
        /// Zooms to given point
        /// </summary>
        /// <param name="point">Point to zoom to</param>
        /// <param name="factor">Scale factor</param>
        public void ZoomToPoint(Point point, double factor)
        {
            Size clientSize = ClientSize;

            double ratio = (factor / scaleFactor);

            scaleFactor = factor;

            offsetPxX = (long)((offsetPxX - (point.X - (clientSize.Width / 2))) * ratio);
            offsetPxY = (long)((offsetPxY - (point.Y - (clientSize.Height / 2))) * ratio);

            OnScaleFactorChanged();
        }

        /// <summary>
        /// Zooms in to center by 1.4
        /// </summary>
        public void ZoomInCenter()
        {
            double factor = scaleFactor * 1.4;

            Size clientSize = ClientSize;
            ZoomToPoint(new Point(clientSize.Width / 2, clientSize.Height / 2), factor);
        }

        /// <summary>
        /// Zooms out to center by 1.4
        /// </summary>
        public void ZoomOutCenter()
        {
            double factor = scaleFactor / 1.4;

            if (factor >= MinScaleFactor) {
                Size clientSize = ClientSize;
                ZoomToPoint(new Point(clientSize.Width / 2, clientSize.Height / 2), factor);
            }
        }

        /// <summary>
        /// Zoom to specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public void ZoomToValue(double x, double y)
        {
            double newScale = ExtremePrecisionScale / Math.Abs(x);
            if (newScale > 0.1 && !double.IsInfinity(newScale) && !double.IsNaN(newScale)) {
                scaleFactor = newScale;
            }

            offsetPxX = -(long)(x * scaleFactor);
            offsetPxY = (long)(y * scaleFactor);

            OnScaleFactorChanged();
        }

        /// <summary>
        /// Resets view to default position and scale
        /// </summary>
        public void ResetView()
        {
            offsetPxX = 0;
            offsetPxY = 0;
            scaleFactor = DefaultScaleFactor;

            OnScaleFactorChanged();
        }

        private void BindDataSource(IEnumerable<ListView.Item> source)
        {
            if (dataSource != null) {
                INotifyCollectionChanged cc = dataSource as INotifyCollectionChanged;
                if (cc != null) {
                    cc.CollectionChanged -= OnCollectionChanged;
                }

                foreach (ListView.Item current in dataSource) {
                    current.PropertyChanged -= OnPropertyChanged;
                }
            }

            dataSource = source;

            if (source != null) {
                INotifyCollectionChanged cc = dataSource as INotifyCollectionChanged;
                if (cc != null) {
                    cc.CollectionChanged += OnCollectionChanged;
                }

                foreach (ListView.Item current in dataSource) {
                    current.PropertyChanged += OnPropertyChanged;
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++) {
                        object current = e.NewItems[i];
                        INotifyPropertyChanged pc = current as INotifyPropertyChanged;
                        if (pc != null) {
                            pc.PropertyChanged += OnPropertyChanged;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++) {
                        object current = e.OldItems[i];
                        INotifyPropertyChanged pc = current as INotifyPropertyChanged;
                        if (pc != null) {
                            pc.PropertyChanged -= OnPropertyChanged;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < e.NewItems.Count; i++) {
                        object current = e.NewItems[i];
                        INotifyPropertyChanged pc = current as INotifyPropertyChanged;
                        if (pc != null) {
                            pc.PropertyChanged += OnPropertyChanged;
                        }
                    }

                    for (int i = 0; i < e.OldItems.Count; i++) {
                        object current = e.OldItems[i];
                        INotifyPropertyChanged pc = current as INotifyPropertyChanged;
                        if (pc != null) {
                            pc.PropertyChanged -= OnPropertyChanged;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }

            InvalidateCachedDifferences();
            Invalidate();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NumericValueSource" || e.PropertyName == "CheckState") {
                InvalidateCachedDifferences();
            }

            if (e.PropertyName == "NumericValueSource" || e.PropertyName == "Color" || e.PropertyName == "CheckState") {
                Invalidate();
            }
        }

        /// <summary>
        /// Adds new difference to cache
        /// </summary>
        /// <param name="x">X of difference</param>
        /// <param name="computedMinY">Min. Y of difference</param>
        /// <param name="computedMaxY">Max. Y of difference</param>
        public void AddDifferenceUnsafe(double x, double computedMinY, double computedMaxY)
        {
            const int CacheDifferencesLimit = CachedDifferencesThreshold - CachedDifferencesThreshold / 200;

            if (cachedDifferences.Count >= CacheDifferencesLimit) {
                return;
            }

            cachedDifferences[x] = new Difference {
                X = x,
                MinY = computedMinY,
                MaxY = computedMaxY,
                Index = cachedDifferencesCount++
            };
        }
    }
}