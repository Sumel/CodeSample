using UnityEngine;
using System.Collections;

public class UtilityInputManager : MonoBehaviour
{
    //this is temporary class with shortcuts and cheats used during debugging.
    //it won't be in the final version of the game

    private static UtilityInputManager _instance;
    public static UtilityInputManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UtilityInputManager>();
            }
            return _instance;
        }
    }

    private GameManager gameManager;
    private MusicManager musicManager;
    void Start()
    {
        gameManager = GameManager.instance;
        musicManager = MusicManager.instance;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            gameManager.RestartLevel();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            gameManager.QuitGame();
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0.0f)
        {
            musicManager.audioSource.volume += scrollInput;
        }

        int SongSkipInput = 0;

        if (Input.GetKey(KeyCode.O))
        {
            SongSkipInput = -2;
        }
        else if (Input.GetKey(KeyCode.P))
        {
            SongSkipInput = 1;
        }
        if (SongSkipInput != 0)
        {
            float newTime = musicManager.ElapsedSongTime + SongSkipInput * 5.0f * Time.deltaTime;
            if (SongSkipInput == 1)
            {
                newTime -= Time.deltaTime;
            }
            else
            {
                newTime += Time.deltaTime;
            }
            if (newTime > musicManager.audioSource.clip.length)
            {
                newTime -= musicManager.audioSource.clip.length;
            }
            if (newTime < 0)
            {
                newTime = 0;
            }
            musicManager.audioSource.time = newTime;
        }

        int ThresholdIncreaseInput = 0;

        if (Input.GetKeyDown(KeyCode.K))
        {
            ThresholdIncreaseInput = -1;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            ThresholdIncreaseInput = 1;
        }
        if (ThresholdIncreaseInput != 0)
        {
            gameManager.ThresholdsMultiplier += ThresholdIncreaseInput * 0.25f;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            musicManager.PauseMusic();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.LoadLevel("TestLevel");
        }

    }

}
