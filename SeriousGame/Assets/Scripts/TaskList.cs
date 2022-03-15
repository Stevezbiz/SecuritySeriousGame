using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskList : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject taskItem;
    [SerializeField] EmployeeList empoyeeList;
    [SerializeField] RectTransform content;
    [SerializeField] EmployeeView employeeView;

    List<GameObject> toDestroy = new List<GameObject>();

    void Load() {
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach (Task t in gameManager.GetAssignedTasks()) {
            GameObject newItem = Instantiate(taskItem, content, false);
            newItem.GetComponent<TaskItem>().Load(gameManager, employeeView, t, true);
            toDestroy.Add(newItem);
        }
        foreach (Task t in gameManager.GetWaitingTasks()) {
            GameObject newItem = Instantiate(taskItem, content, false);
            newItem.GetComponent<TaskItem>().Load(gameManager, employeeView, t, false);
            toDestroy.Add(newItem);
        }
        content.SetPositionAndRotation(new Vector3(content.position.x, 0f, content.position.z), Quaternion.identity);
    }

    public void OpenView() {
        empoyeeList.CloseView();
        Load();
        gameObject.SetActive(true);
    }

    public void CloseView() {
        gameObject.SetActive(false);
    }
}
