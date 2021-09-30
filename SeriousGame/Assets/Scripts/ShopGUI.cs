using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGUI : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject networkShop;
    [SerializeField] GameObject authenticationShop;
    [SerializeField] GameObject softwareShop;
    [SerializeField] GameObject assetShop;
    [SerializeField] GameObject servicesShop;
    [SerializeField] GameObject details;

    float oldTimeScale = 1f;


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
}
