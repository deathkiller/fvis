using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace fVis.Misc
{
    /// <summary>
    /// Provides methods to draw and measure text blocks with basic formatting
    /// </summary>
    internal class FormattedTextBlock
    {
        public delegate Image ImageResourceCallbackDelegate(string resourceName);

        [Flags]
        private enum PartFlags
        {
            Default = 0,
            Alignment = 1
        }
        private struct Part
        {
            public PartFlags Flags;

            public PartText Text;
            public PartAlignment Alignment;
        }

        private struct PartText
        {
            public string Value;
            public Point Location;
            public int Height;
            public Color Color;
            public Font Font;
        }

        private struct PartAlignment
        {
            public int LineWidth;
        }

        private Color defaultColor = Control.DefaultForeColor;
        private Font font;
        private ImageResourceCallbackDelegate imageResourceCallback;
        private int proposedWidth = 4096;
        private string text;

        private Part[] parts;

        /// <summary>
        /// Default text color
        /// </summary>
        public Color DefaultColor
        {
            get
            {
                return defaultColor;
            }
            set
            {
                if (defaultColor == value) {
                    return;
                }

                defaultColor = value;
                parts = null;
            }
        }

        /// <summary>
        /// Default text font
        /// </summary>
        public Font Font
        {
            get
            {
                return font;
            }
            set
            {
                if (font == value) {
                    return;
                }
                if (value == null) {
                    throw new ArgumentNullException(nameof(Font));
                }

                font = value;
                parts = null;
            }
        }

        /// <summary>
        /// Callback for image rendering
        /// </summary>
        public ImageResourceCallbackDelegate ImageResourceCallback
        {
            get { return imageResourceCallback; }
            set
            {
                if (imageResourceCallback == value) {
                    return;
                }

                imageResourceCallback = value;
                parts = null;
            }
        }

        /// <summary>
        /// Max. width of the block
        /// </summary>
        public int ProposedWidth
        {
            get
            {
                return proposedWidth;
            }
            set
            {
                if (proposedWidth == value) {
                    return;
                }
                if (value <= 0) {
                    throw new ArgumentOutOfRangeException(nameof(ProposedWidth));
                }

                proposedWidth = value;
                parts = null;
            }
        }

        /// <summary>
        /// Text contained in the block
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text == value) {
                    return;
                }

                text = value;
                parts = null;
            }
        }

        public FormattedTextBlock()
        {
        }

        public FormattedTextBlock(string text, Font font)
        {
            if (font == null) {
                throw new ArgumentNullException(nameof(Font));
            }

            this.text = text;
            this.font = font;
        }

        /// <summary>
        /// Draw this text block within specified bounds
        /// </summary>
        /// <param name="g">Graphic context</param>
        /// <param name="bounds">Bounds</param>
        public void Draw(GdiGraphics g, Rectangle bounds)
        {
            if (parts == null || proposedWidth != bounds.Width) {
                proposedWidth = bounds.Width;
                RecreateCache(g);
            }

            int i = 0;
            for (; i < parts.Length; i++) {
                Part currentPart = parts[i];

                // TODO: Rework this...
                if (currentPart.Flags == PartFlags.Alignment) {
                    if (currentPart.Alignment.LineWidth > 0) {
                        bounds.X += (bounds.Width - currentPart.Alignment.LineWidth);
                    }
                    continue;
                }

                if (currentPart.Text.Location.Y + currentPart.Text.Height > bounds.Height) {
                    break;
                }

                Point p = currentPart.Text.Location;
                p.X += bounds.X;
                p.Y += bounds.Y;

                if (currentPart.Text.Font == null) {
                    if (imageResourceCallback != null) {
                        Image image = imageResourceCallback(currentPart.Text.Value);
                        if (image != null) {
                            g.ManagedGraphics.DrawImage(image, p);
                        }
                    }
                } else {
                    g.DrawString(currentPart.Text.Value, currentPart.Text.Font, currentPart.Text.Color, p);
                }
            }
        }

        /// <summary>
        /// Measure height of the text block
        /// </summary>
        /// <param name="g">Graphic context</param>
        /// <returns>Height</returns>
        public int MeasureHeight(GdiGraphics g)
        {
            if (parts == null) {
                RecreateCache(g);
            }
            if (parts.Length == 0) {
                return 0;
            }

            Part lastPart = parts[parts.Length - 1];
            return lastPart.Text.Location.Y + lastPart.Text.Height;
        }

        /// <summary>
        /// Measure height of the text block
        /// </summary>
        /// <param name="g">Graphic context</param>
        /// <returns>Height</returns>
        public int MeasureHeight(Lazy<GdiGraphics> g)
        {
            if (parts == null) {
                RecreateCache(g.Value);
            }
            if (parts.Length == 0) {
                return 0;
            }

            Part lastPart = parts[parts.Length - 1];
            return lastPart.Text.Location.Y + lastPart.Text.Height;
        }

        /// <summary>
        /// Measure block content and recreate cache
        /// </summary>
        /// <param name="g">Graphic context</param>
        private void RecreateCache(GdiGraphics g)
        {
            if (text == null) {
                return;
            }

            List<Part> processedParts = new List<Part>();

            string unprocessedText = text;

            Color currentColor = defaultColor;
            Font currentFont = font;

            Point currentLocation = Point.Empty;

            int firstPartOfLine = 0;

            int lineAlignIndex = -1;

            while (unprocessedText.Length > 0) {
                Part part;

                int formatIndex = unprocessedText.IndexOf("\f[", StringComparison.Ordinal);
                while (formatIndex == 0) {
                    int formatIndexEnd = unprocessedText.IndexOf(']', formatIndex + 2);
                    if (formatIndexEnd == -1) {
                        throw new InvalidDataException("Missing closing format bracket");
                    }

                    string formatString = unprocessedText.Substring(formatIndex + 2, formatIndexEnd - (formatIndex + 2));
                    switch (formatString) {
                        case "Rc":  // Reset Color
                            currentColor = defaultColor;
                            break;

                        case "Rs":  // Reset Style
                            currentFont = font;
                            break;

                        case "B":   // Bold
                            currentFont = new Font(font, currentFont.Style ^ FontStyle.Bold);
                            break;

                        case "I":   // Italic
                            currentFont = new Font(font, currentFont.Style ^ FontStyle.Italic);
                            break;

                        case "U":   // Underline
                            currentFont = new Font(font, currentFont.Style ^ FontStyle.Underline);
                            break;

                        case "S":   // Strikeout
                            currentFont = new Font(font, currentFont.Style ^ FontStyle.Strikeout);
                            break;

                        case "-":   // Right Align
                            // TODO: Rework this...
                            if (lineAlignIndex == -1) {
                                lineAlignIndex = processedParts.Count;

                                part = default(Part);
                                part.Flags = PartFlags.Alignment;
                                processedParts.Add(part);
                            }
                            break;

                        default:    // Image Resource
                            if (formatString.StartsWith("image:", StringComparison.Ordinal)) {
                                string resourceName = formatString.Substring(6);

                                if (imageResourceCallback == null) {
                                    throw new NotSupportedException("ImageResourceCallback is not specified");
                                }

                                Image image = imageResourceCallback(resourceName);
                                if (image == null) {
                                    throw new InvalidDataException("Resource \"" + resourceName + "\" not found");
                                }

                                if (currentLocation.X + image.Width > proposedWidth) {
                                    PerformVerticalAlignment(processedParts, firstPartOfLine);
                                    goto End;
                                }

                                part = default(Part);
                                part.Text.Value = resourceName;
                                part.Text.Location = currentLocation;
                                part.Text.Height = image.Height;
                                processedParts.Add(part);

                                currentLocation.X += image.Width;
                            } else { // Custom Color
                                NumberStyles styles;
                                if (formatString.StartsWith("0x", StringComparison.Ordinal)) {
                                    formatString = formatString.Substring(2);
                                    styles = NumberStyles.HexNumber;
                                } else {
                                    styles = NumberStyles.Integer;
                                }

                                uint color;
                                if (!uint.TryParse(formatString, styles, NumberFormatInfo.CurrentInfo, out color)) {
                                    throw new InvalidDataException("Unknown format descriptor \"" + formatString + "\"");
                                }

                                currentColor = Color.FromArgb((int)color);
                            }
                            break;
                    }

                    unprocessedText = unprocessedText.Substring(formatIndexEnd + 1);

                    formatIndex = unprocessedText.IndexOf("\f[", StringComparison.Ordinal);
                }

                if (unprocessedText.Length == 0) {
                    if (lineAlignIndex != -1) {
                        part = processedParts[lineAlignIndex];
                        part.Alignment.LineWidth = currentLocation.X;
                        processedParts[lineAlignIndex] = part;
                    }

                    PerformVerticalAlignment(processedParts, firstPartOfLine);
                    break;
                }

                int charFit, charFitWidth;
                string measuredString = (formatIndex > 0 ? unprocessedText.Substring(0, formatIndex) : unprocessedText);
                Size size = g.MeasureString(measuredString, currentFont, proposedWidth - currentLocation.X, out charFit, out charFitWidth);

                if (formatIndex != -1 && formatIndex <= charFit) {
                    int endOfLine = unprocessedText.IndexOf('\n', 0, formatIndex);
                    if (endOfLine != -1) {
                        // '\n' is nearest
                        part = default(Part);
                        part.Text.Value = unprocessedText.Substring(0, endOfLine);
                        part.Text.Location = currentLocation;
                        part.Text.Color = currentColor;
                        part.Text.Font = currentFont;
                        part.Text.Height = size.Height;
                        processedParts.Add(part);

                        if (lineAlignIndex != -1) {
                            part = processedParts[lineAlignIndex];
                            part.Alignment.LineWidth = currentLocation.X + charFitWidth;
                            processedParts[lineAlignIndex] = part;
                        }

                        PerformVerticalAlignment(processedParts, firstPartOfLine);
                        break;
                    } else {
                        // '\f' is nearest
                        part = default(Part);
                        part.Text.Value = unprocessedText.Substring(0, formatIndex);
                        part.Text.Location = currentLocation;
                        part.Text.Color = currentColor;
                        part.Text.Font = currentFont;
                        part.Text.Height = size.Height;
                        processedParts.Add(part);

                        currentLocation.X += charFitWidth;

                        unprocessedText = unprocessedText.Substring(formatIndex);
                    }
                } else if (charFit < unprocessedText.Length) {
                    if (charFit > 2) {
                        part = default(Part);
                        part.Text.Value = unprocessedText.Substring(0, charFit - 2) + "...";
                        part.Text.Location = currentLocation;
                        part.Text.Color = currentColor;
                        part.Text.Font = currentFont;
                        part.Text.Height = size.Height;
                        processedParts.Add(part);
                    }

                    PerformVerticalAlignment(processedParts, firstPartOfLine);

                    break;
                } else {
                    int endOfLine = unprocessedText.IndexOf('\n');
                    if (endOfLine == -1) {
                        // Value is short enough
                        part = default(Part);
                        part.Text.Value = unprocessedText;
                        part.Text.Location = currentLocation;
                        part.Text.Color = currentColor;
                        part.Text.Font = currentFont;
                        part.Text.Height = size.Height;
                        processedParts.Add(part);

                        if (lineAlignIndex != -1) {
                            part = processedParts[lineAlignIndex];
                            part.Alignment.LineWidth = (currentLocation.X + charFitWidth);
                            processedParts[lineAlignIndex] = part;
                            lineAlignIndex = -1;
                        }

                        PerformVerticalAlignment(processedParts, firstPartOfLine);

                        unprocessedText = string.Empty;
                    } else {
                        // '\n' found
                        part = default(Part);
                        part.Text.Value = unprocessedText.Substring(0, endOfLine);
                        part.Text.Location = currentLocation;
                        part.Text.Color = currentColor;
                        part.Text.Font = currentFont;
                        part.Text.Height = size.Height;
                        processedParts.Add(part);

                        if (lineAlignIndex != -1) {
                            part = processedParts[lineAlignIndex];
                            part.Alignment.LineWidth = (currentLocation.X + charFitWidth);
                            processedParts[lineAlignIndex] = part;
                        }

                        PerformVerticalAlignment(processedParts, firstPartOfLine);
                        break;
                    }
                }
            }

        End:
            if (processedParts.Count > 0) {
                int last = processedParts.Count - 1;
                if (processedParts[last].Flags == PartFlags.Alignment) {
                    processedParts.RemoveAt(last);
                }
            }

            parts = processedParts.ToArray();
        }

        /// <summary>
        /// Perform vertical alignment of current line
        /// </summary>
        /// <param name="processedParts">Block parts</param>
        /// <param name="firstPartOfLine">Start of current line</param>
        /// <returns>Max. height</returns>
        private static int PerformVerticalAlignment(List<Part> processedParts, int firstPartOfLine)
        {
            int maxHeight = -1;

            for (int i = firstPartOfLine; i < processedParts.Count; i++) {
                if (processedParts[i].Flags == PartFlags.Alignment) {
                    continue;
                }

                maxHeight = Math.Max(maxHeight, processedParts[i].Text.Height);
            }

            for (int i = firstPartOfLine; i < processedParts.Count; i++) {
                Part lastPart = processedParts[i];

                if (lastPart.Flags == PartFlags.Alignment) {
                    continue;
                }

                if (lastPart.Text.Height != maxHeight) {
                    int diff = (maxHeight - lastPart.Text.Height) >> 1;
                    lastPart.Text.Location.Y += diff;
                    lastPart.Text.Height = maxHeight - diff;
                    processedParts[i] = lastPart;
                }
            }

            return maxHeight;
        }
    }
}