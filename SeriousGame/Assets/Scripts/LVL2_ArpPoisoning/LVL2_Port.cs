using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_Port : MonoBehaviour {
    [SerializeField] LVL2_Switch hub;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        LVL2_Packet receivedPacket = collision.gameObject.GetComponent<LVL2_Packet>();
        hub.ReceivedPacket(gameObject.name, receivedPacket);
        Destroy(collision.gameObject);
    }
}
