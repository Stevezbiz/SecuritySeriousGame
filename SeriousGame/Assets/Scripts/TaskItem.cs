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
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class TaskItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI targetText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] GameObject progressBar;
    [SerializeField] Image bar;
    [SerializeField] Button button;
    [SerializeField] GameObject stopButton;

    GameManager gameManager;
    EmployeeView employeeView;
    Task task;

    /**
     * <summary></summary>
     */
    public void Load(GameManager gameManager, EmployeeView employeeView, Task t, bool assigned) {
        this.task = t;
        this.employeeView = employeeView;
        this.gameManager = gameManager;
        switch (t.type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                typeText.SetText("INSTALLAZIONE");
                targetText.SetText(gameManager.GetShopItem(t.shopItem).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                stopButton.SetActive(false);
                break;
            case TaskType.UPGRADE:
                typeText.SetText("POTENZIAMENTO");
                targetText.SetText(gameManager.GetShopItem(t.shopItem).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                stopButton.SetActive(false);
                break;
            case TaskType.REPAIR:
                typeText.SetText("RIPARAZIONE");
                targetText.SetText(gameManager.GetAttack(t.attack).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                stopButton.SetActive(false);
                break;
            case TaskType.PREVENT:
                typeText.SetText("PREVENZIONE");
                targetText.SetText(gameManager.GetCategory(t.category).name);
                progressText.SetText("-");
                progressBar.SetActive(false);
                if (assigned) stopButton.SetActive(true);
                else stopButton.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
        if (assigned) {
            button.interactable = false;
            statusText.SetText("ASSEGNATO A\n" + gameManager.GetEmployee(t.executor).name.ToUpper());
        } else {
            button.interactable = true;
            statusText.SetText("NON ASSEGNATO");
        }
    }

    /**
     * <summary></summary>
     */
    public void OpenEmployeeChoice() {
        employeeView.Load(task);
    }

    /**
     * <summary></summary>
     */
    public void EndTask() {
        gameManager.EndTask(task);
        Load(gameManager, employeeView, task, false);
    }
}
