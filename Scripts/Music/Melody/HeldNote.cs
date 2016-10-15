using System;
namespace AssemblyCSharp
{
    public class HeldNote : Note
    {
        private float _holdTime;
        private int _numberOfLoops;
        private int _loopIndex;
        public float HoldTime
        {
            get { return _holdTime; }
            private set { _holdTime = value; }
        }
        public int NumberOfLoops
        {
            get { return _numberOfLoops; }
            private set { _numberOfLoops = value; }
        }
        public int LoopIndex
        {
            get { return _loopIndex; }
            private set { _loopIndex = value; }
        }
        public HeldNote(float position, float nextNotePosition, float holdTime, int numberOfLoops = 1, int loopIndex = 0, bool isMajorNote = false) : base(position, nextNotePosition, isMajorNote)
        {
            HoldTime = holdTime;
            NumberOfLoops = numberOfLoops;
            LoopIndex = loopIndex;
        }

    }
}

