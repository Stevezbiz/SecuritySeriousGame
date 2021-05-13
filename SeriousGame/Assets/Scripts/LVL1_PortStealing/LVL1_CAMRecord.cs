using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_CAMRecord : MonoBehaviour {
    LVL1_Network network;

    [SerializeField] TMPro.TextMeshProUGUI MACText;
    [SerializeField] RectTransform arrowImage;

    string station;
    string port;

    Dictionary<string, Vector3> arrowRotations = new Dictionary<string, Vector3>{
        {"PortLeft", new Vector3(0, 0, 0)},
        {"PortBottom", new Vector3(0, 0, 90)},
        {"PortRight", new Vector3(0, 0, 180)},
        {"PortTop", new Vector3(0, 0, -90)}
    };

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();

        MACText.SetText(network.GetMAC(station));
        arrowImage.Rotate(arrowRotations[port]);
    }

    // Update is called once per frame
    void Update() {

    }

    public void SetStation(string value) {
        station = value;
    }

    public void SetPort(string value) {
        port = value;
    }
}
