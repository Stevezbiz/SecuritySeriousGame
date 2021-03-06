/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Outline = UnityEngine.UI.Outline;

public class SecurityView : MonoBehaviour {
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
    [SerializeField] Image durationBar;
    [SerializeField] Image missBar;
    [SerializeField] Image enduranceBar;
    [SerializeField] Outline durationOutline;
    [SerializeField] Outline missOutline;
    [SerializeField] Outline enduranceOutline;
    [SerializeField] RectTransform durationMarker;
    [SerializeField] RectTransform missMarker;
    [SerializeField] RectTransform enduranceMarker;
    [SerializeField] GameObject bars;

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

    /**
     * <summary></summary>
     */
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
            bars.SetActive(false);
        } else {
            Resistance res = gameManager.GetResistance(attacks[attackDropdown.value].id);
            durationText.SetText("-" + (res.duration * 100).ToString("0.") + " %");
            missText.SetText("+" + (res.miss * 100).ToString("0.") + " %");
            enduranceText.SetText("+" + (res.endurance * 100).ToString("0.") + " %");
            durationBar.fillAmount = res.duration;
            missBar.fillAmount = res.miss;
            enduranceBar.fillAmount = res.endurance;
            SetColor(res);
            durationMarker.anchoredPosition = new Vector2(-840f * (1f - BKTModel.GetDurationL(res.id)), durationMarker.anchoredPosition.y);
            missMarker.anchoredPosition = new Vector2(-840f * (1f - BKTModel.GetMissL(res.id)), missMarker.anchoredPosition.y);
            enduranceMarker.anchoredPosition = new Vector2(-840f * (1f - BKTModel.GetEnduranceL(res.id)), enduranceMarker.anchoredPosition.y);
            bars.SetActive(true);
        }
    }

    /**
     * <summary></summary>
     */
    public void Repair(EmployeeCode id) {
        gameManager.AssignEmployee(id, selected);
        LoadCurrentAttacks();
    }

    /**
     * <summary></summary>
     */
    public void OpenEmployeeChoice(Task t) {
        selected = t.id;
        employeeChoice.Load(t);
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        TimeManager.Pause();
        Load();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        TimeManager.Resume();
        gameObject.SetActive(false);
        bottomPanel.SetActive(true);
    }

    /**
     * <summary></summary>
     */
    public void ResistancesButton() {
        gameManager.DisplayMessage("Le difese attive conferiscono resistenza ad attacchi specifici. Sono indicati i livelli di resistenza minimi consigliati per ottenere una protezione adeguata.", ActionCode.CONTINUE, Role.SECURITY);
    }

    /**
     * <summary></summary>
     */
    public void EnduranceButton() {
        gameManager.DisplayMessage("Complessit?: influisce sulla frequenza degli attacchi, aumentando il tempo medio tra 2 attacchi consecutivi.", ActionCode.CONTINUE, Role.SECURITY);
    }

    /**
     * <summary></summary>
     */
    public void MissButton() {
        gameManager.DisplayMessage("Difesa: aumenta la probabilit? di evitare un attacco.", ActionCode.CONTINUE, Role.SECURITY);
    }

    /**
     * <summary></summary>
     */
    public void DurationButton() {
        gameManager.DisplayMessage("Durata: riduce il tempo necessario a riparare i danni causati da un attacco.", ActionCode.CONTINUE, Role.SECURITY);
    }

    /**
     * <summary></summary>
     */
    void SetColor(Resistance res) {
        Color c1;
        Color c2;
        Color c3;
        if (res.duration >= BKTModel.GetDurationL(res.id)) c1 = COLOR.BLUE;
        else c1 = COLOR.YELLOW;
        if (res.miss >= BKTModel.GetMissL(res.id)) c2 = COLOR.BLUE;
        else c2 = COLOR.YELLOW;
        if (res.endurance >= BKTModel.GetEnduranceL(res.id)) c3 = COLOR.BLUE;
        else c3 = COLOR.YELLOW;
        durationBar.color = c1;
        durationOutline.effectColor = c1;
        foreach (Image i in durationMarker.GetComponentsInChildren<Image>()) i.color = c1;
        missBar.color = c2;
        missOutline.effectColor = c2;
        foreach (Image i in missMarker.GetComponentsInChildren<Image>()) i.color = c2;
        enduranceBar.color = c3;
        enduranceOutline.effectColor = c3;
        foreach (Image i in enduranceMarker.GetComponentsInChildren<Image>()) i.color = c3;
    }
}
