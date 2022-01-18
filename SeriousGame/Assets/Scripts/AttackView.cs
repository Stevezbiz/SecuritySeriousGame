using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Dropdown attackDropdown;
    [SerializeField] TextMeshProUGUI hitValue;
    [SerializeField] TextMeshProUGUI hitPercent;
    [SerializeField] TextMeshProUGUI missValue;
    [SerializeField] TextMeshProUGUI missPercent;
    [SerializeField] TextMeshProUGUI durationText;
    [SerializeField] TextMeshProUGUI missText;
    [SerializeField] TextMeshProUGUI enduranceText;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] EmployeeChoice employeeChoice;
    [SerializeField] GameObject noAttackText;
    [SerializeField] GameObject attackList;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject attackItem;

    float oldTimeScale = 1f;
    List<AttackInfo> attacks;
    List<Task> tasks;
    List<GameObject> toDestroy = new List<GameObject>();
    int selected;

    /**
     * <summary>Initialize the data structures</summary>
     */
    public void Init() {
        attacks = gameManager.GetAttacks();
        // fill the options of the attack dropdown
        List<string> options = new List<string>();
        foreach (AttackInfo a in attacks) {
            options.Add(a.name);
        }
        options.Add("tutti");
        attackDropdown.AddOptions(options);
    }

    /**
     * <summary>Populate the view with the values to show</summary>
     */
    public void Load() {
        attackDropdown.value = attackDropdown.options.Count - 1;
        DisplayStats(0);
        LoadCurrentAttacks();
    }

    void LoadCurrentAttacks() {
        foreach (GameObject obj in toDestroy) {
            Destroy(obj);
        }
        toDestroy.Clear();
        tasks = gameManager.GetAvailableTasksByType(TaskType.REPAIR);
        if (tasks.Count == 0) {
            noAttackText.SetActive(true);
            attackList.SetActive(false);
        } else {
            noAttackText.SetActive(false);
            attackList.SetActive(true);
            foreach (Task t in tasks) {
                GameObject newItem = Instantiate(attackItem, content, false);
                newItem.GetComponent<AttackItem>().Load(t, gameManager.GetAttack(t.attack).name, this);
                toDestroy.Add(newItem);
            }
        }
    }

    /**
     * <summary>Populate the view with the values to show</summary>
     */
    void DisplayStats(int err) {
        // retrieve the stats
        AttackStats stats;
        if (attackDropdown.value == attackDropdown.options.Count - 1) {
            // the "all" selector is active
            stats = gameManager.GetAttackStatsTotal();
        } else {
            stats = gameManager.GetAttackStats(attacks[attackDropdown.value].id);
        }
        // display the retrieved stats
        hitValue.SetText(stats.hit + "/" + stats.n);
        if (stats.n != 0) hitPercent.SetText("(" + (100 * (float)stats.hit / stats.n).ToString("0.") + " %)");
        else hitPercent.SetText("(- %)");
        missValue.SetText(stats.miss + "/" + stats.n);
        if (stats.n != 0) missPercent.SetText("(" + (100 * (float)stats.miss / stats.n).ToString("0.") + " %)");
        else missPercent.SetText("(- %)");
        // retrieve and display the resistances
        if (attackDropdown.value == attackDropdown.options.Count - 1) {
            // the "all" selector is active
            durationText.SetText("- %");
            missText.SetText("- %");
            enduranceText.SetText("- %");
        } else {
            Resistance res = gameManager.GetResistance(attacks[attackDropdown.value].id);
            durationText.SetText("-" + (res.duration * 100).ToString("0.") + " %");
            missText.SetText("+" + (res.miss * 100).ToString("0.") + " %");
            enduranceText.SetText("+" + (res.endurance * 100).ToString("0.") + " %");
        }
    }

    public void Repair(EmployeeCode id) {
        gameManager.AssignEmployee(id, selected);
        LoadCurrentAttacks();
    }

    public void OpenEmployeeChoice(Task t) {
        selected = t.id;
        employeeChoice.Load(t.attack, this);
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Load();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        bottomPanel.SetActive(true);
    }
}
