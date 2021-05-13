using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Switch : MonoBehaviour {
    LVL1_Network network;

    [SerializeField] GameObject gui;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject packet;
    [SerializeField] GameObject switchTab;

    Dictionary<string, string> camTable = new Dictionary<string, string>();
    Dictionary<string, Vector3> destTable = new Dictionary<string, Vector3>();

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();
        camTable.Add("alice", "PortLeft");
        camTable.Add("mallory", "PortBottom");
        camTable.Add("bob", "PortRight");
        destTable.Add("PortLeft", network.GetPos("alice"));
        destTable.Add("PortBottom", network.GetPos("mallory"));
        destTable.Add("PortRight", network.GetPos("bob"));
    }

    // Update is called once per frame
    void Update() {

    }

    public void ReceivedPacket(string port, LVL1_Packet receivedPacket) {
        // update CAM table
        camTable[receivedPacket.GetSourceMAC()] = port;
        // forward packet
        if (network.GetPos(receivedPacket.GetDestIP()) != network.GetPos("switch")) {
            LVL1_Packet packetToSend = Instantiate(packet).GetComponent<LVL1_Packet>();
            packetToSend.SetSourceIP(receivedPacket.GetSourceIP());
            packetToSend.SetDestIP(receivedPacket.GetDestIP());
            packetToSend.SetSourceMAC(receivedPacket.GetSourceMAC());
            packetToSend.SetDestMAC(receivedPacket.GetDestMAC());
            packetToSend.SetBody(receivedPacket.GetBody());
            packetToSend.SetSourcePos(network.GetPos("switch"));
            packetToSend.SetDestPos(destTable[camTable[receivedPacket.GetDestMAC()]]);
        }
    }

    public Dictionary<string, string> GetCAMTable() {
        return camTable;
    }

    public void SwitchClick() {
        Time.timeScale = 0;
        gui.SetActive(false);
        GameObject tab = Instantiate(switchTab);
        tab.transform.SetParent(canvas.transform);
        LVL1_SwitchTab script = tab.GetComponent<LVL1_SwitchTab>();
        script.SetSwitch(gameObject.GetComponent<LVL1_Switch>());
    }
}
