using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmployeeView : MonoBehaviour {
    [SerializeField] TextAsset employeesFileJSON;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeItem;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI statsText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject notAvailable;
    [SerializeField] GameObject hireButton;
    [SerializeField] GameObject fireButton;

    float oldTimeScale = 1f;
    EmployeeCode current = EmployeeCode.GIGI;
    List<EmployeeCode> indexes = new List<EmployeeCode>();

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
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        // set the aspect of the navigation elements
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
        ComposeDetails(EmployeeCode.GIGI);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        bottomPanel.SetActive(true);
    }

    /**
     * <summary>Compose all the details of the employee</summary>
     */
    public void ComposeDetails(EmployeeCode id) {
        EmployeeInfo e = gameManager.GetEmployee(id);
        current = id;
        titleText.SetText(e.name);
        descriptionText.SetText(e.description);
        statsText.SetText("");
        costText.SetText("Costo: " + e.cost + " F/h");
        if (e.owned) {
            notAvailable.SetActive(false);
            hireButton.SetActive(false);
            fireButton.SetActive(true);
        } else {
            notAvailable.SetActive(true);
            hireButton.SetActive(true);
            fireButton.SetActive(false);
        }
    }

    /**
     * <summary>Applies the effects of hiring an employee</summary>
     */
    public void Hire() {
        gameManager.HireEmployee(current);
        ComposeDetails(current);
    }

    /**
     * <summary>Applies the effects of firing an employee</summary>
     */
    public void Fire() {
        gameManager.FireEmployee(current);
        ComposeDetails(current);
    }
}
