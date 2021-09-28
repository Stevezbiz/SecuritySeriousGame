using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] Button saveButton;

    // Start is called before the first frame update
    void Start() {
        string path = Application.persistentDataPath + "/savedata.data";

        if (!System.IO.File.Exists(path)) saveButton.interactable = false;
    }

    public void NewGameButton() {
        SceneLoader.LoadScene("Game");
    }

    public void LoadGameButton() {
        SaveSystem.load = true;
        SceneLoader.LoadScene("Game");
    }

    public void ExitButton() {
        SceneLoader.ExitGame();
    }
}
