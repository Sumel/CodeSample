using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class MelodySizeChanger : SizeChanger, MelodyResponder 
{

    MelodyTracker melodyTracker;
    MusicManager musicManager;
    // Use this for initialization
    public override void Start () 
    {
        base.Start();
        musicManager = MusicManager.instance;
        melodyTracker = musicManager.melodyTracker;
        melodyTracker.Add(this);

    }
    public void OnMelody(Note note)
    {
        TryChangingSize();
        HeldNote heldNote = note as HeldNote;
        if(heldNote != null)
        {
            if(heldNote.LoopIndex>0)
            { 
                ReverseRotation();
            }
            if(musicManager.GetComponent<AudioSource>().pitch!=0)
            {
                TrySpinning(heldNote.HoldTime/musicManager.GetComponent<AudioSource>().pitch);
            }

        }
    }

    public void OnDestroy()
    {
        melodyTracker.Remove(this);
    }

}
