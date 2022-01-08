using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour {
    [SerializeField] GameObject guideIndexColumn;
    [SerializeField] GameObject guideEntry;
    [SerializeField] RectTransform content;
    [SerializeField] GuideDetails guideDetails;
    [SerializeField] TextAsset guideFileJSON;

    float oldTimeScale = 1f;
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
            GameObject newEntry = Instantiate(guideEntry, column.transform, false);
            newEntry.GetComponent<GuideEntry>().Load(entry.id, guideDetails, entry.entryName);
            i = (i + 1) % COLUMN_CAPACITY;
        }
        guideDetails.Init();
    }

    /**
     * <summary>Open the guide</summary>
     */
    public void OpenGuide() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the guide</summary>
     */
    public void CloseLGuide() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        guideDetails.gameObject.SetActive(false);
    }
}
