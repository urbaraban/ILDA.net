using System.Collections.ObjectModel;
using ILDA.net.Interfaces;
using ILDA.net.Services;

namespace ILDA.net
{
    public class IldaPalette : Collection<IldaColor>, IHeadItem
    {
        public string Name { get; set; } = "ILDA.net";
        public string CompanyName { get; set; } = "ILDA.net";
        public byte IldaVersion => 2;
        public byte ScannerNumber { get; set; } = 0;
        public UInt16 ItemsCount => (UInt16)Items.Count;
        public bool IsFull => this.Count == 256;

        public IldaPalette() { }

        public IldaPalette(string name, string companyName, byte scannerNumber)
        {
            Name = name;
            CompanyName = companyName;
            ScannerNumber = scannerNumber;
        }

        public void Add(byte red, byte green, byte blue) => this.Add(new IldaColor(red, green, blue));
        public new void Add(IldaColor ildaColor)
        {
            if (this.Count < byte.MaxValue)
            {
                base.Add(ildaColor);
            }
        }

        /// <summary>
        /// Find near distance in table (but i see problem in this method)
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public int GetNearColorIndex(byte red, byte green, byte blue)
        {
            int index = 0;
            if (this.Count > 0)
            {
                int distance = int.MaxValue;
                for (int i = 0; i < this.Count; i += 1)
                {
                    int thisdistance = Math.Abs(this[i].R - red) + Math.Abs(this[i].G - green) + Math.Abs(this[i].B - blue);
                    if (thisdistance < distance)
                        index = i;
                }
            }
            return index;
        }

        public byte[] GetBodyBytes()
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < this.Count; i += 1)
            {
                bytes.AddRange(this[i].GetBytes());
            }

            return bytes.ToArray();
        }

        void IHeadItem.ParseBodyByte(ByteParser bytes, UInt16 itemsCount)
        {
            for (int i = 0; i < itemsCount && bytes.IsEnded == false; i += 1)
            {
                byte _blue = bytes.GetByte();
                byte _green = bytes.GetByte();
                byte _red = bytes.GetByte();


                IldaColor ildaColor = new IldaColor(_red, _green, _blue);
                this.Add(ildaColor);
            }
        }

        public static IldaPalette GetDefaultPalette()
        {
            IldaPalette palette = new("def pal", "LDA", 0);

            palette.Add(255, 0, 0);
            palette.Add(255, 16, 0);
            palette.Add(255, 32, 0);
            palette.Add(255, 48, 0);
            palette.Add(255, 64, 0);
            palette.Add(255, 80, 0);
            palette.Add(255, 96, 0);
            palette.Add(255, 112, 0);
            palette.Add(255, 128, 0);
            palette.Add(255, 144, 0);
            palette.Add(255, 160, 0);
            palette.Add(255, 176, 0);
            palette.Add(255, 192, 0);
            palette.Add(255, 208, 0);
            palette.Add(255, 224, 0);
            palette.Add(255, 240, 0);
            palette.Add(255, 255, 0);
            palette.Add(224, 255, 0);
            palette.Add(192, 255, 0);
            palette.Add(160, 255, 0);
            palette.Add(128, 255, 0);
            palette.Add(96, 255, 0);
            palette.Add(64, 255, 0);
            palette.Add(32, 255, 0);
            palette.Add(0, 255, 0);
            palette.Add(0, 255, 32);
            palette.Add(0, 255, 64);
            palette.Add(0, 255, 96);
            palette.Add(0, 255, 128);
            palette.Add(0, 255, 160);
            palette.Add(0, 255, 192);
            palette.Add(0, 255, 224);
            palette.Add(0, 130, 255);
            palette.Add(0, 114, 255);
            palette.Add(0, 104, 255);
            palette.Add(10, 96, 255);
            palette.Add(0, 82, 255);
            palette.Add(0, 74, 255);
            palette.Add(0, 64, 255);
            palette.Add(0, 32, 255);
            palette.Add(0, 0, 255);
            palette.Add(32, 0, 255);
            palette.Add(64, 0, 255);
            palette.Add(96, 0, 255);
            palette.Add(128, 0, 255);
            palette.Add(160, 0, 255);
            palette.Add(192, 0, 255);
            palette.Add(224, 0, 255);
            palette.Add(255, 0, 255);
            palette.Add(255, 32, 255);
            palette.Add(255, 64, 255);
            palette.Add(255, 96, 255);
            palette.Add(255, 128, 255);
            palette.Add(255, 160, 255);
            palette.Add(255, 192, 255);
            palette.Add(255, 224, 255);
            palette.Add(255, 255, 255);
            palette.Add(255, 224, 224);
            palette.Add(255, 192, 192);
            palette.Add(255, 160, 160);
            palette.Add(255, 128, 128);
            palette.Add(255, 96, 96);
            palette.Add(255, 64, 64);
            palette.Add(15, 32, 32);

            return palette;
        }
    }

    public class IldaColor
    {
        public byte R { get; set; } = 0;
        public byte G { get; set; } = 0;
        public byte B { get; set; } = 0;

        public IldaColor() { }
        public IldaColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public byte[] GetBytes()
        {
            return new byte[3] { B, G, R };
        }

        public override string ToString()
        {
            return $"RGB {R}:{G}:{B}";
        }
    }
}
