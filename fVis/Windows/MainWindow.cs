using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using fVis.Callbacks;
using fVis.Controls;
using fVis.Extensions;
using fVis.NumericValueSources;
using Unclassified.TxLib;
using ListView = fVis.Controls.ListView;

namespace fVis.Windows
{
    /// <summary>
    /// Represents main window of the application
    /// </summary>
    public partial class MainWindow : Form
    {
        private readonly Random random = new Random();

        private readonly OperatorCallbacks defaultDoubleCallbacks = new DotNetOperatorCallbacks();
        private readonly OperatorCallbacks defaultFloatCallbacks = new DotNetFloatOperatorCallbacks();

        public MainWindow()
        {
            InitializeComponent();

            Text = App.AssemblyTitle;

            BackColor = Color.FromArgb(0xFA, 0xFA, 0xFA);
            toolStrip.BackColor = Color.White;

            implementationButton.DropDownButtonWidth += 2;
            highlightDifferencesButton.DropDownButtonWidth += 2;

            listView.EmptyText = Tx.T("main.list is empty");

            // Create Languages menu
            string currentCulture = Tx.GetCultureName();
            foreach (CultureInfo culture in Tx.AvailableCultures) {
                ToolStripMenuItem item = new ToolStripMenuItem(culture.DisplayName, null, OnLanguageMenuItemClick);
                item.Tag = culture.Name;
                if (culture.Name == currentCulture) {
                    item.Checked = true;
                }
                langMenuItem.DropDownItems.Add(item);
            }


            // Load available implementations
            ToolStripMenuItem implementationNetButton = new ToolStripMenuItem("[mathlib.internal.double]");
            implementationNetButton.Click += OnSetItemCallbacksButtonClick;
            implementationNetButton.Tag = defaultDoubleCallbacks;
            implementationButton.DropDownItems.Add(implementationNetButton);

            ToolStripMenuItem implementationNetFloatButton = new ToolStripMenuItem("[mathlib.internal.float]");
            implementationNetFloatButton.Click += OnSetItemCallbacksButtonClick;
            implementationNetFloatButton.Tag = defaultFloatCallbacks;
            implementationButton.DropDownItems.Add(implementationNetFloatButton);

            implementationButton.DropDownItems.Add(new ToolStripSeparator());

            LoadAllExternalOperators(implementationButton.DropDownItems);

            implementationButton.DropDownItems.Add(new ToolStripSeparator());

            ToolStripMenuItem implementationAddButton = new ToolStripMenuItem("[mathlib.add]");
            implementationAddButton.Click += OnImplementationAddButtonClick;
            implementationButton.DropDownItems.Add(implementationAddButton);

            // Add available highlight differences modes
            ToolStripMenuItem highlightOffButton = new ToolStripMenuItem("[highlight.disabled]");
            highlightOffButton.Checked = true;
            highlightOffButton.Click += OnHighlightOffButtonClick;
            highlightDifferencesButton.DropDownItems.Add(highlightOffButton);

            ToolStripMenuItem highlightConstantButton = new ToolStripMenuItem("[highlight.constant]");
            highlightConstantButton.Click += OnHighlightConstantButtonClick;
            highlightDifferencesButton.DropDownItems.Add(highlightConstantButton);

            ToolStripMenuItem highlightDynamicButton = new ToolStripMenuItem("[highlight.dynamic]");
            highlightDynamicButton.Click += OnHighlightDynamicButtonClick;
            highlightDifferencesButton.DropDownItems.Add(highlightDynamicButton);

            ToolStripMenuItem averageButton = new ToolStripMenuItem("[highlight.only diff]");
            averageButton.Click += OnAverageButtonClick;
            highlightDifferencesButton.DropDownItems.Add(averageButton);

            TxWinForms.Bind(this);
            Tx.DictionaryChanged += OnTxDictionaryChanged;

            // Setup graph
            graph.DataSource = listView.Items;

            OnScaleFactorChanged();

            RefreshToolStrip();
        }

        protected override void DestroyHandle()
        {
            Tx.DictionaryChanged -= OnTxDictionaryChanged;

            base.DestroyHandle();
        }

        private void OnTxDictionaryChanged(object sender, EventArgs e)
        {
            listView.EmptyText = Tx.T("main.list is empty");
        }

