using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GUI gui;
    
    float oldTimeScale = 1;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

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
        SaveSystem.SaveGame(gui.SaveGame());
    }

    public void ExitButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
