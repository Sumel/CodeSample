using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;


public class UINoteTracker : MonoBehaviour
{

    [SerializeField]
    private RectTransform perfectZone = null;
    [SerializeField]
    private RectTransform partialZone = null;
    [SerializeField]
    private Transform noteObject = null;
    [SerializeField]
    private readonly float pixelsToSeconds = 600;
    [SerializeField]
    private float debugOffset = 0;
    MelodyTracker melodyTracker;
    MusicManager musicManager;
    GameManager gameManager;
    private Transform movingNotesObject;

    private void KillAllNotes()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in movingNotesObject)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }


    private void InstantiateNotes()
    {
        Note[] notes = melodyTracker.Notes;

        foreach (Note note in notes)
        {
            Vector3 position = new Vector3(note.Position * pixelsToSeconds, 0, 0);
            RectTransform noteUI = (RectTransform)GameObject.Instantiate(noteObject, new Vector3(), Quaternion.identity);
            position.x -= noteUI.sizeDelta.x / 2.0f;
            noteUI.SetParent(movingNotesObject, false);
            noteUI.localPosition = position;
        }
    }

    public void RecalculateNotes()
    {
        if (musicManager == null)
        {
            musicManager = MusicManager.instance;
            gameManager = GameManager.instance;
            melodyTracker = musicManager.melodyTracker;
        }
        KillAllNotes();
        InstantiateNotes();
    }

    private void Reposition(RectTransform rectTrans)
    {
        Vector3 position = rectTrans.position;
        position.x -= rectTrans.sizeDelta.x / 2.0f;
        rectTrans.position = position;
    }

    public void ResizeZones()
    {
        //this is neccesary so that we can manipulate sizes in runtime
        Vector3 position = perfectZone.position;
        position.x += perfectZone.sizeDelta.x / 2.0f;
        perfectZone.position = position;
        position = partialZone.position;
        position.x += partialZone.sizeDelta.x / 2.0f;
        partialZone.position = position;

        perfectZone.sizeDelta = new Vector2(gameManager.PerfectThreshold * 2 * pixelsToSeconds, perfectZone.sizeDelta.y);
        partialZone.sizeDelta = new Vector2(gameManager.PartialThreshold * 2 * pixelsToSeconds, partialZone.sizeDelta.y);
        Reposition(perfectZone);
        Reposition(partialZone);
    }

    void Awake()
    {
        movingNotesObject = transform.FindChild("MovingNotes");
        if (movingNotesObject == null)
        {
            Debug.LogWarning("MovingNotes object not found in HUD");
        }
    }

    void Start()
    {
        musicManager = MusicManager.instance;
        gameManager = GameManager.instance;
        melodyTracker = musicManager.melodyTracker;

        InstantiateNotes();

        ResizeZones();
    }

    void Update()
    {
        Vector3 pos = perfectZone.position;
        pos.x = transform.position.x;
        pos.x -= (musicManager.ElapsedSongTime + debugOffset) * pixelsToSeconds;
        movingNotesObject.position = pos;
    }
}
