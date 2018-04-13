using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using fVis.Callbacks;
using fVis.Controls;
using fVis.Extensions;
using fVis.NumericValueSources;
using ListView = fVis.Controls.ListView;

namespace fVis.Windows
{
    /// <summary>
    /// Represents "Find Differences" dialog
    /// </summary>
    internal partial class FindDifferencesDialog : Form
    {
        private const int Threshold = 250000;   // 250k

        private readonly Graph graph;
        private TimeSpan estimatedTime;
        private long distance;

        private System.Windows.Forms.Timer refreshTimer;
        private bool refreshNeeded;

        public FindDifferencesDialog(Graph graph, IList<ListView.Item> items, double start, double end)
        {
            InitializeComponent();

            this.graph = graph;

            mainInstructionLabel.Font = new Font(mainInstructionLabel.Font, FontStyle.Bold);

            Font monospacedFont = new Font("Consolas", 8f);
            startTextBox.Font = monospacedFont;
            endTextBox.Font = monospacedFont;

            startTextBox.Text = start.ToExactString();
            endTextBox.Text = end.ToExactString();

            startTextBox.SelectionStart = 0;
            endTextBox.SelectionStart = 0;

            foreach (var item in items) {
                listView.Items.Add(item);
            }

            EstimateTime(true);

            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Tick += OnRefreshTimer;
            refreshTimer.Interval = 4000;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            refreshTimer.Enabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            refreshTimer.Enabled = false;

            base.OnClosed(e);
        }

        private void OnRefreshTimer(object sender, EventArgs e)
        {
            if (!refreshNeeded) {
                return;
            }

            EstimateTime(true);
        }

