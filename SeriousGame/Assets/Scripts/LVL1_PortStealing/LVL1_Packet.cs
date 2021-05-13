using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Packet : MonoBehaviour {
    LVL1_Network network;
    bool colliderOn = false;
    [SerializeField] float speed = 3f;

    string sourceIP;
    string destIP;
    string sourceMAC;
    string destMAC;
    string body;

    Vector3 sourcePos;
    Vector3 destPos;

    void Awake() {

    }

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();
        gameObject.transform.position = sourcePos;
    }

    // Update is called once per frame
    void Update() {

    }

    private void FixedUpdate() {
        if (!colliderOn && Vector3.Distance(transform.position, sourcePos) > 1f) {
            gameObject.AddComponent<CircleCollider2D>();
            colliderOn = true;
        }
        transform.position = Vector3.MoveTowards(transform.position, destPos, speed * Time.deltaTime);
    }

    public string GetSourceIP() {
        return sourceIP;
    }

    public string GetDestIP() {
        return destIP;
    }

    public string GetSourceMAC() {
        return sourceMAC;
    }

    public string GetDestMAC() {
        return destMAC;
    }

    public string GetBody() {
        return body;
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

    public Vector3 GetSourcePos() {
        return sourcePos;
    }

    public Vector3 GetDestPos() {
        return destPos;
    }

    public void SetSourcePos(Vector3 value) {
        sourcePos = value;
    }

    public void SetDestPos(Vector3 value) {
        destPos = value;
    }
}
