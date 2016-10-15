using System;
namespace AssemblyCSharp
{
    public class Note
    {
        //position in timeline (in seconds)
        protected float _position = 0;
        protected float _nextNotePosition = 0;
        protected bool _isMajorNote = false;
        public float Position
        {
            get { return _position; }
            private set { _position = value; }
        }

        public float NextNotePosition
        {
            get { return _nextNotePosition; }
            private set { _nextNotePosition = value; }
        }

        public float TimeToNextNote
        {
            get { return NextNotePosition - Position; }
        }

        public bool IsMajorNote
        {
            get { return _isMajorNote; }
            private set { _isMajorNote = value; }
        }

        public Note(float position, float nextNotePosition, bool isMajorNote = false)
        {
            Position = position;
            NextNotePosition = nextNotePosition;
            IsMajorNote = isMajorNote;

        }
    }
}

