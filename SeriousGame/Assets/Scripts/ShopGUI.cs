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

    float oldTimeScale = 1f;

    Dictionary<int, ShopItemInfo> items = new Dictionary<int, ShopItemInfo>();

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void AddItem(ShopItemInfo sii) {
        if(items.ContainsKey(sii.id)) items[sii.id] = sii;
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
        employeeShop.SetActive(false);
        customerCareShop.SetActive(false);
        details.SetActive(false);
    }
}
