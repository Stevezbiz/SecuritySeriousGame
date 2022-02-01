using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PreventView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] TextMeshProUGUI protectionText;
    [SerializeField] EmployeeView employeeView;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] GameObject networkOutline;
    [SerializeField] GameObject accessOutline;
    [SerializeField] GameObject softwareOutline;
    [SerializeField] GameObject assetOutline;
    [SerializeField] GameObject servicesOutline;

    EmployeeCode employee;
    List<Task> tasks = new List<Task>();

    public void Load(EmployeeCode id) {
        EmployeeInfo e = gameManager.GetEmployee(id);
        nameText.SetText(e.name.ToLower());
        descriptionText.SetText(e.description);
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
        Dictionary<Category, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[Category.NETWORK] / 10;
        accessBar.fillAmount = abilities[Category.ACCESS] / 10;
        softwareBar.fillAmount = abilities[Category.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[Category.ASSET] / 10;
        servicesBar.fillAmount = abilities[Category.SERVICES] / 10;
        tasks.Clear();
        tasks = gameManager.GetAvailableTasksByType(TaskType.PREVENT);
        if (tasks.Count == 0) {
            gameManager.DisplayMessage("Tutti i settori sono occupati", ActionCode.CONTINUE, Role.SECURITY);
        } else {
            employee = id;
            List<string> options = new List<string>();
            foreach (Task t in tasks) {
                switch (t.category) {
                    case Category.NETWORK:
                        options.Add("Rete");
                        break;
                    case Category.ACCESS:
                        options.Add("Accesso");
                        break;
                    case Category.SOFTWARE:
                        options.Add("Software");
                        break;
                    case Category.ASSET:
                        options.Add("Risorse");
                        break;
                    case Category.SERVICES:
                        options.Add("Servizi");
                        break;
                    default:
                        Debug.Log("Error: undefined Category");
                        break;
                }
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.value = 0;
            Display(0);
            gameObject.SetActive(true);
        }
    }

    public void Display(int err) {
        Task t = tasks[dropdown.value];
        float protection = gameManager.GetPreventProtection(employee, t.category);
        protectionText.SetText("Protezione: +" + (100 * protection).ToString(".##") + "%");
        networkOutline.SetActive(false);
        accessOutline.SetActive(false);
        softwareOutline.SetActive(false);
        assetOutline.SetActive(false);
        servicesOutline.SetActive(false);
        switch (t.category) {
            case Category.NETWORK:
                networkOutline.SetActive(true);
                break;
            case Category.ACCESS:
                accessOutline.SetActive(true);
                break;
            case Category.SOFTWARE:
                softwareOutline.SetActive(true);
                break;
            case Category.ASSET:
                assetOutline.SetActive(true);
                break;
            case Category.SERVICES:
                servicesOutline.SetActive(true);
                break;
            default:
                Debug.Log("Error: undefined Category");
                break;
        }
    }

    public void PreventAttacks() {
        gameManager.AssignEmployee(employee, tasks[dropdown.value].id);
        Close();
    }

    public void Close() {
        employeeView.Load();
        gameObject.SetActive(false);
    }
}
