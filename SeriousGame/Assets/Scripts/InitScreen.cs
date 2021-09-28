using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScreen : MonoBehaviour {
    public void PlayButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
