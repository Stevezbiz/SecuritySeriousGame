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

public class GuideEntry : MonoBehaviour {
    [SerializeField] TextMeshProUGUI entryText;

    GuideEntryCode id;
    GuideDetails guideDetails;

    /**
     * <summary></summary>
     */
    public void Load(GuideEntryCode id, GuideDetails guideDetails, string name) {
        this.id = id;
        this.guideDetails = guideDetails;
        entryText.SetText(name.ToLower());
    }

    /**
     * <summary></summary>
     */
    public void ShowEntry() {
        guideDetails.Load(id);
        guideDetails.gameObject.SetActive(true);
    }
}
