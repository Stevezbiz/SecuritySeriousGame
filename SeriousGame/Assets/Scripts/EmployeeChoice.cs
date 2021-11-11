using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;

public class EmployeeChoice : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] ShopItemDetail shopItemDetail;
    [SerializeField] TMP_Dropdown employeeDropdown;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;

    List<EmployeeInfo> employees;

    public void Load() {
        // fill the options of the dropdown element
        employees = gameManager.GetAvailableEmployees();
        List<string> options = new List<string>();
        foreach(EmployeeInfo el in employees) {
            options.Add(el.name);
        }
        employeeDropdown.ClearOptions();
        employeeDropdown.AddOptions(options);
        employeeDropdown.value = 0;
        Display(0);
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
    }

    public void Purchase() {
        shopItemDetail.PurchaseItem(employees[employeeDropdown.value].id);
        gameObject.SetActive(false);
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
