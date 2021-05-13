using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_MessageToSend : MonoBehaviour {
    LVL2_Network network;
    GameObject gui;
    GameObject messageList;
    [SerializeField] TMPro.TMP_Dropdown sourceIPDropdown;
    [SerializeField] TMPro.TMP_Dropdown destIPDropdown;
    [SerializeField] TMPro.TMP_Dropdown sourceMACDropdown;
    [SerializeField] TMPro.TMP_Dropdown destMACDropdown;
    [SerializeField] TMPro.TMP_InputField bodyInpuField;

    [SerializeField] GameObject packet;

    string sourceIP;
    string destIP;
    string sourceMAC;
    string destMAC;
    string body;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
        gui = network.GetGui();
        messageList = network.GetMessageList();
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        sourceIPDropdown.ClearOptions();
        sourceIPDropdown.AddOptions(network.GetIPList());
        destIPDropdown.ClearOptions();
        destIPDropdown.AddOptions(network.GetIPList());
        sourceMACDropdown.ClearOptions();
        sourceMACDropdown.AddOptions(network.GetMACList());
        destMACDropdown.ClearOptions();
        destMACDropdown.AddOptions(network.GetMACList());

        List<TMPro.TMP_Dropdown.OptionData> list = sourceIPDropdown.options;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].text == network.GetIP(sourceIP)) {
                sourceIPDropdown.value = i;
                break;
            }
        }

        list = destIPDropdown.options;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].text == network.GetIP(destIP)) {
                destIPDropdown.value = i;
                break;
            }
        }

        list = sourceMACDropdown.options;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].text == network.GetMAC(sourceMAC)) {
                sourceMACDropdown.value = i;
                break;
            }
        }

        list = destMACDropdown.options;
        for (int i = 0; i < list.Count; i++) {
            if (list[i].text == network.GetMAC(destMAC)) {
                destMACDropdown.value = i;
                break;
            }
        }

        bodyInpuField.text = body;
    }

    // Update is called once per frame
    void Update() {

    }

    public void SendButton() {
        sourceIP = network.GetKeyByIP(sourceIPDropdown.options[sourceIPDropdown.value].text);
        destIP = network.GetKeyByIP(destIPDropdown.options[destIPDropdown.value].text);
        sourceMAC = network.GetKeyByMAC(sourceMACDropdown.options[sourceMACDropdown.value].text);
        destMAC = network.GetKeyByMAC(destMACDropdown.options[destMACDropdown.value].text);
        body = bodyInpuField.text;
        messageList.GetComponent<LVL2_MessageList>().AddMessage(sourceIP, destIP, sourceMAC, destMAC, body);
        LVL2_Packet packetToSend = Instantiate(packet).GetComponent<LVL2_Packet>();
        packetToSend.SetSourceIP(sourceIP);
        packetToSend.SetDestIP(destIP);
        packetToSend.SetSourceMAC(sourceMAC);
        packetToSend.SetDestMAC(destMAC);
        packetToSend.SetBody(body);
        packetToSend.SetSourcePos(network.GetPos("mallory"));
        packetToSend.SetDestPos(network.GetPos("switch"));
        Time.timeScale = 1;
        gui.SetActive(true);
        Destroy(gameObject);
    }

    public void CloseButton() {
        Time.timeScale = 1;
        gui.SetActive(true);
        Destroy(gameObject);
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
