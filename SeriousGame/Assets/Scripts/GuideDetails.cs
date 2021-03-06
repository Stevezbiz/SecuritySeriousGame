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

public class GuideDetails : MonoBehaviour {
    [SerializeField] Guide guide;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] RectTransform content;

    string link;

    /**
     * <summary></summary>
     */
    public void Load(GuideEntryCode id) {
        GuideEntryData entry = guide.GetEntry(id);
        titleText.SetText(entry.entryName.ToLower());
        descriptionText.SetText(entry.entryText);
        this.link = entry.link;
        content.localPosition = new Vector3(content.localPosition.x, 0f, content.localPosition.z);
    }

    /**
     * <summary></summary>
     */
    public void Back() {
        gameObject.SetActive(false);
    }

    /**
     * <summary></summary>
     */
    public void OpenLink() {
        Application.OpenURL(link);
    }
}
