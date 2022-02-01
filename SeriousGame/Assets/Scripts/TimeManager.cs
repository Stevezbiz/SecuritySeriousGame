using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeManager {
    static float timeScale = 0f;
    static int n;
    
    public static void Pause() {
        if (n == 0) {
            // first to pause
            timeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        n++;
    }

    public static void Resume() {
        n--;
        if (n == 0) {
            // last to resume
            Time.timeScale = timeScale;
        }
    }
}
