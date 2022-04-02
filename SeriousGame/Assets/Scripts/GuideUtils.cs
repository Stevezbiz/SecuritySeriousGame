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

public enum GuideEntryCode {
    NONE,
    DOS,
    MITM,
    BRUTE_FORCE,
    DICTIONARY,
    RAINBOW_TABLE,
    API_VULNERABILITY,
    SOCIAL_ENGINEERING,
    PHISHING,
    WORM,
    VIRUS,
    SPYWARE,
    RANSOMWARE,
    FIREWALL,
    DNSSEC,
    TWO_F_A,
    HASH,
    CAPTCHA,
    IDS,
    ANTIVIRUS,
    BACK_UP
}

[System.Serializable]
public class GuideEntryData {
    public GuideEntryCode id;
    public string entryName;
    public string entryText;
    public string link;
}

[System.Serializable]
public class GuideJSON {
    public GuideEntryData[] entries;
}

public static class GuideUtils {
    /**
     * <summary></summary>
     */
    public static Dictionary<GuideEntryCode, GuideEntryData> LoadFromFile(TextAsset file) {
        Dictionary<GuideEntryCode, GuideEntryData> entries = new Dictionary<GuideEntryCode, GuideEntryData>();

        GuideJSON guideJSON = JsonUtility.FromJson<GuideJSON>(file.text);
        foreach (GuideEntryData data in guideJSON.entries) {
            entries.Add(data.id, data);
        }

        return entries;
    }
}
