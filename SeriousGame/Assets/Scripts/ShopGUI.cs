using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGUI : MonoBehaviour {
    [SerializeField] GameObject networkShop;
    [SerializeField] GameObject accessShop;
    [SerializeField] GameObject softwareShop;
    [SerializeField] GameObject assetShop;
    [SerializeField] GameObject servicesShop;
    [SerializeField] GameObject details;
    [SerializeField] GameObject bottomPanel;

    float oldTimeScale = 1f;

    /**
     * <summary>Initialize the data structures</summary>
     */
    public void Init() {
        networkShop.GetComponent<Shop>().Init();
        accessShop.GetComponent<Shop>().Init();
        softwareShop.GetComponent<Shop>().Init();
        assetShop.GetComponent<Shop>().Init();
        servicesShop.GetComponent<Shop>().Init();
    }

    /**
     * <summary>Load all the items in the shop</summary>
     */
    public void Load() {
        networkShop.GetComponent<Shop>().Load();
        accessShop.GetComponent<Shop>().Load();
        softwareShop.GetComponent<Shop>().Load();
        assetShop.GetComponent<Shop>().Load();
        servicesShop.GetComponent<Shop>().Load();
    }

    /**
     * <summary>Open the shop view</summary>
     */
    public void OpenShop() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the shop view</summary>
     */
    public void CloseShop() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        networkShop.SetActive(false);
        accessShop.SetActive(false);
        softwareShop.SetActive(false);
        assetShop.SetActive(false);
        servicesShop.SetActive(false);
        details.SetActive(false);
        bottomPanel.SetActive(true);
    }
}
