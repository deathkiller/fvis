using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using fVis.Misc;

namespace fVis.Controls
{
    /// <summary>
    /// Control that shows list of mathematical functions
    /// </summary>
    internal partial class ListView : Control
    {
        private enum ColumnSizingState
        {
            Normal,
            Hot,
            Active
        }

        public int MouseWheelScrollExtent = SystemInformation.MouseWheelScrollLines;

        private const int borderPadding = 1;

        private const int imageSpacing = 2;
        private int rowHeight;
        private int imageMarginSize;
        private const int imageSize = 8;

        private string emptyText;

        private readonly ScrollBar vScrollBar;
        private readonly TextBox labelEdit = new TextBox();

        private Item stateHotItem;
        private Item stateFocusedItem;
        private Item statePressedCheckBox;
        private Item stateEditItem;

        private ColumnSizingState columnSizingState;
        private int columnSizingOffset;

        private int descriptionWidth = 160;
        private const int descriptionMinWidth = 20;
        private const int descriptionMaxWidth = 250;

        private bool stateSelectedItem;

        private bool cachedUseRenderers;
        private VisualStyleRenderer itemHotRenderer;
        private VisualStyleRenderer itemFocusedRenderer;
        private VisualStyleRenderer itemHotFocusedRenderer;
        private VisualStyleRenderer itemSelectedRenderer;
        private VisualStyleRenderer itemHotSelectedRenderer;
        private VisualStyleRenderer itemDisabledRenderer;
        private VisualStyleRenderer columnLineRenderer;

        private readonly Font itemFont;

        private readonly ObservableCollection<Item> items = new ObservableCollection<Item>();

        public event PropertyChangedEventHandler ItemPropertyChanged;
        public event CancelEventHandler BeforeEdit;
        public event EventHandler ItemSelectionChanged;

        public Rectangle ClientRectangleInner { get; private set; }

        public string EmptyText
        {
            get { return emptyText; }
            set
            {
                if (emptyText == value) {
                    return;
                }

                emptyText = value;
                Invalidate();
            }
        }

        public IList<Item> Items
        {
            get { return items; }
        }

        public Item FocusedItem
        {
            get { return stateFocusedItem; }
            set
            {
                stateFocusedItem = value;
                stateSelectedItem = true;

                Invalidate();

                OnItemSelectionChanged();
            }
        }

        public Item SelectedItem
        {
            get
            {
                if (stateSelectedItem) {
                    return stateFocusedItem;
                } else {
                    return null;
                }
            }
        }

        public ListView()
        {
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.Selectable |
                ControlStyles.StandardDoubleClick | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, false);

            TabStop = true;

            rowHeight = 22;
            imageMarginSize = rowHeight - (imageSpacing << 1);

            itemFont = new Font(SystemFonts.MessageBoxFont.FontFamily, 8f, FontStyle.Italic);

            vScrollBar = new VScrollBar();
            vScrollBar.ValueChanged += OnScrollBarScroll;
            Controls.Add(vScrollBar);

            labelEdit.AutoSize = false;
            labelEdit.Visible = false;
            labelEdit.LostFocus += OnLabelEditLostFocus;
            Controls.Add(labelEdit);

            items.CollectionChanged += OnCollectionChanged;

            PrepareVisualElements();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                // TODO
                itemFont.Dispose();

                vScrollBar.Dispose();
                labelEdit.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;

            base.OnHandleDestroyed(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            OnRefreshLayout();

            base.OnLayout(e);
        }

        protected override void OnResize(EventArgs e)
        {
            EndEdit(true);

            Invalidate();

            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.Clear(Enabled ? SystemColors.Window : SystemColors.Control);

            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);

            Region oldClip = e.Graphics.Clip;
            e.Graphics.SetClip(ClientRectangleInner, CombineMode.Intersect);

