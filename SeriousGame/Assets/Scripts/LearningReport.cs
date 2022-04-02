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
    bool init = true;

    public void Init(Dictionary<SkillCode, KnowledgeComponent> kcs) {
        this.kcs = kcs;
        Transform lastRecord = null;
        if (init) {
            init = false;
            foreach (KnowledgeComponent kc in kcs.Values) {
                lastRecord = Instantiate(learningRecord, content, false).transform;
                records[kc.id] = lastRecord.gameObject.GetComponent<LearningRecord>();
                records[kc.id].Init(kc);
            }
            if (lastRecord != null) {
                footer.SetParent(lastRecord, false);
                footer.localPosition = new Vector3(0, -50, 0);
            }
            InitDebugView();
        }
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
            debugRecords[kc.id] = Instantiate(debugLearningRecord, debugContent, false).GetComponent<DebugLearningRecord>();
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
