using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using fVis.Callbacks;
using fVis.Misc;
using fVis.NumericValueSources;

namespace fVis.Controls
{
    partial class ListView
    {
        public enum ItemCheckBoxState
        {
            Normal,
            Hot,
            Pressed
        }

        internal class Item : INotifyPropertyChanged
        {
            public INumericValueSource NumericValueSource
            {
                get { return numericValueSource; }
                set
                {
                    if (Equals(value, numericValueSource)) {
                        return;
                    }

                    numericValueSource = value;
                    OnPropertyChanged();
                }
            }

            public OperatorCallbacks OperatorCallbacks
            {
                get { return operatorCallbacks; }
                set
                {
                    if (Equals(value, operatorCallbacks)) {
                        return;
                    }

                    operatorCallbacks = value;
                    OnPropertyChanged();
                }
            }

            public Color Color
            {
                get { return color; }
                set
                {
                    if (value.Equals(color)) {
                        return;
                    }

                    color = value;
                    OnPropertyChanged();
                }
            }

            public string Text
            {
                get { return text; }
                set
                {
                    if (value == text) {
                        return;
                    }

                    text = value;
                    OnPropertyChanged();
                }
            }

            public string TextDisplay
            {
                get { return textDisplay.Text; }
                set
                {
                    if (value == textDisplay.Text) {
                        return;
                    }

                    textDisplay.Text = value;
                    OnPropertyChanged();
                }
            }

            public string Description
            {
                get { return description; }
                set
                {
                    if (value == description) {
                        return;
                    }

                    description = value;
                    OnPropertyChanged();
                }
            }

            public CheckState CheckState
            {
                get { return checkState; }
                set
                {
                    if (value == checkState) {
                        return;
                    }

                    checkState = value;
                    OnPropertyChanged();
                }
            }

            public FormattedTextBlock.ImageResourceCallbackDelegate ImageResourceCallback
            {
                get { return textDisplay.ImageResourceCallback; }
                set { textDisplay.ImageResourceCallback = value; }
            }

            private INumericValueSource numericValueSource;
            private OperatorCallbacks operatorCallbacks;

            private Color color;
            private string text;
            internal FormattedTextBlock textDisplay = new FormattedTextBlock();
            private string description;

            public int Index;

            public bool CheckEnabled = true;
            public ItemCheckBoxState StateCheckBox;

            private CheckState checkState;

            public Rectangle BoundsSelection { get; internal set; }
            public Rectangle BoundsCheckBox { get; internal set; }
            public Rectangle BoundsIcon { get; internal set; }
            public Rectangle BoundsText { get; internal set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}