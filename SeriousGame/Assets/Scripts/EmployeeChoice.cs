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
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextMeshProUGUI durationText;

    List<EmployeeInfo> employees;
    ShopItemCode id;

    public void Load(ShopItemCode id) {
        this.id = id;
        // fill the options of the dropdown element
        employees = gameManager.GetAvailableEmployees();
        if (employees.Count == 0) {
            GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
            newWindow.transform.SetParent(gameManager.gameObject.transform, false);
            newWindow.GetComponent<WindowPopUp>().Load("Tutti gli impiegati sono già occupati", ActionCode.CONTINUE);
        } else {
            List<string> options = new List<string>();
            foreach(EmployeeInfo el in employees) {
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
        durationText.SetText("Durata: " + gameManager.GetInstallDuration(e.id, id) + " h");
    }

    public void AssignEmployee() {
        shopItemDetail.InstallItem(employees[employeeDropdown.value].id);
        gameObject.SetActive(false);
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
