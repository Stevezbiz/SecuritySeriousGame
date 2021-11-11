using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class EmployeeRecord : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] GameObject employeeCard;

    EmployeeInfo employee;
    GameManager gameManager;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(EmployeeInfo e, GameManager gameManager) {
        this.employee = e;
        this.gameManager = gameManager;
        nameText.SetText(e.name);
        if (e.status == TaskType.NONE) statusText.SetText("DISPONIBILE");
        else statusText.SetText("OCCUPATO");
    }

    /**
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {
        if (employee.status == TaskType.NONE) {
            gameObject.GetComponent<Button>().Select();
        }
    }

    public void PrintInfo() {
        GameObject newCard = Instantiate(employeeCard, new Vector3(0, 0, 0), Quaternion.identity);
        newCard.transform.SetParent(gameManager.gameObject.transform, false);
        newCard.GetComponent<EmployeeCard>().Load(employee);
    }
}
