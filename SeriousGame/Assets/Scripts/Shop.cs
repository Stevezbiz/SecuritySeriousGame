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
    List<GameObject> toDestroy = new List<GameObject>();

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
        foreach (GameObject obj in toDestroy) {
            Destroy(obj);
        }
        toDestroy.Clear();
        foreach (ShopItemCode id in indexes) {
            AddShopRecord(gameManager.GetShopItem(id));
        }
    }

    /**
     * <summary>Load an item in the list</summary>
     */
    void AddShopRecord(ShopItemInfo sii) {
        // create the new item
        GameObject newItem = Instantiate(shopItem);
        newItem.transform.SetParent(content, false);
        newItem.GetComponent<ShopItem>().Load(sii, gameManager, this, details);
        toDestroy.Add(newItem);
    }

    /**
     * <summary>Open the list of items</summary>
     */
    public void OpenList() {
        Load();
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the list of items</summary>
     */
    public void CloseList() {
        gameObject.SetActive(false);
    }
}
