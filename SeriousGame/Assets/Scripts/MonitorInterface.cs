using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorInterface : MonoBehaviour {
    [SerializeField] CategoryView categoryView;

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
}
