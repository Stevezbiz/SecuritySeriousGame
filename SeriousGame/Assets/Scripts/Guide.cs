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
using UnityEngine;

public class Guide : MonoBehaviour {
    [SerializeField] GameObject guideIndexColumn;
    [SerializeField] GameObject guideEntry;
    [SerializeField] RectTransform content;
    [SerializeField] GuideDetails guideDetails;
    [SerializeField] TextAsset guideFileJSON;
    [SerializeField] GameObject bottomPanel;

    const int COLUMN_CAPACITY = 7;
    Dictionary<GuideEntryCode, GuideEntryData> entries = new Dictionary<GuideEntryCode, GuideEntryData>();

    public void Init() {
        // load the guide from file
        entries = GuideUtils.LoadFromFile(guideFileJSON);
        // populate the guide index
        int i = 0;
        GameObject column = null;
        foreach (GuideEntryData entry in entries.Values) {
            if (i == 0) column = Instantiate(guideIndexColumn, content, false);
            Instantiate(guideEntry, column.transform, false).GetComponent<GuideEntry>().Load(entry.id, guideDetails, entry.entryName);
            i = (i + 1) % COLUMN_CAPACITY;
        }
    }

    /**
     * <summary>Open the guide</summary>
     */
    public void OpenGuide() {
        TimeManager.Pause();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the guide</summary>
     */
    public void CloseLGuide() {
        TimeManager.Resume();
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
        guideDetails.gameObject.SetActive(false);
    }

    public GuideEntryData GetEntry(GuideEntryCode id) {
        return entries[id];
    }
}
