using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeList : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeItem;
    [SerializeField] TaskList taskList;
    [SerializeField] RectTransform content;

    List<GameObject> toDestroy = new List<GameObject>();

    void Load() {
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach(EmployeeInfo e in gameManager.GetHiredEmployees()) {
            GameObject newItem = Instantiate(employeeItem, content, false);
            newItem.GetComponent<EmployeeItem>().Load(gameManager, e, gameManager.GetEmployeeIcon(e.id));
            toDestroy.Add(newItem);
        }
    }

    public void OpenView() {
        taskList.CloseView();
        Load();
        gameObject.SetActive(true);
    }

    public void CloseView() {
        gameObject.SetActive(false);
    }
}
