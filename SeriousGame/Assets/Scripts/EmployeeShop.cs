using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class EmployeeShop : MonoBehaviour {
    [SerializeField] TextAsset employeesFileJSON;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeItem;
    [SerializeField] RectTransform listContent;
    [SerializeField] RectTransform descriptionContent;
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
        employees[e.id] = Instantiate(employeeItem, listContent, false).GetComponent<EmployeeItem>();
        employees[e.id].Load(e, this);
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        // set the aspect of the navigation elements
        gameObject.SetActive(true);
        ComposeDetails(indexes[0]);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        // set the aspect of the navigation elements
        gameObject.SetActive(false);
    }

    /**
     * <summary>Compose all the details of the employee</summary>
     */
    public void ComposeDetails(EmployeeCode id) {
        EmployeeInfo e = gameManager.GetEmployee(id);
        current = id;
        // set the description fields
        titleText.SetText(e.name.ToLower());
        descriptionText.SetText(e.description);
        Dictionary<Category, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[Category.NETWORK] / 10;
        accessBar.fillAmount = abilities[Category.ACCESS] / 10;
        softwareBar.fillAmount = abilities[Category.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[Category.ASSET] / 10;
        servicesBar.fillAmount = abilities[Category.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
        // set the graphic aspect
        int tot = gameManager.GetTotalEmployeesN();
        int hired = gameManager.GetHiredEmployeesN();
        if (tot == hired) full = true;
        else full = false;
        countText.SetText(hired + "/" + tot);
        if (full) {
            countText.color = COLOR.YELLOW;
            employeeIcon.color = COLOR.YELLOW;
            notAvailable.SetActive(true);
            hireButton.SetActive(false);
        } else {
            countText.color = COLOR.GREEN;
            employeeIcon.color = COLOR.GREEN;
            notAvailable.SetActive(false);
            hireButton.SetActive(true);
        }
        if (e.owned) {
            notAvailable.SetActive(false);
            hireButton.SetActive(false);
        }
        descriptionContent.SetPositionAndRotation(new Vector3(descriptionContent.position.x, 0f, descriptionContent.position.z), Quaternion.identity);
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