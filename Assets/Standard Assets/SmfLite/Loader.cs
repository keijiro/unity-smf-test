using System.Collections.Generic;

namespace SmfLite
{
    public class Loader
    {
        // MIDI message class.
        struct Message
        {
            public byte status;
            public byte data1;
            public byte data2;
            
            public Message (byte status, byte data1, byte data2)
            {
                this.status = status;
                this.data1 = data1;
                this.data2 = data2;
            }
        };

        // Variable-length value class.
        struct VariableLengthValue
        {
            public int value;
            public int length;

            public VariableLengthValue (int value, int length)
            {
                this.value = value;
                this.length = length;
            }

            public static VariableLengthValue ReadStream(System.IO.BinaryReader reader)
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
                return new VariableLengthValue(value, length);
            }
        };

        public static string Load (byte[] data)
        {
            return Load (new System.IO.MemoryStream (data));
        }

        public static string Load (System.IO.Stream stream)
        {
            var reader = new System.IO.BinaryReader (stream);

            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MThd") {
                return "Invalid file header.";
            }

            // Chunk length.
            if (ReadBE32 (reader) != 6) {
                return "Invalid file header.";
            }

            // Format (unused).
            reader.ReadBytes (2);

            // Number of tracks.
            var trackCount = ReadBE16 (reader);

            // Delta-time divisions.
            var division = ReadBE16 (reader);
            if ((division & 0x8000) != 0) {
                return "Unsupported file type (SMPTE time code).";
            }

            // Read the tracks.
            for (var trackIndex = 0; trackIndex < trackCount; trackIndex++) {
                var error = ReadTrack (reader);
                if (error != null)
                    return error;
            }

            return "Ok";
        }

        static string ReadTrack (System.IO.BinaryReader reader)
        {
            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MTrk") {
                return "Invalid file header.";
            }
            
            // Chunk length.
            var remains = ReadBE32 (reader);

            List<Message> events = new List<Message>();

            // Read delta-time and event pairs.
            while (remains > 0) {
                var varlen = VariableLengthValue.ReadStream (reader);
                remains -= varlen.length;

                var status = reader.ReadByte ();
                remains--;

                var messageType = status >> 4;
                if (messageType == 0xf) {
                    if (status == 0xf0 || status == 0xf7) {
                        // Sysex message.
                        var sysexLength = VariableLengthValue.ReadStream (reader);
                        reader.ReadBytes (sysexLength.value);
                        remains -= sysexLength.value + sysexLength.length;
                    } else if (status == 0xff) {
                        // Meta-event.
                        reader.ReadByte ();
                        var metaEventLength = VariableLengthValue.ReadStream (reader);
                        reader.ReadBytes (metaEventLength.value);
                        remains -= 1 + metaEventLength.value + metaEventLength.length;
                    }
                } else if (messageType == 0xc || messageType == 0xd) {
                    var b1 = reader.ReadByte ();
                    remains--;
                    events.Add(new Message(status, b1, 0));
                } else {
                    var b1 = reader.ReadByte ();
                    var b2 = reader.ReadByte ();
                    remains -= 2;
                    events.Add(new Message(status, b1, b2));
                }
            }
            return null;
        }

        static int ReadBE32 (System.IO.BinaryReader reader)
        {
            var bytes = reader.ReadBytes (4);
            return bytes [3] + (bytes [2] << 8) + (bytes [1] << 16) + (bytes [0] << 24);
        }

        static int ReadBE16 (System.IO.BinaryReader reader)
        {
            var bytes = reader.ReadBytes (2);
            return bytes [1] + (bytes [0] << 8);
        }
    }
}
