using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject enableButton;
    [SerializeField] GameObject disableButton;

    GUI gui;
    ShopGUI shop;
    ShopItem parent;
    int id;

    public ShopGUI Shop { get => shop; set => shop = value; }
    public int Id { get => id; set => id = value; }
    public ShopItem Parent { get => parent; set => parent = value; }

    // Start is called before the first frame update
    void Start() {
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>();
        ShopItemInfo sii = shop.GetItem(id);
        titleText.SetText(sii.name + " - costo " + sii.cost.ToString());
        descriptionText.SetText(sii.description);
        if (sii.owned) {
            purchaseButton.SetActive(false);
            if (sii.on) disableButton.SetActive(true);
            else enableButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void ConfirmPurchaseOrEnableDisable() {
        if (shop.ItemIsOwned(id)) {
            if (shop.ItemIsOn(id)) {
                gui.DisableShopItem(id);
                parent.Disable();
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("attiva");
            } else {
                gui.EnableShopItem(id);
                parent.Enable();
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("disattiva");
            }
        } else {
            gui.Purchase(id);
            parent.Purchase();
            Destroy(gameObject);
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
        Destroy(gameObject);
    }
}
