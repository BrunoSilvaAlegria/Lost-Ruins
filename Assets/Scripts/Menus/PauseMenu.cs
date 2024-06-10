using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    //[SerializeField] private AudioSource clickSound;

    public void Pause()
    {
        //clickSound.Play();
        pauseMenu.SetActive(true);
        Time.timeScale = 0.000001f;
    }
    public void BackToMainMenu()
    {
        //clickSound.Play();
        SceneManager.LoadScene(0);
    }
    public void Resume()
    {
        //clickSound.Play();
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void Restart()
    {
        //clickSound.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }
}
