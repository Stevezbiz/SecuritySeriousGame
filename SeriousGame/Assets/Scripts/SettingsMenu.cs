using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    
    float oldTimeScale = 1;

    /**
     * <summary>Open the setting menu</summary>
     */
    public void OpenSettings() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
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
    }

    /**
     * <summary>Go to the main menu</summary>
     */
    public void ExitButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
