using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_MessageList : MonoBehaviour {
    [SerializeField] GameObject listItem;
    [SerializeField] Transform spawnPoint;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject noMessageInfo;
    [SerializeField] GameObject messageToSend;
    int height = 100;

    GameObject canvas;
    GameObject gui;
    LVL2_Network network;
    List<LVL2_MessageRecord> messages = new List<LVL2_MessageRecord>();

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
        canvas = network.GetCanvas();
        gui = network.GetGui();

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        content.sizeDelta = new Vector2(0, messages.Count * height);
    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseButton() {
        Time.timeScale = 1;
        gui.SetActive(true);
        gameObject.SetActive(false);
    }

    public void AddNewMessageButton() {
        GameObject newMessage = Instantiate(messageToSend);
        newMessage.transform.SetParent(canvas.transform);
        LVL2_MessageToSend newMessageToSend = newMessage.GetComponent<LVL2_MessageToSend>();
        newMessageToSend.SetSourceIP("mallory");
        newMessageToSend.SetDestIP("mallory");
        newMessageToSend.SetSourceMAC("mallory");
        newMessageToSend.SetDestMAC("mallory");
        newMessageToSend.SetBody("");
        gameObject.SetActive(false);
    }

    public void AddMessage(string sourceIP, string destIP, string sourceMAC, string destMAC, string body) {
        Vector3 newPos = new Vector3(0, -content.sizeDelta.y, 0);
        GameObject newItem = Instantiate(listItem, newPos, Quaternion.identity);
        newItem.transform.SetParent(spawnPoint, false);
        LVL2_MessageRecord messageRecord = newItem.GetComponent<LVL2_MessageRecord>();
        messageRecord.SetId(messages.Count + 1);
        messageRecord.SetSourceIP(sourceIP);
        messageRecord.SetDestIP(destIP);
        messageRecord.SetSourceMAC(sourceMAC);
        messageRecord.SetDestMAC(destMAC);
        messageRecord.SetBody(body);
        messages.Add(messageRecord);
        content.sizeDelta = new Vector2(0, messages.Count * height);
        noMessageInfo.SetActive(false);
    }

    public void ClearMessages() {
        messages.Clear();
    }
}
