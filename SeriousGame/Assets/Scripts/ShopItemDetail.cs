using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button purchaseButton;

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
            //purchaseButton.interactable = false;
            //purchaseButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(.0f, 1.0f, .0f, .5f);
            if (sii.on) {
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("disattiva");
            } else {
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("attiva");
            }
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

    public void CancelPurchase() {
        Destroy(gameObject);
    }
}
