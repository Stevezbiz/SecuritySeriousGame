using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningReport : MonoBehaviour {
    [SerializeField] GameObject learningRecord;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform footer;

    float oldTomeScale = 1f;
    Dictionary<SkillCode, KnowledgeComponent> kcs;
    Dictionary<SkillCode, LearningRecord> records = new Dictionary<SkillCode, LearningRecord>();

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
}
