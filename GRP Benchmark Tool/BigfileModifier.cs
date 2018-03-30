using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GRP_Benchmark_Tool
{
    public class BigfileModifier
    {
        public static void ReplaceBenchmark(string basepath, BenchmarkOffset target, BenchmarkOffset source)
        {
            if (target == null || source == null)
            {
                Error.ErrorBox("Either target was null or source was null!");
                return;
            }

            string bigfileName = basepath + @"\Yeti.big";
            if (!File.Exists(bigfileName))
            {
                Error.ErrorBox("Bigfile could not be found!");
                return;
            }

            try
            {
                int targetOffset = Convert.ToInt32(target.Offset, 16);
                int targetKey = Convert.ToInt32(target.Key, 16);
                int sourceOffset = Convert.ToInt32(source.Offset, 16);
                int sourceKey = Convert.ToInt32(source.Key, 16);

                using (FileStream fs = File.Open(bigfileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.Seek(targetOffset, SeekOrigin.Begin);
                    fs.Write(sourceKey.ToBytes(), 0, 4);
                    if (targetOffset != sourceOffset)
                    {
                        fs.Seek(sourceOffset, SeekOrigin.Begin);
                        fs.Write((-1).ToBytes(), 0, 4);
                    }
                }
            }
            catch(Exception ex)
            {
                Error.ErrorBox(ex, "Error replacing benchmark!");
                return;
            }
        }
    }
}
