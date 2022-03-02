using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] GameObject employeeCard;
    [SerializeField] EmployeeList employeeList;
    [SerializeField] GameObject employeeChoice;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject selection;
    [SerializeField] GameObject unselection;
    [SerializeField] GameObject assignButton;
    [SerializeField] TextMeshProUGUI employeeText;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] TextMeshProUGUI titleText;

    List<GameObject> toDestroy = new List<GameObject>();
    EmployeeCode selected;
    Task task;

    public void Load(Task t) {
        this.task = t;
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach (EmployeeInfo e in gameManager.GetAvailableEmployees()) {
            GameObject newItem = Instantiate(employeeCard, content, false);
            newItem.GetComponent<EmployeeCard>().Load(gameManager, e, this, t);
            toDestroy.Add(newItem);
        }
        if (toDestroy.Count == 0) {
            gameManager.DisplayMessage("Nessun impiegato è al momento disponibile.", ActionCode.CONTINUE, Role.SECURITY);
        } else {
            switch (task.type) {
                case TaskType.NONE:
                    break;
                case TaskType.INSTALL:
                    titleText.SetText("INSTALLAZIONE " + gameManager.GetShopItem(task.shopItem).name);
                    break;
                case TaskType.UPGRADE:
                    titleText.SetText("POTENZIAMENTO " + gameManager.GetShopItem(task.shopItem).name);
                    break;
                case TaskType.REPAIR:
                    titleText.SetText("RIPARAZIONE " + gameManager.GetAttack(task.attack).name);
                    break;
                case TaskType.PREVENT:
                    titleText.SetText("PREVENZIONE " + gameManager.GetCategory(task.category).name);
                    break;
                default:
                    Debug.Log("Error: unexpected TaskType");
                    break;
            }
            selection.SetActive(false);
            assignButton.SetActive(false);
            unselection.SetActive(true);
            employeeChoice.SetActive(true);
        }
    }

    public void OpenView() {
        TimeManager.Pause();
        employeeList.OpenView();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void CloseView() {
        TimeManager.Resume();
        bottomPanel.SetActive(true);
        employeeChoice.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Back() {
        employeeChoice.SetActive(false);
    }

    public void SelectEmployee(EmployeeCode id) {
        selected = id;
        unselection.SetActive(false);
        selection.SetActive(true);
        assignButton.SetActive(true);
        employeeText.SetText("Impiegato: " + gameManager.GetEmployee(id).name);
        switch (task.type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                valueText.SetText("Durata: " + gameManager.GetInstallTaskDuration(id, task.shopItem) + " h");
                break;
            case TaskType.UPGRADE:
                valueText.SetText("Durata: " + gameManager.GetUpgradeTaskDuration(id, task.shopItem) + " h");
                break;
            case TaskType.REPAIR:
                valueText.SetText("Durata: " + gameManager.GetRepairTaskDuration(id, task.attack) + " h");
                break;
            case TaskType.PREVENT:
                valueText.SetText("Protezione: " + (100 * gameManager.GetPreventProtection(id, task.category)).ToString("+0.") + "%");
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
    }

    public void Assign() {
        gameManager.AssignEmployee(selected, task.id);
        employeeList.OpenView();
        employeeChoice.SetActive(false);
    }
}
