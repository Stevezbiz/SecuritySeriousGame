using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI requirementsText;
    [SerializeField] TextMeshProUGUI resistancesText;
    [SerializeField] TextMeshProUGUI costsText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject enableButton;
    [SerializeField] GameObject disableButton;
    [SerializeField] GameObject lockImage;
    [SerializeField] TextMeshProUGUI purchaseText;
    [SerializeField] Outline purchaseOutline;
    [SerializeField] GameManager gameManager;
    [SerializeField] Log logManager;

    ShopItem parent;
    ShopItemCode id;

    /**
     * <summary>Compose all the details of the item</summary>
     */
    void ComposeDetails(ShopItemInfo sii) {
        titleText.SetText(sii.name + "\n" + sii.cost.ToString() + " Fondi");
        descriptionText.SetText(sii.description);
        // set the technical details about requirements
        string requirements = "";
        if (sii.locked) {
            requirements += "Per sbloccare questo potenziamento devi prima acquistare:\n";
            foreach (ShopItemCode code in sii.requirements) {
                requirements += "    " + gameManager.GetShopItem(code).name + "\n";
            }
        }
        requirementsText.SetText(requirements);
        // set the technical details about resistances
        string resistances = "";
        resistances += "Resistenze:\n";
        foreach (Resistance res in sii.resistances) {
            resistances += "    " + gameManager.GetAttack(res.id).name + "\n";
            if (res.duration != 0) resistances += "        durata dell'attacco -" + (res.duration * 100) + "%\n";
            if (res.miss != 0) resistances += "        probabilità di bloccare l'attacco +" + (res.miss * 100) + "%\n";
            if (res.endurance != 0) resistances += "        tempo medio tra 2 attacchi consecutivi +" + (res.endurance * 100) + "%\n";
        }
        if (sii.resistances.Length == 0) resistances += "nessuna\n";
        if (sii.moneyMalus < 0) resistances += "Guadagno aggiuntivo: " + (-sii.moneyMalus) + " F/h\n";
        if (sii.usersMod > 1) resistances += "Prestazioni e usabilità: +" + ((sii.usersMod - 1) * 100) + "%\n";
        resistancesText.SetText(resistances);
        // set the technical details about costs and usability
        string costs = "";
        if (sii.moneyMalus < 0) costs += "Costo: 0 F/h\n";
        else costs += "Costo: " + sii.moneyMalus + " F/h\n";
        if (sii.usersMod != 0 && sii.usersMod < 1) costs += "Prestazioni e usabilità: -" + (sii.usersMod * 100) + "%\n";
        costsText.SetText(costs);
    }

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(ShopItemCode id, ShopItem parent) {
        this.id = id;
        this.parent = parent;
        ShopItemInfo sii = gameManager.GetShopItem(id);
        if (sii.locked) {
            bool ok = true;
            foreach (ShopItemCode code in sii.requirements) {
                if (!gameManager.ShopItemIsOwned(code)) ok = false;
            }
            if (ok) gameManager.ShopItemUnlock(id);
        }
        ComposeDetails(sii);
        // set the visual aspect
        purchaseButton.SetActive(false);
        enableButton.SetActive(false);
        disableButton.SetActive(false);
        purchaseButton.GetComponentInChildren<Button>().interactable = true;
        purchaseText.color = new Color(0f, 1f, 0f, 1f);
        purchaseOutline.effectColor = new Color(0f, 1f, 0f, 1f);
        lockImage.SetActive(false);
        if (sii.owned) {
            if (sii.on) disableButton.SetActive(true);
            else enableButton.SetActive(true);
        } else {
            if (sii.locked) {
                purchaseButton.GetComponentInChildren<Button>().interactable = false;
                purchaseText.color = new Color(0f, 1f, 0f, .4f);
                purchaseOutline.effectColor = new Color(0f, 1f, 0f, .4f);
                lockImage.SetActive(true);
            }
            purchaseButton.SetActive(true);
        }
    }

    /**
     * <summary>Applies the effects of buying an item of the shop</summary>
     */
    public void ConfirmPurchase() {
        gameManager.PurchaseShopItem(id);
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
