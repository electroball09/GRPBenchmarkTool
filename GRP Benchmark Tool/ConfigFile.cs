﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace GRP_Benchmark_Tool
{
    public class ConfigFile
    {
        const string FILE_NAME = "config.cfg";
        const char SEPARATOR = '|';
        const char OFFSET_SEPARATOR = ':';

        List<string> gamePaths = new List<string>();
        public List<string> GamePaths { get { return gamePaths; } }
        List<string> customOffsets = new List<string>();
        public List<string> CustomOffsets { get { return customOffsets; } }
        List<BenchmarkOffset> benchmarkOffsets = new List<BenchmarkOffset>();
        public List<BenchmarkOffset> BenchmarkOffsets { get { return benchmarkOffsets; } }
        private BenchmarkOffset lockedTargetOffset;
        public BenchmarkOffset LockedTargetOffset { get { return lockedTargetOffset; } }
        string customArguments = "";
        public string CustomArguments { get { return customArguments; } }
        string executableName = "";
        public string ExecutableName { get { return executableName; } }
        string isUnlocked = "";
        public bool IsUnlocked { get { return isUnlocked == "true"; } }
        
        public Visibility TargetBoxVisibility
        {
            get { return IsUnlocked ? Visibility.Visible : Visibility.Collapsed; }
        }

        public double SourceBoxHeight
        {
            get { return IsUnlocked ? 170 : 360; }
        }

        public double SourceListHeight
        {
            get { return IsUnlocked ? 120 : 310; }
        }

        public void LoadFile()
        {
            gamePaths = new List<string>();
            customOffsets = new List<string>();
            benchmarkOffsets = new List<BenchmarkOffset>();
            customArguments = "";

            if (!File.Exists(FILE_NAME))
            {
                File.Create(FILE_NAME);
                return;
            }

            string[] lines = File.ReadAllLines(FILE_NAME);
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    string[] split = lines[i].Split('|');
                    if (split.Length != 2)
                        Error.ErrorBox("Config file is corrupt!", true);

                    if (split[0] == "executable")
                        executableName = split[1];
                    if (split[0] == "gamePaths")
                        gamePaths.Add(split[1]);
                    if (split[0] == "customOffsets")
                        customOffsets.Add(split[1]);
                    if (split[0] == "benchmarkOffset")
                    {
                        string[] benchOffsetSplit = split[1].Split(OFFSET_SEPARATOR);
                        benchmarkOffsets.Add(new BenchmarkOffset(benchOffsetSplit[0], benchOffsetSplit[1], benchOffsetSplit[2]));
                    }
                    if (split[0] == "args")
                        customArguments = split[1];
                    if (split[0] == "isUnlocked")
                        isUnlocked = split[1];
                }
                catch(Exception ex)
                {
                    Error.ErrorBox(ex, "Error loading config file!", true);
                }
            }

            if (!IsUnlocked)
            {
                lockedTargetOffset = benchmarkOffsets[0];
                benchmarkOffsets.RemoveAt(0);
            }
        }

        public void SaveFile()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (string line in gamePaths)
                    lines.Add("gamePaths" + SEPARATOR + line);
                foreach (string line in customOffsets)
                    lines.Add("customOffsets" + SEPARATOR + line);
                foreach (BenchmarkOffset benchOffset in benchmarkOffsets)
                    lines.Add(string.Format("benchmarkOffset{4}{0}{3}{1}{3}{2}", benchOffset.Name, benchOffset.Offset, benchOffset.Key, OFFSET_SEPARATOR, SEPARATOR));
                lines.Add("args" + SEPARATOR + customArguments);
                lines.Add("executable" + SEPARATOR + executableName);
                if (IsUnlocked)
                    lines.Add("isUnlocked" + SEPARATOR + "true");
                string[] linesArray = lines.ToArray();
                FileInfo info = new FileInfo(FILE_NAME);
                File.WriteAllLines(info.FullName, linesArray);
            }
            catch (Exception ex)
            {
                Error.ErrorBox(ex, "Error saving config file!", true);
            }
        }
    }

    public class BenchmarkOffset
    {
        private string name = "";
        public string Name { get { return name; } set { name = value; } }
        private string offset = "";
        public string Offset { get { return offset; } set { offset = value; } }
        private string key = "";
        public string Key { get { return key; } set { key = value; } }

        public BenchmarkOffset (string _name, string _offset, string _key)
        {
            name = _name;
            offset = _offset;
            key = _key;
        }
    }
}
