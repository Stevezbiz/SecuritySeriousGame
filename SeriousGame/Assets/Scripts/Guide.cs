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
        foreach(GuideEntryData entry in entries.Values) {
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
