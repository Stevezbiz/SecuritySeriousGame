using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LVL2_Network : MonoBehaviour {
    [SerializeField] GameObject packet;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject gui;
    [SerializeField] GameObject messageList;
    [SerializeField] GameObject notes;
    [SerializeField] GameObject victoryMessage;
    [SerializeField] LVL2_Station alice;
    [SerializeField] LVL2_Station bob;
    [SerializeField] LVL2_Station mallory;
    [SerializeField] LVL2_Station hub;

    SortedDictionary<string, LVL2_Station> stations = new SortedDictionary<string, LVL2_Station>();
    Dictionary<string, int> codes = new Dictionary<string, int> {
        {"18632745",10},
        {"5up3r_53cur3",10},
        {"87a6jhy6",10},
    };

    private void Awake() {
        // set the stations in the network
        alice.SetIP("192.168.1.1 (alice)");
        alice.SetMAC("AA:AA:AA:AA:AA:AA (alice)");
        stations.Add("alice", alice);
        bob.SetIP("192.168.1.2 (bob)");
        bob.SetMAC("BB:BB:BB:BB:BB:BB (bob)");
        stations.Add("bob", bob);
        mallory.SetIP("192.168.1.3 (you)");
        mallory.SetMAC("CC:CC:CC:CC:CC:CC (you)");
        stations.Add("mallory", mallory);
        hub.SetIP("192.168.1.4 (switch)");
        hub.SetMAC("DD:DD:DD:DD:DD:DD (switch)");
        stations.Add("switch", hub);
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    private void Update() {

    }

    // positions utilities
    public Vector3 GetPos(string id) {
        return stations[id].GetPos();
    }

    // addresses utilities
    public string GetIP(string id) {
        return stations[id].GetIP();
    }
    public string GetMAC(string id) {
        return stations[id].GetMAC();
    }

    public List<string> GetIPList() {
        List<string> list = new List<string>();
        foreach (LVL2_Station s in stations.Values) {
            list.Add(s.GetIP());
        }
        return list;
    }

    public List<string> GetMACList() {
        List<string> list = new List<string>();
        foreach (LVL2_Station s in stations.Values) {
            list.Add(s.GetMAC());
        }
        return list;
    }

    public int GetStationCount() {
        return stations.Count;
    }

    public string GetKeyByIP(string IP) {
        foreach (KeyValuePair<string, LVL2_Station> s in stations) {
            if (s.Value.GetIP() == IP) {
                return s.Key;
            }
        }
        return "";
    }

    public string GetKeyByMAC(string MAC) {
        foreach (KeyValuePair<string, LVL2_Station> s in stations) {
            if (s.Value.GetMAC() == MAC) {
                return s.Key;
            }
        }
        return "";
    }

    // objects utiities
    public GameObject GetCanvas() {
        return canvas;
    }

    public GameObject GetGui() {
        return gui;
    }

    public GameObject GetMessageList() {
        return messageList;
    }

    public LVL2_Notes GetNotes() {
        return notes.GetComponent<LVL2_Notes>();
    }

    public int EvaluateCode(string code) {
        int points;
        if (codes.TryGetValue(code, out points)) {
            codes[code] = 0;
            StartCoroutine("Victory");
            return points;
        }
        return -1;
    }

    IEnumerator Victory() {
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 0;
        GameObject newPanel = Instantiate(victoryMessage);
        newPanel.transform.SetParent(canvas.transform);
        newPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }

    public void BeginLevel() {
        alice.GetComponent<LVL2_Alice>().BeginLevel();
        bob.GetComponent<LVL2_Bob>().BeginLevel();
        Time.timeScale = 1;
    }
}
