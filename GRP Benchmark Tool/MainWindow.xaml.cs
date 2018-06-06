using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace GRP_Benchmark_Tool
{
    public partial class MainWindow : Window
    {
        static DependencyProperty ConfigFileProperty =
            DependencyProperty.Register("ConfigFile", typeof(ConfigFile), typeof(MainWindow));
        static DependencyProperty selectedIndexProperty =
            DependencyProperty.Register("SelectedPath", typeof(int), typeof(MainWindow));
        static DependencyProperty selectedBenchmarkProperty =
            DependencyProperty.Register("SelectedBenchmark", typeof(int), typeof(MainWindow));
        static DependencyProperty targetBenchmarkProperty =
            DependencyProperty.Register("TargetBenchmark", typeof(int), typeof(MainWindow));
        static DependencyProperty currentDirectoryProperty =
            DependencyProperty.Register("CurrentDirectory", typeof(string), typeof(MainWindow));
        static DependencyProperty isReadyProperty =
            DependencyProperty.Register("IsReady", typeof(bool), typeof(MainWindow));
        static DependencyProperty doModifyProperty =
            DependencyProperty.Register("DoModify", typeof(bool), typeof(MainWindow));

        public ConfigFile CurrConfigFile
        {
            get { return (ConfigFile)GetValue(ConfigFileProperty); }
            set { SetValue(ConfigFileProperty, value); }
        }

        public int SelectedPathIndex
        {
            get { return (int)GetValue(selectedIndexProperty); }
            set { SetValue(selectedIndexProperty, value); }
        }

        public int SelectedBenchmarkIndex
        {
            get { return (int)GetValue(selectedBenchmarkProperty); }
            set { SetValue(selectedBenchmarkProperty, value); }
        }

        public int SelectedTargetBenchmarkIndex
        {
            get { return (int)GetValue(targetBenchmarkProperty); }
            set { SetValue(targetBenchmarkProperty, value); }
        }

        public string CurrentPath
        {
            get { return CurrConfigFile.GamePaths[SelectedPathIndex]; }
        }

        public string WorkingDirectory
        {
            get { return (string)GetValue(currentDirectoryProperty); }
            set { SetValue(currentDirectoryProperty, value); }
        }

        public bool IsReady
        {
            get { return (bool)GetValue(isReadyProperty); }
            set { SetValue(isReadyProperty, value); }
        }

        public bool DoModify
        {
            get { return (bool)GetValue(doModifyProperty); }
            set { SetValue(doModifyProperty, value); }
        }

        BackgroundWorker bgworker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            IsReady = true;

            WorkingDirectory = Environment.CurrentDirectory;

            LoadConfig();

            bgworker.DoWork += Bgworker_DoWork;
        }

        private void Bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            Action<bool> action = (val) => IsReady = val;
            Application.Current.Dispatcher.BeginInvoke(action, false);
            System.Threading.Thread.Sleep(1500);
            Application.Current.Dispatcher.BeginInvoke(action, true);
        }

        void TriggerWorker()
        {
            if (bgworker.IsBusy)
                return;
            bgworker.RunWorkerAsync();
        }

        void LoadConfig()
        {
            ConfigFile cfgFile = new ConfigFile();
            cfgFile.LoadFile();
            CurrConfigFile = cfgFile;
            SelectedPathIndex = 0;
            SelectedBenchmarkIndex = 0;
        }

        void SaveConfig()
        {
            Environment.CurrentDirectory = WorkingDirectory; //apparently OpenFileDialog changes working directory
            CurrConfigFile.SaveFile();
            LoadConfig();
        }

        private void btnReloadConfig_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();

            TriggerWorker();
        }

        private void btnResetBigfile_Click(object sender, RoutedEventArgs e)
        {
            foreach (BenchmarkOffset benchOffset in CurrConfigFile.BenchmarkOffsets)
            {
                BigfileModifier.ReplaceBenchmark(CurrentPath, benchOffset, benchOffset);
            }
            if (!CurrConfigFile.IsUnlocked)
            {
                BigfileModifier.ReplaceBenchmark(CurrentPath, CurrConfigFile.LockedTargetOffset, CurrConfigFile.LockedTargetOffset);
            }
            TriggerWorker();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            bool didModify = true;
            if (DoModify)
            {
                if (CurrConfigFile.IsUnlocked)
                {
                    didModify = BigfileModifier.ReplaceBenchmark(CurrentPath, CurrConfigFile.BenchmarkOffsets[SelectedTargetBenchmarkIndex], CurrConfigFile.BenchmarkOffsets[SelectedBenchmarkIndex]);
                }
                else
                {
                    didModify = BigfileModifier.ReplaceBenchmark(CurrentPath, CurrConfigFile.LockedTargetOffset, CurrConfigFile.BenchmarkOffsets[SelectedBenchmarkIndex]);
                }
            }
            if (didModify)
            {
                try
                {
                    Process.Start(CurrentPath + "\\" + CurrConfigFile.ExecutableName, CurrConfigFile.CustomArguments);
                    TriggerWorker();
                }
                catch (Exception ex)
                {
                    Error.ErrorBox(ex);
                }
            }
        }

        private void btnAddPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.DefaultExt = ".big";
            dialog.Title = "Locate the Yeti.big file";
            dialog.Filter = "Bigfile (*.big)|*.big";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                FileInfo fileInfo = new FileInfo(dialog.FileName);

                CurrConfigFile.GamePaths.Add(fileInfo.Directory.FullName);
                SaveConfig();
            }

            TriggerWorker();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();

            TriggerWorker();
        }
    }
}
