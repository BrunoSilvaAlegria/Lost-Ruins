using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScene : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;
    
    public void QuitToMenu()
    {
        clickSound.Play();
        SceneManager.LoadSceneAsync(0);
    }
}
