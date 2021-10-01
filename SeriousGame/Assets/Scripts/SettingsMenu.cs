using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    
    float oldTimeScale = 1;

    public void OpenSettings() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void CancelButton() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
    }

    public void SaveButton() {
        SaveSystem.SaveGame(gameManager.SaveGame());
    }

    public void ExitButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
