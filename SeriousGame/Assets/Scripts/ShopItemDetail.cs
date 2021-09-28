using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI detailsText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject enableButton;
    [SerializeField] GameObject disableButton;
    [SerializeField] ShopGUI shop;
    [SerializeField] GUI gui;
    [SerializeField] AttacksManager attacksManager;
    [SerializeField] LogManager logManager;

    ShopItem parent;
    int id;

    void ComposeDetails(ShopItemInfo sii) {
        titleText.SetText(sii.name + " - costo " + sii.cost.ToString());
        descriptionText.SetText(sii.description);

        string details = "Resistenze:\n";
        foreach (Resistance res in sii.resistances) {
            details += "    " + attacksManager.Attack(res.id).name + "\n";
            if (res.duration != 0) details += "        durata dell'attacco -" + (res.duration * 100) + "%\n";
            if (res.miss != 0) details += "        probabilità di bloccare l'attacco +" + (res.miss * 100) + "%\n";
            if (res.endurance != 0) details += "        tempo medio tra 2 attacchi consecutivi +" + (res.endurance * 100) + "%\n";
        }
        if (sii.resistances.Length == 0) details += "nessuna\n";

        details += "Costo: " + sii.moneyMalus + " F/h";

        if (sii.usersMod != 0) {
            if (sii.usersMod < 1) details += "\nPrestazioni e usabilità: -" + (sii.usersMod * 100) + "%\n";
            else details += "\nPrestazioni e usabilità: +" + ((sii.usersMod - 1) * 100) + "%\n";
        }

        detailsText.SetText(details);
    }

    public void Load(int id, ShopItem parent) {
        this.id = id;
        this.parent = parent;
        ShopItemInfo sii = shop.GetItem(id);
        ComposeDetails(sii);

        purchaseButton.SetActive(false);
        enableButton.SetActive(false);
        disableButton.SetActive(false);
        if (sii.owned) {
            if (sii.on) disableButton.SetActive(true);
            else enableButton.SetActive(true);
        } else {
            purchaseButton.SetActive(true);
        }
    }

    public void ConfirmPurchase() {
        ShopItemInfo sii = shop.GetItem(id);

        sii.owned = true;
        shop.AddItem(sii);

        gui.Purchase(id);
        parent.Purchase();

        logManager.LogPrintItem(shop.GetItem(id).name, ActionCode.PURCHASE);
        
        purchaseButton.SetActive(false);

        EnableItem();
    }

    public void EnableItem() {
        ShopItemInfo sii = shop.GetItem(id);

        sii.on = true;
        shop.AddItem(sii);

        gui.EnableShopItem(id);
        parent.Enable();

        attacksManager.EnableShopItem(sii.resistances);
        logManager.LogPrintItem(shop.GetItem(id).name, ActionCode.ENABLE);
        
        disableButton.SetActive(true);
        enableButton.SetActive(false);
    }

    public void DisableItem() {
        ShopItemInfo sii = shop.GetItem(id);

        sii.on = false;
        shop.AddItem(sii);

        gui.DisableShopItem(id);
        parent.Disable();

        attacksManager.DisableShopItem(sii.resistances);
        logManager.LogPrintItem(shop.GetItem(id).name, ActionCode.DISABLE);
        
        enableButton.SetActive(true);
        disableButton.SetActive(false);
    }

    public void Back() {
        gameObject.SetActive(false);
    }
}
