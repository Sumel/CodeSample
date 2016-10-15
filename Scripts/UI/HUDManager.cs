using UnityEngine;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    private static HUDManager _instance;
    public static HUDManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HUDManager>();
            }
            return _instance;
        }
    }
    Transform _shotRating;
    public Transform ShotRating
    {
        get { return _shotRating; }
        private set { _shotRating = value; }
    }
    Transform _chargeRating;
    public Transform ChargeRating
    {
        get { return _chargeRating; }
        private set { _chargeRating = value; }
    }
    Transform _healthDisplay;
    public Transform HealthDisplay
    {
        get { return _healthDisplay; }
        private set { _healthDisplay = value; }
    }
    Transform _ammoDisplay;
    public Transform AmmoDisplay
    {
        get { return _ammoDisplay; }
        private set { _ammoDisplay = value; }
    }
    Transform _noteTracker;
    public Transform NoteTracker
    {
        get { return _noteTracker; }
        private set { _noteTracker = value; }
    }

    void Awake()
    {
        ShotRating = transform.FindChild("ShotRating");
        if (ShotRating == null)
        {
            Debug.LogWarning("ShotRating object not found in HUD");
        }
        ChargeRating = transform.FindChild("ChargeRating");
        if (ChargeRating == null)
        {
            Debug.LogWarning("ChargeRating object not found in HUD");
        }
        HealthDisplay = transform.FindChild("HealthDisplay");
        if (HealthDisplay == null)
        {
            Debug.LogWarning("HealthDisplay object not found in HUD");
        }
        AmmoDisplay = transform.FindChild("AmmoDisplay");
        if (AmmoDisplay == null)
        {
            Debug.LogWarning("AmmoDisplay object not found in HUD");
        }
        NoteTracker = transform.FindChild("NoteTracker");
        if (NoteTracker == null)
        {
            Debug.LogWarning("NoteTracker object not found in HUD");
        }

    }


    void Start()
    {

    }


}
