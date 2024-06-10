using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;

    public void StartGame()
    {
        clickSound.Play();
        SceneManager.LoadSceneAsync(1);
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        clickSound.Play();
        Application.Quit();
    }
}