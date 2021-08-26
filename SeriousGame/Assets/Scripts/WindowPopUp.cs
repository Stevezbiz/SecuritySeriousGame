using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowPopUp : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;

    bool resume;
    string message;

    public bool Resume { get => resume; set => resume = value; }
    public string Message { get => message; set => message = value; }

    // Start is called before the first frame update
    void Start() {
        messageText.SetText(message);
    }

    // Update is called once per frame
    void Update() {

    }

    public void OkButton() {
        if (Resume) Time.timeScale = 1;
        Destroy(gameObject);
    }
}
