using System.Collections.Generic;

namespace SmfLite
{
    public class Loader
    {
        public List<Track> Load (byte[] data)
        {
            return Load (new System.IO.MemoryStream (data));
        }

        public List<Track> Load (System.IO.Stream stream)
        {
            var tracks = new List<Track> ();
            var reader = new System.IO.BinaryReader (stream);

            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MThd") {
                throw new System.FormatException ("Can't find header chunk.");
            }

            // Chunk length.
            if (Utility.ReadBE32 (reader) != 6) {
                throw new System.FormatException ("Length of header chunk must be 6.");
            }

            // Format (unused).
            reader.ReadBytes (2);

            // Number of tracks.
            var trackCount = Utility.ReadBE16 (reader);

            // Delta-time divisions.
            var division = Utility.ReadBE16 (reader);
            if ((division & 0x8000) != 0) {
                throw new System.FormatException ("SMPTE time code is not supported.");
            }

            // Read the tracks.
            for (var trackIndex = 0; trackIndex < trackCount; trackIndex++) {
                tracks.Add (ReadTrack (reader));
            }

            return tracks;
        }

        Track ReadTrack (System.IO.BinaryReader reader)
        {
            var track = new Track ();

            // Chunk type.
            if (new string (reader.ReadChars (4)) != "MTrk") {
                throw new System.FormatException ("Can't find track chunk.");
            }
            
            // Chunk length.
            var remains = Utility.ReadBE32 (reader);

            // Read delta-time and event pairs.
            while (remains > 0) {
                var delta = VariableLengthValue.ReadStream (reader);
                remains -= delta.length;

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
                    track.AddDeltaAndMessage (delta.value, new Message (status, b1, 0));
                } else {
                    var b1 = reader.ReadByte ();
                    var b2 = reader.ReadByte ();
                    remains -= 2;
                    track.AddDeltaAndMessage (delta.value, new Message (status, b1, b2));
                }
            }
            return track;
        }
    }
}