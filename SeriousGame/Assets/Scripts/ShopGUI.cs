using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGUI : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] GameObject networkShop;
    [SerializeField] GameObject authenticationShop;
    [SerializeField] GameObject softwareShop;
    [SerializeField] GameObject assetShop;
    [SerializeField] GameObject servicesShop;
    [SerializeField] GameObject details;

    float oldTimeScale = 1f;

    Dictionary<int, ShopItemInfo> items = new Dictionary<int, ShopItemInfo>();

    public void Init() {
        networkShop.GetComponent<Shop>().Init();
        authenticationShop.GetComponent<Shop>().Init();
        softwareShop.GetComponent<Shop>().Init();
        assetShop.GetComponent<Shop>().Init();
        servicesShop.GetComponent<Shop>().Init();
    }

    public void Load() {
        networkShop.GetComponent<Shop>().Load();
        authenticationShop.GetComponent<Shop>().Load();
        softwareShop.GetComponent<Shop>().Load();
        assetShop.GetComponent<Shop>().Load();
        servicesShop.GetComponent<Shop>().Load();
    }

    public void AddItem(ShopItemInfo sii) {
        if (items.ContainsKey(sii.id)) items[sii.id] = sii;
        else items.Add(sii.id, sii);
    }

    public ShopItemInfo GetItem(int id) {
        return items[id];
    }

    public bool ItemIsOn(int id) {
        return items[id].on;
    }

    public bool ItemIsOwned(int id) {
        return items[id].owned;
    }

    public void OpenShop() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void CloseShop() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        networkShop.SetActive(false);
        authenticationShop.SetActive(false);
        softwareShop.SetActive(false);
        assetShop.SetActive(false);
        servicesShop.SetActive(false);
        details.SetActive(false);
    }

    public ShopItemRecap[] GetShopItemRecap() {
        List<ShopItemRecap> sir = new List<ShopItemRecap>();

        foreach (ShopItemInfo sii in items.Values) {
            sir.Add(new ShopItemRecap(sii.id, sii.owned, sii.on));
        }

        return sir.ToArray();
    }

    public void SetShopItemRecap(ShopItemRecap[] sir) {
        foreach (ShopItemRecap s in sir) {
            if (s.owned) {
                items[s.id].owned = true;
                if (s.on) {
                    items[s.id].on = true;
                    gui.EnableShopItem(s.id);
                }
            }
        }
    }
}
