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
    [SerializeField] GameManager gameManager;
    [SerializeField] Log logManager;

    ShopItem parent;
    int id;

    /**
     * <summary>Compose all the details of the item</summary>
     */
    void ComposeDetails(ShopItemInfo sii) {
        titleText.SetText(sii.name + "\n" + sii.cost.ToString() + " Fondi");
        descriptionText.SetText(sii.description);
        // set the technical details about resistances
        string details = "Resistenze:\n";
        foreach (Resistance res in sii.resistances) {
            details += "    " + gameManager.GetAttack(res.id).name + "\n";
            if (res.duration != 0) details += "        durata dell'attacco -" + (res.duration * 100) + "%\n";
            if (res.miss != 0) details += "        probabilità di bloccare l'attacco +" + (res.miss * 100) + "%\n";
            if (res.endurance != 0) details += "        tempo medio tra 2 attacchi consecutivi +" + (res.endurance * 100) + "%\n";
        }
        if (sii.resistances.Length == 0) details += "nessuna\n";
        // set the technical details about costs and usability
        if (sii.moneyMalus < 0) {
            details += "Costo: 0 F/h";
            details += "\nGuadagno aggiuntivo: " + (-sii.moneyMalus) + " F/h";
        } else {
            details += "Costo: " + sii.moneyMalus + " F/h";
        }
        if (sii.usersMod != 0) {
            if (sii.usersMod < 1) details += "\nPrestazioni e usabilità: -" + (sii.usersMod * 100) + "%\n";
            else details += "\nPrestazioni e usabilità: +" + ((sii.usersMod - 1) * 100) + "%\n";
        }
        detailsText.SetText(details);
    }

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(int id, ShopItem parent) {
        this.id = id;
        this.parent = parent;
        ShopItemInfo sii = gameManager.GetShopItem(id);
        ComposeDetails(sii);
        // set the visual aspect
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

    /**
     * <summary>Applies the effects of buying an item of the shop</summary>
     */
    public void ConfirmPurchase() {
        gameManager.Purchase(id);
        parent.Purchase();
        purchaseButton.SetActive(false);
        // print in the log
        logManager.LogPrintItem(gameManager.GetShopItem(id).name, ActionCode.PURCHASE_ITEM);
        // enable automatically after purchase
        EnableItem();
    }

    /**
     * <summary>Applies the effects of enabling an item of the shop</summary>
     */
    public void EnableItem() {
        gameManager.EnableShopItem(id);
        parent.Enable();
        disableButton.SetActive(true);
        enableButton.SetActive(false);
        // print in the log
        logManager.LogPrintItem(gameManager.GetShopItem(id).name, ActionCode.ENABLE_ITEM);
    }

    /**
     * <summary>Applies the effects of disabling an item of the shop</summary>
     */
    public void DisableItem() {
        gameManager.DisableShopItem(id);
        parent.Disable();
        enableButton.SetActive(true);
        disableButton.SetActive(false);
        // print in the log
        logManager.LogPrintItem(gameManager.GetShopItem(id).name, ActionCode.DISABLE_ITEM);
    }

    /**
     * <summary>Close the details</summary>
     */
    public void Back() {
        gameObject.SetActive(false);
    }
}
