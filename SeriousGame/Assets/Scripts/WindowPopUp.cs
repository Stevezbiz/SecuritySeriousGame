using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowPopUp : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;

    float oldTimeScale;

    /**
     * <summary>Populate the message with the text to show</summary>
     */
    public void Load(string message) {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        messageText.SetText(message);
    }

    /**
     * <summary>Close the pop-up window</summary>
     */
    public void CloseButton() {
        Time.timeScale = oldTimeScale;
        Destroy(gameObject);
    }
}
