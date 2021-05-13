using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Station : MonoBehaviour {
    string IP;
    string MAC;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public string GetIP() {
        return IP;
    }

    public void SetIP(string value) {
        IP = value;
    }

    public string GetMAC() {
        return MAC;
    }

    public void SetMAC(string value) {
        MAC = value;
    }

    public Vector3 GetPos() {
        return gameObject.transform.position;
    }
}
