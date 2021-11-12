using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmployeeView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] GameObject employeeCard;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject installButton;
    [SerializeField] GameObject repairButton;
    [SerializeField] GameObject preventButton;
    [SerializeField] TextMeshProUGUI labelText;
    [SerializeField] InstallView installView;

    float oldTimeScale = 1f;
    List<EmployeeInfo> employees;
    List<GameObject> toDestroy = new List<GameObject>();
    EmployeeCode selected;

    void Load() {
        employees = gameManager.GetHiredEmployees();
        foreach(EmployeeInfo e in employees) {
            GameObject newEmployee = Instantiate(employeeCard);
            newEmployee.transform.SetParent(content, false);
            newEmployee.GetComponent<EmployeeCard>().Load(e, gameManager, this);
            toDestroy.Add(newEmployee);
        }
        DisableButtons();
        labelText.SetText("Seleziona un impiegato per visualizzare le varie opzioni");
    }

    public void Select(EmployeeCode id) {
        selected = id;
        EmployeeInfo employee = gameManager.GetEmployee(id);
        if (employee.status != TaskType.NONE) {
            DisableButtons();
            labelText.SetText(employee.name + " ha già un incarico in questo momento.");
        } else {
            EnableButtons();
            labelText.SetText("Cosa deve fare " + employee.name + "?");
        }
    }

    void EnableButtons() {
        installButton.SetActive(true);
        repairButton.SetActive(true);
        preventButton.SetActive(true);
    }

    void DisableButtons() {
        installButton.SetActive(false);
        repairButton.SetActive(false);
        preventButton.SetActive(false);
    }

    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        foreach (GameObject obj in toDestroy) {
            Destroy(obj);
        }
        toDestroy.Clear();
        Load();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void CloseView() {
        Time.timeScale = oldTimeScale;
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Install() {
        installView.Load(selected);
    }

    public void Repair() {

    }

    public void Prevent() {

    }
}
