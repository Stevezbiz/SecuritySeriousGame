using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] RectTransform content;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] ShopItemDetail details;
    [SerializeField] GameManager gameManager;

    List<ShopItemCode> indexes = new List<ShopItemCode>();

    /**
     * <summary>Initialize the data structures</summary>
     */
    public void Init() {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        ShopJSON shopContent = JsonUtility.FromJson<ShopJSON>(shopFileJSON.text);
        foreach (ShopItemInfo item in shopContent.powerUps) {
            gameManager.AddToShopItems(item);
            indexes.Add(item.id);
        }
    }

    /**
     * <summary>Load all the items in the list</summary>
     */
    public void Load() {
        foreach (ShopItemCode id in indexes) {
            AddShopRecord(gameManager.GetShopItem(id));
        }
    }

    /**
     * <summary>Load an item in the list</summary>
     */
    void AddShopRecord(ShopItemInfo sii) {
        // create the new item
        GameObject newRecord = Instantiate(shopItem);
        newRecord.transform.SetParent(content, false);
        newRecord.GetComponent<ShopItem>().Load(sii, details);
    }

    /**
     * <summary>Open the list of items</summary>
     */
    public void OpenList() {
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the list of items</summary>
     */
    public void CloseList() {
        gameObject.SetActive(false);
    }
}
