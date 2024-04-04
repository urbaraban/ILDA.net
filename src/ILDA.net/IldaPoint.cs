namespace ILDA.net
{
    public class IldaPoint 
    {
        public short X { get; set; } = 255;
        public short Y { get; set; } = 255;
        public short Z { get; set; } = 255;
        public IldaColor Color { get; set; }
        public byte PalIndex { get; set; } = 0;
        public bool IsBlanked { get; set; } = false;

        public IldaPoint() { }

        public IldaPoint(short x, short y, short z, bool isBlanked)
        {
            X = x;
            Y = y;
            Z = z;
            IsBlanked = isBlanked;
        }

        public byte[] GetBytes(int version, bool isLast)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(this.X).Reverse());
            bytes.AddRange(BitConverter.GetBytes(this.Y).Reverse());
            if (version % 2  == 0)
            {
                bytes.AddRange(BitConverter.GetBytes(this.Z).Reverse());
            }

            byte status = 0x00;
            if (this.IsBlanked == true)
            {
                status |= 0x64;
            }
            if (isLast == true)
            {
                status |= 128;
            }
            bytes.Add(status);

            if (version < 2)
            {
                bytes.Add(this.PalIndex);
            } 
            else
            {
                bytes.AddRange(this.Color.GetBytes());
            }
            return bytes.ToArray();
        }

        public override string ToString() => $"{X} : {Y} : {Z} : {IsBlanked}";
    }
}
