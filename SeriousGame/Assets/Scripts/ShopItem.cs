using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject ownedImage;
    [SerializeField] GameObject pausedImage;

    ShopItemDetail details;
    int id;

    public void Load(ShopItemInfo sii, ShopItemDetail details) {
        id = sii.id;
        this.details = details;
        gameObject.name = "ShopItem" + id.ToString();
        itemText.SetText(sii.name);
        costText.SetText(sii.cost.ToString());
        if (sii.owned) {
            Purchase();
            if (!sii.on) Disable();
        }
    }

    public void ShowDetails() {
        details.Load(id, this);
        details.gameObject.SetActive(true);
    }

    public void Purchase() {
        costText.SetText("");
        Enable();
    }

    public void Enable() {
        ownedImage.SetActive(true);
        pausedImage.SetActive(false);
    }

    public void Disable() {
        ownedImage.SetActive(false);
        pausedImage.SetActive(true);
    }

}