        /// <summary>
        /// Run time estamination process
        /// </summary>
        /// <param name="silent">Suppress warning messages</param>
        /// <returns>Returns true if input data are correct; false, otherwise</returns>
        private unsafe bool EstimateTime(bool silent)
        {
            refreshNeeded = false;

            int progressRefreshRate = 0;

            int count = 0;
            foreach (ListView.Item item in listView.Items) {
                if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                    continue;

                count++;

                ArithmeticExpression ae = item.NumericValueSource as ArithmeticExpression;
                if (ae?.Callbacks is NativeOperatorRemotingCallbacks) {
                    // Compute fewer samples with x86 Remoting
                    progressRefreshRate += 3000;
                } else {
                    progressRefreshRate += 1;
                }
            }

            if (count < 2) {
                infoLabel.Text = "";
                if (!silent) {
                    MessageBox.Show(this, "Musíte vybrat alespoň dvě matematické funkce.", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            double xF_, xL_;
            if (!double.TryParse(startTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out xF_)) {
                infoLabel.Text = "";
                if (!silent) {
                    MessageBox.Show(this, "Hodnotu začátku intervalu nelze převést na číslo.\r\nZkontrolujte, že číslo neobsahuje mezery a je použita desetinná tečka.", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            if (!double.TryParse(endTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out xL_)) {
                infoLabel.Text = "";
                if (!silent) {
                    MessageBox.Show(this, "Hodnotu konce intervalu nelze převést na číslo.\r\nZkontrolujte, že číslo neobsahuje mezery a je použita desetinná tečka.", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            if (xF_ >= xL_) {
                infoLabel.Text = "";
                if (!silent) {
                    MessageBox.Show(this, "Chyba při stanovování intervalu pro vyhledávání.", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            if (Math.Sign(xF_) != Math.Sign(xL_)) {
                infoLabel.Text = "";
                if (!silent) {
                    MessageBox.Show(this, "Nelze vyhledat rozdíly v intervalu procházející nulou.\r\nObsahuje příliš mnoho hodnot.", "Chyba",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }

            // Swap first and last for negative "x" values
            long xFi = *(long*)&xF_;
            long xLi = *(long*)&xL_;
            if (xFi > xLi) {
                long swap = xLi;
                xLi = xFi;
                xFi = swap;
            }
            distance = (xLi - xFi);

            int thresholdPerItem = (Threshold / progressRefreshRate);
            if (distance < thresholdPerItem) {
                estimatedTime = TimeSpan.MinValue;
                infoLabel.Text = "Odhadovaný čas: Okamžitě | Počet hodnot: " + distance.ToString("N0");
                return true;
            }

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < thresholdPerItem; i++) {
                long xi = *(long*)&xF_ + i;
                double x = *(double*)&xi;

                bool isFirst = true;
                double computedMinY = double.MaxValue, computedMaxY = double.MinValue;

                foreach (ListView.Item item in listView.Items) {
                    if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                        continue;

                    double y = item.NumericValueSource.Evaluate(x);

                    if (isFirst) {
                        isFirst = false;
                        computedMinY = computedMaxY = y;
                    } else {
                        if (y > computedMaxY) {
                            computedMaxY = y;
                        } else if (y < computedMinY) {
                            computedMinY = y;
                        }
                    }
                }
            }

            sw.Stop();

            const int MultithreadPenalty = 4;

            int threadCount = Math.Max(Environment.ProcessorCount, 1);
            ulong secs = ((ulong)(((sw.ElapsedMilliseconds * distance / 1000) / thresholdPerItem) / threadCount) * MultithreadPenalty);
            if (secs > 1000L * 365L * 24L * 60L * 60L) { // 1000 years in seconds
                estimatedTime = TimeSpan.MaxValue;
                infoLabel.Text = "Odhadovaný čas: Více než 1000 let | Počet hodnot: " + distance.ToString("N0");
            } else {
                estimatedTime = TimeSpan.FromSeconds(secs);
                infoLabel.Text = "Odhadovaný čas: " + estimatedTime.ToTextString() + " | Počet hodnot: " + distance.ToString("N0");
            }

            return true;
        }

        private unsafe void OnRunButtonClick(object sender, EventArgs e)
        {
            if (!EstimateTime(false)) {
                return;
            }

            double start, end;
            double.TryParse(startTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out start);
            double.TryParse(endTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out end);

            if (estimatedTime.TotalMinutes > 0.5) {
                string timeString;
                if (estimatedTime == TimeSpan.MaxValue) {
                    timeString = "Více než 1000 let";
                } else {
                    timeString = estimatedTime.ToTextString();
                }

                if (MessageBox.Show(this, "Počet hodnot v intervalu: " + distance.ToString("N0") + "\r\nNalezení rozdílů v tomto intervalu pravděpodobně bude trvat:\r\n" + timeString + "\r\n\r\nOpravdu si přejete prohledat zadaný interval?", "Varování",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) {
                    return;
                }
            }

            long starti = *(long*)&start;
            long endi = *(long*)&end;
            if (starti > endi) {
                long swap = starti;
                starti = endi;
                endi = swap;
            }
            distance = (endi - starti);

            long currentProcessed = 0, lastProcessed = 0;
            long differencesFound = 0;

            SpinLock syncLock = new SpinLock();

            ProgressDialog progressDialog = new ProgressDialog {
                Text = "Vyhledávání rozdílů",
                MainInstruction = "Vyhledávání rozdílů",
                Line1 = "Počet zpracovaných hodnot: " + "0" + " / " + distance.ToString("N0"),
                ShowInTaskbar = false,
                MinimizeBox = false
            };

            ThreadPool.UnsafeQueueUserWorkItem(delegate {
                BeginInvoke((MethodInvoker)delegate {
                    progressDialog.ShowDialog(this);
                });

                Stopwatch sw = Stopwatch.StartNew();

                Parallel.For(starti, endi, (xi, state) => {
                    double x = *(double*)&xi;

                    bool isFirst = true;
                    double computedMinY = double.MaxValue, computedMaxY = double.MinValue;

                    foreach (ListView.Item item in listView.Items) {
                        if (item.NumericValueSource == null || item.CheckState != CheckState.Checked)
                            continue;

                        double y = item.NumericValueSource.Evaluate(x);

                        if (isFirst) {
                            isFirst = false;
                            computedMinY = computedMaxY = y;
                        } else {
                            if (y > computedMaxY) {
                                computedMaxY = y;
                            } else if (y < computedMinY) {
                                computedMinY = y;
                            }
                        }
                    }

                    bool lockTaken = false;
                    syncLock.Enter(ref lockTaken);

                    if (!isFirst && computedMinY != computedMaxY) {
                        graph.AddDifferenceUnsafe(x, computedMinY, computedMaxY);
                        differencesFound++;
                    }

                    if (sw.ElapsedMilliseconds > 4000) {
                        if (progressDialog.IsCancelled) {
                            if (lockTaken) {
                                syncLock.Exit();
                            }
                            state.Stop();
                            return;
                        }

                        sw.Stop();

                        long delta = (currentProcessed - lastProcessed);
                        long rate = delta * 1000 / (long)sw.Elapsed.TotalMilliseconds;
                        TimeSpan remaining = TimeSpan.FromSeconds((distance - currentProcessed) / rate);

                        lastProcessed = currentProcessed;

                        BeginInvoke((MethodInvoker)delegate {
                            progressDialog.Line1 = "Počet zpracovaných hodnot: " + lastProcessed.ToString("N0") + " / " + distance.ToString("N0");
                            progressDialog.Line2 = "Zbývá přibližně " + remaining.ToTextString() + ".";
                            progressDialog.Progress = (int)(lastProcessed * 100 / distance);
                        });

                        sw.Restart();
                    }

                    currentProcessed++;

                    if (lockTaken) {
                        syncLock.Exit();
                    }
                });

                sw.Stop();

                BeginInvoke((MethodInvoker)delegate {
                    progressDialog.TaskCompleted();

                    if (!progressDialog.IsCancelled) {
                        MessageBox.Show(this,
                            "Prohledávání bylo dokončeno.\r\n\r\nPočet hodnot v intervalu: " +
                            distance.ToString("N0") +
                            "\r\nPočet nalezených rozdílů: " + differencesFound.ToString("N0") + " (" +
                            (differencesFound * 100 / distance).ToString("N0") + " %)", "Prohledávání dokončeno",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                });

            }, null);
        }

        private void OnStartTextBoxTextChanged(object sender, EventArgs e)
        {
            refreshNeeded = true;
        }

        private void OnEndTextBoxTextChanged(object sender, EventArgs e)
        {
            refreshNeeded = true;
        }
    }
}