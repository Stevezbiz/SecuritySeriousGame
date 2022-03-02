using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LearningReport : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject learningRecord;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform footer;
    [SerializeField] GameObject debugView;
    [SerializeField] GameObject debugLearningRecord;
    [SerializeField] RectTransform debugContent;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] TMP_InputField inputField;

    ActionCode action;
    Dictionary<SkillCode, KnowledgeComponent> kcs;
    Dictionary<SkillCode, LearningRecord> records = new Dictionary<SkillCode, LearningRecord>();
    Dictionary<SkillCode, DebugLearningRecord> debugRecords = new Dictionary<SkillCode, DebugLearningRecord>();

    public void Init(Dictionary<SkillCode, KnowledgeComponent> kcs) {
        this.kcs = kcs;
        Transform lastRecord = null;
        foreach (KnowledgeComponent kc in kcs.Values) {
            lastRecord = Instantiate(learningRecord, content, false).transform;
            records.Add(kc.id, lastRecord.gameObject.GetComponent<LearningRecord>());
            records[kc.id].Init(kc);
        }
        if (lastRecord != null) {
            footer.SetParent(lastRecord, false);
            footer.localPosition = new Vector3(0, -50, 0);
        }
        InitDebugView();
    }

    public void Load(ActionCode action) {
        TimeManager.Pause();
        this.action = action;
        foreach (KnowledgeComponent kc in kcs.Values) {
            records[kc.id].Load(kc);
        }
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Close() {
        TimeManager.Resume();
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
        if (action == ActionCode.GAME_OVER) SceneLoader.LoadScene("MainMenu");
    }

    public void OpenDebugView() {
        foreach (KnowledgeComponent kc in kcs.Values) {
            debugRecords[kc.id].Load(kc);
        }
        debugView.SetActive(true);
    }

    public void CloseDebugView() {
        debugView.SetActive(false);
    }

    void InitDebugView() {
        foreach (KnowledgeComponent kc in kcs.Values) {
            debugRecords.Add(kc.id, Instantiate(debugLearningRecord, debugContent, false).GetComponent<DebugLearningRecord>());
            debugRecords[kc.id].Init(kc);
        }
    }

    public void DebugPrint() {
        switch (inputField.text[0]) {
            case 'A':
                gameManager.DebugPrintAttack((AttackCode)int.Parse(inputField.text.Replace("A", "")));
                break;
            case 'S':
                gameManager.DebugPrintShopItem((ShopItemCode)int.Parse(inputField.text.Replace("S", "")));
                break;
            default:
                Debug.Log("Unknown code");
                break;
        }
    }
}
