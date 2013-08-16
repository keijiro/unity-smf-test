namespace SmfLite
{
    // Variable-length value class.
    public struct MultiByteValue
    {
        public int value;
        public int length;
        
        public MultiByteValue (int value, int length)
        {
            this.value = value;
            this.length = length;
        }
    }

    // Simple binary reader class.
    public class SimpleReader
    {
        byte[] data;
        int offset;

        public SimpleReader (byte[] data)
        {
            this.data = data;
        }

        public int PeekByte()
        {
            if (offset < data.Length) {
                return data[offset];
            } else {
                return 0x100;
            }
        }

        public byte ReadByte ()
        {
            return data [offset++];
        }

        public char[] ReadChars (int length)
        {
            var temp = new char[length];
            for (var i = 0; i < length; i++) {
                temp [i] = (char)ReadByte ();
            }
            return temp;
        }

        public int ReadBEInt32 ()
        {
            int b1 = ReadByte ();
            int b2 = ReadByte ();
            int b3 = ReadByte ();
            int b4 = ReadByte ();
            return b4 + (b3 << 8) + (b2 << 16) + (b1 << 24);
        }
        
        public int ReadBEInt16 ()
        {
            int b1 = ReadByte ();
            int b2 = ReadByte ();
            return b2 + (b1 << 8);
        }

        public MultiByteValue ReadMultiByteValue ()
        {
            int value = 0;
            int length = 0;
            while (true) {
                int b = ReadByte ();
                length++;
                value += b & 0x7f;
                if (b < 0x80)
                    break;
                value <<= 7;
            }
            return new MultiByteValue (value, length);
        }
    }
}