using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;

public class EmployeeItem : MonoBehaviour {
    [SerializeField] GameObject employeeDetails;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI targetText;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] GameObject progressBar;
    [SerializeField] Image bar;

    GameManager gameManager;
    EmployeeInfo employee;

    public void Load(GameManager gameManager, EmployeeInfo e, Sprite s) {
        this.gameManager = gameManager;
        this.employee = e;
        icon.sprite = s;
        nameText.SetText(e.name.ToLower());
        switch (e.status) {
            case TaskType.NONE:
                statusText.SetText("NON ASSEGNATO");
                targetText.SetText("-");
                progressText.SetText("-");
                progressBar.SetActive(false);
                break;
            case TaskType.INSTALL:
                statusText.SetText("INSTALLAZIONE");
                targetText.SetText(gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.UPGRADE:
                statusText.SetText("POTENZIAMENTO");
                targetText.SetText(gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.REPAIR:
                statusText.SetText("RIPARAZIONE");
                targetText.SetText(gameManager.GetAttack((AttackCode)gameManager.GetTaskTarget(e.id)).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = gameManager.GetTaskProgress(e.id);
                break;
            case TaskType.PREVENT:
                statusText.SetText("PREVENZIONE");
                targetText.SetText(gameManager.GetCategory((CategoryCode)gameManager.GetTaskTarget(e.id)).name);
                progressText.SetText("-");
                progressBar.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
    }

    public void OpenDetails() {
        Instantiate(employeeDetails, gameManager.gameObject.transform, false).GetComponent<EmployeeDetails>().Load(employee, gameManager);
    }
}
