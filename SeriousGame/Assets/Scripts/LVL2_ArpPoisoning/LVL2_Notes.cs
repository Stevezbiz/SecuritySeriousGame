using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_Notes : MonoBehaviour {
    LVL2_Network network;
    GameObject gui;

    [SerializeField] TMPro.TMP_InputField notesBody;
    [SerializeField] TMPro.TextMeshProUGUI resultText;
    [SerializeField] TMPro.TMP_InputField inputText;

    List<string> initNotes = new List<string> {
        "Alice | IP: 192.168.1.1 | MAC: AA:AA:AA:AA:AA:AA",
        "\nBob | IP: 192.168.1.2 | MAC: BB:BB:BB:BB:BB:BB",
        "\nYou | IP: 192.168.1.3 | MAC: CC:CC:CC:CC:CC:CC",
    };

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
        gui = network.GetGui();
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    public void AddNote(string note) {
        string text = notesBody.text;
        notesBody.text = text + "\n" + note;
    }

    public void InsertButton() {
        int res = network.EvaluateCode(inputText.text);
        inputText.text = "";
        if (res == -1) {
            resultText.text = "Codice non riconosciuto";
        } else if (res == 0) {
            resultText.text = "Hai già inserito questo codice";
        } else {
            resultText.text = "Congratulazioni! Hai individuato un codice, guadagni " + res + " punti";
        }
    }

    public void CloseButton() {
        Time.timeScale = 1;
        gui.SetActive(true);
        inputText.text = "";
        resultText.text = "";
        gameObject.SetActive(false);
    }
}
