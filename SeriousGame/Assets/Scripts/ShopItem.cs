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

public class ShopItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] GameObject installButton;
    [SerializeField] GameObject installingImage;
    [SerializeField] GameObject lockedImage;
    [SerializeField] Image installBar;
    [SerializeField] Image installText;
    [SerializeField] Image category;

    GameManager gameManager;
    Shop parent;
    ShopItemDetail details;
    ShopItemCode id;

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(ShopItemInfo sii, GameManager gameManager, Shop parent, ShopItemDetail details) {
        this.id = sii.id;
        this.gameManager = gameManager;
        this.parent = parent;
        this.details = details;
        gameObject.name = "ShopItem" + id.ToString();
        itemText.SetText(sii.name + " - Lv." + sii.level);
        category.sprite = gameManager.GetCategoryImage(sii.category);
        if (sii.status == ShopItemStatus.NOT_OWNED && sii.locked[sii.level]) {
            bool ok = true;
            foreach (Requirement r in sii.reqArray[sii.level].requirements) {
                if (!gameManager.RequirementIsSatisfied(r)) {
                    ok = false;
                    break;
                }
            }
            if (ok) gameManager.ShopItemUnlock(id);
        }
        if (sii.status == ShopItemStatus.NOT_OWNED && sii.locked[sii.level]) {
            Lock();
        } else {
            switch (sii.status) {
                case ShopItemStatus.NOT_OWNED:
                    NotOwned();
                    break;
                case ShopItemStatus.NOT_INSTALLED:
                    NotInstalled();
                    break;
                case ShopItemStatus.INSTALLING:
                    Installing();
                    break;
                case ShopItemStatus.UPGRADING:
                    Upgrading();
                    break;
                case ShopItemStatus.ACTIVE:
                    Enable();
                    break;
                case ShopItemStatus.INACTIVE:
                    Disable();
                    break;
                default:
                    Debug.Log("Error: undefined ShopItemStatus");
                    break;
            }
        }
    }

    /**
     * <summary>Show the details of the item of the shop</summary>
     */
    public void ShowDetails() {
        details.Load(id, this);
        details.gameObject.SetActive(true);
    }

    /**
     * <summary></summary>
     */
    public void NotOwned() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
    }

    /**
    * <summary>Change the aspect of the item of the shop</summary>
    */
    public void NotInstalled() {
        installButton.SetActive(true);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
    }

    /**
    * <summary>Change the aspect of the item of the shop</summary>
    */
    public void Installing() {
        float fill = gameManager.GetTaskProgress(id);
        installBar.fillAmount = fill;
        installText.fillAmount = fill;
        installButton.SetActive(false);
        installingImage.SetActive(true);
        lockedImage.SetActive(false);
    }

    /**
     * <summary></summary>
     */
    public void Upgrading() {
        float fill = gameManager.GetTaskProgress(id);
        installBar.fillAmount = fill;
        installText.fillAmount = fill;
        installButton.SetActive(false);
        installingImage.SetActive(true);
        lockedImage.SetActive(false);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Enable() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Disable() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Lock() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(true);
    }

    /**
     * <summary></summary>
     */
    public void SelectEmployee() {
        details.Load(id, this);
        details.OpenEmployeeChoiceToInstall();
    }
}
