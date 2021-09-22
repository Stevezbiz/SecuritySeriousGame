using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] GameObject logRecord;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject next;
    [SerializeField] GameObject previous;

    float oldTimeScale;
    int nLines = 0;
    int nPages = 1;
    int currentPage;
    const int nLinesStep = 20;
    List<LogLine> lines = new List<LogLine>();
    List<GameObject> toDestroy = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void PrintCurrentPage() {
        foreach (GameObject go in toDestroy) Destroy(go);
        toDestroy.Clear();

        int i = (currentPage - 1) * nLinesStep;
        int N;
        if (currentPage == nPages) N = i + nLines;
        else N = i + nLinesStep;

        for (int j = i; j < N; j++) {
            GameObject newLog = Instantiate(logRecord);
            newLog.transform.SetParent(content, false);
            toDestroy.Add(newLog);
            TextMeshProUGUI text = newLog.GetComponent<TextMeshProUGUI>();
            text.SetText(lines[j].line);
            text.color = lines[j].color;
        }
    }

    public void LogPrintAttack(string attack, bool hit) {
        if (nLines == nLinesStep) {
            nLines = 0;
            nPages++;
        }

        string dateTime = gui.GetDateTime();
        string desc;
        Color color;

        if (hit) {
            desc = "Individuato attacco " + attack;
            color = COLOR.LOG_RED;
        } else {
            desc = "Sventato attacco " + attack;
            color = COLOR.LOG_BLUE;
        }
        lines.Add(new LogLine(dateTime + desc, color));
        nLines++;
    }

    public void LogPrintItem(string item, ActionCode action) {
        if (nLines == nLinesStep) {
            nLines = 0;
            nPages++;
        }

        string dateTime = gui.GetDateTime();
        string desc;

        switch (action) {
            case ActionCode.PURCHASE:
                desc = "Acquistato " + item;
                break;
            case ActionCode.ENABLE:
                desc = "Abilitato " + item;
                break;
            case ActionCode.DISABLE:
                desc = "Disabilitato " + item;
                break;
            default:
                return;
        }
        lines.Add(new LogLine(dateTime + desc, COLOR.LOG_GREEN));
        nLines++;
    }

    public void OpenLog() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        currentPage = nPages;

        if (currentPage > 1) previous.SetActive(true);

        PrintCurrentPage();

        gameObject.SetActive(true);
    }

    public void CloseLog() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        previous.SetActive(false);
        next.SetActive(false);
    }

    public void NextPage() {
        currentPage++;
        PrintCurrentPage();

        previous.SetActive(true);
        if (currentPage == nPages) next.SetActive(false);
    }

    public void PreviousPage() {
        currentPage--;
        PrintCurrentPage();

        next.SetActive(true);
        if (currentPage == 1) previous.SetActive(false);
    }
}
