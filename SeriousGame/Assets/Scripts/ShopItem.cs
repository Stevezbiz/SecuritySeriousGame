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
    [SerializeField] GameObject slider;
    [SerializeField] Image enableBar;
    [SerializeField] Image enableHandle;
    [SerializeField] Image installBar;
    [SerializeField] Image installText;

    GameManager gameManager;
    Shop parent;
    ShopItemDetail details;
    ShopItemCode id;
    int lastValue;

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(ShopItemInfo sii, GameManager gameManager, Shop parent, ShopItemDetail details) {
        this.id = sii.id;
        this.gameManager = gameManager;
        this.parent = parent;
        this.details = details;
        gameObject.name = "ShopItem" + id.ToString();
        itemText.SetText(sii.name);
        if (sii.locked) {
            bool ok = true;
            foreach (ShopItemCode code in sii.requirements) {
                if (!gameManager.ShopItemIsInstalled(code)) {
                    ok = false;
                    break;
                }
            }
            if (ok) gameManager.ShopItemUnlock(id);
        }
        if (sii.locked) {
            Lock();
        } else {
            switch (sii.status) {
                case ShopItemStatus.NOT_OWNED:
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
                    lastValue = 1;
                    slider.GetComponent<Slider>().value = 1;
                    Enable();
                    break;
                case ShopItemStatus.INACTIVE:
                    lastValue = 0;
                    slider.GetComponent<Slider>().value = 0;
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
    * <summary>Change the aspect of the item of the shop</summary>
    */
    public void NotInstalled() {
        installButton.SetActive(true);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
        slider.SetActive(false);
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
        slider.SetActive(false);
    }

    public void Upgrading() {
        float fill = gameManager.GetTaskProgress(id);
        installBar.fillAmount = fill;
        installText.fillAmount = fill;
        installButton.SetActive(false);
        installingImage.SetActive(true);
        lockedImage.SetActive(false);
        slider.SetActive(false);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Enable() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
        slider.SetActive(true);
        enableBar.color = COLOR.GREEN;
        enableHandle.color = COLOR.GREEN;
        slider.GetComponent<Slider>().value = 1;
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Disable() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(false);
        slider.SetActive(true);
        enableBar.color = COLOR.GREEN_DISABLED;
        enableHandle.color = COLOR.GREEN_DISABLED;
        slider.GetComponent<Slider>().value = 0;
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Lock() {
        installButton.SetActive(false);
        installingImage.SetActive(false);
        lockedImage.SetActive(true);
        slider.SetActive(false);
    }

    public void SliderValueChange() {
        details.Load(id, this);
        int newValue = (int)slider.GetComponent<Slider>().value;
        if (lastValue != newValue) {
            lastValue = newValue;
            if (newValue == 1) details.EnableItem();
            else details.DisableItem();
        }
    }

    public void SelectEmployee() {
        details.Load(id, this);
        details.OpenEmployeeChoiceToInstall();
    }
}
