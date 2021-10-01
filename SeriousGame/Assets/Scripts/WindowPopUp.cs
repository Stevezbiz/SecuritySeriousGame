using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowPopUp : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;

    float oldTimeScale;

    public void Load(string message) {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        messageText.SetText(message);
    }

    public void CloseButton() {
        Time.timeScale = oldTimeScale;
        Destroy(gameObject);
    }
}
