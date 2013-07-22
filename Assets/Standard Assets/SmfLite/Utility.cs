namespace SmfLite
{
    // Variable-length value class.
    public struct VariableLengthValue
    {
        public int value;
        public int length;
        
        public VariableLengthValue (int value, int length)
        {
            this.value = value;
            this.length = length;
        }
        
        public static VariableLengthValue ReadStream (System.IO.BinaryReader reader)
        {
            int value = 0;
            int length = 0;
            while (true) {
                int b = reader.ReadByte ();
                length++;
                value += b & 0x7f;
                if ((b & 0x80) == 0)
                    break;
                value <<= 7;
            }
            return new VariableLengthValue (value, length);
        }
    }

    // Miscellaneous utility class.
    public static class Utility
    {
        public static int ReadBE32 (System.IO.BinaryReader reader)
        {
            var bytes = reader.ReadBytes (4);
            return bytes [3] + (bytes [2] << 8) + (bytes [1] << 16) + (bytes [0] << 24);
        }
        
        public static int ReadBE16 (System.IO.BinaryReader reader)
        {
            var bytes = reader.ReadBytes (2);
            return bytes [1] + (bytes [0] << 8);
        }
    }
}