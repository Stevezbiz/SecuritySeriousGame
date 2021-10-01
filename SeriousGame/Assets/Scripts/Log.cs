using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Log : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject logRecord;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject next;
    [SerializeField] GameObject previous;

    float oldTimeScale;
    int nLines = 0;
    int nPages = 1;
    int currentPage;
    const int nLinesStep = 20;
    List<GameObject> toDestroy = new List<GameObject>();

    /**
     * <summary>Populates the logs with the current page</summary>
     */
    void PrintCurrentPage() {
        // destroy the page previously shown
        foreach (GameObject go in toDestroy) Destroy(go);
        toDestroy.Clear();
        // calculate the range of indices for the current page
        int i = (currentPage - 1) * nLinesStep;
        int N;
        if (currentPage == nPages) N = i + nLines;
        else N = i + nLinesStep;
        // print the lines
        for (int j = i; j < N; j++) {
            GameObject newLog = Instantiate(logRecord);
            newLog.transform.SetParent(content, false);
            toDestroy.Add(newLog);
            TextMeshProUGUI text = newLog.GetComponent<TextMeshProUGUI>();
            LogLine line = gameManager.GetLog(j);
            text.SetText(line.line);
            text.color = Deserialize(line.color);
        }
    }

    /**
      * <summary>Insert into the logs a new line regarding the specified attack</summary>
      */
    public void LogPrintAttack(string attack, bool hit) {
        string dateTime = gameManager.GetDateTime() + " | ";
        string desc;
        Color color;
        // select the content and the aspect of the log line
        if (hit) {
            desc = "Individuato attacco " + attack;
            color = COLOR.LOG_RED;
        } else {
            desc = "Sventato attacco " + attack;
            color = COLOR.LOG_BLUE;
        }
        // add the line to logs
        gameManager.AddToLogs(new LogLine(dateTime + desc, Serialize(color)));
        if (nLines == nLinesStep) {
            nLines = 0;
            nPages++;
        }
        nLines++;
    }

    /**
     * <summary>Insert into the logs a new line regarding the specified action on the specified item in the shop</summary>
     */
    public void LogPrintItem(string item, ActionCode action) {
        string dateTime = gameManager.GetDateTime() + " | ";
        string desc;
        // select the content of the log line
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
        // add the line to logs
        gameManager.AddToLogs(new LogLine(dateTime + desc, Serialize(COLOR.LOG_GREEN)));
        if (nLines == nLinesStep) {
            nLines = 0;
            nPages++;
        }
        nLines++;
    }

    /**
     * <summary>Open the logs</summary>
     */
    public void OpenLog() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // print the page
        currentPage = nPages;
        PrintCurrentPage();
        // set the aspect of the navigation elements
        if (currentPage > 1) previous.SetActive(true);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the logs</summary>
     */
    public void CloseLog() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        previous.SetActive(false);
        next.SetActive(false);
    }

    /**
     * <summary>Load the next page of the logs</summary>
     */
    public void NextPage() {
        // print the page
        currentPage++;
        PrintCurrentPage();
        // set the aspect of the navigation elements
        previous.SetActive(true);
        if (currentPage == nPages) next.SetActive(false);
    }

    /**
     * <summary>Load the previous page of the logs</summary>
     */
    public void PreviousPage() {
        // print the page
        currentPage--;
        PrintCurrentPage();
        // set the aspect of the navigation elements
        next.SetActive(true);
        if (currentPage == 1) previous.SetActive(false);
    }

    /**
     * <summary>Initialize the logs</summary>
     */
    public void LoadGameData(int nLines, int nPages) {
        this.nLines = nLines;
        this.nPages = nPages;
    }

    /**
     * <summary>Deserialize data about the color</summary>
     */
    Color Deserialize(float[] c) {
        return new Color(c[0], c[1], c[2], c[3]);
    }

    /**
     * <summary>Serialize data about the color</summary>
     */
    float[] Serialize(Color c) {
        return new float[4] { c.r, c.g, c.b, c.a };
    }

    /**
     * <summary>Return the number of lines in the logs</summary>
     */
    public int GetNLines() {
        return nLines;
    }

    /**
     * <summary>Return the number of pages containing the logs</summary>
     */
    public int GetNPages() {
        return nPages;
    }
}
