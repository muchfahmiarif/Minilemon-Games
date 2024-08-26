using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script used in "MainMenu" Scene
public class MainMenu : MonoBehaviour
{
    //Load Main Scene
    public void Main()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //Load Credits Scene
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    //Load Game Scene
    public void StartGame() {
        SceneManager.LoadScene(1);
    }

    //Quit Game
    public void QuitGame() {
        Debug.Log("Button pressed");
        Application.Quit();
    }

    //Open Browser to a link
    public void OpenUrl(string link){
        Application.OpenURL(link);
    }
}
