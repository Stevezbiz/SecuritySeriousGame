using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] RectTransform content;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] ShopItemDetail details;
    [SerializeField] GameManager gameManager;

    List<int> items = new List<int>();

    public void Init() {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        ShopJSON shopContent = JsonUtility.FromJson<ShopJSON>(shopFileJSON.text);
        foreach (ShopItemInfo item in shopContent.powerUps) {
            gameManager.AddToShopItems(item);
            items.Add(item.id);
        }
    }

    public void Load() {
        foreach (int id in items) {
            AddShopRecord(gameManager.GetShopItem(id));
        }
    }

    void AddShopRecord(ShopItemInfo sii) {
        // create the new item
        GameObject newRecord = Instantiate(shopItem);
        newRecord.transform.SetParent(content, false);
        newRecord.GetComponent<ShopItem>().Load(sii, details);
    }

    public void OpenList() {
        gameObject.SetActive(true);
    }

    public void CloseList() {
        gameObject.SetActive(false);
    }
}
