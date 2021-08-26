using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour {
    [SerializeField] GameObject shopItemDetail;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject ownedImage;

    RectTransform spawnPoint;
    int id;
    string item;
    string description;
    int cost;
    bool owned;

    public RectTransform SpawnPoint { get => spawnPoint; set => spawnPoint = value; }
    public int Id { get => id; set => id = value; }
    public string Item { get => item; set => item = value; }
    public string Description { get => description; set => description = value; }
    public int Cost { get => cost; set => cost = value; }
    public bool Owned { get => owned; set => owned = value; }

    // Start is called before the first frame update
    void Start() {
        itemText.SetText(Item);
        costText.SetText(Cost.ToString());
    }

    // Update is called once per frame
    void Update() {

    }

    public void ShowDetails() {
        Vector3 newPos = new Vector3(0, 0, 0);
        GameObject details = Instantiate(shopItemDetail, newPos, Quaternion.identity);
        details.transform.SetParent(SpawnPoint, false);
        ShopItemDetail newShopItemDetail = details.GetComponent<ShopItemDetail>();
        newShopItemDetail.Parent = this;
        newShopItemDetail.Item = Item;
        newShopItemDetail.Description = Description;
        newShopItemDetail.Cost = Cost;
        newShopItemDetail.Owned = Owned;
    }

    public void Purchase() {
        ownedImage.SetActive(true);
        costText.SetText("");
        Owned = true;
    }
}
