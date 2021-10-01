using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] Button saveButton;

    // Start is called before the first frame update
    void Start() {
        // disable the possibility to load a game in case of missing save file
        string path = Application.persistentDataPath + "/savedata.data";
        if (!System.IO.File.Exists(path)) saveButton.interactable = false;
    }

    /**
     * <summary>Create a new game</summary>
     */
    public void NewGameButton() {
        SceneLoader.LoadScene("Game");
    }

    /**
     * <summary>Load a saved game</summary>
     */
    public void LoadGameButton() {
        SaveSystem.load = true;
        SceneLoader.LoadScene("Game");
    }

    /**
     * <summary>Close the game</summary>
     */
    public void ExitButton() {
        SceneLoader.ExitGame();
    }
}
