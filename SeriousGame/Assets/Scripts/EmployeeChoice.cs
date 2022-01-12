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
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextMeshProUGUI durationText;


    ShopItemDetail shopItemDetail;
    AttackView attackView;
    List<EmployeeInfo> employees;
    ShopItemCode sid;
    AttackCode aid;
    TaskType type;

    public void Load(ShopItemCode id, ShopItemDetail shopItemDetail) {
        this.sid = id;
        this.shopItemDetail = shopItemDetail;
        type = TaskType.INSTALL;
        // fill the options of the dropdown element
        employees = gameManager.GetAvailableEmployees();
        if (employees.Count == 0) {
            Instantiate(windowPopUp, gameManager.gameObject.transform, false).GetComponent<WindowPopUp>().Load("Tutti gli impiegati sono già occupati", ActionCode.CONTINUE);
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

    public void Load(AttackCode id, AttackView attackView) {
        this.aid = id;
        this.attackView = attackView;
        type = TaskType.REPAIR;
        // fill the options of the dropdown element
        employees = gameManager.GetAvailableEmployees();
        if (employees.Count == 0) {
            Instantiate(windowPopUp, gameManager.gameObject.transform, false).GetComponent<WindowPopUp>().Load("Tutti gli impiegati sono già occupati", ActionCode.CONTINUE);
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
                durationText.SetText("Durata: " + gameManager.GetInstallDuration(e.id, sid) + " h");
                break;
            case TaskType.REPAIR:
                durationText.SetText("Durata: " + gameManager.GetAttackDuration(e.id, aid) + " h");
                break;
            default:
                Debug.Log("Error: undefined TaskType");
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
