using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILDA.net.Services
{
    internal class ByteParser
    {
        public bool IsEnded => Index >= b.Length;
        private byte[] b { get; }
        public int Length => b.Length;
        public int Index { get; private set; } = 0;

        public ByteParser(byte[] B)
        {
            b = B;
            Index = 0;
        }

        public byte GetByte()
        {
            return (byte)(b[Index++] & 0xff);
        }

        public short GetShort()
        {
            byte[] _b = b.Skip(Index).Take(2).Reverse().ToArray();
            short shr = BitConverter.ToInt16(_b);
            Index += 2;
            return shr;
        }

        public UInt16 GetBEUInt16()
        {
            byte[] _b = b[Index..(Index + 2)].Reverse().ToArray();
            UInt16 int16 = BitConverter.ToUInt16(_b, 0);
            Index += 2;
            return int16;
        }

        public float GetFloat()
        {
            float flt = BitConverter.ToSingle(b, Index);
            Index += 4;
            return flt;
        }

        public double GetDouble()
        {
            double dbl = BitConverter.ToDouble(b, Index);
            Index += 8;
            return dbl;
        }
        public int GetInt()
        {
            int integer = BitConverter.ToInt32(b, Index);
            Index += 4;
            return integer;
        }

        public string GetString(int length)
        {
            StringBuilder outt = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                outt.Append((char)b[Index++]);
            }
            return outt.ToString();
        }

        public void Skip(int points) => Index += points;

        public void Reset() => Index = 0;

    }
}
