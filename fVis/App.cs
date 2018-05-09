﻿using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace fVis
{
    public static class App
    {
        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0) {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title)) {
                        return titleAttribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length > 0) {
                    AssemblyCopyrightAttribute titleAttribute = (AssemblyCopyrightAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Copyright)) {
                        return titleAttribute.Copyright;
                    }
                }
                return "";
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Windows.MainWindow());
        }
    }
}