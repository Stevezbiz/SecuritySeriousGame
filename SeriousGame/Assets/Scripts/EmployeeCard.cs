using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class EmployeeCard : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] GameObject employeeDetails;
    [SerializeField] GameObject progressBar;
    [SerializeField] Image bar;

    EmployeeInfo employee;
    GameManager gameManager;
    EmployeeView parent;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(EmployeeInfo e, GameManager gameManager, EmployeeView parent) {
        this.employee = e;
        this.gameManager = gameManager;
        this.parent = parent;
        nameText.SetText(e.name.ToLower());
        switch (e.status) {
            case TaskType.NONE:
                statusText.SetText("DISPONIBILE");
                progressBar.SetActive(false);
                break;
            case TaskType.INSTALL:
                statusText.SetText("INSTALLAZIONE");
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.REPAIR:
                statusText.SetText("RIPARAZIONE");
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
        }
    }

    /**
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {
        parent.Select(employee.id);
    }

    public void PrintDetails() {
        GameObject newDetails = Instantiate(employeeDetails, new Vector3(0, 0, 0), Quaternion.identity);
        newDetails.transform.SetParent(gameManager.gameObject.transform, false);
        newDetails.GetComponent<EmployeeDetails>().Load(employee, gameManager);
    }
}
