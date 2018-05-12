using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using fVis.Controls;
using fVis.Extensions;
using Unclassified.TxLib;
using ListView = fVis.Controls.ListView;

namespace fVis.Windows
{
    internal partial class ZoomToValueDialog : Form
    {
        private readonly Graph graph;

        public ZoomToValueDialog(Graph graph, IList<ListView.Item> items, double value)
        {
            InitializeComponent();

            TxWinForms.Bind(this);

            this.graph = graph;

            mainInstructionLabel.Font = new Font(mainInstructionLabel.Font, FontStyle.Bold);

            listView.EmptyText = Tx.T("main.list is empty");

            Font monospacedFont = new Font("Consolas", 8f);
            valueTextBox.Font = monospacedFont;

            valueTextBox.Text = value.ToExactString();
            valueTextBox.SelectionStart = 0;

            foreach (var item in items) {
                listView.Items.Add(item);
            }

            RefreshUI();
        }

        private void OnFindButtonClick(object sender, EventArgs e)
        {
            ListView.Item selection = listView.SelectedItem;
            if (selection?.NumericValueSource != null) {
                double x;
                if (!double.TryParse(valueTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out x) || x == 0) {
                    return;
                }

                double y = selection.NumericValueSource.Evaluate(x);
                graph.ZoomToValue(x, y);

                Close();
            }
        }

        private void OnItemSelectionChanged(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshUI();
        }

        private void OnValueTextBoxTextChanged(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            ListView.Item selection = listView.SelectedItem;
            double value;
            findButton.Enabled = (selection?.NumericValueSource != null &&
                !string.IsNullOrWhiteSpace(valueTextBox.Text) &&
                double.TryParse(valueTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out value) &&
                value != 0);
        }
    }
}