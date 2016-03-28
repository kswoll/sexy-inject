using System.Collections.Generic;

namespace SexyInject.Emit.Signatures
{
    public class SignatureDecompressor
    {
        public static int[] Decompress(params byte[] input)
        {
            var result = new List<int>();
            for (var i = 0; i < input.Length; i++)
            {
                var b1 = input[i];
                if (!IsBitSet(b1, 7))
                {
                    result.Add(b1);
                }
                else
                {
                    var b2 = input[++i];
                    b1 = (byte)UnsetBit(b1, 7);
                    var twoByteValue = (b1 << 8) + b2;
                    if (!IsBitSet(twoByteValue, 14))
                    {
                        result.Add(twoByteValue);
                    }
                    else
                    {
                        twoByteValue = (int)UnsetBit(twoByteValue, 14);
                        var b3 = input[++i];
                        var b4 = input[++i];
                        var fourByteValue = (twoByteValue << 16) + (b3 << 8) + b4;
                        result.Add(fourByteValue);
                    }
                }
            }
            return result.ToArray();
        } 

        private static bool IsBitSet(long b, int position)
        {
            return (b & (1 << position)) != 0;
        }

        private static long UnsetBit(long value, int position)
        {
            int mask = 1 << position;
            return value & ~mask;
        }
    }
}