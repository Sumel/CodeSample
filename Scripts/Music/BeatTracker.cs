using System;
using UnityEngine;
using System.Collections.Generic;
namespace AssemblyCSharp
{
    public class BeatTracker
    {
        private float BPM;
        private float offset;
        private float beatInterval;
        private float beatIntervalMultiplier;
        private float _lastBeat = 0;
        private float lastBeat
        {
            get { return _lastBeat; }
            set { _lastBeat = value; }
        }
        private float beatCounter = 0;
        private List<BeatResponder> responders;
        private MusicManager manager;
        private void fireBeatEvents(float time)
        {
            foreach (BeatResponder responder in responders)
            {
                responder.OnBeat(time, beatCounter);
            }
        }
        public BeatTracker(float BPM, float beatIntervalMultiplier, float offset, List<BeatResponder> startingResponders)
        {
            this.BPM = BPM;
            this.offset = offset;
            this.beatIntervalMultiplier = beatIntervalMultiplier;
            beatInterval = (1 / (BPM / 60)) * beatIntervalMultiplier;
            lastBeat = offset - beatInterval;
            if (startingResponders != null)
                responders = startingResponders;
            else
                responders = new List<BeatResponder>();
            manager = MusicManager.instance;
        }
        public BeatTracker(float BPM, float beatIntervalMultiplier, float offset) : this(BPM, beatIntervalMultiplier, offset, null)
        {
        }

        public void ChangeMusicParameters(float newBPM, float newOffset)
        {
            BPM = newBPM;
            offset = newOffset;
            beatInterval = (1 / (BPM / 60)) * beatIntervalMultiplier;
            lastBeat = offset - beatInterval;
            beatCounter = 0;
            recalculateLastBeat();
        }

        private void recalculateLastBeat()
        {

            while (manager.ElapsedSongTime > lastBeat)
            {
                lastBeat += beatInterval;
                beatCounter += beatIntervalMultiplier;
            }
        }


        private void onBeat(float time)
        {
            fireBeatEvents(time);
            lastBeat += beatInterval;
            beatCounter += beatIntervalMultiplier;
            if (beatCounter > MusicManager.biggestBeatInterval)
            {
                beatCounter = beatIntervalMultiplier;
            }
        }
        public void Update()
        {
            if (manager.ElapsedSongTime > lastBeat + beatInterval)
            {
                onBeat(lastBeat + beatInterval);
            }
        }

        public void Add(BeatResponder newResponder)
        {
            responders.Add(newResponder);
        }

        public void Remove(BeatResponder oldResponder)
        {
            responders.Remove(oldResponder);
        }
    }
}

