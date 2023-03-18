using ILDA.net.Interfaces;

namespace ILDA.net.Services
{
    internal static class ByteGetter
    {
        public static byte[] GetStringBytes(string str, int bytesCount = 0)
        {
            if (bytesCount == 0)
            {
                bytesCount = str.Length;
            }

            byte[] bytes = new byte[bytesCount];

            for (int i = 0; i < bytesCount; i += 1)
            {
                if (i < str.Length)
                    bytes[i] = (byte)str[i];
                else
                    bytes[i] = (byte)' ';
            }

            return bytes;
        }
    }
}
