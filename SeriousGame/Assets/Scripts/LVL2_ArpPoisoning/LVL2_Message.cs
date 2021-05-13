using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_Message : MonoBehaviour {
    LVL2_Network network;
    GameObject canvas;
    GameObject gui;
    GameObject messageList;
    LVL2_Notes notes;

    [SerializeField] GameObject messageToSend;
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] TMPro.TextMeshProUGUI sourceIPText;
    [SerializeField] TMPro.TextMeshProUGUI destIPText;
    [SerializeField] TMPro.TextMeshProUGUI sourceMACText;
    [SerializeField] TMPro.TextMeshProUGUI destMACText;
    [SerializeField] TMPro.TMP_InputField bodyText;

    int id;
    string sourceIP;
    string destIP;
    string sourceMAC;
    string destMAC;
    string body;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
        canvas = network.GetCanvas();
        gui = network.GetGui();
        messageList = network.GetMessageList();
        notes = network.GetNotes();

        titleText.SetText("MESSAGGIO " + id.ToString());
        sourceIPText.SetText(network.GetIP(sourceIP));
        destIPText.SetText(network.GetIP(destIP));
        sourceMACText.SetText(network.GetMAC(sourceMAC));
        destMACText.SetText(network.GetMAC(destMAC));
        bodyText.text = body;
    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseButton() {
        Time.timeScale = 1;
        Destroy(gameObject);
        gui.SetActive(true);
    }

    public void ModifyButton() {
        GameObject newMessage = Instantiate(messageToSend);
        newMessage.transform.SetParent(canvas.transform);
        newMessage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        LVL2_MessageToSend newMessageToSend = newMessage.GetComponent<LVL2_MessageToSend>();
        newMessageToSend.SetSourceIP(sourceIP);
        newMessageToSend.SetDestIP(destIP);
        newMessageToSend.SetSourceMAC(sourceMAC);
        newMessageToSend.SetDestMAC(destMAC);
        newMessageToSend.SetBody(body);
        Destroy(gameObject);
    }

    public void CopyButton() {
        int init;
        int end;
        if (bodyText.caretPosition > bodyText.selectionAnchorPosition) {
            init = bodyText.selectionAnchorPosition;
            end = bodyText.caretPosition;
        } else {
            init = bodyText.caretPosition;
            end = bodyText.selectionAnchorPosition;
        }
        if (init != end) {
            Debug.Log(bodyText.text.Substring(init, end - init));
            notes.AddNote(bodyText.text.Substring(init, end - init));
        }
    }

    public void BackArrowButton() {
        messageList.SetActive(true);
        Destroy(gameObject);
    }

    public void SetId(int value) {
        id = value;
    }

    public void SetSourceIP(string value) {
        sourceIP = value;
    }

    public void SetDestIP(string value) {
        destIP = value;
    }

    public void SetSourceMAC(string value) {
        sourceMAC = value;
    }

    public void SetDestMAC(string value) {
        destMAC = value;
    }

    public void SetBody(string value) {
        body = value;
    }
}
