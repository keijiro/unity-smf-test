using System.Collections.Generic;

namespace SmfLite
{
    // MIDI track class.
    public class Track
    {
        struct DeltaMessagePair
        {
            public int delta;
            public Message message;

            public DeltaMessagePair (int delta, Message message)
            {
                this.delta = delta;
                this.message = message;
            }

            public override string ToString ()
            {
                return "(" + delta + ":" + message + ")";
            }
        }

        List<DeltaMessagePair> sequence;

        public Track ()
        {
            sequence = new List<DeltaMessagePair> ();
        }

        public void AddDeltaAndMessage (int delta, Message message)
        {
            sequence.Add (new DeltaMessagePair (delta, message));
        }

        public override string ToString ()
        {
            var s = "";
            foreach (var pair in sequence)
                s += pair;
            return s;
        }
    }
}