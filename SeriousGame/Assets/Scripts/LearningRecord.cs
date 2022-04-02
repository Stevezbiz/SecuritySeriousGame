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

public class LearningRecord : MonoBehaviour {
    [SerializeField] TextMeshProUGUI skillText;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Outline barOutline;
    [SerializeField] Image barImage;
    [SerializeField] Image arrow1Image;
    [SerializeField] Image arrow2Image;
    [SerializeField] RectTransform marker;
    [SerializeField] RectTransform bar;

    public void Init(KnowledgeComponent kc) {
        skillText.SetText("Competenza: " + kc.name);
        marker.localPosition = new Vector3((float)(-bar.sizeDelta.x * (1 - BKTModel.COGNITIVE_MASTERY)), 0, 0);
    }

    public void Load(KnowledgeComponent kc) {
        valueText.SetText("Livello di abilità: " + (100 * kc.GetLearned()).ToString(".##") + "%");
        if (kc.IsMastered()) SetColor(COLOR.BLUE);
        else SetColor(COLOR.YELLOW);
        barImage.fillAmount = (float)kc.GetLearned();
    }

    void SetColor(Color color) {
        skillText.color = color;
        valueText.color = color;
        barOutline.effectColor = color;
        barImage.color = color;
        arrow1Image.color = color;
        arrow2Image.color = color;
    }
}
