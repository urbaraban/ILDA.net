using ILDA.net.Interfaces;
using ILDA.net.Services;
using System.Collections.ObjectModel;

namespace ILDA.net
{
    public class IldaFrame : Collection<IldaPoint>, IHeadItem
    {
        public string Name { get; set; } = "ILDA.net";
        public string CompanyName { get; set; } = "ILDA.net";
        public byte IldaVersion { get; set; } = 4;
        public byte ScannerNumber { get; set; } = 0;
        public UInt16 ItemsCount => (UInt16)Items.Count;
        public bool IsFull => this.Count == UInt16.MaxValue;

        public IldaFrame(byte ildaVersion)
        {
            this.IldaVersion = ildaVersion;
        }

        public IldaFrame(string name, string companyName, byte scannerNumber, byte ildaVersion)
        {
            Name = name;
            CompanyName = companyName;
            ScannerNumber = scannerNumber;
            IldaVersion = ildaVersion;
        }

        public void Add(short x, short y, short z, bool isblanked) => 
            this.Add(new IldaPoint(x, y, z, isblanked));

        public new void Add(IldaPoint ildaPoint)
        {
            if (this.Count < UInt16.MaxValue)
            {
                base.Add(ildaPoint);
            }
        }

        public byte[] GetBodyBytes()
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < Items.Count; i += 1)
            {
                bytes.AddRange(this[i].GetBytes(this.IldaVersion, i == Items.Count - 1));
            }
            return bytes.ToArray();
        }

        public static byte[] GetEndFrameBytes()
        {
            List<byte> endframe = new List<byte>();
            endframe.AddRange(ByteGetter.GetStringBytes("ILDA"));          //Bytes 1-4: "ILDA"
            endframe.Add(0);                                               //Bytes 5-7: Reserved for big version...
            endframe.Add(0);
            endframe.Add(0);
            endframe.Add((byte)0);                            //Byte 8: IldaVersion

            endframe.AddRange(ByteGetter.GetStringBytes("Last frm", 8));            //Bytes 9-16: Name
            endframe.AddRange(ByteGetter.GetStringBytes("ILDA.net", 8));     //Bytes 17-24: Company Name

            endframe.Add((byte)0);
            endframe.Add((byte)0);
            endframe.Add((byte)0);
            endframe.Add((byte)0);
            endframe.Add((byte)0);
            endframe.Add((byte)0);
            endframe.Add((byte)0);
            endframe.Add((byte)0);

            return endframe.ToArray();
        }

        void IHeadItem.ParseBodyByte(ByteParser bytes, UInt16 itemsCount)
        {
            for (int i = 0; i < itemsCount && bytes.IsEnded == false; i += 1)
            {
                IldaPoint ildaPoint = new IldaPoint();

                ildaPoint.X = bytes.GetShort();
                ildaPoint.Y = bytes.GetShort();
                ildaPoint.Z = 0;
                if (this.IldaVersion % 2  == 0)
                {
                    ildaPoint.Z = bytes.GetShort();
                }
                ildaPoint.IsBlanked = (bytes.GetByte() == 0x40);

                if (this.IldaVersion < 2)
                {
                    ildaPoint.PalIndex = bytes.GetByte();
                }
                else
                {
                    IldaColor ildaColor = new IldaColor();
                    ildaColor.B = bytes.GetByte();
                    ildaColor.G = bytes.GetByte();
                    ildaColor.R = bytes.GetByte();
                    ildaPoint.Color = ildaColor;
                }

                this.Add(ildaPoint);
            }
        }
    }
}
