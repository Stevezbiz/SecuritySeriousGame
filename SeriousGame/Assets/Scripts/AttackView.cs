using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    float oldTimeScale = 1f;

    /**
     * <summary>Initialize the data structures</summary>
     */
    public void Init(int n) {
        // fill the options of the attack dropdown
        List<string> options = new List<string>();
        for (int i = 0; i < n; i++) {
            options.Add(gameManager.GetAttack(i).name);
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
    }

    /**
     * <summary>Populate the view with the values to show</summary>
     */
    public void DisplayStats(int err) {
        // retrieve the stats
        AttackStats stats = gameManager.GetAttackStats(attackDropdown.value);
        if (stats == null) {
            // the "all" selector is active
            stats = gameManager.GetAttackStatsTotal();
        }
        // display the retrieved stats
        hitValue.SetText(stats.hit + "/" + stats.n);
        if (stats.n != 0) hitPercent.SetText("(" + (100 * (float)stats.hit / stats.n).ToString("0.") + " %)");
        else hitPercent.SetText("(- %)");
        missValue.SetText(stats.miss + "/" + stats.n);
        if (stats.n != 0) missPercent.SetText("(" + (100 * (float)stats.miss / stats.n).ToString("0.") + " %)");
        else missPercent.SetText("(- %)");
        // retrieve and display the resistances
        Resistance res = gameManager.GetResistance(attackDropdown.value);
        if (res == null) {
            // the "all" selector is active
            durationText.SetText("- %");
            missText.SetText("- %");
            enduranceText.SetText("- %");
        } else {
            if (res.duration == -1) durationText.SetText("- %");
            else durationText.SetText("-" + (res.duration * 100).ToString("0.") + " %");
            missText.SetText("+" + (res.miss * 100).ToString("0.") + " %");
            enduranceText.SetText("+" + (res.endurance * 100).ToString("0.") + " %");
        }
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Load();
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
    }
}
