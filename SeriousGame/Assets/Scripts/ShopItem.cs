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

    public void SelectEmployee() {
        details.Load(id, this);
        details.OpenEmployeeChoiceToInstall();
    }
}
