using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_MessageRecord : MonoBehaviour {
    [SerializeField] GameObject message;
    [SerializeField] TMPro.TextMeshProUGUI idText;
    [SerializeField] TMPro.TextMeshProUGUI sourceIPText;
    [SerializeField] TMPro.TextMeshProUGUI destIPText;

    int id;
    string sourceIP;
    string destIP;
    string sourceMAC;
    string destMAC;
    string body;

    LVL2_Network network;
    GameObject canvas;
    GameObject messageList;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
        canvas = network.GetCanvas();
        messageList = network.GetMessageList();

        idText.SetText(id.ToString());
        sourceIPText.SetText(network.GetIP(sourceIP));
        destIPText.SetText(network.GetIP(destIP));
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClick() {
        GameObject newMessage = Instantiate(message);
        newMessage.transform.SetParent(canvas.transform);
        newMessage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        LVL2_Message mess = newMessage.GetComponent<LVL2_Message>();
        mess.SetId(id);
        mess.SetSourceIP(sourceIP);
        mess.SetDestIP(destIP);
        mess.SetSourceMAC(sourceMAC);
        mess.SetDestMAC(destMAC);
        mess.SetBody(body);
        messageList.SetActive(false);
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
