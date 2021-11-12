using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstallView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] GameObject windowPopUp;

    EmployeeCode employee;
    List<Task> tasks = new List<Task>();

    public void Load(EmployeeCode id) {
        tasks.Clear();
        tasks = gameManager.GetTasksByType(TaskType.INSTALL);
        if (tasks.Count == 0) {
            GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
            newWindow.transform.SetParent(gameManager.gameObject.transform, false);
            newWindow.GetComponent<WindowPopUp>().Load("Non ci sono installazioni richieste al momento", ActionCode.CONTINUE);
        } else {
            employee = id;
            List<string> options = new List<string>();
            foreach (Task t in tasks) {
                options.Add(gameManager.GetShopItem(t.shopItem).name);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = 0;
            gameObject.SetActive(true);
        }
    }

    public void InstallItem() {
        gameManager.InstallShopItem(tasks[dropdown.value].shopItem, employee);
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
