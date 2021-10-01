using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader {
    /**
     * <summary>Load the specified scene</summary>
     */
    public static void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    /**
     * <summary>Close the game</summary>
     */
    public static void ExitGame() {
        Application.Quit();
    }
}
