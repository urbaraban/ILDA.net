using ILDA.net.Services;
using System;

namespace ILDA.net.Interfaces
{
    public interface IHeadItem
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public byte IldaVersion { get; }
        public byte ScannerNumber { get; set; }
        public UInt16 ItemsCount { get; }
        public bool IsFull { get; }
        public byte[] GetBodyBytes();
        internal void ParseBodyByte(ByteParser bytes, UInt16 itemsCount);
    }
}
