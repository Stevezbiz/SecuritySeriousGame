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

    ShopItem parent;
    int id;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

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
        gui.Purchase(id);
        parent.Purchase();
        disableButton.SetActive(true);
        purchaseButton.SetActive(false);
    }

    public void EnableItem() {
        gui.EnableShopItem(id);
        parent.Enable();
        disableButton.SetActive(true);
        enableButton.SetActive(false);
    }

    public void DisableItem() {
        gui.DisableShopItem(id);
        parent.Disable();
        enableButton.SetActive(true);
        disableButton.SetActive(false);
    }

    public void Back() {
        gameObject.SetActive(false);
    }
}
