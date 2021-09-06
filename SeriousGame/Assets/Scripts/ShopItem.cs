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
    ShopItemInfo shopItemInfo;

    public RectTransform SpawnPoint { get => spawnPoint; set => spawnPoint = value; }
    public ShopItemInfo ShopItemInfo { get => shopItemInfo; set => shopItemInfo = value; }

    // Start is called before the first frame update
    void Start() {
        itemText.SetText(shopItemInfo.name);
        costText.SetText(shopItemInfo.cost.ToString());
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
    }

    public void Purchase() {
        ownedImage.SetActive(true);
        costText.SetText("");
    }
}
