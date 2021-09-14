using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGUI : MonoBehaviour {
    [SerializeField] GameObject networkShop;
    [SerializeField] GameObject authenticationShop;
    [SerializeField] GameObject softwareShop;
    [SerializeField] GameObject employeeShop;
    [SerializeField] GameObject customerCareShop;
    [SerializeField] GameObject details;
    [SerializeField] GameObject shopButton;

    float oldTimeScale = 1f;
    int shopItemId = 0;

    Dictionary<int, ShopItemInfo> items = new Dictionary<int, ShopItemInfo>();

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public int AddItem(ShopItemInfo si) {
        items.Add(shopItemId, si);
        return shopItemId++;
    }

    public ShopItemInfo GetItem(int id) {
        return items[id];
    }

    public void SetItem(ShopItemInfo sii) {
        items[sii.id] = sii;
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
        shopButton.SetActive(false);
    }

    public void CloseShop() {
        if (details.transform.childCount > 0) {
            Destroy(details.transform.GetChild(0).gameObject);
        }
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        shopButton.SetActive(true);
        networkShop.SetActive(false);
        authenticationShop.SetActive(false);
        softwareShop.SetActive(false);
        employeeShop.SetActive(false);
        customerCareShop.SetActive(false);
    }
}
