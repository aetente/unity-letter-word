using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
    }
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
