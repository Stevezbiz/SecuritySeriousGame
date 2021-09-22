using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowPopUp : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;

    string message;
    float oldTimeScale;

    public string Message { get => message; set => message = value; }

    // Start is called before the first frame update
    void Start() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        messageText.SetText(message);
    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseButton() {
        Time.timeScale = oldTimeScale;
        Destroy(gameObject);
    }
}
