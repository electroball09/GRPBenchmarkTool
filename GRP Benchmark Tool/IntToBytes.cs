using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRP_Benchmark_Tool
{
    public static class IntToBytes
    {
        public static byte[] ToBytes(this int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value >> 0);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            return bytes;
        }
    }
}
