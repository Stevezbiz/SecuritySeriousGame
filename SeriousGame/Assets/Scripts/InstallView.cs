using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstallView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextMeshProUGUI durationText;
    [SerializeField] EmployeeView employeeView;

    EmployeeCode employee;
    List<Task> tasks = new List<Task>();

    public void Load(EmployeeCode id) {
        tasks.Clear();
        tasks = gameManager.GetAvailableTasksByType(TaskType.INSTALL);
        if (tasks.Count == 0) {
            Instantiate(windowPopUp, gameManager.gameObject.transform, false).GetComponent<WindowPopUp>().Load("Non ci sono installazioni richieste al momento", ActionCode.CONTINUE);
        } else {
            employee = id;
            List<string> options = new List<string>();
            foreach (Task t in tasks) {
                options.Add(gameManager.GetShopItem(t.shopItem).name);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = 0;
            Display(0);
            gameObject.SetActive(true);
        }
    }

    public void Display(int err) {
        int duration = gameManager.GetInstallDuration(employee, tasks[dropdown.value].shopItem);
        durationText.SetText("Durata: " + duration + " h");
    }

    public void InstallItem() {
        gameManager.AssignEmployee(employee, tasks[dropdown.value].id);
        Close();
    }

    public void Close() {
        employeeView.Load();
        gameObject.SetActive(false);
    }
}
