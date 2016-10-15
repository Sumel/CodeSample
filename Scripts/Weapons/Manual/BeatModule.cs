using UnityEngine;
using System;
namespace AssemblyCSharp
{
    public class BeatModule : BeatResponder, RhythmAccuracyModule
    {
        protected MusicManager musicManager;

        private float partialThreshold;
        private float perfectThreshold;

        private float timeOfLastBeat;
        private float timeOfNextBeat;

        private bool lastBeatUsed = false;
        private bool nextBeatUsed = false;
        float beatInterval;

        public BeatModule(float beatInterval, float partialThreshold, float perfectThreshold, int skipBeats = 0)
        {
            this.beatInterval = beatInterval;
            this.partialThreshold = partialThreshold;
            this.perfectThreshold = perfectThreshold;
            musicManager = MusicManager.instance;
            musicManager.AddNewResponder(this, beatInterval);
        }

        public void OnBeat(float timeOfBeat, float beatNum)
        {
            timeOfLastBeat = timeOfBeat;
            timeOfNextBeat = timeOfBeat + musicManager.BeatInterval * beatInterval;
            lastBeatUsed = nextBeatUsed;
            nextBeatUsed = false;
        }

        protected float getTimeDifference(float time)
        {
            return Math.Abs(time - musicManager.ElapsedSongTime);
        }

        protected bool isTimeValid(float time)
        {
            if (getTimeDifference(time) > partialThreshold)
            {
                return false;
            }

            return true;
        }

        protected float getFirstTime()
        {
            if (isTimeValid(timeOfLastBeat) && !lastBeatUsed)
            {
                lastBeatUsed = true;
                return timeOfLastBeat;
            }
            if (isTimeValid(timeOfNextBeat) && !nextBeatUsed)
            {
                nextBeatUsed = true;
                return timeOfNextBeat;
            }
            return -1;
        }


        public RhythmAccuracy Activate()
        {
            float t = getFirstTime();
            if (t == -1)
            {
                return RhythmAccuracy.Miss;
            }
            if (Math.Abs(t - musicManager.ElapsedSongTime) < perfectThreshold)
            {
                return RhythmAccuracy.Full;
            }
            return RhythmAccuracy.Partial;

        }
    }
}

