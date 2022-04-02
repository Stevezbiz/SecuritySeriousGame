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

public class DebugLearningRecord : MonoBehaviour {
    [SerializeField] TextMeshProUGUI skillText;
    [SerializeField] TextMeshProUGUI learnedText;
    [SerializeField] TextMeshProUGUI posText;
    [SerializeField] TextMeshProUGUI testText;

    public void Init(KnowledgeComponent kc) {
        skillText.SetText(kc.name);
    }

    public void Load(KnowledgeComponent kc) {
        learnedText.SetText("p(L) = " + kc.GetLearned().ToString("0.###"));
        posText.SetText("pos(T) = " + kc.GetTransitionPos());
        testText.SetText("tests:\t" +
            kc.GetTestsVector() + "\n" +
            "learned:\t" +
            kc.GetLearnedVector());
    }
}
