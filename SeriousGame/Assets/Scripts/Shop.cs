using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] RectTransform content;
    [SerializeField] ShopItemDetail details;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bottomPanel;

    List<ShopItemCode> indexes = new List<ShopItemCode>();
    List<GameObject> toDestroy = new List<GameObject>();

    /**
     * <summary>Initialize the data structures</summary>
     */
    public void Init() {
        foreach (CategoryCode c in typeof(CategoryCode).GetEnumValues()) {
            indexes.AddRange(gameManager.GetShopItemsByCategory(c));
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
        GameObject newItem = Instantiate(shopItem, content, false);
        newItem.GetComponent<ShopItem>().Load(sii, gameManager, this, details);
        toDestroy.Add(newItem);
    }

    /**
     * <summary>Open the list of items</summary>
     */
    public void OpenShop() {
        TimeManager.Pause();
        bottomPanel.SetActive(false);
        Load();
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the list of items</summary>
     */
    public void CloseShop() {
        TimeManager.Resume();
        details.gameObject.SetActive(false);
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
