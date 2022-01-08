using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuideEntryCode {
    FIREWALL
}

[System.Serializable]
public class GuideEntryData {
    public GuideEntryCode id;
    public string entryName;
    public string entryText;
}

[System.Serializable]
public class GuideJSON {
    public GuideEntryData[] entries;
}

public static class GuideUtils {
    public static Dictionary<GuideEntryCode, GuideEntryData> LoadFromFile(TextAsset file) {
        Dictionary<GuideEntryCode, GuideEntryData> entries = new Dictionary<GuideEntryCode, GuideEntryData>();

        GuideJSON guideJSON = JsonUtility.FromJson<GuideJSON>(file.text);
        foreach(GuideEntryData data in guideJSON.entries) {
            entries.Add(data.id, data);
        }

        return entries;
    }
}
