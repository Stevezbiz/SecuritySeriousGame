using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningReport : MonoBehaviour {
    [SerializeField] GameObject learningRecord;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform footer;
    [SerializeField] GameObject debugView;
    [SerializeField] GameObject debugLearningRecord;
    [SerializeField] RectTransform debugContent;

    float oldTomeScale = 1f;
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

    public void Load() {
        oldTomeScale = Time.timeScale;
        Time.timeScale = 0f;
        foreach (KnowledgeComponent kc in kcs.Values) {
            records[kc.id].Load(kc);
        }
        gameObject.SetActive(true);
    }

    public void Close() {
        Time.timeScale = oldTomeScale;
        gameObject.SetActive(false);
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
}
