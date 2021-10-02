using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance { get; private set; }

    public bool gameOver;
    public AudioMixer mixer;
    
    int currentScene;
    int totalLevel;

    void Start()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        totalLevel = SceneManager.sceneCountInBuildSettings;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrent();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Screen.fullScreen = false; 
            UiManager.Instance.UpdateFullScreenText(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleFullScreen();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            float volume;
            mixer.GetFloat("MusicVolume", out volume);
            if (volume == -80f)
                volume = 0;
            else 
                volume = -80f;
            mixer.SetFloat("MusicVolume", volume);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            float volume;
            mixer.GetFloat("SfxVolume", out volume);
            if (volume == -80f)
                volume = 0;
            else 
                volume = -80f;
            mixer.SetFloat("SfxVolume", volume);
        }
    }

    public void RestartCurrent()
    {
        gameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gameOver = true;
        UiManager.Instance.ShowEnd();
    }

    public void NextLevel()
    {
        StartCoroutine(DelayNextLevel(1));
    }

    // IEnumerator DelayRestart(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     RestartCurrent();
    // }

    IEnumerator DelayNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(++currentScene);
        Time.timeScale = 1;
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(currentScene = index);
        Time.timeScale = 1;
    }

    public void PlayAgain()
    {
        LoadLevel(0);
        Time.timeScale = 1;
    }

    public void ToggleFullScreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            UiManager.Instance.UpdateFullScreenText(false);
        }
        else
        {
            Resolution[] resolutions = Screen.resolutions;
            Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);
            UiManager.Instance.UpdateFullScreenText(true);
        }
    }
}