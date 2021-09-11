using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] Transform spawnPoint;
    [SerializeField] RectTransform content;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] GameObject details;
    [SerializeField] ShopGUI shop;

    int height = 100;
    int count = 0;

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
        count++;
        Vector3 newPos = new Vector3(0, -content.sizeDelta.y, 0);
        GameObject newRecord = Instantiate(shopItem, newPos, Quaternion.identity);
        newRecord.transform.SetParent(spawnPoint, false);
        ShopItem newShopItem = newRecord.GetComponent<ShopItem>();
        newShopItem.Id = shop.AddItem(shopItemInfo);
        newShopItem.SpawnPoint = details.GetComponent<RectTransform>();
        newShopItem.Shop = shop;
        newRecord.name = "ShopItem" + newShopItem.Id.ToString();
        content.sizeDelta = new Vector2(0, count * height);
    }

    public void OpenList() {
        gameObject.SetActive(true);
    }

    public void CloseList() {
        gameObject.SetActive(false);
    }
}
