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
    RectTransform spawnPoint;
    int id;

    public ShopGUI Shop { get => shop; set => shop = value; }
    public RectTransform SpawnPoint { get => spawnPoint; set => spawnPoint = value; }
    public int Id { get => id; set => id = value; }

    // Start is called before the first frame update
    void Start() {
        ShopItemInfo sii = shop.GetItem(id);
        itemText.SetText(sii.name);
        costText.SetText(sii.cost.ToString());
    }

    // Update is called once per frame
    void Update() {

    }

    public void ShowDetails() {
        Vector3 newPos = new Vector3(0, 0, 0);
        GameObject details = Instantiate(shopItemDetail, newPos, Quaternion.identity);
        details.transform.SetParent(spawnPoint, false);
        ShopItemDetail newShopItemDetail = details.GetComponent<ShopItemDetail>();
        newShopItemDetail.Shop = shop;
        newShopItemDetail.Parent = this;
        newShopItemDetail.Id = id;
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
