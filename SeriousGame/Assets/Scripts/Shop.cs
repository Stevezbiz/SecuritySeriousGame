using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopJSONObj {
    public string name;
    public string description;
    public int cost;
}

[System.Serializable]
public class ShopJSON {
    public ShopJSONObj[] powerUps;
}

public class Shop : MonoBehaviour {
    [SerializeField] GameObject shopItem;
    [SerializeField] Transform spawnPoint;
    [SerializeField] RectTransform content;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] GameObject details;

    GameObject canvas;
    float oldTimeScale = 1;
    int height = 100;
    int id = 0;

    Dictionary<int, ShopItem> items = new Dictionary<int, ShopItem>();

    // Start is called before the first frame update
    void Start() {
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        ShopJSON shopContent = JsonUtility.FromJson<ShopJSON>(shopFileJSON.text);
        foreach (ShopJSONObj p in shopContent.powerUps) {
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
            AddShopRecord(p.name, p.description, p.cost);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    void AddShopRecord(string name, string description, int cost) {
        Vector3 newPos = new Vector3(0, -content.sizeDelta.y, 0);
        GameObject newRecord = Instantiate(shopItem, newPos, Quaternion.identity);
        newRecord.transform.SetParent(spawnPoint, false);
        newRecord.name = "ShopItem" + id.ToString();
        ShopItem newShopItem = newRecord.GetComponent<ShopItem>();
        newShopItem.Id = id;
        newShopItem.Item = name;
        newShopItem.Description = description;
        newShopItem.Cost = cost;
        newShopItem.SpawnPoint = details.GetComponent<RectTransform>();
        items.Add(id++, newShopItem);
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
