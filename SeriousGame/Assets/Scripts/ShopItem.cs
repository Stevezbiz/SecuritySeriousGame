using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour {
    [SerializeField] GameObject shopItemDetail;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject ownedImage;
    [SerializeField] GameObject pausedImage;

    ShopGUI shop;
    ShopItemDetail details;
    int id;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Load(ShopItemInfo sii, ShopGUI shop, ShopItemDetail details) {
        id = sii.id;
        this.shop = shop;
        this.details = details;
        shop.AddItem(sii);
        gameObject.name = "ShopItem" + id.ToString();
        itemText.SetText(sii.name);
        costText.SetText(sii.cost.ToString());
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
