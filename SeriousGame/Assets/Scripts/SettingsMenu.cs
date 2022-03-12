using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject audioSettings;
    [SerializeField] AudioSource audioSource;
    [SerializeField] SaveSystem saveSystem;

    /**
     * <summary>Open the setting menu</summary>
     */
    public void OpenSettings() {
        TimeManager.Pause();
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
        saveSystem.SaveGame(gameManager.SaveGame(), gameManager.SaveModel());
    }

    /**
     * <summary>Load a saved game</summary>
     */
    public void LoadGameButton() {
        IOUtils.load = true;
        SceneLoader.ReloadScene();
    }

    /**
     * <summary>Go to the main menu</summary>
     */
    public void ExitButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
