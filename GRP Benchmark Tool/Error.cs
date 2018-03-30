using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GRP_Benchmark_Tool
{
    public static class Error
    {
        public static void ErrorBox(Exception ex, string customMessage = "", bool fatal = false)
        {
            MessageBox.Show(customMessage + "\n\n" + ex.Message + "\n\n" + ex.StackTrace, fatal ? "Fatal error!" : "Non-fatal error!", MessageBoxButton.OK, MessageBoxImage.Error);
            if (fatal)
                Environment.Exit(69);
        }

        public static void ErrorBox(string message, bool fatal = false)
        {
            MessageBox.Show(message, fatal ? "Fatal error!" : "Non-fatal error!", MessageBoxButton.OK, MessageBoxImage.Error);
            if (fatal)
                Environment.Exit(420);
        }
    }
}
