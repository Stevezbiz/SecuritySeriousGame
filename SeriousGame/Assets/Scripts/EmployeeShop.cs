using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;
using Outline = UnityEngine.UI.Outline;
using Button = UnityEngine.UI.Button;

public class EmployeeShop : MonoBehaviour {
    [SerializeField] TextAsset employeesFileJSON;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeItem;
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] GameObject notAvailable;
    [SerializeField] GameObject hireButton;
    [SerializeField] Image employeeIcon;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] TextMeshProUGUI hireText;

    bool full = false;
    EmployeeCode current = EmployeeCode.LUIGI;
    List<EmployeeCode> indexes = new List<EmployeeCode>();
    Dictionary<EmployeeCode, EmployeeItem> employees = new Dictionary<EmployeeCode, EmployeeItem>();

    /**
     * <summary>Initialize the data structures</summary>
     */
    public void Init() {
        EmployeesJSON employeesContent = JsonUtility.FromJson<EmployeesJSON>(employeesFileJSON.text);
        foreach (EmployeeInfo e in employeesContent.employees) {
            gameManager.AddToEmployees(e);
            indexes.Add(e.id);
        }
    }

    /**
     * <summary>Load all the items in the list</summary>
     */
    public void Load() {
        foreach (EmployeeCode id in indexes) {
            AddEmployeeRecord(gameManager.GetEmployee(id));
        }
    }

    /**
     * <summary>Load an item in the list</summary>
     */
    void AddEmployeeRecord(EmployeeInfo e) {
        // create the new item
        GameObject newRecord = Instantiate(employeeItem);
        newRecord.transform.SetParent(content, false);
        newRecord.GetComponent<EmployeeItem>().Load(e, this);
        employees[e.id] = newRecord.GetComponent<EmployeeItem>();
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        // set the aspect of the navigation elements
        gameObject.SetActive(true);
        ComposeDetails(0);
    }

    /**
     * <summary>Compose all the details of the employee</summary>
     */
    public void ComposeDetails(EmployeeCode id) {
        EmployeeInfo e = gameManager.GetEmployee(id);
        current = id;
        // set the description fields
        titleText.SetText(e.name);
        descriptionText.SetText(e.description);
        Dictionary<ShopItemCategory, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[ShopItemCategory.NETWORK] / 10;
        accessBar.fillAmount = abilities[ShopItemCategory.ACCESS] / 10;
        softwareBar.fillAmount = abilities[ShopItemCategory.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[ShopItemCategory.ASSET] / 10;
        servicesBar.fillAmount = abilities[ShopItemCategory.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
        // set the graphic aspect
        int tot = gameManager.GetTotalEmployees();
        int hired = gameManager.GetHiredEmployees();
        if (tot == hired) full = true;
        else full = false;
        countText.SetText(hired + "/" + tot);
        if (full) {
            countText.color = COLOR.YELLOW;
            employeeIcon.color = COLOR.YELLOW;
            notAvailable.SetActive(true);
            hireButton.GetComponent<Outline>().effectColor = COLOR.GREEN_DISABLED;
            hireButton.SetActive(true);
            hireButton.GetComponentInChildren<Button>().interactable = false;
            hireText.color = COLOR.GREEN_DISABLED;
        } else {
            countText.color = COLOR.GREEN;
            employeeIcon.color = COLOR.GREEN;
            notAvailable.SetActive(false);
            hireButton.GetComponent<Outline>().effectColor = COLOR.GREEN;
            hireButton.SetActive(true);
            hireButton.GetComponentInChildren<Button>().interactable = true;
            hireText.color = COLOR.GREEN;
        }
        if (e.owned) {
            notAvailable.SetActive(false);
            hireButton.SetActive(false);
        }
    }

    /**
     * <summary>Applies the effects of hiring an employee</summary>
     */
    public void Hire() {
        gameManager.HireEmployee(current);
        employees[current].Hire();
        ComposeDetails(current);
    }
}