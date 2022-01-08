using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideEntry : MonoBehaviour {
    [SerializeField] TextMeshProUGUI entryText;

    GuideEntryCode id;
    GuideDetails guideDetails;

    public void Load(GuideEntryCode id, GuideDetails guideDetails, string name) {
        this.id = id;
        this.guideDetails = guideDetails;
        entryText.SetText(name);
    }

    public void ShowEntry() {
        guideDetails.Load();
        guideDetails.gameObject.SetActive(true);
    }
}
