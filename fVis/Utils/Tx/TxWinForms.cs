// TxLib – Tx Translation & Localisation for .NET and WPF
// © Yves Goergen, Made in Germany
// Website: http://unclassified.software/source/txtranslation
//
// © 2018 Dan R.
//
// This library is free software: you can redistribute it and/or modify it under the terms of
// the GNU Lesser General Public License as published by the Free Software Foundation, version 3.
//
// This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with this
// library. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Unclassified.TxLib
{
    /// <summary>
    /// Provides data binding of Windows Forms controls to translated texts.
    /// </summary>
    public class TxDictionaryBinding
    {
        public enum BindingType
        {
            Text,
            ToolTip,
        }

        private struct BindingKey : IEquatable<BindingKey>
        {
            public object Target;
            public BindingType Type;

            public BindingKey(object target, BindingType type)
            {
                this.Target = target;
                this.Type = type;
            }

            public override bool Equals(object obj)
            {
                return obj is BindingKey && Equals((BindingKey)obj);
            }

            public bool Equals(BindingKey other)
            {
                return Target == other.Target &&
                       Type == other.Type;
            }

            public override int GetHashCode()
            {
                var hashCode = -676658667;
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Target);
                hashCode = hashCode * -1521134295 + Type.GetHashCode();
                return hashCode;
            }
        }

        private static SynchronizationContext context;
        private static Dictionary<BindingKey, string> controlBindings = new Dictionary<BindingKey, string>();
        private static HashSet<IDisposable> controls = new HashSet<IDisposable>();

        /// <summary>
        /// Adds or replaces text dictionary bindings for the Text property of a control and all
        /// its child controls.
        /// </summary>
        /// <param name="control">Control or Form to start with.</param>
        public static void AddTextBindings(Control control)
        {
            // Check whether the control's Text property has a value that looks like a text key,
            // then add the binding for it
            if (control.Text != null && control.Text.Length > 2 && control.Text.StartsWith("[") && control.Text.EndsWith("]")) {
                string key = control.Text.Substring(1, control.Text.Length - 2);
                AddBinding(control, BindingType.Text, key);
            }

            // Recurse to all child controls
            foreach (Control child in control.Controls) {
                AddTextBindings(child);
            }

            ToolStrip toolStrip = control as ToolStrip;
            if (toolStrip != null) {
                AddToolStripBindings(toolStrip.Items);
            }
        }

        private static void AddToolStripBindings(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items) {
                if (item.Text != null && item.Text.Length > 2 && item.Text.StartsWith("[") && item.Text.EndsWith("]")) {
                    string key = item.Text.Substring(1, item.Text.Length - 2);
                    AddBinding(item, BindingType.Text, key);
                }

                if (item.ToolTipText != null && item.ToolTipText.Length > 2 && item.ToolTipText.StartsWith("[") && item.ToolTipText.EndsWith("]")) {
                    string key = item.ToolTipText.Substring(1, item.ToolTipText.Length - 2);
                    AddBinding(item, BindingType.ToolTip, key);
                }

                ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
                if (dropDown != null) {
                    AddToolStripBindings(dropDown.DropDownItems);
                }
            }
        }

        public static void AddBinding(Control control, BindingType type, string key)
        {
            lock (controlBindings) {
                TryInitializeBindings();

                switch (type) {
                    case BindingType.Text: control.Text = Tx.T(key); break;
                    default: throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(BindingType));
                }

                controlBindings.Add(new BindingKey(control, type), key);

                if (controls.Add(control)) {
                    control.Disposed += OnControlDisposed;
                }
            }
        }

        public static void AddBinding(ToolStripItem item, BindingType type, string key)
        {
            lock (controlBindings) {
                TryInitializeBindings();

                switch (type) {
                    case BindingType.Text: item.Text = Tx.T(key); break;
                    case BindingType.ToolTip: item.ToolTipText = Tx.T(key); break;
                    default: throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(BindingType));
                }

                controlBindings.Add(new BindingKey(item, type), key);

                if (controls.Add(item)) {
                    item.Disposed += OnControlDisposed;
                }
            }
        }

        private static void OnControlDisposed(object sender, EventArgs args)
        {
            switch (sender) {
                case Control control: {
                    controlBindings.Remove(new BindingKey(control, BindingType.Text));
                    break;
                }
                case ToolStripItem item: {
                    controlBindings.Remove(new BindingKey(item, BindingType.Text));
                    controlBindings.Remove(new BindingKey(item, BindingType.ToolTip));
                    break;
                }
            }

            TryDisposeBindings();
        }

        private static void TryInitializeBindings()
        {
            if (controlBindings.Count > 0 || context != null) {
                return;
            }

            context = SynchronizationContext.Current;
            if (context == null) {
                throw new InvalidOperationException("SynchronizationContext is not available. The DictionaryBinding instance must be created on the UI thread.");
            }

            Tx.DictionaryChanged += OnTxDictionaryChanged;
        }

        private static void TryDisposeBindings()
        {
            if (controlBindings.Count > 0) {
                return;
            }

            context = null;

            Tx.DictionaryChanged -= OnTxDictionaryChanged;
        }

        private static void OnTxDictionaryChanged(object sender, EventArgs args)
        {
            context.Post(delegate {
                lock (controlBindings) {
                    foreach (var binding in controlBindings) {
                        switch (binding.Key.Target) {
                            case Control control: {
                                switch (binding.Key.Type) {
                                    case BindingType.Text: control.Text = Tx.T(binding.Value); break;
                                }
                                break;
                            }
                            case ToolStripItem item: {
                                switch (binding.Key.Type) {
                                    case BindingType.Text: item.Text = Tx.T(binding.Value); break;
                                    case BindingType.ToolTip: item.ToolTipText = Tx.T(binding.Value); break;
                                }
                                break;
                            }
                        }

                    }
                }
            }, null);
        }
    }
}