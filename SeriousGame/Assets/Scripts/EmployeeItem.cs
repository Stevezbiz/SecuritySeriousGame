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

public class EmployeeItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] GameObject progressBar;
    [SerializeField] Image bar;

    public void Load(GameManager gameManager, EmployeeInfo e, Sprite s) {
        icon.sprite = s;
        nameText.SetText(e.name.ToLower());
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
                statusText.SetText("NON ASSEGNATO");
                progressBar.SetActive(false);
                break;
            case TaskType.INSTALL:
                statusText.SetText("INSTALLAZIONE " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.UPGRADE:
                statusText.SetText("POTENZIAMENTO " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.REPAIR:
                statusText.SetText("RIPARAZIONE " + gameManager.GetAttack((AttackCode)gameManager.GetTaskTarget(e.id)).name);
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.PREVENT:
                statusText.SetText("PREVENZIONE " + gameManager.GetCategory((CategoryCode)gameManager.GetTaskTarget(e.id)).name);
                progressBar.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
    }
}
