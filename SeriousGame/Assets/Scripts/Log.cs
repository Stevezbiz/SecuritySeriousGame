/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
    [SerializeField] GameObject bottomPanel;

    int nLines = 0;
    int nPages = 1;
    int currentPage;
    const int nLinesStep = 30;
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
            GameObject newLog = Instantiate(logRecord, content, false);
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
            color = COLOR.RED;
        } else {
            desc = "Sventato attacco " + attack;
            color = COLOR.BLUE;
        }
        // add the line to logs
        AddToLogs(new LogLine(dateTime + desc, Serialize(color)));
    }

    /**
     * <summary>Insert into the logs a new line regarding the specified action on the specified item in the shop</summary>
     */
    public void LogPrintItem(string item, ActionCode action) {
        string dateTime = gameManager.GetDateTime() + " | ";
        string desc;
        // select the content of the log line
        switch (action) {
            case ActionCode.PURCHASE_ITEM:
                desc = "Acquistato " + item;
                break;
            case ActionCode.UPGRADE_ITEM:
                desc = "Aumentato livello " + item;
                break;
            case ActionCode.ENABLE_ITEM:
                desc = "Abilitato " + item;
                break;
            case ActionCode.DISABLE_ITEM:
                desc = "Disabilitato " + item;
                break;
            default:
                return;
        }
        // add the line to logs
        AddToLogs(new LogLine(dateTime + desc, Serialize(COLOR.GREEN)));
    }

    /**
     * <summary></summary>
     */
    public void LogPrintStartTask(Task t) {
        string dateTime = gameManager.GetDateTime() + " | ";
        string desc;
        switch (t.type) {
            case TaskType.INSTALL:
                desc = gameManager.GetEmployee(t.executor).name + " ha iniziato l'installazione di " + gameManager.GetShopItem(t.shopItem).name;
                break;
            case TaskType.UPGRADE:
                desc = gameManager.GetEmployee(t.executor).name + " ha iniziato il potenziamento di " + gameManager.GetShopItem(t.shopItem).name;
                break;
            case TaskType.REPAIR:
                desc = gameManager.GetEmployee(t.executor).name + " ha iniziato la riparazione di " + gameManager.GetAttack(t.attack).name;
                break;
            case TaskType.PREVENT:
                desc = gameManager.GetEmployee(t.executor).name + " è stato assegnato all'incarico di prevenzione in " + gameManager.GetCategory(t.category).name;
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                return;
        }
        // add the line to logs
        AddToLogs(new LogLine(dateTime + desc, Serialize(COLOR.GREEN)));
    }

    /**
     * <summary></summary>
     */
    public void LogPrintEndTask(Task t) {
        string dateTime = gameManager.GetDateTime() + " | ";
        string desc;
        switch (t.type) {
            case TaskType.INSTALL:
                desc = gameManager.GetEmployee(t.executor).name + " ha terminato l'installazione di " + gameManager.GetShopItem(t.shopItem).name;
                break;
            case TaskType.UPGRADE:
                desc = gameManager.GetEmployee(t.executor).name + " ha terminato il potenziamento di " + gameManager.GetShopItem(t.shopItem).name;
                break;
            case TaskType.REPAIR:
                desc = gameManager.GetEmployee(t.executor).name + " ha terminato la riparazione di " + gameManager.GetAttack(t.attack).name;
                break;
            case TaskType.PREVENT:
                desc = gameManager.GetEmployee(t.executor).name + " è stato rimosso dall'incarico di prevenzione in " + gameManager.GetCategory(t.category).name;
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                return;
        }
        // add the line to logs
        AddToLogs(new LogLine(dateTime + desc, Serialize(COLOR.GREEN)));
    }

    /**
     * <summary></summary>
     */
    void AddToLogs(LogLine logLine) {
        gameManager.AddToLogs(logLine);
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
        TimeManager.Pause();
        // print the page
        currentPage = nPages;
        PrintCurrentPage();
        // set the aspect of the navigation elements
        if (currentPage > 1) previous.SetActive(true);
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the logs</summary>
     */
    public void CloseLog() {
        TimeManager.Resume();
        gameObject.SetActive(false);
        previous.SetActive(false);
        next.SetActive(false);
        bottomPanel.SetActive(true);
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
