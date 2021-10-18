using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowPopUp : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;

    float oldTimeScale;
    ActionCode action;

    /**
     * <summary>Populate the message with the text to show</summary>
     */
    public void Load(string message, ActionCode action) {
        oldTimeScale = Time.timeScale;
        this.action = action;
        Time.timeScale = 0;
        messageText.SetText(message);
    }

    /**
     * <summary>Close the pop-up window</summary>
     */
    public void CloseButton() {
        switch (action) {
            case ActionCode.CONTINUE:
                Time.timeScale = oldTimeScale;
                break;
            case ActionCode.GAME_OVER:
                SceneLoader.LoadScene("MainMenu");
                break;
            default:
                Time.timeScale = oldTimeScale;
                break;
        }
        Destroy(gameObject);
    }
}
