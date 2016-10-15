using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class BeatSizeChanger : SizeChanger, BeatResponder
{

    [SerializeField]
    private float beatInterval = 1f;
    MusicManager musicManager;
    public override void Start()
    {
        base.Start();
        musicManager = MusicManager.instance;
        musicManager.AddNewResponder(this, beatInterval);
    }

    public void OnBeat(float timeOfBeat, float beat)
    {
        TryChangingSize();
    }

    public void OnDestroy()
    {
        musicManager.RemoveResponder(this, beatInterval);
    }

}
