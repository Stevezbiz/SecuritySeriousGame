using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] Button loadButton;
    [SerializeField] TextMeshProUGUI loadText;
    [SerializeField] Outline loadOutline;

    // Start is called before the first frame update
    void Start() {
        // disable the possibility to load a game in case of missing save file
        if (!System.IO.File.Exists(IOUtils.GetPlayerGameSavePath(SaveSystem.player))) {
            loadButton.interactable = false;
            loadText.color = COLOR.GREEN_DISABLED;
            loadOutline.effectColor = COLOR.GREEN_DISABLED;
        }
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

    public void Logout() {
        SaveSystem.player = "";
        SaveSystem.load = false;
        SceneLoader.LoadScene("LoginMenu");
    }
}
