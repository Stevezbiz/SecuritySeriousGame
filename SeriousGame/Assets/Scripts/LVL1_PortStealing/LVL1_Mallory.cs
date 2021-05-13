using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Mallory : MonoBehaviour {
    LVL1_Network network;
    [SerializeField] LVL1_MessageList messageList;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        LVL1_Packet receivedPacket = collision.gameObject.GetComponent<LVL1_Packet>();
        messageList.AddMessage(receivedPacket.GetSourceIP(), receivedPacket.GetDestIP(), receivedPacket.GetSourceMAC(), receivedPacket.GetDestMAC(), receivedPacket.GetBody());
        Destroy(collision.gameObject);
    }
}
