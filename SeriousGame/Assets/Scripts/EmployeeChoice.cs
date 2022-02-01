using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class EmployeeChoice : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown employeeDropdown;
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
    [SerializeField] TextMeshProUGUI durationText;

    ShopItemDetail shopItemDetail;
    AttackView attackView;
    List<EmployeeInfo> employees;
    ShopItemCode sid;
    AttackCode aid;
    TaskType type;
    Category category;

    public void Load(ShopItemCode id, Category category, ShopItemDetail shopItemDetail, TaskType type) {
        this.sid = id;
        this.category = category;
        this.shopItemDetail = shopItemDetail;
        this.type = type;
        // fill the options of the dropdown element
        employees = gameManager.GetAvailableEmployees();
        if (employees.Count == 0) {
            gameManager.DisplayMessage("Tutti gli impiegati sono già occupati", ActionCode.CONTINUE, Role.SECURITY);
        } else {
            List<string> options = new List<string>();
            foreach (EmployeeInfo el in employees) {
                options.Add(el.name);
            }
            employeeDropdown.ClearOptions();
            employeeDropdown.AddOptions(options);
            employeeDropdown.value = 0;
            Display(0);
            gameObject.SetActive(true);
        }
    }

    public void Load(AttackCode id, Category category, AttackView attackView) {
        this.aid = id;
        this.category = category;
        this.attackView = attackView;
        type = TaskType.REPAIR;
        // fill the options of the dropdown element
        employees = gameManager.GetAvailableEmployees();
        if (employees.Count == 0) {
            gameManager.DisplayMessage("Tutti gli impiegati sono già occupati", ActionCode.CONTINUE, Role.SECURITY);
        } else {
            List<string> options = new List<string>();
            foreach (EmployeeInfo el in employees) {
                options.Add(el.name);
            }
            employeeDropdown.ClearOptions();
            employeeDropdown.AddOptions(options);
            employeeDropdown.value = 0;
            Display(0);
            gameObject.SetActive(true);
        }
    }

    public void Display(int err) {
        EmployeeInfo e = employees[employeeDropdown.value];
        descriptionText.SetText(e.description);
        Dictionary<Category, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[Category.NETWORK] / 10;
        accessBar.fillAmount = abilities[Category.ACCESS] / 10;
        softwareBar.fillAmount = abilities[Category.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[Category.ASSET] / 10;
        servicesBar.fillAmount = abilities[Category.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
        switch (type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                durationText.SetText("Durata: " + gameManager.GetInstallTaskDuration(e.id, sid) + " h");
                break;
            case TaskType.REPAIR:
                durationText.SetText("Durata: " + gameManager.GetRepairTaskDuration(e.id, aid) + " h");
                break;
            case TaskType.UPGRADE:
                durationText.SetText("Durata: " + gameManager.GetUpgradeTaskDuration(e.id, sid) + " h");
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
        }
        networkOutline.SetActive(false);
        accessOutline.SetActive(false);
        softwareOutline.SetActive(false);
        assetOutline.SetActive(false);
        servicesOutline.SetActive(false);
        switch (category) {
            case Category.NETWORK:
                networkOutline.SetActive(true);
                break;
            case Category.ACCESS:
                accessOutline.SetActive(true);
                break;
            case Category.SOFTWARE:
                softwareOutline.SetActive(true);
                break;
            case Category.ASSET:
                assetOutline.SetActive(true);
                break;
            case Category.SERVICES:
                servicesOutline.SetActive(true);
                break;
            default:
                Debug.Log("Error: undefined Category");
                break;
        }
    }

    public void AssignEmployee() {
        switch (type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                shopItemDetail.InstallItem(employees[employeeDropdown.value].id);
                break;
            case TaskType.REPAIR:
                attackView.Repair(employees[employeeDropdown.value].id);
                break;
            case TaskType.UPGRADE:
                shopItemDetail.UpgradeItem(employees[employeeDropdown.value].id);
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
        }
        Close();
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
