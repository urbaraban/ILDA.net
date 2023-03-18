using ILDA.net;
using System;

namespace Test_ILDA.net
{
    public class UnitTest1
    {
        [Fact]
        public void TestOneRead()
        {
            IldaFile file = IldaFile.Open("TestFile/test_1.ild");
            Assert.NotNull(file);
        }

        [Fact]
        public void TestAllRead()
        {
            for (int i = 1; i < 8; i += 1)
            {
                IldaFile file = IldaFile.Open($"TestFile/test_{i}.ild");
                Assert.NotNull(file);
            }
        }

        [Fact]
        public void TestAllReadAndWrite()
        {
            for (int i = 1; i < 10; i += 1)
            {
                IldaFile file = IldaFile.Open($"TestFile/test_{i}.ild");
                file.Save($"SaveFile/save_{i}.ild", true);
                Assert.NotNull(file);
            }
        }

        [Fact]
        public void TestColorCount()
        {
            IldaFile file = new IldaFile();
            file.Palette = new IldaPalette();
            Random random = new Random();
            for (int i = 1; i < 300; i += 1)
            {
                file.Palette.Add((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));
            }
            Assert.Equal(256, file.Palette.ItemsCount);
        }

        [Fact]
        public void TestIldaVersion0()
        {
            IldaFile file = new IldaFile();
            file.Palette = IldaPalette.GetDefaultPalette();
            IldaFrame frame = GetRandomFrame(0, 300, file.Palette.ItemsCount);
            file.Add(frame);
            file.Save("SaveFile/Test_Verson_0.ild", true);
        }

        [Fact]
        public void TestIldaVersion1()
        {
            IldaFile file = new IldaFile();
            file.Palette = IldaPalette.GetDefaultPalette();
            IldaFrame frame = GetRandomFrame(1, 300, file.Palette.ItemsCount);
            file.Add(frame);
            file.Save("SaveFile/Test_Verson_1.ild", true);
        }

        [Fact]
        public void TestMultpleFrame()
        {
            IldaFile file = new IldaFile();
            file.Palette = IldaPalette.GetDefaultPalette();
            for (int i = 0; i < 5; i += 1)
            {
                IldaFrame frame = GetRandomFrame(1, 300, file.Palette.ItemsCount);
                file.Add(frame);
            }

            file.Save("SaveFile/Test_Verson_Multple.ild", true);
        }

        public IldaFrame GetRandomFrame(byte version, int count, int pal_count)
        {
            IldaFrame frame = new IldaFrame(version);
            Random random = new Random();
            short half = short.MaxValue / 2;
            for (int i = 0; i < count; i += 1)
            {
                IldaPoint ildaPoint = new IldaPoint(
                    (short)(random.Next(0, short.MaxValue) - half),
                    (short)(random.Next(0, short.MaxValue) - half),
                    (short)(random.Next(0, short.MaxValue) - half),
                    false);

                ildaPoint.PalIndex = (byte)random.Next(0, pal_count - 1);
                frame.Add(ildaPoint);
            }
            return frame;
        }
    }
}