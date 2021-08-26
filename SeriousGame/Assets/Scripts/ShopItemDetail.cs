using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] Button purchaseButton;

    GUI gui;

    ShopItem parent;
    string item;
    string description;
    int cost;
    bool owned;

    public ShopItem Parent { get => parent; set => parent = value; }
    public string Item { get => item; set => item = value; }
    public string Description { get => description; set => description = value; }
    public int Cost { get => cost; set => cost = value; }
    public bool Owned { get => owned; set => owned = value; }

    // Start is called before the first frame update
    void Start() {
        gui = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUI>();

        titleText.SetText(Item + " - costo " + Cost.ToString());
        descriptionText.SetText(Description);
        if (Owned) {
            purchaseButton.interactable = false;
            purchaseButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(.0f, 1.0f, .0f, .5f);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void ConfirmPurchase() {
        switch (gui.Purchase(Cost)) {
            case ECode.INSUFFICIENT_MONEY:
                Vector3 newPos = new Vector3(0, 0, 0);
                GameObject errorWindow = Instantiate(windowPopUp, newPos, Quaternion.identity);
                errorWindow.transform.SetParent(gui.gameObject.transform, false);
                WindowPopUp newWindow = errorWindow.GetComponent<WindowPopUp>();
                newWindow.Resume = false;
                newWindow.Message = "Non hai abbastanza fondi";
                break;
            default:
                Parent.Purchase();
                Destroy(gameObject);
                break;
        }
    }

    public void CancelPurchase() {
        Destroy(gameObject);
    }
}
