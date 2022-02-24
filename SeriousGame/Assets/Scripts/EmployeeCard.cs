using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class EmployeeCard : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] GameObject networkOutline;
    [SerializeField] GameObject accessOutline;
    [SerializeField] GameObject softwareOutline;
    [SerializeField] GameObject assetOutline;
    [SerializeField] GameObject servicesOutline;

    EmployeeInfo employee;
    GameManager gameManager;
    EmployeeView parent;
    Task task;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(GameManager gameManager, EmployeeInfo e, EmployeeView parent, Task t) {
        this.employee = e;
        this.gameManager = gameManager;
        this.parent = parent;
        this.task = t;
        nameText.SetText(e.name.ToLower());
        icon.sprite = gameManager.GetEmployeeIcon(e.id);
        descriptionText.SetText(e.description);
        Dictionary<CategoryCode, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[CategoryCode.NETWORK] / 10;
        accessBar.fillAmount = abilities[CategoryCode.ACCESS] / 10;
        softwareBar.fillAmount = abilities[CategoryCode.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[CategoryCode.ASSET] / 10;
        servicesBar.fillAmount = abilities[CategoryCode.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.GetMoneyGain() + " F/h");
        networkOutline.SetActive(false);
        accessOutline.SetActive(false);
        softwareOutline.SetActive(false);
        assetOutline.SetActive(false);
        servicesOutline.SetActive(false);
        switch (t.category) {
            case CategoryCode.NETWORK:
                networkOutline.SetActive(true);
                break;
            case CategoryCode.ACCESS:
                accessOutline.SetActive(true);
                break;
            case CategoryCode.SOFTWARE:
                softwareOutline.SetActive(true);
                break;
            case CategoryCode.ASSET:
                assetOutline.SetActive(true);
                break;
            case CategoryCode.SERVICES:
                servicesOutline.SetActive(true);
                break;
            default:
                Debug.Log("Error: undefined Category");
                break;
        }
    }

    /**
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {
        parent.SelectEmployee(employee.id);
    }
}
