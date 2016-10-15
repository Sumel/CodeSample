using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float perfectThreshold = 0.1f;
    [SerializeField]
    private float partialThreshold = 0.3f;
    [SerializeField]
    private float invulnerabilityTime = 1.0f;
    [SerializeField]
    private float outOfControlTime = 0.2f;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float _playerStartingHealth = 100;
    private Vector2 playerStartingPosition;

    private HUDManager hudManager;

    private float thresholdsMultiplier = 1.0f;

    #endregion
    #region Properties
    public float ThresholdsMultiplier
    {
        get { return thresholdsMultiplier; }
        set
        {
            thresholdsMultiplier = value;
            hudManager.NoteTracker.gameObject.GetComponent<UINoteTracker>().ResizeZones();
        }
    }
    public float PerfectThreshold
    {
        get { return perfectThreshold * ThresholdsMultiplier; }
    }
    public float PartialThreshold
    {
        get { return partialThreshold * ThresholdsMultiplier; }
    }
    public GameObject PlayerGameObject
    {
        get { return player; }
    }
    public float InvulnerabilityTime
    {
        get { return invulnerabilityTime; }
    }
    public float OutOfControlTime
    {
        get { return outOfControlTime; }
    }
    public float PlayerStartingHealth
    {
        get { return _playerStartingHealth; }
    }
    #endregion
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    void Start()
    {
        playerStartingPosition = PlayerGameObject.transform.position;
        player.SendMessage("SetHealth", PlayerStartingHealth);
        hudManager = HUDManager.instance;
    }

    void Update()
    {

    }

    public void OnPlayerDeath()
    {
        player.SendMessage("SetHealth", PlayerStartingHealth);
        player.transform.position = playerStartingPosition;
        hudManager.HealthDisplay.SendMessage("SetDisplayedHealth", PlayerStartingHealth);
    }

    public static void DelayedFunction(float delay, Action action)
    {
        instance.StartCoroutine(IEDelayedFunction(delay, action));
    }

    private static IEnumerator IEDelayedFunction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public void PauseEditor()
    {
        Time.timeScale = 0;
        EditorApplication.isPaused = true;
    }

    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
