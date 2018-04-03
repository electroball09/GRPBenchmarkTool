using System;
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

        List<string> gamePaths = new List<string>();
        public List<string> GamePaths { get { return gamePaths; } }
        List<string> customOffsets = new List<string>();
        public List<string> CustomOffsets { get { return customOffsets; } }
        List<BenchmarkOffset> benchmarkOffsets = new List<BenchmarkOffset>();
        public List<BenchmarkOffset> BenchmarkOffsets { get { return benchmarkOffsets; } }
        BenchmarkOffset baseOffset;
        public BenchmarkOffset BaseOffset { get { return baseOffset; } }
        string customArguments = "";
        public string CustomArguments { get { return customArguments; } }
        string executableName = "";
        public string ExecutableName { get { return executableName; } }

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
                        continue;

                    if (split[0] == "executable")
                        executableName = split[1];
                    if (split[0] == "baseOffset")
                    {
                        string[] baseOffsetSplit = split[1].Split(':');
                        baseOffset = new BenchmarkOffset(baseOffsetSplit[0], baseOffsetSplit[1], baseOffsetSplit[2]);
                    }
                    if (split[0] == "gamePaths")
                        gamePaths.Add(split[1]);
                    if (split[0] == "customOffsets")
                        customOffsets.Add(split[1]);
                    if (split[0] == "benchmarkOffset")
                    {
                        string[] benchOffsetSplit = split[1].Split(':');
                        benchmarkOffsets.Add(new BenchmarkOffset(benchOffsetSplit[0], benchOffsetSplit[1], benchOffsetSplit[2]));
                    }
                    if (split[0] == "args")
                    {
                        customArguments = split[1];
                    }
                }
                catch(Exception ex)
                {
                    Error.ErrorBox(ex, "Error loading config file!", true);
                }
            }
        }

        public void SaveFile()
        {
            List<string> lines = new List<string>();
            foreach (string line in gamePaths)
                lines.Add("gamePaths=" + line);
            foreach (string line in customOffsets)
                lines.Add("customOffsets=" + line);
            foreach (BenchmarkOffset benchOffset in benchmarkOffsets)
                lines.Add(string.Format("benchmarkOffset={0}:{1}:{2}", benchOffset.Name, benchOffset.Offset, benchOffset.Key));
            lines.Add("args=" + customArguments);
            lines.Add(string.Format("baseOffset={0}:{1}:{2}", baseOffset.Name, baseOffset.Offset, baseOffset.Key));
            lines.Add("executable=" + executableName);
            string[] linesArray = lines.ToArray();
            FileInfo info = new FileInfo(FILE_NAME);
            File.WriteAllLines(info.FullName, linesArray);
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