            if (items.Count > 0) {
                Size sizeCheckBox = CheckBoxRenderer.GetGlyphSize(e.Graphics, CheckBoxState.CheckedNormal);

                using (Brush brush = new SolidBrush(Color.FromArgb(0x05000000))) {
                    e.Graphics.FillRectangle(brush,
                        new Rectangle(ClientRectangleInner.Left, ClientRectangleInner.Top,
                            6 + sizeCheckBox.Width + 6 - ClientRectangleInner.Left - 1, ClientRectangleInner.Height));
                }

                PaintColumnLine(e.Graphics, 6 + sizeCheckBox.Width + 6, ClientRectangleInner.Top, ClientRectangleInner.Height);
                PaintColumnLine(e.Graphics, ClientRectangleInner.Width - (descriptionWidth + 6 - 2), ClientRectangleInner.Top, ClientRectangleInner.Height);

                int y = -vScrollBar.Value + ClientRectangleInner.Top + 1;
                // TODO: Optimization - adjust lower and upper bounds of 'i'
                for (int _i = 0; _i < items.Count; _i++) {
                    if (y >= -rowHeight && y <= ClientRectangleInner.Height) {
                        Item i = items[_i];

                        int itemX = 6;
                        Rectangle boundsCheckBox = new Rectangle(itemX, y + ((rowHeight - sizeCheckBox.Height) >> 1),
                            sizeCheckBox.Width, sizeCheckBox.Height);
                        itemX += boundsCheckBox.Width + 8;

                        Rectangle boundsIcon = new Rectangle(itemX + ((imageMarginSize - imageSize) >> 1),
                            y + imageSpacing + ((imageMarginSize - imageSize) >> 1), imageSize, imageSize);

                        itemX += imageSize + 10;

                        Rectangle boundsText = new Rectangle(itemX, y + 1,
                            ClientRectangleInner.Width - itemX - 1 - (descriptionWidth + 6), rowHeight - 2);

                        Rectangle boundsDescription = new Rectangle(boundsText.Right + 6, y + 1, descriptionWidth,
                            rowHeight - 2);

                        i.BoundsCheckBox = boundsCheckBox;
                        i.BoundsSelection = new Rectangle(ClientRectangleInner.Left + 1, y, ClientRectangleInner.Width - 2, rowHeight);
                        i.BoundsIcon = boundsIcon;
                        i.BoundsText = boundsText;

                        // Selection
                        Color foreColor = ForeColor;
                        VisualStyleRenderer r = null;
                        if (stateFocusedItem == i) {
                            if (stateSelectedItem) {
                                if (!Enabled || (!Focused && stateHotItem != i && stateEditItem != i)) {
                                    r = itemDisabledRenderer;
                                } else if (stateHotItem == i || stateEditItem == i) {
                                    r = itemHotSelectedRenderer;
                                } else {
                                    r = itemSelectedRenderer;
                                }

                                if (r == null) {
                                    e.Graphics.FillRectangle(SystemBrushes.Highlight, i.BoundsSelection);
                                    foreColor = SystemColors.HighlightText;
                                }
                            } else if (Enabled) {
                                if (stateHotItem == i) {
                                    r = itemHotFocusedRenderer;

                                    if (r == null) {
                                        ControlPaint.DrawFocusRectangle(e.Graphics, i.BoundsSelection);
                                    }
                                } else if (Focused) {
                                    r = itemFocusedRenderer;

                                    if (r == null) {
                                        ControlPaint.DrawFocusRectangle(e.Graphics, i.BoundsSelection);
                                    }
                                }
                            }
                        } else if (stateHotItem == i && Enabled) {
                            r = itemHotRenderer;
                        }

                        if (r != null) {
                            r.DrawBackground(e.Graphics, i.BoundsSelection);
                        }

                        // CheckBox
                        if (e.ClipRectangle.IntersectsWith(boundsCheckBox)) {
                            int state = 1; // UncheckedNormal
                            if (!Enabled || !i.CheckEnabled) {
                                state = 4; // UncheckedDisabled
                            } else if (i.StateCheckBox == ItemCheckBoxState.Hot) {
                                state = 2; // UncheckedHot
                            } else if (i.StateCheckBox == ItemCheckBoxState.Pressed) {
                                state = 3; // UncheckedPressed
                            }

                            if (i.CheckState == CheckState.Indeterminate) {
                                state += 9 - 1;
                            } else if (i.CheckState == CheckState.Checked) {
                                state += 5 - 1;
                            }

                            Point pointCheckBox = new Point(boundsCheckBox.X, boundsCheckBox.Y);
                            CheckBoxRenderer.DrawCheckBox(e.Graphics, pointCheckBox, (CheckBoxState)state);
                        }

                        // Icon (Color)
                        if (e.ClipRectangle.IntersectsWith(boundsIcon)) {
                            using (Brush brush = new SolidBrush(i.Color)) {
                                e.Graphics.FillRectangle(brush, boundsIcon);

                                using (Pen pen = new Pen(Color.FromArgb(0x66000000))) {
                                    e.Graphics.DrawRectangle(pen,
                                        new Rectangle(boundsIcon.X - 1, boundsIcon.Y - 1, boundsIcon.Width + 1,
                                            boundsIcon.Height + 1));
                                }
                                using (Pen pen = new Pen(Color.FromArgb(0x16000000))) {
                                    e.Graphics.DrawRectangle(pen,
                                        new Rectangle(boundsIcon.X - 2, boundsIcon.Y - 2, boundsIcon.Width + 3,
                                            boundsIcon.Height + 3));
                                }
                                using (Pen pen = new Pen(Color.FromArgb(0x26ffffff))) {
                                    e.Graphics.DrawRectangle(pen,
                                        new Rectangle(boundsIcon.X, boundsIcon.Y, boundsIcon.Width - 1,
                                            boundsIcon.Height - 1));
                                }
                            }
                        }

                        // Content
                        if (!string.IsNullOrEmpty(i.TextDisplay) && e.ClipRectangle.IntersectsWith(boundsText)) {
                            using (GdiGraphics g = GdiGraphics.FromGraphics(e.Graphics)) {
                                i.textDisplay.DefaultColor = foreColor;
                                i.textDisplay.Font = itemFont;
                                int height2 = i.textDisplay.MeasureHeight(g);
                                i.textDisplay.Draw(g,
                                    new Rectangle(boundsText.X + 3, boundsText.Y + ((boundsText.Height - height2) / 2),
                                        boundsText.Width - 6, height2));
                            }
                        }

                        if (!string.IsNullOrEmpty(i.Description) && e.ClipRectangle.IntersectsWith(boundsDescription)) {
                            const TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.SingleLine |
                                                          TextFormatFlags.VerticalCenter |
                                                          TextFormatFlags.PreserveGraphicsClipping |
                                                          TextFormatFlags.EndEllipsis;

                            TextRenderer.DrawText(e.Graphics, i.Description, itemFont, boundsDescription, foreColor, flags);
                        }
                    }

                    y += rowHeight - 1;
                }

                // Column resize line
                if (columnSizingState != ColumnSizingState.Normal) {
                    int x = ClientRectangleInner.Width - (descriptionWidth + 6 - 2);

                    using (Pen pen = new Pen(Color.FromArgb(columnSizingState == ColumnSizingState.Active ? 80 : 40, 0, 0, 0))) {
                        e.Graphics.DrawLine(pen, x - 1, ClientRectangleInner.Top, x - 1, ClientRectangleInner.Bottom);
                    }

                    using (Pen pen = new Pen(Color.FromArgb(0x78ffffff))) {
                        e.Graphics.DrawLine(pen, x - 2, ClientRectangleInner.Top, x - 2, ClientRectangleInner.Bottom);
                        e.Graphics.DrawLine(pen, x, ClientRectangleInner.Top, x, ClientRectangleInner.Bottom);
                    }
                }
            } else if (!string.IsNullOrEmpty(emptyText)) {
                TextRenderer.DrawText(e.Graphics, emptyText, Font, ClientRectangle,
                    SystemColors.GrayText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            e.Graphics.Clip = oldClip;
            oldClip.Dispose();

            base.OnPaint(e);
        }

        /// <summary>
        /// Paints column line
        /// </summary>
        /// <param name="g">Graphic context</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="height">Height</param>
        private void PaintColumnLine(Graphics g, int x, int y, int height)
        {
            if (columnLineRenderer != null) {
                columnLineRenderer.DrawBackground(g, new Rectangle(x - 2, y, 2, height));
            } else {
                g.DrawLine(SystemPens.Control, x - 1, y, x - 1, y + height);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta != 0) {
                if (vScrollBar.Visible) {
                    int scroll = -e.Delta * MouseWheelScrollExtent / 120;
                    vScrollBar.Value =
                        Math.Min(Math.Max(vScrollBar.Value + scroll * vScrollBar.SmallChange, vScrollBar.Minimum),
                            vScrollBar.Maximum - vScrollBar.LargeChange + 1);
                }

                OnMouseMove(e);
            }

            base.OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int x = ClientRectangleInner.Width - (descriptionWidth + 6 - 2);

            if (columnSizingState == ColumnSizingState.Active) {
                int newWidth = ClientRectangleInner.Width - e.X + (columnSizingOffset - 6 + 2);
                if (newWidth > descriptionMaxWidth)
                    newWidth = descriptionMaxWidth;
                else if (newWidth < descriptionMinWidth)
                    newWidth = descriptionMinWidth;

                if (descriptionWidth != newWidth) {
                    descriptionWidth = newWidth;
                    Invalidate();
                }
            } else if (items.Count > 0 && x - 5 < e.X && x + 2 > e.X) {
                if (columnSizingState == ColumnSizingState.Normal) {
                    columnSizingState = ColumnSizingState.Hot;
                    Cursor = Cursors.SizeWE;
                    Invalidate();
                }
            } else {
                if (columnSizingState == ColumnSizingState.Hot) {
                    columnSizingState = ColumnSizingState.Normal;
                    Cursor = Cursors.Default;
                    Invalidate();
                }

                int offset = (e.Y + vScrollBar.Value - 2);
                int index = offset / (rowHeight - 1);

                if (index < 0 || index >= items.Count || e.X < ClientRectangleInner.Left + 1 || e.X > ClientRectangleInner.Width - 1) {
                    if (stateHotItem != null) {
                        stateHotItem.StateCheckBox = ItemCheckBoxState.Normal;
                        stateHotItem = null;
                        Invalidate();
                    }
                } else {
                    bool doRepaint = false;
                    if (stateHotItem != items[index]) {
                        stateHotItem = items[index];
                        doRepaint = true;
                    }

                    bool s = stateHotItem.BoundsCheckBox.Contains(e.Location);
                    if (!s && stateHotItem.StateCheckBox == ItemCheckBoxState.Hot) {
                        stateHotItem.StateCheckBox = ItemCheckBoxState.Normal;
                        doRepaint = true;
                    } else if (s && stateHotItem.StateCheckBox == ItemCheckBoxState.Normal) {
                        stateHotItem.StateCheckBox = ItemCheckBoxState.Hot;
                        doRepaint = true;
                    }

                    if (doRepaint) {
                        Invalidate();
                    }
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            bool invalidate = false;

            if (stateHotItem != null) {
                stateHotItem = null;
                invalidate = true;
            }

            if (columnSizingState != ColumnSizingState.Normal) {
                columnSizingState = ColumnSizingState.Normal;
                invalidate = true;
            }

            if (invalidate) {
                Invalidate();
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            if (e.Button == MouseButtons.Left) {
                if (columnSizingState == ColumnSizingState.Hot) {
                    columnSizingState = ColumnSizingState.Active;

                    int x = ClientRectangleInner.Width - (descriptionWidth + 6 - 2);
                    columnSizingOffset = e.X - x;

                    Invalidate();
                } else if (stateHotItem != null) {
                    if (stateHotItem.CheckEnabled && stateHotItem.StateCheckBox == ItemCheckBoxState.Hot) {
                        stateHotItem.StateCheckBox = ItemCheckBoxState.Pressed;
                        statePressedCheckBox = stateHotItem;
                        Invalidate();
                    } else if (stateHotItem.BoundsIcon.Contains(e.Location)) {
                        Item i = stateHotItem;

                        using (ColorDialog dialog = new ColorDialog()) {
                            dialog.Color = i.Color;
                            dialog.ShowDialog(this);
                            i.Color = dialog.Color;

                            Invalidate();
                        }
                    } else {
                        stateFocusedItem = stateHotItem;
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                } else {
                    if (stateSelectedItem) {
                        stateSelectedItem = false;

                        Invalidate();

                        OnItemSelectionChanged();
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                if (columnSizingState == ColumnSizingState.Active) {
                    columnSizingState = ColumnSizingState.Normal;
                    Cursor = Cursors.Default;
                    Invalidate();
                } else if (statePressedCheckBox != null) {
                    statePressedCheckBox.StateCheckBox = ItemCheckBoxState.Normal;

                    if (statePressedCheckBox.BoundsCheckBox.Contains(e.Location)) {
                        statePressedCheckBox.CheckState = (statePressedCheckBox.CheckState == CheckState.Unchecked
                            ? CheckState.Checked
                            : CheckState.Unchecked);
                    }

                    statePressedCheckBox = null;

                    Invalidate();
                }
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && stateHotItem != null) {
                if (stateHotItem.BoundsText.Contains(e.Location)) {
                    BeginEdit();
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keys = keyData & ~(Keys.Shift | Keys.Control);
            switch (keys) {
                case Keys.Enter:
                    if (stateEditItem == null) {
                        if (stateFocusedItem != null) {
                            BeginEdit();
                        }
                    } else {
                        EndEdit(true);
                    }
                    return true;

                case Keys.Escape:
                    if (stateEditItem != null) {
                        EndEdit(false);
                    }
                    return true;

                case Keys.Up:
                    if (stateEditItem == null && stateFocusedItem != null && stateFocusedItem.Index > 0) {
                        stateFocusedItem = items[stateFocusedItem.Index - 1];
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                    return true;

                case Keys.Down:
                    if (stateEditItem == null && stateFocusedItem != null && stateFocusedItem.Index < items.Count - 1) {
                        stateFocusedItem = items[stateFocusedItem.Index + 1];
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                    return true;

                case Keys.Home:
                    if (stateEditItem == null && items.Count > 0) {
                        stateFocusedItem = items[0];
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                    return true;

                case Keys.End:
                    if (stateEditItem == null && items.Count > 0) {
                        stateFocusedItem = items[items.Count - 1];
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                    return true;

                case Keys.PageUp:
                    if (stateEditItem == null && stateFocusedItem != null) {
                        int index = Math.Max(stateFocusedItem.Index - ((Height - (borderPadding << 1)) / (rowHeight - 1)), 0);

                        stateFocusedItem = items[index];
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                    return true;

                case Keys.PageDown:
                    if (stateEditItem == null && stateFocusedItem != null) {
                        int index = Math.Min(stateFocusedItem.Index + ((Height - (borderPadding << 1)) / (rowHeight - 1)), items.Count - 1);

                        stateFocusedItem = items[index];
                        stateSelectedItem = true;

                        if (!EnsureVisible(stateFocusedItem)) {
                            Invalidate();
                        }

                        OnItemSelectionChanged();
                    }
                    return true;
                    
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        private void OnScrollBarScroll(object sender, EventArgs e)
        {
            EndEdit(false);

            Invalidate();
            Update();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException(nameof(e.Action));
            }

            if (e.NewItems != null) {
                for (int i = 0; i < e.NewItems.Count; i++) {
                    Item current = (Item)e.NewItems[i];
                    current.Index = e.NewStartingIndex + i;
                    current.PropertyChanged += OnPropertyChanged;
                }

                for (int i = e.NewStartingIndex + e.NewItems.Count; i < items.Count; i++) {
                    items[i].Index = i;
                }
            }

            if (e.OldItems != null) {
                int index = int.MaxValue;

                for (int i = 0; i < e.OldItems.Count; i++) {
                    Item current = (Item)e.OldItems[i];

                    if (index > current.Index) {
                        index = current.Index;
                    }

                    current.Index = -1;
                    current.PropertyChanged -= OnPropertyChanged;

                    if (stateFocusedItem == current) {
                        FocusedItem = null;
                    }
                }

                for (int i = index; i < items.Count; i++) {
                    items[i].Index = i;
                }
            }

            EndEdit(false);
            OnRefreshLayout();
            Invalidate();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(sender, e);

            // TODO: Optimize
            if (e.PropertyName == "TextDisplay" || e.PropertyName == "Description" || e.PropertyName == "Color" ||
                e.PropertyName == "CheckState") {
                Invalidate();
            }
        }

        protected void OnItemSelectionChanged()
        {
            ItemSelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Ensure that the item is visible
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns true if the control was repaint; false, otherwise</returns>
        public bool EnsureVisible(Item item)
        {
            if (item == null || !vScrollBar.Visible) {
                return false;
            }

            Rectangle bounds = new Rectangle(ClientRectangleInner.Left + 1, -vScrollBar.Value + ClientRectangleInner.Top + 1 + (rowHeight - 1) * item.Index, ClientRectangleInner.Width - 2, rowHeight);
            if (bounds.Width <= 0 || bounds.Height <= 0) {
                return false;
            }

            if (bounds.Top < ClientRectangleInner.Top) {
                vScrollBar.Value -= ClientRectangleInner.Top - bounds.Top + 1;
            } else if (bounds.Bottom > ClientRectangleInner.Bottom) {
                vScrollBar.Value += bounds.Bottom - ClientRectangleInner.Bottom + 1;
            } else {
                return false;
            }

            return true;
        }

        private void OnRefreshLayout()
        {
            Size clientSize = ClientSize;

            int contentHeight = items.Count * (rowHeight - 1) + 2; // TODO: Is this '2' correct?
            int widthInner = clientSize.Width - (borderPadding << 1);
            int heightInner = clientSize.Height - (borderPadding << 1);

            vScrollBar.Top = borderPadding;
            vScrollBar.Height = heightInner;
            vScrollBar.Left = Width - borderPadding - vScrollBar.Width;

            if (contentHeight < Height) {
                vScrollBar.Visible = false;
                vScrollBar.Value = 0;

                ClientRectangleInner = new Rectangle(borderPadding, borderPadding, widthInner, heightInner);
            } else {
                vScrollBar.Visible = true;

                vScrollBar.Maximum = contentHeight;
                vScrollBar.SmallChange = rowHeight - 1;
                vScrollBar.LargeChange = Math.Max(heightInner, 0);

                if (contentHeight < heightInner + vScrollBar.Value) {
                    vScrollBar.Value = Math.Max(0, contentHeight - heightInner + 1);
                }

                ClientRectangleInner = new Rectangle(borderPadding, borderPadding, widthInner - vScrollBar.Width, heightInner);
            }
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            PrepareVisualElements();
        }

        private void PrepareVisualElements()
        {
            bool isSupported = VisualStyleRenderer.IsSupported;
            if (cachedUseRenderers == isSupported) {
                return;
            }

            cachedUseRenderers = isSupported;

            if (isSupported) {
                VisualStyleElement e;

                e = VisualStyleElement.CreateElement("Explorer::ListView", 1, 2);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    itemHotRenderer = new VisualStyleRenderer(e);
                }
                e = VisualStyleElement.CreateElement("ItemsView", 3, 1);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    itemFocusedRenderer = new VisualStyleRenderer(e);
                }
                e = VisualStyleElement.CreateElement("ItemsView", 3, 2);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    itemHotFocusedRenderer = new VisualStyleRenderer(e);
                }
                e = VisualStyleElement.CreateElement("Explorer::ListView", 1, 3);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    itemSelectedRenderer = new VisualStyleRenderer(e);
                }
                e = VisualStyleElement.CreateElement("Explorer::ListView", 1, 6);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    itemHotSelectedRenderer = new VisualStyleRenderer(e);
                }
                e = VisualStyleElement.CreateElement("Explorer::ListView", 1, 5);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    itemDisabledRenderer = new VisualStyleRenderer(e);
                }
                e = VisualStyleElement.CreateElement("Explorer::ListView", 10, 1);
                if (VisualStyleRenderer.IsElementDefined(e)) {
                    columnLineRenderer = new VisualStyleRenderer(e);
                }
            } else {
                itemHotRenderer = null;
                itemFocusedRenderer = null;
                itemHotFocusedRenderer = null;
                itemSelectedRenderer = null;
                itemHotSelectedRenderer = null;
                itemDisabledRenderer = null;
                columnLineRenderer = null;
            }
        }

        #region Label Edit
        private void OnLabelEditLostFocus(object sender, EventArgs e)
        {
            EndEdit(true);
        }

        public void BeginEdit()
        {
            if (stateFocusedItem == null || stateEditItem != null) {
                return;
            }

            if (stateFocusedItem.BoundsText.Width <= 0 || stateFocusedItem.BoundsText.Height <= 0) {
                return;
            }

            CancelEventArgs args = new CancelEventArgs();
            BeforeEdit?.Invoke(stateFocusedItem, args);
            if (args.Cancel) {
                return;
            }

            stateEditItem = stateFocusedItem;

            if (EnsureVisible(stateEditItem)) {
                Update();
            }

            labelEdit.Text = stateEditItem.Text;
            labelEdit.Bounds = new Rectangle(stateEditItem.BoundsText.X, stateEditItem.BoundsText.Y + 1,
                stateEditItem.BoundsText.Width, stateEditItem.BoundsText.Height - 2);
            labelEdit.Font = itemFont;
            labelEdit.TextAlign = HorizontalAlignment.Left;

            labelEdit.Visible = true;
            labelEdit.Focus();
        }

        public void EndEdit(bool acceptChanges = true)
        {
            if (stateEditItem == null) {
                return;
            }

            if (acceptChanges) {
                stateEditItem.Text = labelEdit.Text;
            }

            stateEditItem = null;

            labelEdit.Visible = false;

            Focus();
        }
        #endregion
    }
}