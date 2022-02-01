using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] GameObject employeeCard;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject installButton;
    [SerializeField] GameObject repairButton;
    [SerializeField] GameObject preventButton;
    [SerializeField] GameObject endTaskButton;
    [SerializeField] TextMeshProUGUI labelText;
    [SerializeField] InstallOrUpgradeView installView;
    [SerializeField] RepairView repairView;
    [SerializeField] PreventView preventView;

    float oldTimeScale = 1f;
    List<EmployeeInfo> employees;
    List<GameObject> toDestroy = new List<GameObject>();
    EmployeeCode selected;

    public void Load() {
        foreach (GameObject obj in toDestroy) {
            Destroy(obj);
        }
        toDestroy.Clear();
        employees = gameManager.GetHiredEmployees();
        Debug.Log("EmployeeView::employees.Count: " + employees.Count);
        foreach (EmployeeInfo e in employees) {
            GameObject newEmployee = Instantiate(employeeCard, content, false);
            newEmployee.GetComponent<EmployeeCard>().Load(e, gameManager, this);
            toDestroy.Add(newEmployee);
        }
        DisableButtons();
        labelText.SetText("Seleziona un impiegato per visualizzare le varie opzioni");
    }

    public void Select(EmployeeCode id) {
        selected = id;
        EmployeeInfo employee = gameManager.GetEmployee(id);
        DisableButtons();
        switch (employee.status) {
            case TaskType.NONE:
                EnableButtons();
                labelText.SetText("Cosa deve fare " + employee.name + "?");
                break;
            case TaskType.INSTALL:
                labelText.SetText(employee.name + " sta installando " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(id)).name);
                break;
            case TaskType.REPAIR:
                labelText.SetText(employee.name + " sta riparando i danni provocati dall'attacco " + gameManager.GetAttack((AttackCode)gameManager.GetTaskTarget(id)).name);
                break;
            case TaskType.UPGRADE:
                labelText.SetText(employee.name + " sta installando " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(id)).name);
                break;
            case TaskType.PREVENT:
                endTaskButton.SetActive(true);
                string category;
                switch ((Category)gameManager.GetTaskTarget(id)) {
                    case Category.NETWORK:
                        category = "Rete";
                        break;
                    case Category.ACCESS:
                        category = "Accesso";
                        break;
                    case Category.SOFTWARE:
                        category = "Software";
                        break;
                    case Category.ASSET:
                        category = "Risorse";
                        break;
                    case Category.SERVICES:
                        category = "Servizi";
                        break;
                    default:
                        Debug.Log("Error: undefined Category");
                        category = "";
                        break;
                }
                labelText.SetText(employee.name + " sta facendo prevenzione nel settore " + category);
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
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
        endTaskButton.SetActive(false);
    }

    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;
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
        repairView.Load(selected);
    }

    public void Prevent() {
        preventView.Load(selected);
    }

    public void EndTask() {
        gameManager.EndTask(gameManager.GetPreventionTask(selected));
        Load();
    }
}
