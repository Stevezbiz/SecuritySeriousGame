using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

public class TaskItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI targetText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] GameObject progressBar;
    [SerializeField] Image bar;
    [SerializeField] Button button;

    EmployeeView employeeView;
    Task task;

    public void Load(GameManager gameManager, EmployeeView employeeView, Task t, bool assigned) {
        this.task = t;
        this.employeeView = employeeView;
        switch (t.type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                typeText.SetText("INSTALLAZIONE");
                targetText.SetText(gameManager.GetShopItem(t.shopItem).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                break;
            case TaskType.UPGRADE:
                typeText.SetText("POTENZIAMENTO");
                targetText.SetText(gameManager.GetShopItem(t.shopItem).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                break;
            case TaskType.REPAIR:
                typeText.SetText("RIPARAZIONE");
                targetText.SetText(gameManager.GetAttack(t.attack).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                break;
            case TaskType.PREVENT:
                typeText.SetText("PREVENZIONE");
                targetText.SetText(gameManager.GetCategory(t.category).name);
                progressText.SetText("-");
                progressBar.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
        if (assigned) {
            button.interactable = false;
            statusText.SetText("ASSEGNATO");
        } else {
            statusText.SetText("NON ASSEGNATO");
        }
    }

    public void OpenEmployeeChoice() {
        employeeView.Load(task);
    }
}
