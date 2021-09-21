using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] RectTransform content;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] GameObject details;
    [SerializeField] ShopGUI shop;

    // Start is called before the first frame update
    void Start() {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        ShopJSON shopContent = JsonUtility.FromJson<ShopJSON>(shopFileJSON.text);
        foreach (ShopItemInfo item in shopContent.powerUps) {
            AddShopRecord(item);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void AddShopRecord(ShopItemInfo shopItemInfo) {
        // create the new item
        GameObject newRecord = Instantiate(shopItem);
        newRecord.transform.SetParent(content, false);
        ShopItem newShopItem = newRecord.GetComponent<ShopItem>();
        newShopItem.Id = shopItemInfo.id;
        shop.AddItem(shopItemInfo);
        newShopItem.SpawnPoint = details.GetComponent<RectTransform>();
        newShopItem.Shop = shop;
        newRecord.name = "ShopItem" + newShopItem.Id.ToString();
    }

    public void OpenList() {
        gameObject.SetActive(true);
    }

    public void CloseList() {
        gameObject.SetActive(false);
    }
}
