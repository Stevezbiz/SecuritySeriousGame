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
        networkShop.GetComponent<Shop>().Init(Category.NETWORK);
        accessShop.GetComponent<Shop>().Init(Category.ACCESS);
        softwareShop.GetComponent<Shop>().Init(Category.SOFTWARE);
        assetShop.GetComponent<Shop>().Init(Category.ASSET);
        servicesShop.GetComponent<Shop>().Init(Category.SERVICES);
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
