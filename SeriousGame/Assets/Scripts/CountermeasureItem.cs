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
using Slider = UnityEngine.UI.Slider;

public class CountermeasureItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject upgradingImage;
    [SerializeField] GameObject slider;
    [SerializeField] Image enableBar;
    [SerializeField] Image enableHandle;

    CountermeasureItemDetails details;
    ShopItemCode id;
    int lastValue = 0;

    public void Load(ShopItemInfo sii, CountermeasureItemDetails details) {
        this.id = sii.id;
        this.details = details;
        gameObject.name = "CountermeasureItem" + id.ToString();
        nameText.SetText(sii.name);
        levelText.SetText("Lv." + sii.level);
        // set the visual aspect
        switch (sii.status) {
            case ShopItemStatus.NOT_OWNED:
                upgradingImage.SetActive(false);
                slider.SetActive(false);
                break;
            case ShopItemStatus.NOT_INSTALLED:
                upgradingImage.SetActive(false);
                slider.SetActive(false);
                break;
            case ShopItemStatus.INSTALLING:
                upgradingImage.SetActive(false);
                slider.SetActive(false);
                break;
            case ShopItemStatus.UPGRADING:
                upgradingImage.SetActive(true);
                slider.SetActive(false);
                break;
            case ShopItemStatus.ACTIVE:
                upgradingImage.SetActive(false);
                enableBar.color = COLOR.GREEN;
                enableHandle.color = COLOR.GREEN;
                lastValue = 1;
                slider.GetComponent<Slider>().value = 1;
                slider.SetActive(true);
                break;
            case ShopItemStatus.INACTIVE:
                upgradingImage.SetActive(false);
                enableBar.color = COLOR.GREEN_DISABLED;
                enableHandle.color = COLOR.GREEN_DISABLED;
                lastValue = 0;
                slider.GetComponent<Slider>().value = 0;
                slider.SetActive(true);
                break;
            default:
                Debug.Log("Error: undefined ShopItemStatus");
                break;
        }
    }

    public void SliderValueChange() {
        int newValue = (int)slider.GetComponent<Slider>().value;
        if (lastValue != newValue) {
            details.Load(id, this);
            lastValue = newValue;
            if (newValue == 1) details.EnableItem();
            else details.DisableItem();
        }
    }

    public void ShowDetails() {
        details.Load(id, this);
        details.gameObject.SetActive(true);
    }

    public void Enable() {
        enableBar.color = COLOR.GREEN;
        enableHandle.color = COLOR.GREEN;
        lastValue = 1;
        slider.GetComponent<Slider>().value = 1;
    }

    public void Disable() {
        enableBar.color = COLOR.GREEN_DISABLED;
        enableHandle.color = COLOR.GREEN_DISABLED;
        lastValue = 0;
        slider.GetComponent<Slider>().value = 0;
    }
}
