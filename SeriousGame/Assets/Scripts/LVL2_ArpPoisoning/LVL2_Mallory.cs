using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_Mallory : MonoBehaviour {
    LVL2_Network network;
    [SerializeField] LVL2_MessageList messageList;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        LVL2_Packet receivedPacket = collision.gameObject.GetComponent<LVL2_Packet>();
        messageList.AddMessage(receivedPacket.GetSourceIP(), receivedPacket.GetDestIP(), receivedPacket.GetSourceMAC(), receivedPacket.GetDestMAC(), receivedPacket.GetBody());
        Destroy(collision.gameObject);
    }
}
