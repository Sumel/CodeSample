emielusing UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using System;

public class MusicManager : MonoBehaviour
{
    //everything should be stored in seconds (except BPM which is beats per minute)
    [SerializeField]
    MusicParameters startingMusicParameters;

    //this is used in case a song has very slow buldup
    [SerializeField]
    float minStartingTime = 21.5f;

    MusicParameters musicParameters;

    private AudioSource _audioSource;

    public AudioSource audioSource
    {
        get { return _audioSource; }
        set { _audioSource = value; }
    }

    public float BPM
    {
        get { return musicParameters.BPM; }
    }

    public float BeatInterval
    {
        get { return 1.0f / (BPM / 60.0f); }
    }


    public float offset
    {
        get { return musicParameters.Offset; }
    }
    //1/16 means it will fire 16 times per beat. 
    //16 means it will fire every 16 beats.
    private readonly float[] availableBeatIntervals = new float[] { 16.0f, 8.0f, 4.0f, 2.0f, 1.0f, 1.0f / 2.0f, 1.0f / 4.0f, 1.0f / 8.0f, 1.0f / 16.0f };
    public static readonly float smallestBeatInterval = 1.0f / 16.0f;
    public static readonly float biggestBeatInterval = 16;
    private readonly Dictionary<float, BeatTracker> beatTrackers = new Dictionary<float, BeatTracker>();

    private MelodyTracker _melodyTracker;
    public MelodyTracker melodyTracker
    {
        get { return _melodyTracker; }
        private set { _melodyTracker = value; }
    }
    public float SongLength
    {
        get { return GetComponent<AudioSource>().clip.length; }
    }

    private static MusicManager _instance;
    public static MusicManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MusicManager>();
                //if you want this to be persistent through scenes try adding some non-destroy code here
            }
            return _instance;
        }
    }

    public void AddNewResponder(BeatResponder newResponder, float wantedBeatInterval)
    {
        if (beatTrackers.ContainsKey(wantedBeatInterval))
        {
            beatTrackers[wantedBeatInterval].Add(newResponder);
        }
        else
        {
            Debug.LogError("BeatInterval invalid: " + wantedBeatInterval.ToString(), this);
        }
    }

    public void RemoveResponder(BeatResponder oldResponder, float oldBeatInterval)
    {
        if (beatTrackers.ContainsKey(oldBeatInterval))
        {
            beatTrackers[oldBeatInterval].Remove(oldResponder);
        }
        else
        {
            Debug.LogError("BeatInterval invalid: " + oldBeatInterval.ToString(), this);
        }
    }

    public float ElapsedSongTime
    {
        get { return (float)(audioSource.timeSamples) / (float)(audioSource.clip.frequency); }
    }

    void Awake()
    {
        //the 180s should be changed when weapon is selected(change music parameters will be called)
        if (musicParameters == null)
        {
            musicParameters = startingMusicParameters;
        }


        foreach (float num in availableBeatIntervals)
        {
            beatTrackers.Add(num, new BeatTracker(180, num, offset));
        }
        melodyTracker = new MelodyTracker(180);
        _audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator playMusicAfterDelay(float delay)
    {
        //yield return new WaitForSeconds(delay);
        yield return 0;
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<AudioSource>().isPlaying)
        {
            foreach (KeyValuePair<float, BeatTracker> kvp in beatTrackers)
            {
                kvp.Value.Update();
            }
            melodyTracker.Update();
        }
    }

    public void ChangeMusicParameters(MusicParameters newParameters)
    {
        musicParameters = newParameters;
        float timeInSong = ElapsedSongTime;
        GetComponent<AudioSource>().Stop();

        GetComponent<AudioSource>().clip = newParameters.Song;
        GetComponent<AudioSource>().Play();

        if (timeInSong < minStartingTime)
            timeInSong = minStartingTime;
        audioSource.time = (audioSource.clip.length > timeInSong) ? timeInSong : 0;

        foreach (KeyValuePair<float, BeatTracker> kvp in beatTrackers)
        {
            BeatTracker bt = kvp.Value;
            bt.ChangeMusicParameters(newParameters.BPM, newParameters.Offset);
        }
        if (newParameters.Beatmap != null)
        {
            melodyTracker.ChangeMusicParameters(newParameters.Beatmap);
        }
        else
        {
            melodyTracker.ChangeMusicParameters(newParameters.BPM);
        }
        if (HUDManager.instance != null && HUDManager.instance.NoteTracker != null)
            HUDManager.instance.NoteTracker.SendMessage("RecalculateNotes");
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }


}
