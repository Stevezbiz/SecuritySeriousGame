using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuideEntry : MonoBehaviour {
    [SerializeField] TextMeshProUGUI entryText;

    GuideEntryCode id;
    GuideDetails guideDetails;

    public void Load(GuideEntryCode id, GuideDetails guideDetails, string name) {
        this.id = id;
        this.guideDetails = guideDetails;
        entryText.SetText(name.ToLower());
    }

    public void ShowEntry() {
        guideDetails.Load(id);
        guideDetails.gameObject.SetActive(true);
    }
}
