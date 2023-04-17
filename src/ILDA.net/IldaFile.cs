using ILDA.net.Interfaces;
using ILDA.net.Services;
using System.Collections.ObjectModel;

namespace ILDA.net
{
    public class IldaFile : Collection<IldaFrame>
    {
        public IldaPalette Palette { get; set; } = IldaPalette.GetDefaultPalette();
        public string Location { get; set; } = "empty";

        public void Add(IHeadItem headItem)
        {
            if (headItem is IldaPalette palette)
            {
                this.Palette = palette;
            }
            if (headItem is IldaFrame frame)
            {
                base.Add(frame);
            }
        }

        public void Save(string path, bool copy)
        {
            string usepath = path;
            if (string.IsNullOrEmpty(path) == true)
            {
                usepath = this.Location;
            }

            if (copy == false)
            {
                this.Location = usepath;
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(usepath));
                byte[] data = this.GetBytes();
                FileStream file = new FileStream(usepath, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, FileOptions.Asynchronous);
                file.Write(data, 0, data.Length);
                file.Close();
            }
            catch
            {
                Console.Write("Error when exporting ilda file: ");
            }
        }

        public byte[] GetBytes()
        {
            List<IHeadItem> items = new List<IHeadItem>();
            if (this.Count > 0)
            {
                items.AddRange(this.Items);
            }
            if (this.Palette != null)
            {
                items.Add(this.Palette);
            }

            List<byte> FileBytes = new List<byte>();

            for (short i = 0; i < items.Count; i += 1)
            {
                List<byte> itemBytes = new List<byte>();
                itemBytes.AddRange(ByteGetter.GetStringBytes("ILDA"));          //Bytes 1-4: "ILDA"
                itemBytes.Add(0);                                               //Bytes 5-7: Reserved for big version...
                itemBytes.Add(0);
                itemBytes.Add(0);
                itemBytes.Add(items[i].IldaVersion);                            //Byte 8: IldaVersion

                itemBytes.AddRange(ByteGetter.GetStringBytes(items[i].Name, 8));            //Bytes 9-16: Name
                itemBytes.AddRange(ByteGetter.GetStringBytes(items[i].CompanyName, 8));     //Bytes 17-24: Company Name
                itemBytes.AddRange(BitConverter.GetBytes(items[i].ItemsCount).Reverse());             //Bytes 25-26: Elements count int Items (point or colors)                      

                itemBytes.AddRange(BitConverter.GetBytes(i));                               //Bytes 27-28: Item number

                itemBytes.AddRange(BitConverter.GetBytes((short)items.Count));              //Bytes 29-30: Items total
                itemBytes.Add((byte)items[i].ScannerNumber);                                //Byte 31: Scanner head
                itemBytes.Add((byte)0);                                                     //Byte 32: Reserved
                itemBytes.AddRange(items[i].GetBodyBytes());

                FileBytes.AddRange(itemBytes.ToArray());
            }
            FileBytes.AddRange(IldaFrame.GetEndFrameBytes());
            return FileBytes.ToArray();
        }



        /**
         * Set the ilda version this frame uses.
         * 0 = frame 3D, palette
         * 1 = frame 2D, palette
         * 2 = palette
         * 4 = frame 3D, RGB
         * 5 = frame 2D, RGB
         * Internally, all frames are 3D and use RGB.
         *
         * @param versionNumber integer, can be 0, 1, 4 or 5
         * @throws IllegalArgumentException when using invalid version number
         */

        public static IldaFile? Open(string path)
        {
            if (File.Exists(path) == true)
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    try
                    {
                        if (reader.ReadBytes((int)reader.BaseStream.Length) is byte[] b)
                        {
                            if (IldaFile.Parse(b) is IldaFile ildas)
                            {
                                ildas.Location = path;
                                return ildas;
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Error in read file");
                    }
                }
            }
            return null;
        }

        public static IldaFile? Parse(byte[] b)
        {
            IldaFile? result = null;
            if (b != null)
            {
                result = new IldaFile();

                ByteParser bp = new ByteParser(b);
                while (bp.Index < bp.Length - 32)
                {
                    string hdr = bp.GetString(4);                       //Bytes 1-4: ILDA
                    if (hdr.Equals("ILDA") == false)
                    {
                        return null;
                    }
                    bp.Skip(3);                                         //Bytes 5-7: Reserved


                    byte ildaversion = bp.GetByte();                    //Byte 8: format code
                    string name = bp.GetString(8).TrimEnd(' ');         //Bytes 9-16: Name
                    string companyname = bp.GetString(8).TrimEnd(' ');  //Bytes 17-24: Company Name
                    UInt16 itemscount = bp.GetBEUInt16();               //Bytes 25-26: Elements count int Items (point or colors)  
                    bp.Skip(4);                                         //Bytes 27-28: Item number //Bytes 29-30: Items total
                    byte scannerNumber = bp.GetByte();                  //Byte 31: Scanner head
                    bp.Skip(1);                                         //Byte 32: Reserved

                    if (itemscount > 0)
                    {
                        IHeadItem? head;
                        if (ildaversion == 2)
                        {
                            head = new IldaPalette(name, companyname, scannerNumber);
                            head.ParseBodyByte(bp, itemscount);
                        } 
                        else
                        {
                            head = new IldaFrame(name, companyname, scannerNumber, ildaversion);
                            head.ParseBodyByte(bp, itemscount);
                        }
                        result.Add(head);
                    }
                }

            }
            return result;
        }

    }
}
