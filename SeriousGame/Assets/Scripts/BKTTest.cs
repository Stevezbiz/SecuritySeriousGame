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

public class BKTTest : MonoBehaviour {
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI answers;
    [SerializeField] TextMeshProUGUI learned;

    KnowledgeComponent kc;

    // Start is called before the first frame update
    void Start() {
        // Instantiate and initialize the data structures of BKT model
        ResetButton();
    }

    /**
     * <summary></summary>
     */
    public void CorrectAnswer() {
        // Apply the observation to the model
        kc.AddTestResult(true);
        Refresh();
    }

    /**
     * <summary></summary>
     */
    public void WrongAnswer() {
        // Apply the observation to the model
        kc.AddTestResult(false);
        Refresh();
    }

    /**
     * <summary></summary>
     */
    public void ResetButton() {
        kc = new KnowledgeComponent(SkillCode.NONE, "None");
        answers.SetText(kc.GetTestsVector());
        learned.SetText(kc.GetLearnedVector());
        for (int i = 0; i < content.childCount; i++) {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    /**
     * <summary></summary>
     */
    void Refresh() {
        // Print the parameters of the model
        GameObject newRecord = Instantiate(new GameObject());
        newRecord.transform.SetParent(content, false);
        newRecord.AddComponent<CanvasRenderer>();
        TextMeshProUGUI text = newRecord.AddComponent<TextMeshProUGUI>();
        newRecord.GetComponent<RectTransform>().sizeDelta = new Vector2(1800, 50);
        text.color = Color.black;
        text.fontSize = 30;
        text.SetText("test" + kc.GetTestN() + "\tp(L) = " + kc.GetLearned() + "\tp(T) = " + kc.GetTransit() + "\tp(G) = " + kc.GetGuess() +
            "\tp(S) = " + kc.GetSlip() + "\ttransition = " + kc.GetTransitionPos() + "\tacc = " + (int)(kc.GetAccuracy() * 100) + "%");
        answers.SetText(kc.GetTestsVector());
        learned.SetText(kc.GetLearnedVector());
    }
}
