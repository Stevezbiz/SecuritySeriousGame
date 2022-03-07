using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] Button loadButton;
    [SerializeField] TextMeshProUGUI loadText;
    [SerializeField] Outline loadOutline;

    /**
     * <summary>Open the setting menu</summary>
     */
    public void OpenSettings() {
        TimeManager.Pause();
        CheckGameSave();
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the setting menu and resume the game</summary>
     */
    public void CancelButton() {
        TimeManager.Resume();
        gameObject.SetActive(false);
    }

    /**
     * <summary>Save the game data</summary>
     */
    public void SaveButton() {
        SaveSystem.SaveGame(gameManager.SaveGame());
        SaveSystem.SaveModel(gameManager.SaveModel());
        gameManager.DisplayMessage("Partita salvata", ActionCode.CONTINUE, Role.SECURITY);
        CheckGameSave();
    }

    /**
     * <summary>Load a saved game</summary>
     */
    public void LoadGameButton() {
        SaveSystem.load = true;
        SceneLoader.ReloadScene();
    }

    /**
     * <summary>Go to the main menu</summary>
     */
    public void ExitButton() {
        SceneLoader.LoadScene("MainMenu");
    }

    void CheckGameSave() {
        // disable the possibility to load a game in case of missing save file
        if (!System.IO.File.Exists(IOUtils.GetPlayerGameSavePath(SaveSystem.player))) {
            loadButton.interactable = false;
            loadText.color = COLOR.GREEN_DISABLED;
            loadOutline.effectColor = COLOR.GREEN_DISABLED;
        } else {
            loadButton.interactable = true;
            loadText.color = COLOR.GREEN;
            loadOutline.effectColor = COLOR.GREEN;
        }
    }
}
