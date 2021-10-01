using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScreen : MonoBehaviour {
    /**
     * <summary>Load the scene of the main menu</summary>
     */
    public void PlayButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
