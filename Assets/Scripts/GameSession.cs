using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    [SerializeField] AudioSource ost;
    [SerializeField] Canvas EndGameCanvas;
    [SerializeField] GameManager gameManager;
    [SerializeField] Canvas timerCanvas;
    float timeElapsed = 0;
    bool timerStarted = false;
    float startTime = 0;

    [SerializeField] TMP_Text timerText;
    private void Awake()
    {
        if (!timerCanvas.gameObject.activeSelf)
        {
            timerCanvas.gameObject.SetActive(true);
        }
        SingletonInit();
        if (!timerStarted)
        {
            StartCoroutine(TimerInit());
        }
    }

    public void StopTimer()
    {
        timerStarted = false;
        EndGameCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    /// <summary>
    /// Initialization of the singleton pattern for the Game Session
    /// </summary>
    void SingletonInit()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    /// <summary>
    /// Waits for the first move of the player (sent by GameManager) to initialize the timer and turn timerStarted to true
    /// </summary>
    /// <returns>coroutine</returns>
    IEnumerator TimerInit()
    {
        yield return new WaitUntil(() => gameManager.PlayerHasMoved());

        ost.Play();

        timerStarted = true;

        startTime = Time.time;
    }
    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void Kill()
    {
        Destroy(this.gameObject);
    }

    private void Update()
    {
        if (timerStarted)
        {
            timeElapsed = Time.time - startTime;
        }
        timerText.text = FormatTime(timeElapsed);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // This will quit the game in the Unity Editor
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

}
