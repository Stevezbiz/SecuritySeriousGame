using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class EmployeeList : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeItem;
    [SerializeField] TaskList taskList;
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI selector;
    [SerializeField] Button selectorButton;

    List<GameObject> toDestroy = new List<GameObject>();

    void Load() {
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach (EmployeeInfo e in gameManager.GetHiredEmployees()) {
            GameObject newItem = Instantiate(employeeItem, content, false);
            newItem.GetComponent<EmployeeItem>().Load(gameManager, e, gameManager.GetEmployeeIcon(e.id));
            toDestroy.Add(newItem);
        }
        content.localPosition = new Vector3(content.localPosition.x, 0f, content.localPosition.z);
    }

    public void OpenView() {
        selector.color = COLOR.GREEN;
        selectorButton.interactable = false;
        taskList.CloseView();
        Load();
        gameObject.SetActive(true);
    }

    public void CloseView() {
        selector.color = COLOR.GREEN_DISABLED;
        selectorButton.interactable = true;
        gameObject.SetActive(false);
    }
}
