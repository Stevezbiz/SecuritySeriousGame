using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class PauseManager : MonoBehaviour {
    [SerializeField] Button pauseButton;
    [SerializeField] Button playButton;
    [SerializeField] Button fastButton;

    public void Pause() {
        pauseButton.interactable = false;
        playButton.interactable = true;
        fastButton.interactable = true;
        Time.timeScale = 0f;
    }

    public void Play() {
        pauseButton.interactable = true;
        playButton.interactable = false;
        fastButton.interactable = true;
        Time.timeScale = 1f;
    }

    public void Fast() {
        pauseButton.interactable = true;
        playButton.interactable = true;
        fastButton.interactable = false;
        Time.timeScale = 2f;
    }
}
