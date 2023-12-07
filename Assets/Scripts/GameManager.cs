using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameSession gameSession;
    [SerializeField] PlayerMovement playerScript;
    [SerializeField] Canvas deathScreen;
    bool playerHasMoved = false;


    private void Awake()
    {
        gameSession = FindObjectOfType<GameSession>();
    }
    private void Update()
    {
        if (!playerScript.IsAlive) // If player is dead
        {
            InvokeDeathScreen(2.2f);
        }
    }

    /// <summary>
    /// Calls the ShowDeathScreen function after a set time
    /// </summary>
    /// <param name="time">time before call of ShowDeathScreen</param>
    void InvokeDeathScreen(float time)
    {
        Invoke("ShowDeathScreen", time);
    }
    /// <summary>
    /// Sets the DeathScreen canvas gameobject visibility to "true";
    /// </summary>
    void ShowDeathScreen()
    {
        deathScreen.gameObject.SetActive(true);
    }
    /// <summary>
    /// Reloads active scene
    /// </summary>
    public void ReloadActiveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// Quits the game, whether in release build, or in editor mode
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // This will quit the game in the Unity Editor
        EditorApplication.ExitPlaymode();
        #else    
        Application.Quit();
        #endif
    }
    /// <summary>
    /// Co-routine that waits 2 seconds before checking the next level.
    /// <para>If next level doesn't exist, goes to end screen</para>
    /// <para>Else, goes to next level</para>
    /// </summary>
    /// <returns>Coroutine</returns>
    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings)
        {
            gameSession.StopTimer();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    /// <summary>
    /// Starts the LoadNextLevelAfterDelay() co-routine
    /// </summary>
    public void NextLevelRoutineStart()
    {
        StartCoroutine(LoadNextLevelAfterDelay(0));
    }
    /// <summary>
    /// Turns the playerHasMoved bool true
    /// </summary>
    public void PlayerMoved()
    {
        playerHasMoved = true;
    }
    /// <summary>
    /// Checks on the hasPlayerMoved bool and returns its status for public access
    /// </summary>
    /// <returns></returns>
    public bool PlayerHasMoved()
    {
        if (playerHasMoved) { return true; }
        return false;
    }
}
