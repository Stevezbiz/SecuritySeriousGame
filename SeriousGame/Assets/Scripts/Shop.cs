using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] Transform spawnPoint;
    [SerializeField] RectTransform content;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] GameObject details;

    float oldTimeScale = 1;
    int height = 100;

    Dictionary<int, ShopItem> items = new Dictionary<int, ShopItem>();

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

    public ShopItemInfo GetItem(int id) {
        return items[id].ShopItemInfo;
    }

    public void SetItem(ShopItemInfo sii) {
        items[sii.id].ShopItemInfo = sii;
    }

    void AddShopRecord(ShopItemInfo shopItemInfo) {
        // create the new item
        Vector3 newPos = new Vector3(0, -content.sizeDelta.y, 0);
        GameObject newRecord = Instantiate(shopItem, newPos, Quaternion.identity);
        newRecord.transform.SetParent(spawnPoint, false);
        newRecord.name = "ShopItem" + shopItemInfo.id.ToString();
        ShopItem newShopItem = newRecord.GetComponent<ShopItem>();
        newShopItem.ShopItemInfo = shopItemInfo;
        newShopItem.SpawnPoint = details.GetComponent<RectTransform>();
        items.Add(shopItemInfo.id, newShopItem);
        content.sizeDelta = new Vector2(0, items.Count * height);
    }

    public void OpenShop() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void CloseShop() {
        if (details.transform.childCount > 0) {
            Destroy(details.transform.GetChild(0).gameObject);
        }
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
    }
}
