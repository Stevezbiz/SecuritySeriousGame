using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] GameObject logRecord;
    [SerializeField] RectTransform content;

    float oldTimeScale;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void LogPrint(string attack, bool hit) {
        GameObject newLog = Instantiate(logRecord);
        newLog.transform.SetParent(content, false);
        TextMeshProUGUI text = newLog.GetComponent<TextMeshProUGUI>();
        string dateTime = gui.GetDateTime();
        string desc;

        if (hit) {
            desc = "Individuato attacco " + attack;
            text.color = new Color(1f, .25f, .0f, 1f);
        } else {
            desc = "Sventato attacco " + attack;
            text.color = new Color(.0f, 1f, 1f, 1f);
        }
        text.SetText(dateTime + desc);
    }

    public void OpenLog() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void CloseLog() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
    }
}
