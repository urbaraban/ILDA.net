# ILDA.net

> I apologize, but my spoken English is pretty bad. I use a translator for this documentation.

This .net6 library allows you to read, edit, and write ILDA (International Laser Display Association) format of all formats.

ILDA formats according to the [documentation](https://www.ilda.com/resources/StandardsDocs/ILDA_IDTF14_rev011.pdf):

- 0 = 3D, palette;
- 1 = 2D, palette;
- (2 = palette header);
- (3 = deprecated);
- 4 = 3D, RGB;
- 5 = 2D, RGB

# Features

- Open, edit and save *.ild files
- Add or remove frames, color and points


# How to use

```cs
IldaFile file = IldaFile.Open("Folder/File.ild");
```
or
```cs
IldaFile file = IldaFile.Parse(byte[]);
```

If you want to create a file, then don't deny yourself anything.

## For example

```cs
IldaFile file = new IldaFile();
file.Palette = IldaPalette.GetDefaultPalette();
IldaFrame frame = new IldaFrame(byte Version);
for (int i = 0; i < 100; i += 1)
{
    IldaPoint ildaPoint = new IldaPoint(
        (short)(random.Next(0, short.MaxValue) - half),
        (short)(random.Next(0, short.MaxValue) - half),
        (short)(random.Next(0, short.MaxValue) - half),
        false);

    ildaPoint.PalIndex = (byte)random.Next(0, pal_count - 1);
    frame.Add(ildaPoint);
}
file.Add(frame);
file.Save("Folder/File.ild", false);
```

