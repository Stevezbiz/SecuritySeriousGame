/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

    /**
     * <summary></summary>
     */
    public void OpenNetworkView() {
        categoryView.OpenView(CategoryCode.NETWORK);
    }

    /**
     * <summary></summary>
     */
    public void OpenAccessView() {
        categoryView.OpenView(CategoryCode.ACCESS);
    }

    /**
     * <summary></summary>
     */
    public void OpenSoftwareView() {
        categoryView.OpenView(CategoryCode.SOFTWARE);
    }

    /**
     * <summary></summary>
     */
    public void OpenAssetView() {
        categoryView.OpenView(CategoryCode.ASSET);
    }

    /**
     * <summary></summary>
     */
    public void OpenServicesView() {
        categoryView.OpenView(CategoryCode.SERVICES);
    }

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
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
