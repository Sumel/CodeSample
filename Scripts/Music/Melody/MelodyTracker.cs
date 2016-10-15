using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class MelodyTracker
{
    private TextAsset beatmap = null;
    private List<Note> notes = new List<Note>();
    public Note[] Notes
    {
        get { return notes.ToArray(); }
    }
    //[SerializeField]
    private float offset = 0;
    MusicManager musicManager;
    List<MelodyResponder> melodyResponders = new List<MelodyResponder>();
    private int _noteIndex = 0;
    public int NoteIndex
    {
        get { return _noteIndex; }
        private set { _noteIndex = value; }
    }

    //because I have no idea how osu saves sliders. This might not work for other beatmaps
    private readonly float HeldNoteTimeMultiplier = 3.876923361332f;

    private void ConvertBeatmapToArray()
    {
        if (beatmap != null)
        {
            notes.Clear();
            string times = beatmap.text.Substring(beatmap.text.IndexOf("[HitObjects]"));
            string[] lines = times.Split('\n');
            //intentionally skipping index zero and last index
            for (int i = 1; i < lines.Length - 1; i++)
            {
                string line = lines[i];
                string[] splitLine = line.Split(',');
                float time = float.Parse(splitLine[2]) / 1000.0f + offset;
                float timeOfNext = float.MaxValue;
                if (i < lines.Length - 2)
                {
                    timeOfNext = float.Parse(lines[i + 1].Split(',')[2]) / 1000.0f + offset;
                }

                bool isMajor = int.Parse(splitLine[3]) >= 5;
                bool isHeld = splitLine[3] == "2" || splitLine[3] == "6";
                if (!isHeld)
                {
                    notes.Add(new Note(time, timeOfNext, isMajor));
                }
                else
                {
                    float length = float.Parse(splitLine[7]) * HeldNoteTimeMultiplier / 1000.0f;
                    int numberOfLoops = int.Parse(splitLine[6]);
                    for (int ind = 0; ind < numberOfLoops; ind++)
                    {
                        HeldNote noteToAdd = new HeldNote(time, timeOfNext, length, numberOfLoops, ind, isMajor);
                        notes.Add(noteToAdd);
                    }
                }

            }
        }
    }

    private void GenerateArrayFromBPM(float BPM)
    {
        float timeBetweenBeats = 1.0f / (BPM / 60.0f);
        notes.Clear();
        //make sure this is called after song has been changed in musicManager
        int m = (int)(musicManager.SongLength / timeBetweenBeats);
        for (int i = 0; i < m; i++)
        {
            float time = i * timeBetweenBeats + offset;
            float timeOfNext = time + timeBetweenBeats;
            if (timeOfNext > musicManager.SongLength)
            {
                timeOfNext = float.MaxValue;
            }
            notes.Add(new Note(time, timeOfNext));
        }
    }

    private MelodyTracker()
    {
        musicManager = MusicManager.instance;
    }

    public MelodyTracker(TextAsset _beatmap)
        : this()
    {
        beatmap = _beatmap;

        float locOffset = musicManager.offset;
        string times = beatmap.text.Substring(beatmap.text.IndexOf("[HitObjects]"));
        string[] lines = times.Split('\n');
        offset = (musicManager.offset - float.Parse(lines[1].Split(',')[2]) / 1000.0f);
        ConvertBeatmapToArray();
    }

    public MelodyTracker(float BPM)
        : this()
    {
        offset = musicManager.offset;
        GenerateArrayFromBPM(BPM);
    }

    public void ChangeMusicParameters(float BPM)
    {
        offset = musicManager.offset;
        beatmap = null;
        GenerateArrayFromBPM(BPM);
        RecalculateNoteIndex();
    }

    public void ChangeMusicParameters(TextAsset _beatmap)
    {
        beatmap = _beatmap;
        float locOffset = musicManager.offset;
        string times = beatmap.text.Substring(beatmap.text.IndexOf("[HitObjects]"));
        string[] lines = times.Split('\n');
        offset = (musicManager.offset - float.Parse(lines[1].Split(',')[2]) / 1000.0f);
        ConvertBeatmapToArray();
        RecalculateNoteIndex();
    }

    private void RecalculateNoteIndex()
    {
        NoteIndex = 0;
        while (musicManager.ElapsedSongTime > notes[NoteIndex].Position)
        {
            NoteIndex++;
            if (NoteIndex > notes.Count)
            {
                Debug.LogError("Ran out of notes when recalculating note index");
                break;
            }
        }
    }

    public void OnMusicEnd()
    {

    }

    public void Update()
    {
        if (musicManager.ElapsedSongTime > notes[NoteIndex].Position)
        {
            //done in this way so that responders can check the next note during onMelody;
            Note n = notes[NoteIndex];
            NoteIndex++;
            FireAllResponders(n);
            if (NoteIndex >= notes.Count)
            {
                Debug.Log("Resetting notes");
                NoteIndex = 0;
            }
        }
    }
    private void FireAllResponders(Note note)
    {
        foreach (MelodyResponder responder in melodyResponders)
        {
            responder.OnMelody(note);
        }
    }

    public Note NextNote
    {
        get { return notes[NoteIndex]; }
    }

    public Note NoteAtIndex(int index)
    {
        if (index < 0 || index >= notes.Count)
        {
            return null;
        }
        return notes[index];
    }

    public void Add(MelodyResponder newResponder)
    {
        melodyResponders.Add(newResponder);
    }

    public void Remove(MelodyResponder oldResponder)
    {
        melodyResponders.Remove(oldResponder);
    }
}
