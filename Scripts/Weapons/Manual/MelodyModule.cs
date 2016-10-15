
using System;
namespace AssemblyCSharp
{
    public class MelodyModule : MelodyResponder, RhythmAccuracyModule
    {
        protected MelodyTracker melodyTracker;
        protected MusicManager musicManager;
        protected GameManager gameManager;
        protected Note _lastNote;
        protected Note LastNote
        {
            get { return _lastNote; }
            set { _lastNote = value; }
        }
        private bool lastNoteUsed = false;
        private bool nextNoteUsed = false;
        public MelodyModule()
        {
            musicManager = MusicManager.instance;
            melodyTracker = musicManager.melodyTracker;
            gameManager = GameManager.instance;
            melodyTracker.Add(this);
            ChangeNote(melodyTracker.NextNote);
        }

        protected void ChangeNote(Note newNote)
        {
            LastNote = newNote;
            lastNoteUsed = nextNoteUsed;
            nextNoteUsed = false;
        }
        public void OnMelody(Note note)
        {
            ChangeNote(note);
        }

        protected float getTimeDifference(Note note)
        {
            return Math.Abs(note.Position - musicManager.ElapsedSongTime);
        }

        protected bool isNoteTimeValid(Note note)
        {
            if (getTimeDifference(note) > gameManager.PartialThreshold)
            {
                return false;
            }

            return true;
        }

        protected Note getFirstValidNote()
        {
            if (isNoteTimeValid(LastNote) && !lastNoteUsed)
            {
                lastNoteUsed = true;
                return LastNote;
            }
            if (isNoteTimeValid(melodyTracker.NextNote) && !nextNoteUsed)
            {
                nextNoteUsed = true;
                return melodyTracker.NextNote;
            }
            return null;
        }

        public RhythmAccuracy Activate()
        {
            Note shootNote = getFirstValidNote();
            if (shootNote == null || getTimeDifference(shootNote) > gameManager.PartialThreshold)
            {
                return RhythmAccuracy.Miss;
            }
            else if (getTimeDifference(shootNote) < gameManager.PerfectThreshold)
            {
                return RhythmAccuracy.Full;
            }
            else
            {
                return RhythmAccuracy.Partial;
            }
        }
    }
}

