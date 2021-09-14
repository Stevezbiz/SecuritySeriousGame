using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
    [SerializeField] GameObject pauseImage;
    [SerializeField] GameObject playImage;

    bool stop;

    // Start is called before the first frame update
    void Start() {
        stop = false;
    }

    // Update is called once per frame
    void Update() {

    }

    public void StopOrResume() {
        if (stop) {
            // game is frozen -> resume
            Time.timeScale = 1;
            pauseImage.SetActive(true);
            playImage.SetActive(false);
        } else {
            // game is going -> stop
            Time.timeScale = 0;
            pauseImage.SetActive(false);
            playImage.SetActive(true);
        }
        stop = !stop;
    }
}