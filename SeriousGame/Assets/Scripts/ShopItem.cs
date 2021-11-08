using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject ownedImage;
    [SerializeField] GameObject pausedImage;
    [SerializeField] GameObject upgradingImage;

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
        if (sii.status != ShopItemStatus.NOT_OWNED) {
            Purchase();
            if (sii.status == ShopItemStatus.ACTIVE) Enable();
            if (sii.status == ShopItemStatus.INACTIVE) Disable();
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
    public void Purchase() {
        costText.SetText("");
        Upgrade();
    }

    /**
    * <summary>Change the aspect of the item of the shop</summary>
    */
    public void Upgrade() {
        upgradingImage.SetActive(true);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Enable() {
        ownedImage.SetActive(true);
        pausedImage.SetActive(false);
        upgradingImage.SetActive(false);
    }

    /**
     * <summary>Change the aspect of the item of the shop</summary>
     */
    public void Disable() {
        ownedImage.SetActive(false);
        pausedImage.SetActive(true);
        upgradingImage.SetActive(false);
    }
}
