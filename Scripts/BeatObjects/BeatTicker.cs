using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class BeatTicker : MonoBehaviour, BeatResponder
{

    private MusicManager musicManager;
    public void OnBeat(float timeOfBeat, float beatInterval)
    {
        GetComponent<AudioSource>().Play();
    }

    void Start()
    {
        musicManager = MusicManager.instance;
        musicManager.AddNewResponder(this, 1.0f);
    }

}
