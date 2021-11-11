using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public static class SceneLoader {
    [DllImport("__Internal")]
    private static extern void Redirect(string url);
    
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
        Redirect("about:blank");
#endif
    }
}
