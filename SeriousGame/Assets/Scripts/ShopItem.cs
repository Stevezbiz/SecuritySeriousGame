using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

public class ShopItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject upgradingImage;
    [SerializeField] GameObject slider;
    [SerializeField] Image bar;
    [SerializeField] Image handle;


    ShopItemDetail details;
    ShopItemCode id;

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(ShopItemInfo sii, ShopItemDetail details) {
        id = sii.id;
        this.details = details;
        gameObject.name = "ShopItem" + id.ToString();
        itemText.SetText(sii.name);
        costText.SetText(sii.cost.ToString());
        switch (sii.status) {
            case ShopItemStatus.NOT_OWNED:
                break;
            case ShopItemStatus.UPGRADING:
                Upgrade();
                break;
            case ShopItemStatus.ACTIVE:
                slider.GetComponent<Slider>().value = 1;
                Enable();
                break;
            case ShopItemStatus.INACTIVE:
                slider.GetComponent<Slider>().value = 0;
                Disable();
                break;
            default:
                Debug.Log("Error: undefined ShopItemStatus");
                break;
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
    public void Upgrade() {
        costText.SetText("");
        upgradingImage.SetActive(true);
        slider.SetActive(false);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Enable() {
        costText.SetText("");
        upgradingImage.SetActive(false);
        slider.SetActive(true);
        bar.color = COLOR.GREEN;
        handle.color = COLOR.GREEN;
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Disable() {
        costText.SetText("");
        upgradingImage.SetActive(false);
        slider.SetActive(true);
        bar.color = COLOR.GREEN_DISABLED;
        handle.color = COLOR.GREEN_DISABLED;
    }

    public void SliderValueChange() {
        details.Load(id, this);
        if (slider.GetComponent<Slider>().value == 1) details.EnableItem();
        else details.DisableItem();
    }
}
