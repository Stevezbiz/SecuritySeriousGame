using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class MonitorInterface : MonoBehaviour {
    [SerializeField] CategoryView categoryView;
    [SerializeField] GameObject networkAttackIcon;
    [SerializeField] GameObject accessAttackIcon;
    [SerializeField] GameObject softwareAttackIcon;
    [SerializeField] GameObject assetAttackIcon;
    [SerializeField] GameObject servicesAttackIcon;
    [SerializeField] GameObject networkEmployeeIcon;
    [SerializeField] GameObject accessEmployeeIcon;
    [SerializeField] GameObject softwareEmployeeIcon;
    [SerializeField] GameObject assetEmployeeIcon;
    [SerializeField] GameObject servicesEmployeeIcon;

    Dictionary<CategoryCode, int> counters = new Dictionary<CategoryCode, int>() {
        { CategoryCode.NETWORK, 0 },
        { CategoryCode.ACCESS, 0 },
        { CategoryCode.SOFTWARE, 0 },
        { CategoryCode.ASSET, 0 },
        { CategoryCode.SERVICES, 0 }
    };

    public void OpenNetworkView() {
        categoryView.OpenView(CategoryCode.NETWORK);
    }

    public void OpenAccessView() {
        categoryView.OpenView(CategoryCode.ACCESS);
    }

    public void OpenSoftwareView() {
        categoryView.OpenView(CategoryCode.SOFTWARE);
    }

    public void OpenAssetView() {
        categoryView.OpenView(CategoryCode.ASSET);
    }

    public void OpenServicesView() {
        categoryView.OpenView(CategoryCode.SERVICES);
    }

    public void EnableAttackIcon(CategoryCode c) {
        switch (c) {
            case CategoryCode.NETWORK:
                counters[c]++;
                networkAttackIcon.SetActive(true);
                break;
            case CategoryCode.ACCESS:
                counters[c]++;
                accessAttackIcon.SetActive(true);
                break;
            case CategoryCode.SOFTWARE:
                counters[c]++;
                softwareAttackIcon.SetActive(true);
                break;
            case CategoryCode.ASSET:
                counters[c]++;
                assetAttackIcon.SetActive(true);
                break;
            case CategoryCode.SERVICES:
                counters[c]++;
                servicesAttackIcon.SetActive(true);
                break;
            default:
                Debug.Log("Error: unexpected CategoryCode");
                break;
        }
    }

    public void DisableAttackIcon(CategoryCode c) {
        switch (c) {
            case CategoryCode.NETWORK:
                counters[c]--;
                if (counters[c] == 0) networkAttackIcon.SetActive(false);
                break;
            case CategoryCode.ACCESS:
                counters[c]--;
                if (counters[c] == 0) accessAttackIcon.SetActive(false);
                break;
            case CategoryCode.SOFTWARE:
                counters[c]--;
                if (counters[c] == 0) softwareAttackIcon.SetActive(false);
                break;
            case CategoryCode.ASSET:
                counters[c]--;
                if (counters[c] == 0) assetAttackIcon.SetActive(false);
                break;
            case CategoryCode.SERVICES:
                counters[c]--;
                if (counters[c] == 0) servicesAttackIcon.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected CategoryCode");
                break;
        }
    }

    public void EnableEmployeeIcon(CategoryCode c, Sprite s) {
        switch (c) {
            case CategoryCode.NETWORK:
                networkEmployeeIcon.GetComponent<Image>().sprite = s;
                networkEmployeeIcon.SetActive(true);
                break;
            case CategoryCode.ACCESS:
                accessEmployeeIcon.GetComponent<Image>().sprite = s;
                accessEmployeeIcon.SetActive(true);
                break;
            case CategoryCode.SOFTWARE:
                softwareEmployeeIcon.GetComponent<Image>().sprite = s;
                softwareEmployeeIcon.SetActive(true);
                break;
            case CategoryCode.ASSET:
                assetEmployeeIcon.GetComponent<Image>().sprite = s;
                assetEmployeeIcon.SetActive(true);
                break;
            case CategoryCode.SERVICES:
                servicesEmployeeIcon.GetComponent<Image>().sprite = s;
                servicesEmployeeIcon.SetActive(true);
                break;
            default:
                Debug.Log("Error: unexpected CategoryCode");
                break;
        }
    }

    public void DisableEmployeeIcon(CategoryCode c) {
        switch (c) {
            case CategoryCode.NETWORK:
                networkEmployeeIcon.SetActive(false);
                break;
            case CategoryCode.ACCESS:
                accessEmployeeIcon.SetActive(false);
                break;
            case CategoryCode.SOFTWARE:
                softwareEmployeeIcon.SetActive(false);
                break;
            case CategoryCode.ASSET:
                assetEmployeeIcon.SetActive(false);
                break;
            case CategoryCode.SERVICES:
                servicesEmployeeIcon.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected CategoryCode");
                break;
        }
    }
}
