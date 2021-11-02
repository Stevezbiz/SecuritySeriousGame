using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Message : MonoBehaviour {
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI messageText;

    public void Load(string name, string message) {
        nameText.SetText(name);
        messageText.SetText(message);
    }

    public void Close() {
        Destroy(gameObject);
    }
}
