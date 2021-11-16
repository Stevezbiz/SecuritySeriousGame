using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RepairView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextMeshProUGUI durationText;

    EmployeeCode employee;
    List<Task> tasks = new List<Task>();

    public void Load(EmployeeCode id) {
        tasks.Clear();
        tasks = gameManager.GetTasksByType(TaskType.REPAIR);
        if (tasks.Count == 0) {
            GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
            newWindow.transform.SetParent(gameManager.gameObject.transform, false);
            newWindow.GetComponent<WindowPopUp>().Load("Non ci sono attacchi in corso al momento", ActionCode.CONTINUE);
        } else {
            employee = id;
            List<string> options = new List<string>();
            foreach (Task t in tasks) {
                options.Add(gameManager.GetAttack(t.attack).name);
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = 0;
            Display(0);
            gameObject.SetActive(true);
        }
    }

    public void Display(int err) {
        int duration = gameManager.GetAttackDuration(employee, tasks[dropdown.value].attack);
        durationText.SetText("Durata: " + duration + " h");
    }

    public void RepairAttack() {
        gameManager.RepairAttack(tasks[dropdown.value].attack, employee);
    }

    public void Close() {
        gameObject.SetActive(false);
    }
}
