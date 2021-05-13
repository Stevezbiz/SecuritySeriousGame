using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_SwitchTab : MonoBehaviour {
    [SerializeField] GameObject camRecord;
    [SerializeField] Transform spawnPoint;
    [SerializeField] RectTransform content;

    LVL1_Network network;
    GameObject gui;
    LVL1_Switch switchScript;

    int height = 100;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();
        gui = network.GetGui();
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        int i = 0;
        foreach (KeyValuePair<string, string> record in switchScript.GetCAMTable()) {
            Vector3 newPos = new Vector3(0, -i * height, 0);
            GameObject newItem = Instantiate(camRecord, newPos, Quaternion.identity);
            newItem.transform.SetParent(spawnPoint, false);
            LVL1_CAMRecord newRecord = newItem.GetComponent<LVL1_CAMRecord>();
            newRecord.SetStation(record.Key);
            newRecord.SetPort(record.Value);
            i++;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void CloseButton() {
        Time.timeScale = 1;
        gui.SetActive(true);
        Destroy(gameObject);
    }

    public void SetSwitch(LVL1_Switch value) {
        switchScript = value;
    }
}