        /// <summary>
        /// Loads all mathematical libraries from x86 and x64 folders
        /// </summary>
        /// <param name="items"></param>
        private void LoadAllExternalOperators(ToolStripItemCollection items)
        {
            string pathRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string pathBitness = Path.Combine(pathRoot, Environment.Is64BitProcess ? "x64" : "x86");

            if (Directory.Exists(pathBitness)) {
                foreach (string filename in Directory.EnumerateFiles(pathBitness, "*.dll")) {
                    try {
                        NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks(filename);

                        ToolStripMenuItem item = new ToolStripMenuItem(callbacks.ImplementationName);
                        item.Click += OnSetItemCallbacksButtonClick;
                        item.Tag = callbacks;

                        string missingCallbacks = ConvertMissingCallbacksToString(callbacks);
                        if (!string.IsNullOrEmpty(missingCallbacks)) {
                            item.ToolTipText = Tx.T("mathlib.missing callbacks") + missingCallbacks;
                        }

                        items.Add(item);
                    } catch (Exception ex) {
                        Debug.WriteLine("Can't load operator callbacks from \"" + Path.GetFileName(filename) + "\": " + ex.Message);
                    }
                }
            }

            if (Environment.Is64BitProcess) {
                pathBitness = Path.Combine(pathRoot, "x86");

                if (Directory.Exists(pathBitness)) {
                    items.Add(new ToolStripSeparator());

                    foreach (string filename in Directory.EnumerateFiles(pathBitness, "*.dll")) {
                        try {
                            NativeOperatorRemotingCallbacks callbacks = new NativeOperatorRemotingCallbacks(filename);

                            ToolStripMenuItem item = new ToolStripMenuItem(callbacks.ImplementationName);
                            item.ShortcutKeyDisplayString = "(x86)";
                            item.Click += OnSetItemCallbacksButtonClick;
                            item.Tag = callbacks;

                            string missingCallbacks = ConvertMissingCallbacksToString(callbacks);
                            if (!string.IsNullOrEmpty(missingCallbacks)) {
                                item.ToolTipText = Tx.T("mathlib.missing callbacks") + missingCallbacks;
                            }

                            items.Add(item);
                        } catch (Exception ex) {
                            Debug.WriteLine("Can't load operator callbacks from \"" + Path.GetFileName(filename) + "\": " + ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts list of missing callbacks to readable string
        /// </summary>
        /// <param name="callbacks">Operator Callbacks</param>
        /// <returns>String of missing callbacks</returns>
        private string ConvertMissingCallbacksToString(OperatorCallbacks callbacks)
        {
            string[] missingCallbacks;
            NativeOperatorCallbacks noc = callbacks as NativeOperatorCallbacks;
            if (noc != null) {
                missingCallbacks = noc.MissingCallbacks;
            } else {
                NativeOperatorRemotingCallbacks norc = callbacks as NativeOperatorRemotingCallbacks;
                if (norc != null) {
                    missingCallbacks = norc.MissingCallbacks;
                } else {
                    return null;
                }
            }

            StringBuilder text = new StringBuilder();
            for (int i = 0; i < missingCallbacks.Length; i++) {
                if (i > 0) {
                    text.Append(", ");
                }

                string current = missingCallbacks[i];
                if (current.StartsWith("constant_", StringComparison.Ordinal)) {
                    text.Append(current, 9, current.Length - 9);
                } else if (current.StartsWith("operator_", StringComparison.Ordinal)) {
                    text.Append(current, 9, current.Length - 9);
                } else {
                    text.Append(current);
                }
            }
            return text.ToString();
        }

        /// <summary>
        /// Refreshes state of toolstrip
        /// </summary>
        private void RefreshToolStrip()
        {
            ListView.Item selection = listView.SelectedItem;
            if (selection == null) {
                removeButton.Enabled = false;
                duplicateButton.Enabled = false;
                exportButton.Enabled = false;
                implementationButton.Enabled = false;
            } else {
                removeButton.Enabled = true;

                ArithmeticExpression arithmeticExpression = selection.NumericValueSource as ArithmeticExpression;
                if (arithmeticExpression != null) {
                    duplicateButton.Enabled = true;
                    exportButton.Enabled = true;
                    implementationButton.Enabled = true;

                    foreach (ToolStripItem item in implementationButton.DropDownItems) {
                        if (item.Tag != null) {
                            ((ToolStripMenuItem)item).Checked = (item.Tag == arithmeticExpression.Callbacks);
                        }
                    }
                } else {
                    duplicateButton.Enabled = false;
                    exportButton.Enabled = false;
                    implementationButton.Enabled = false;
                }
            }

        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text") {
                ListView.Item item = (ListView.Item)sender;
                if (string.IsNullOrEmpty(item.Text)) {
                    item.NumericValueSource = null;
                    item.TextDisplay = "\f[0x999999]" + Tx.T("main.enter expression");
                } else {
                    try {
                        ArithmeticExpression ae = ArithmeticExpression.Parse(item.Text);
                        ae.Callbacks = item.OperatorCallbacks;

                        item.NumericValueSource = ae;

                        int i = item.Text.IndexOf('#');
                        if (i != -1) {
                            while (i > 0 && item.Text[i - 1] == ' ') {
                                i--;
                            }

                            StringBuilder sb = new StringBuilder();
                            sb.Append(item.Text, 0, i);

                            if (ae.VariableName == null && !ae.IsSimpleConstantOnly) {
                                double result = ae.Evaluate(double.NaN);

                                sb.Append("\f[I]\f[0x999999] = ");
                                sb.Append(result.ToExactString().LimitSize(32));
                                sb.Append(" \f[I]");
                            }

                            sb.Append("\f[0x498E3E]");
                            sb.Append(item.Text, i, item.Text.Length - i);

                            CheckCallbacks(item, sb);

                            item.TextDisplay = sb.ToString();
                        } else {
                            if (ae.VariableName == null && !ae.IsSimpleConstantOnly) {
                                StringBuilder sb = new StringBuilder();
                                sb.Append(item.Text);

                                double result = ae.Evaluate(double.NaN);

                                sb.Append("\f[I]\f[0x999999] = ");
                                if (double.IsPositiveInfinity(result)) {
                                    sb.Append("+∞");
                                } else if (double.IsNegativeInfinity(result)) {
                                    sb.Append("−∞");
                                } else {
                                    sb.Append(result.ToExactString().LimitSize(32));
                                }
                                sb.Append("\f[I]");

                                CheckCallbacks(item, sb);

                                item.TextDisplay = sb.ToString();
                            } else {
                                StringBuilder sb = new StringBuilder();
                                sb.Append(item.Text);

                                CheckCallbacks(item, sb);

                                item.TextDisplay = sb.ToString();
                            }
                        }
                    } catch (SyntaxException ex) {
                        item.NumericValueSource = null;

                        StringBuilder sb = new StringBuilder();
                        sb.Append("\f[0x8b0000]");

                        int length = ex.Input.IndexOf('#');
                        if (length == -1) {
                            length = ex.Input.Length;
                        }

                        if (length < 3) {
                            sb.Append(ex.Input);
                        } else {
                            if (ex.Index - 1 < 0) {
                                sb.Append("\f[U]");
                            } else {
                                sb.Append(ex.Input, 0, ex.Index - 1);

                                sb.Append("\f[U]\f[0xd00000]");
                                sb.Append(ex.Input[ex.Index - 1]);
                            }

                            sb.Append("\f[0xff0000]");
                            sb.Append(ex.Input[ex.Index]);

                            if (ex.Index + 1 < length) {
                                sb.Append("\f[0xd00000]");
                                sb.Append(ex.Input[ex.Index + 1]);

                                sb.Append("\f[0x8b0000]\f[U]");

                                if (ex.Index + 2 < length) {
                                    sb.Append(ex.Input, ex.Index + 2, length - (ex.Index + 2));
                                }
                            } else {
                                sb.Append("\f[0xd00000] \f[U]");
                            }
                        }

                        switch (ex.ExceptionType) {
                            case SyntaxException.Type.Unknown:
                                break;
                            case SyntaxException.Type.InvalidNumber:
                                sb.Append("      \f[-]\f[image:warning]  \f[0xaa6400]\f[I]");
                                sb.Append(Tx.T("expression.errors.invalid number"));
                                break;
                            case SyntaxException.Type.DistinctVariableCountExceeded:
                                sb.Append("      \f[-]\f[image:warning]  \f[0xaa6400]\f[I]");
                                sb.Append(Tx.T("expression.errors.distinct variable count exceeded"));
                                break;
                            case SyntaxException.Type.ParenthesesCountMismatch:
                                sb.Append("      \f[-]\f[image:warning]  \f[0xaa6400]\f[I]");
                                sb.Append(Tx.T("expression.errors.parentheses count mismatch"));
                                break;
                            default:
                                throw new InvalidEnumArgumentException("ex.ExceptionType", (int)ex.ExceptionType, typeof(SyntaxException.Type));
                        }

                        item.TextDisplay = sb.ToString();
                    } catch (Exception ex) {
                        item.NumericValueSource = null;

                        item.TextDisplay = "\f[0x8b0000]" + item.Text + "      \f[-]\f[image:warning]  \f[0xaa6400]\f[I]" + ex.Message;
                    }
                }

                item.CheckEnabled = (item.NumericValueSource != null);

                RefreshToolStrip();
            } else if (e.PropertyName == "OperatorCallbacks") {
                ListView.Item item = (ListView.Item)sender;
                ArithmeticExpression arithmeticExpression = item.NumericValueSource as ArithmeticExpression;
                if (arithmeticExpression != null) {
                    arithmeticExpression.Callbacks = item.OperatorCallbacks;

                    OnItemPropertyChanged(item, new PropertyChangedEventArgs("Text"));
                }

                item.Description = item.OperatorCallbacks.ImplementationName;
            }
        }

        private void OnItemSelectionChanged(object sender, EventArgs e)
        {
            RefreshToolStrip();
        }

        private void OnItemBeforeEdit(object sender, CancelEventArgs e)
        {
            ListView.Item item = (ListView.Item)sender;
            if (item.NumericValueSource is MemoryDataSet || item.NumericValueSource is FileDataSet) {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Checks missing callbacks for given arithmetic expression
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="text">String builder to append warings to</param>
        private void CheckCallbacks(ListView.Item item, StringBuilder text)
        {
            if (item?.OperatorCallbacks == null) {
                return;
            }
            ArithmeticExpression ae = item.NumericValueSource as ArithmeticExpression;
            if (ae == null) {
                return;
            }

            bool isFirst = true;
            bool iconAppended = false;

            string[] missingCallbacks = null;

            NativeOperatorCallbacks callbacks = item.OperatorCallbacks as NativeOperatorCallbacks;
            if (callbacks != null) {
                missingCallbacks = callbacks.MissingCallbacks;
                if (missingCallbacks.Length == 0) {
                    return;
                }
            } else {
                NativeOperatorRemotingCallbacks proxyCallbacks = item.OperatorCallbacks as NativeOperatorRemotingCallbacks;
                if (proxyCallbacks != null) {
                    iconAppended = true;
                    text.Append("      \f[-]\f[image:warning]  \f[0xaa6400]\f[I]" + Tx.T("mathlib.remoting"));

                    missingCallbacks = proxyCallbacks.MissingCallbacks;
                    if (missingCallbacks.Length == 0) {
                        text.Append("\f[I]\f[Rc]");
                        return;
                    }
                }
            }

            if (missingCallbacks == null) {
                return;
            }

            for (int i = 0; i < missingCallbacks.Length; i++) {
                string current = missingCallbacks[i];
                if (!ae.IsCallbackUsed(current)) {
                    continue;
                }

                if (isFirst) {
                    isFirst = false;
                    if (iconAppended) {
                        text.Append(" \f[0x999999]|\f[0xaa6400] " + Tx.T("mathlib.missing callbacks"));
                    } else {
                        text.Append("      \f[-]\f[image:warning]  \f[0xaa6400]\f[I]" + Tx.T("mathlib.missing callbacks"));
                    }
                } else {
                    text.Append(", ");
                }

                if (current.StartsWith("constant_", StringComparison.Ordinal)) {
                    text.Append("\f[I]");
                    text.Append(current, 9, current.Length - 9);
                    text.Append("\f[I]");
                } else if (current.StartsWith("operator_", StringComparison.Ordinal)) {
                    text.Append(current, 9, current.Length - 9);
                } else {
                    text.Append(current);
                }
            }

            if (!isFirst) {
                text.Append("\f[I]\f[Rc]");
            }
        }

        /// <summary>
        /// Tries to find random color that is different than any other color in the list
        /// </summary>
        /// <returns>New color</returns>
        private Color FindNewColor()
        {
            int difference = 160;

            while (difference > 5) {
                for (int i = 0; i < 20; i++) {
                    int r = random.Next(20, 200);
                    int g = random.Next(20, 200);
                    int b = random.Next(20, 200);

                    bool allowed = true;
                    foreach (ListView.Item current in listView.Items) {
                        if (Math.Abs((current.Color.R + current.Color.G + current.Color.B) - (r + g + b)) < difference) {
                            allowed = false;
                            break;
                        }
                    }

                    if (allowed) {
                        return Color.FromArgb(r, g, b);
                    }
                }

                if (difference > 100) {
                    difference -= 20;
                } else if (difference > 40) {
                    difference -= 10;
                } else {
                    difference /= 2;
                }
            }

            return Color.Black;
        }

        private static Image OnImageResourceCallback(string resourceName)
        {
            if (resourceName == "warning") {
                return Properties.Resources.Warning;
            } else {
                return null;
            }
        }

        private void OnAddButtonClick(object sender, EventArgs e)
        {
            ListView.Item item = new ListView.Item {
                OperatorCallbacks = defaultDoubleCallbacks,
                Color = FindNewColor(),
                CheckState = CheckState.Checked,
                CheckEnabled = false,
                ImageResourceCallback = OnImageResourceCallback,
                TextDisplay = "\f[0x999999]" + Tx.T("main.enter expression")
            };

            OnItemPropertyChanged(item, new PropertyChangedEventArgs("OperatorCallbacks"));

            listView.Items.Add(item);

            listView.FocusedItem = item;

            listView.Invalidate();
            listView.Update();

            listView.EnsureVisible(item);
        }

        private void OnRemoveButtonClick(object sender, EventArgs e)
        {
            ListView.Item selected = listView.FocusedItem;
            if (selected != null) {
                int index = selected.Index;

                listView.Items.Remove(selected);

                if (listView.Items.Count > 0) {
                    listView.FocusedItem = listView.Items[Math.Max(0, index - 1)];
                }

                IDisposable sourceDisposable = selected.NumericValueSource as IDisposable;
                if (sourceDisposable != null) {
                    sourceDisposable.Dispose();
                }
            }
        }

        private void OnDuplicateButtonClick(object sender, EventArgs e)
        {
            ListView.Item selected = listView.FocusedItem;
            if (selected != null && selected.NumericValueSource is ArithmeticExpression) {
                ArithmeticExpression ae = ArithmeticExpression.Parse(selected.Text);
                ae.Callbacks = selected.OperatorCallbacks;

                ListView.Item item = new ListView.Item {
                    OperatorCallbacks = selected.OperatorCallbacks,
                    Color = FindNewColor(),
                    CheckState = selected.CheckState,
                    CheckEnabled = selected.CheckEnabled,
                    ImageResourceCallback = OnImageResourceCallback,

                    Description = selected.Description,
                    NumericValueSource = ae,
                    Text = selected.Text,
                    TextDisplay = selected.TextDisplay
                };

                listView.Items.Add(item);

                listView.FocusedItem = item;

                listView.Invalidate();
                listView.Update();

                listView.EnsureVisible(item);
            }
        }

        private void OnSetItemCallbacksButtonClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) {
                return;
            }

            ListView.Item selected = listView.FocusedItem;
            if (selected != null && !(selected.NumericValueSource is MemoryDataSet) && !(selected.NumericValueSource is FileDataSet)) {
                selected.OperatorCallbacks = item.Tag as OperatorCallbacks;
            }
        }

        private void OnImplementationAddButtonClick(object sender, EventArgs e)
        {
            ListView.Item selected = listView.FocusedItem;
            if (selected == null) {
                return;
            }

            using (OpenFileDialog dialog = new OpenFileDialog()) {
                dialog.CheckFileExists = true;
                dialog.Filter = Tx.T("mathlib.filetype") + " (*.dll)|*.dll";
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.Title = Tx.T("mathlib.load");

                if (dialog.ShowDialog(this) != DialogResult.OK) {
                    return;
                }

                string filename = dialog.FileName;

                try {
                    NativeOperatorCallbacks callbacks = new NativeOperatorCallbacks(filename);

                    ToolStripMenuItem item = new ToolStripMenuItem(callbacks.ImplementationName);
                    item.ShortcutKeyDisplayString = "(" + Path.GetFileName(filename) + ")";
                    item.Click += OnSetItemCallbacksButtonClick;
                    item.Tag = callbacks;

                    string missingCallbacks = ConvertMissingCallbacksToString(callbacks);
                    if (!string.IsNullOrEmpty(missingCallbacks)) {
                        item.ToolTipText = Tx.T("mathlib.missing callbacks") + missingCallbacks;
                    }

                    implementationButton.DropDownItems.Add(item);

                    OnSetItemCallbacksButtonClick(item, EventArgs.Empty);
                } catch (BadImageFormatException) {
                    if (Environment.Is64BitProcess) {
                        try {
                            NativeOperatorRemotingCallbacks callbacks = new NativeOperatorRemotingCallbacks(filename);

                            ToolStripMenuItem item = new ToolStripMenuItem(callbacks.ImplementationName);
                            item.ShortcutKeyDisplayString = "(x86, " + Path.GetFileName(filename) + ")";
                            item.Click += OnSetItemCallbacksButtonClick;
                            item.Tag = callbacks;

                            string missingCallbacks = ConvertMissingCallbacksToString(callbacks);
                            if (!string.IsNullOrEmpty(missingCallbacks)) {
                                item.ToolTipText = Tx.T("mathlib.missing callbacks") + missingCallbacks;
                            }

                            implementationButton.DropDownItems.Add(item);

                            OnSetItemCallbacksButtonClick(item, EventArgs.Empty);
                        } catch (InvalidOperationException) {
                            MessageBox.Show(this, Tx.T("mathlib.errors.not valid"), Tx.T("mathlib.errors.title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } catch (Exception ex) {
                            MessageBox.Show(this, ex.Message, Tx.T("mathlib.errors.title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } else {
                        MessageBox.Show(this, Tx.T("mathlib.errors.platform mismatch"), Tx.T("mathlib.errors.title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (InvalidOperationException) {
                    MessageBox.Show(this, Tx.T("mathlib.errors.not valid"), Tx.T("mathlib.errors.title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                } catch (Exception ex) {
                    MessageBox.Show(this, ex.Message, Tx.T("mathlib.errors.title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnScaleFactorChanged()
        {
            scaleFactorBox.Text = (graph.ScaleFactor * 100).ToString("#,##0", CultureInfo.CurrentCulture) + " %";
        }

        private void OnZoomOutButtonClick(object sender, EventArgs e)
        {
            graph.ZoomOutCenter();
        }

        private void OnZoomInButtonClick(object sender, EventArgs e)
        {
            graph.ZoomInCenter();
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            graph.ResetView();
        }

        private void OnHighlightDifferencesButtonClick(object sender, EventArgs e)
        {
            ((ToolStripSplitButton)sender).ShowDropDown();
        }

        private void OnHighlightOffButtonClick(object sender, EventArgs e)
        {
            graph.HighlightDifferences = Graph.HighlightDifferencesMode.None;

            foreach (ToolStripMenuItem item in highlightDifferencesButton.DropDownItems) {
                item.Checked = false;
            }

            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void OnHighlightConstantButtonClick(object sender, EventArgs e)
        {
            graph.HighlightDifferences = Graph.HighlightDifferencesMode.ConstantHighlight;

            foreach (ToolStripMenuItem item in highlightDifferencesButton.DropDownItems) {
                item.Checked = false;
            }

            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void OnHighlightDynamicButtonClick(object sender, EventArgs e)
        {
            graph.HighlightDifferences = Graph.HighlightDifferencesMode.DynamicHighlight;

            foreach (ToolStripMenuItem item in highlightDifferencesButton.DropDownItems) {
                item.Checked = false;
            }

            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void OnAverageButtonClick(object sender, EventArgs e)
        {
            graph.HighlightDifferences = Graph.HighlightDifferencesMode.Average;

            foreach (ToolStripMenuItem item in highlightDifferencesButton.DropDownItems) {
                item.Checked = false;
            }

            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void OnImplementationButtonClick(object sender, EventArgs e)
        {
            ((ToolStripSplitButton)sender).ShowDropDown();
        }

        private void OnScaleFactorBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                string input = scaleFactorBox.Text;

                // Filter non-digit characters
                StringBuilder sb = new StringBuilder(input.Length);
                for (int i = 0; i < input.Length; i++) {
                    if (char.IsDigit(input[i])) {
                        sb.Append(input[i]);
                    }
                }

                // Try to parse input and change scale factor
                double scale;
                if (double.TryParse(sb.ToString(), out scale)) {
                    scale /= 100;

                    if (scale >= Graph.MinScaleFactor) {
                        graph.ScaleFactor = scale;
                    }
                }

                e.SuppressKeyPress = true;
            }
        }

        private void OnImportButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog()) {
                const string ext = ".fvis-values";

                dialog.CheckFileExists = true;
                dialog.Filter = Tx.T("dataset.filetype") + " (*" + ext + ")|*" + ext;
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.Title = Tx.T("dataset.load");

                if (dialog.ShowDialog(this) != DialogResult.OK) {
                    return;
                }

                string filename = dialog.FileName;
                using (FileStream s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader br = new BinaryReader(s)) {
                    byte[] header = br.ReadBytes(4);
                    if (header[0] != 'f' || header[1] != 'V' || header[2] != 'i' || header[3] != 's') {
                        MessageBox.Show(this, Tx.T("dataset.errors.file not valid"), Tx.T("main.error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int flags = br.ReadInt32();
                    string description = br.ReadString();

                    long axisX = br.ReadInt64();
                    long axisY = br.ReadInt64();
                    double scaleFactor = br.ReadDouble();

                    ulong count = br.ReadUInt64();
                    if (count <= 0) {
                        MessageBox.Show(this, Tx.T("dataset.errors.file not valid"), Tx.T("main.error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ulong offset = (ulong)s.Position;

                    // Close stream and open the same file in memory-mapped mode
                    s.Close();

                    FileDataSet fileDataSet = new FileDataSet(filename, offset, count);

                    // Add item to ListView
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\f[I]\f[0x888888]" + Tx.T("dataset.part of function") + "  \f[Rc]\f[I]");

                    int i = description.IndexOf('#');
                    if (i != -1) {
                        while (i > 0 && description[i - 1] == ' ') {
                            i--;
                        }

                        sb.Append(description, 0, i);
                        sb.Append("\f[0x498E3E]");
                        sb.Append(description, i, description.Length - i);
                    } else {
                        sb.Append(description);
                    }

                    ListView.Item item = new ListView.Item {
                        NumericValueSource = fileDataSet,
                        Text = description,
                        TextDisplay = sb.ToString(),
                        Color = FindNewColor(),
                        CheckState = CheckState.Checked,
                        CheckEnabled = true,
                        ImageResourceCallback = OnImageResourceCallback
                    };

                    listView.Items.Add(item);

                    listView.FocusedItem = item;

                    listView.Invalidate();
                    listView.Update();

                    listView.EnsureVisible(item);

                    // Zoom to values in graph
                    graph.ScaleFactor = scaleFactor;
                    graph.AxisX = axisX;
                    graph.AxisY = axisY;
                }
            }
        }

        private unsafe void OnExportSelectedButtonClick(object sender, EventArgs e)
        {
            ListView.Item selected = listView.FocusedItem;
            if (selected == null) {
                // Nothing to export...
                return;
            }

            ArithmeticExpression ae = selected.NumericValueSource as ArithmeticExpression;
            if (ae == null) {
                MessageBox.Show(this, Tx.T("dataset.errors.item not supported"), Tx.T("main.error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long xFi = 0, xLi = 0, distance = 0;

            if (ae.VariableName != null) {
                double xF_ = graph.MinVisibleX;
                double xL_ = graph.MaxVisibleX;

                if (xF_ >= xL_) {
                    MessageBox.Show(this, Tx.T("dataset.errors.cannot determine interval"), Tx.T("main.error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (Math.Sign(xF_) != Math.Sign(xL_)) {
                    MessageBox.Show(this, Tx.T("dataset.errors.interval with zero"), Tx.T("main.error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Swap first and last for negative "x" values
                xFi = *(long*)&xF_;
                xLi = *(long*)&xL_;
                if (xFi > xLi) {
                    long swap = xLi;
                    xLi = xFi;
                    xFi = swap;
                }
                distance = (xLi - xFi);

                if (distance < 0) {
                    MessageBox.Show(this, Tx.T("dataset.errors.interval too big"), Tx.T("main.error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (distance > 10 * 1000 * 1000) {
                    long sizeInMB = (distance * 8 * 2 / 1024 / 1024);
                    string sizeString;
                    if (sizeInMB > 100L * 1024L) {
                        sizeString = Tx.N(sizeInMB / 1024) + " GB";
                    } else {
                        sizeString = Tx.N(sizeInMB) + " MB";
                    }

                    if (MessageBox.Show(this, Tx.T("dataset.errors.file too big", Tx.N(distance), sizeString), Tx.T("main.warning"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) {
                        return;
                    }
                }
            }

            using (SaveFileDialog dialog = new SaveFileDialog()) {
                const string ext = ".fvis-values";

                dialog.Filter = Tx.T("dataset.filetype") + " (*" + ext + ")|*" + ext;
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.Title = Tx.T("dataset.save");

                string filename = selected.Text;
                char[] invalidChars = Path.GetInvalidFileNameChars();
                for (int i = 0; i < invalidChars.Length; i++) {
                    filename = filename.Replace(invalidChars[i], '_');
                }

                dialog.FileName = filename + ext;

                if (dialog.ShowDialog(this) != DialogResult.OK) {
                    return;
                }

                filename = dialog.FileName;

                ProgressDialog progressDialog = new ProgressDialog();
                progressDialog.Text = Tx.T("dataset.saving.title");
                progressDialog.MainInstruction = Tx.T("dataset.saving.description");
                progressDialog.Line1 = Tx.T("dataset.saving.progress", "0", Tx.N(xLi - xFi));
                progressDialog.Show(this);

                ThreadPool.UnsafeQueueUserWorkItem(delegate {
                    using (FileStream s = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (BinaryWriter bw = new BinaryWriter(s)) {
                        bw.Write(new[] { (byte)'f', (byte)'V', (byte)'i', (byte)'s' });

                        const int flags = 0;
                        bw.Write(flags);

                        string description = selected.Text;
                        bw.Write(description);

                        // Save graph viewport settings
                        long axisX = graph.AxisX;
                        long axisY = graph.AxisY;
                        double scaleFactor = graph.ScaleFactor;

                        bw.Write(axisX);
                        bw.Write(axisY);
                        bw.Write(scaleFactor);

                        if (ae.VariableName == null) {
                            // Constant function, save only one value
                            double y = ae.Evaluate(double.NaN);

                            const long count = 2;
                            bw.Write(count);

                            bw.Write(double.MinValue);
                            bw.Write(y);
                            bw.Write(double.MaxValue);
                            bw.Write(y);
                        } else {
                            bw.Write(distance + 1);

                            Stopwatch sw = Stopwatch.StartNew();
                            long lastProcessed = xFi;

                            for (long xi = xFi; xi <= xLi; xi++) {
                                double x = *(double*)&xi;
                                double y = ae.Evaluate(x);

                                bw.Write(x);
                                bw.Write(y);

                                if (sw.ElapsedMilliseconds > 4000) {
                                    if (progressDialog.IsCancelled) {
                                        break;
                                    }

                                    sw.Stop();

                                    long delta = (xi - lastProcessed);
                                    long rate = delta * 1000 / (long)sw.Elapsed.TotalMilliseconds;
                                    TimeSpan remaining = TimeSpan.FromSeconds((xLi - xi) / rate);

                                    lastProcessed = xi;

                                    BeginInvoke((MethodInvoker)delegate {
                                        progressDialog.Line1 = Tx.T("dataset.saving.progress", Tx.N(lastProcessed - xFi), Tx.N(xLi - xFi));
                                        progressDialog.Line2 = Tx.T("main.remaining time", remaining.ToTextString());
                                        progressDialog.Progress = (int)((lastProcessed - xFi) * 100 / (xLi - xFi));
                                    });

                                    sw.Restart();
                                }
                            }

                            sw.Stop();
                        }
                    }

                    if (progressDialog.IsCancelled) {
                        try {
                            File.Delete(filename);
                        } catch {
                            // Nothing to do...
                        }
                    }

                    BeginInvoke((MethodInvoker)delegate {
                        progressDialog.TaskCompleted();
                    });
                }, null);
            }
        }

        private void OnAnalyzeButtonClick(object sender, EventArgs e)
        {
            using (FindDifferencesDialog dialog = new FindDifferencesDialog(graph, listView.Items, graph.MinVisibleX, graph.MaxVisibleX)) {
                dialog.ShowDialog(this);
            }
        }

        private void OnZoomToValueButtonClick(object sender, EventArgs e)
        {
            using (ZoomToValueDialog dialog =
                new ZoomToValueDialog(graph, listView.Items, (graph.MinVisibleX + graph.MaxVisibleX) * 0.5)) {
                dialog.ShowDialog(this);
            }
        }

        public void OnLanguageMenuItemClick(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) {
                return;
            }

            foreach (ToolStripMenuItem current in langMenuItem.DropDownItems) {
                current.Checked = false;
            }
            item.Checked = true;

            Tx.SetCulture(item.Tag as string);
        }

        private void OnAboutMenuItemClick(object sender, EventArgs e)
        {
            using (AboutDialog dialog = new AboutDialog()) {
                dialog.ShowDialog(this);
            }
        }
    }
}