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
using Image = UnityEngine.UI.Image;

public class EmployeeChoice : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeCard;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject selection;
    [SerializeField] GameObject unselection;
    [SerializeField] GameObject assignButton;
    [SerializeField] TextMeshProUGUI employeeText;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] ShopItemDetail shopItemDetail;
    [SerializeField] SecurityView securityView;

    List<GameObject> toDestroy = new List<GameObject>();
    EmployeeCode selected;
    Task task;

    /**
     * <summary></summary>
     */
    public void Load(Task t) {
        this.task = t;
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach (EmployeeInfo e in gameManager.GetAvailableEmployees()) {
            GameObject newItem = Instantiate(employeeCard, content, false);
            newItem.GetComponent<EmployeeCard>().Load(gameManager, e, this, t.category);
            toDestroy.Add(newItem);
        }
        if (toDestroy.Count == 0) {
            gameManager.DisplayMessage("Nessun impiegato ? al momento disponibile.", ActionCode.CONTINUE, Role.SECURITY);
        } else {
            switch (task.type) {
                case TaskType.NONE:
                    break;
                case TaskType.INSTALL:
                    titleText.SetText("INSTALLAZIONE " + gameManager.GetShopItem(task.shopItem).name);
                    break;
                case TaskType.UPGRADE:
                    titleText.SetText("POTENZIAMENTO " + gameManager.GetShopItem(task.shopItem).name);
                    break;
                case TaskType.REPAIR:
                    titleText.SetText("RIPARAZIONE " + gameManager.GetAttack(task.attack).name);
                    break;
                case TaskType.PREVENT:
                    break;
                default:
                    Debug.Log("Error: unexpected TaskType");
                    break;
            }
            selection.SetActive(false);
            assignButton.SetActive(false);
            unselection.SetActive(true);
            gameObject.SetActive(true);
            content.localPosition = new Vector3(content.localPosition.x, 0f, content.localPosition.z);
        }
    }

    /**
     * <summary></summary>
     */
    public void SelectEmployee(EmployeeCode id) {
        selected = id;
        unselection.SetActive(false);
        selection.SetActive(true);
        assignButton.SetActive(true);
        employeeText.SetText("Impiegato: " + gameManager.GetEmployee(id).name);
        switch (task.type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                valueText.SetText("Durata: " + gameManager.GetInstallTaskDuration(id, task.shopItem) + " h");
                break;
            case TaskType.UPGRADE:
                valueText.SetText("Durata: " + gameManager.GetUpgradeTaskDuration(id, task.shopItem) + " h");
                break;
            case TaskType.REPAIR:
                valueText.SetText("Durata: " + gameManager.GetRepairTaskDuration(id, task.attack) + " h");
                break;
            case TaskType.PREVENT:
                valueText.SetText("Protezione: " + (100 * gameManager.GetPreventProtection(id, task.category)).ToString("+0.") + "%");
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
    }

    /**
     * <summary></summary>
     */
    public void AssignEmployee() {
        switch (task.type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                shopItemDetail.InstallItem(selected);
                break;
            case TaskType.REPAIR:
                securityView.Repair(selected);
                break;
            case TaskType.UPGRADE:
                shopItemDetail.UpgradeItem(selected);
                break;
            case TaskType.PREVENT:
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
        }
        Close();
    }

    /**
     * <summary></summary>
     */
    public void Close() {
        gameObject.SetActive(false);
    }
}
