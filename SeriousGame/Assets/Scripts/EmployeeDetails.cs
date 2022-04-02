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

public class EmployeeDetails : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] TextMeshProUGUI taskText;

    GameManager gameManager;

    /**
     * <summary></summary>
     */
    public void Load(EmployeeInfo e, GameManager gameManager) {
        this.gameManager = gameManager;
        titleText.SetText(e.name);
        icon.sprite = gameManager.GetEmployeeIcon(e.id);
        descriptionText.SetText(e.description);
        Dictionary<CategoryCode, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[CategoryCode.NETWORK] / 10;
        accessBar.fillAmount = abilities[CategoryCode.ACCESS] / 10;
        softwareBar.fillAmount = abilities[CategoryCode.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[CategoryCode.ASSET] / 10;
        servicesBar.fillAmount = abilities[CategoryCode.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.GetMoneyGain() + " F/h");
        switch (e.status) {
            case TaskType.NONE:
                taskText.SetText("");
                moneyGainText.color = COLOR.BLUE;
                break;
            case TaskType.INSTALL:
                taskText.SetText("Sta installando " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                moneyGainText.color = COLOR.YELLOW;
                break;
            case TaskType.REPAIR:
                taskText.SetText("Sta riparando i danni provocati dall'attacco " + gameManager.GetAttack((AttackCode)gameManager.GetTaskTarget(e.id)).name);
                moneyGainText.color = COLOR.YELLOW;
                break;
            case TaskType.UPGRADE:
                taskText.SetText("Sta installando " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                moneyGainText.color = COLOR.YELLOW;
                break;
            case TaskType.PREVENT:
                string category;
                switch ((CategoryCode)gameManager.GetTaskTarget(e.id)) {
                    case CategoryCode.NETWORK:
                        category = "Rete";
                        break;
                    case CategoryCode.ACCESS:
                        category = "Accesso";
                        break;
                    case CategoryCode.SOFTWARE:
                        category = "Software";
                        break;
                    case CategoryCode.ASSET:
                        category = "Risorse";
                        break;
                    case CategoryCode.SERVICES:
                        category = "Servizi";
                        break;
                    default:
                        Debug.Log("Error: undefined Category");
                        category = "";
                        break;
                }
                taskText.SetText("Sta facendo prevenzione nel settore " + category);
                moneyGainText.color = COLOR.YELLOW;
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
        }
    }

    /**
     * <summary></summary>
     */
    public void Close() {
        Destroy(gameObject);
    }
}
