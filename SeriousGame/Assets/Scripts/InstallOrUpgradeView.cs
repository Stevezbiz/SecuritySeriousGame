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

public class InstallOrUpgradeView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] TextMeshProUGUI durationText;
    [SerializeField] EmployeeView employeeView;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] GameObject networkOutline;
    [SerializeField] GameObject accessOutline;
    [SerializeField] GameObject softwareOutline;
    [SerializeField] GameObject assetOutline;
    [SerializeField] GameObject servicesOutline;

    EmployeeCode employee;
    List<Task> tasks = new List<Task>();

    /**
     * <summary></summary>
     */
    public void Load(EmployeeCode id) {
        EmployeeInfo e = gameManager.GetEmployee(id);
        nameText.SetText(e.name.ToLower());
        descriptionText.SetText(e.description);
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
        Dictionary<CategoryCode, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[CategoryCode.NETWORK] / 10;
        accessBar.fillAmount = abilities[CategoryCode.ACCESS] / 10;
        softwareBar.fillAmount = abilities[CategoryCode.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[CategoryCode.ASSET] / 10;
        servicesBar.fillAmount = abilities[CategoryCode.SERVICES] / 10;
        tasks.Clear();
        tasks = gameManager.GetAvailableTasksByType(TaskType.INSTALL);
        if (tasks.Count == 0) {
            gameManager.DisplayMessage("Non ci sono installazioni richieste al momento", ActionCode.CONTINUE, Role.SECURITY);
        } else {
            employee = id;
            List<string> options = new List<string>();
            foreach (Task t in tasks) {
                options.Add(gameManager.GetShopItem(t.shopItem).name);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = 0;
            Display(0);
            gameObject.SetActive(true);
        }
    }

    /**
     * <summary></summary>
     */
    public void Display(int err) {
        Task t = tasks[dropdown.value];
        ShopItemInfo sii = gameManager.GetShopItem(t.shopItem);
        int duration;
        switch (t.type) {
            case TaskType.INSTALL:
                duration = gameManager.GetInstallTaskDuration(employee, sii.id);
                break;
            case TaskType.UPGRADE:
                duration = gameManager.GetUpgradeTaskDuration(employee, sii.id);
                break;
            default:
                Debug.Log("Error: unhandled TaskType");
                duration = 0;
                break;
        }
        durationText.SetText("Durata: " + duration + " h");
        networkOutline.SetActive(false);
        accessOutline.SetActive(false);
        softwareOutline.SetActive(false);
        assetOutline.SetActive(false);
        servicesOutline.SetActive(false);
        switch (sii.category) {
            case CategoryCode.NETWORK:
                networkOutline.SetActive(true);
                break;
            case CategoryCode.ACCESS:
                accessOutline.SetActive(true);
                break;
            case CategoryCode.SOFTWARE:
                softwareOutline.SetActive(true);
                break;
            case CategoryCode.ASSET:
                assetOutline.SetActive(true);
                break;
            case CategoryCode.SERVICES:
                servicesOutline.SetActive(true);
                break;
            default:
                Debug.Log("Error: undefined Category");
                break;
        }
    }

    /**
     * <summary></summary>
     */
    public void InstallItem() {
        gameManager.AssignEmployee(employee, tasks[dropdown.value].id);
        Close();
    }

    /**
     * <summary></summary>
     */
    public void Close() {
        //employeeView.Load();
        gameObject.SetActive(false);
    }
}
