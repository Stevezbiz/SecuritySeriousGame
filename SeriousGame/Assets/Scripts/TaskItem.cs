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
    [SerializeField] GameObject stopButton;

    GameManager gameManager;
    EmployeeView employeeView;
    Task task;

    public void Load(GameManager gameManager, EmployeeView employeeView, Task t, bool assigned) {
        this.task = t;
        this.employeeView = employeeView;
        this.gameManager = gameManager;
        switch (t.type) {
            case TaskType.NONE:
                break;
            case TaskType.INSTALL:
                typeText.SetText("INSTALLAZIONE");
                targetText.SetText(gameManager.GetShopItem(t.shopItem).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                stopButton.SetActive(false);
                break;
            case TaskType.UPGRADE:
                typeText.SetText("POTENZIAMENTO");
                targetText.SetText(gameManager.GetShopItem(t.shopItem).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                stopButton.SetActive(false);
                break;
            case TaskType.REPAIR:
                typeText.SetText("RIPARAZIONE");
                targetText.SetText(gameManager.GetAttack(t.attack).name);
                progressText.SetText("");
                progressBar.SetActive(true);
                bar.fillAmount = (float)t.progress / (t.duration + 1);
                stopButton.SetActive(false);
                break;
            case TaskType.PREVENT:
                typeText.SetText("PREVENZIONE");
                targetText.SetText(gameManager.GetCategory(t.category).name);
                progressText.SetText("-");
                progressBar.SetActive(false);
                if (assigned) stopButton.SetActive(true);
                else stopButton.SetActive(false);
                break;
            default:
                Debug.Log("Error: unexpected TaskType");
                break;
        }
        if (assigned) {
            button.interactable = false;
            statusText.SetText("ASSEGNATO A\n" + gameManager.GetEmployee(t.executor).name.ToUpper());
        } else {
            statusText.SetText("NON ASSEGNATO");
        }
    }

    public void OpenEmployeeChoice() {
        employeeView.Load(task);
    }

    public void EndTask() {
        gameManager.EndTask(task);
        Load(gameManager, employeeView, task, false);
    }
}
