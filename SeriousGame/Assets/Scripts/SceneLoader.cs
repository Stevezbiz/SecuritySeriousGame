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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.OpenURL("about:blank");
#endif
    }
}
