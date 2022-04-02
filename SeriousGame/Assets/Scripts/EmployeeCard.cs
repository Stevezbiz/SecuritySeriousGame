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

public class EmployeeCard : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image icon;
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

    EmployeeInfo employee;
    EmployeeView employeeView;
    EmployeeChoice employeeChoice;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(GameManager gameManager, EmployeeInfo e, EmployeeView parent, CategoryCode c) {
        this.employee = e;
        _Load(gameManager, c);
        this.employeeView = parent;
    }

    public void Load(GameManager gameManager, EmployeeInfo e, EmployeeChoice parent, CategoryCode c) {
        this.employee = e;
        _Load(gameManager, c);
        this.employeeChoice = parent;
    }

    void _Load(GameManager gameManager, CategoryCode c) {
        this.employeeChoice = null;
        this.employeeView = null;
        nameText.SetText(employee.name.ToLower());
        icon.sprite = gameManager.GetEmployeeIcon(employee.id);
        descriptionText.SetText(employee.description);
        Dictionary<CategoryCode, float> abilities = EmployeeUtils.GetAbilities(employee.abilities);
        networkBar.fillAmount = abilities[CategoryCode.NETWORK] / 10;
        accessBar.fillAmount = abilities[CategoryCode.ACCESS] / 10;
        softwareBar.fillAmount = abilities[CategoryCode.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[CategoryCode.ASSET] / 10;
        servicesBar.fillAmount = abilities[CategoryCode.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + employee.GetMoneyGain() + " F/h");
        networkOutline.SetActive(false);
        accessOutline.SetActive(false);
        softwareOutline.SetActive(false);
        assetOutline.SetActive(false);
        servicesOutline.SetActive(false);
        switch (c) {
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
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {
        if (employeeView != null) employeeView.SelectEmployee(employee.id);
        else if (employeeChoice != null) employeeChoice.SelectEmployee(employee.id);
    }
}
