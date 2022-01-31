using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] Button loadButton;
    [SerializeField] TextMeshProUGUI loadText;
    [SerializeField] Outline loadOutline;

    float oldTimeScale = 1;

    /**
     * <summary>Open the setting menu</summary>
     */
    public void OpenSettings() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // disable the possibility to load a game in case of missing save file
        string path = Application.persistentDataPath + "/savedata.data";
        if (!System.IO.File.Exists(path)) {
            loadButton.interactable = false;
            loadText.color = COLOR.GREEN_DISABLED;
            loadOutline.effectColor = COLOR.GREEN_DISABLED;
        } else {
            loadButton.interactable = true;
            loadText.color = COLOR.GREEN;
            loadOutline.effectColor = COLOR.GREEN;
        }
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the setting menu and resume the game</summary>
     */
    public void CancelButton() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
    }

    /**
     * <summary>Save the game data</summary>
     */
    public void SaveButton() {
        SaveSystem.SaveGame(gameManager.SaveGame());
        BKTModel.SaveModel(gameManager.SaveModel());
        GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
        newWindow.transform.SetParent(gameObject.transform, false);
        gameManager.DisplayMessage("Partita salvata", ActionCode.CONTINUE, Role.SECURITY);
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
}
