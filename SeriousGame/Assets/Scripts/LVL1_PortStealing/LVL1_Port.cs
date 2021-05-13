using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Port : MonoBehaviour {
    [SerializeField] LVL1_Switch hub;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        LVL1_Packet receivedPacket = collision.gameObject.GetComponent<LVL1_Packet>();
        hub.ReceivedPacket(gameObject.name, receivedPacket);
        Destroy(collision.gameObject);
    }
}
