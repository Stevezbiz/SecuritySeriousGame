using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject message;

    public void OpenQuiz() {
        Application.OpenURL("https://forms.gle/HcSwFLqoxUGMFFf76");
        message.SetActive(false);
    }

    public void Back() {
        message.SetActive(false);
    }

    /**
     * <summary>Create a new game</summary>
     */
    public void NewGameButton() {
        IOUtils.tutorial = true;
        SceneLoader.LoadScene("Game");
    }

    /**
     * <summary>Load a saved game</summary>
     */
    public void LoadGameButton() {
        IOUtils.load = true;
        SceneLoader.LoadScene("Game");
    }

    /**
     * <summary>Close the game</summary>
     */
    public void ExitButton() {
        SceneLoader.ExitGame();
    }

    public void Logout() {
        IOUtils.player = "";
        IOUtils.load = false;
        SceneLoader.LoadScene("LoginMenu");
    }
}
