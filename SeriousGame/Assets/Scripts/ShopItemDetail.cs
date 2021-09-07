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

    ShopItem parent;

    public ShopItem Parent { get => parent; set => parent = value; }

    // Start is called before the first frame update
    void Start() {
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>();

        titleText.SetText(Parent.ShopItemInfo.name + " - costo " + Parent.ShopItemInfo.cost.ToString());
        descriptionText.SetText(Parent.ShopItemInfo.description);
        if (Parent.ShopItemInfo.owned) {
            //purchaseButton.interactable = false;
            //purchaseButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(.0f, 1.0f, .0f, .5f);
            if (Parent.ShopItemInfo.on) {
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
        if (Parent.ShopItemInfo.owned) {
            if (Parent.ShopItemInfo.on) {
                gui.DisableShopItem(Parent.ShopItemInfo.id);
                parent.Disable();
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("attiva");
            } else {
                gui.EnableShopItem(Parent.ShopItemInfo.id);
                parent.Enable();
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("disattiva");
            }
        } else {
            gui.Purchase(Parent.ShopItemInfo.id);
            Parent.Purchase();
            Destroy(gameObject);
        }
    }

    public void CancelPurchase() {
        Destroy(gameObject);
    }
}
