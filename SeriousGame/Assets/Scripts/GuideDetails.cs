using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuideDetails : MonoBehaviour {
    [SerializeField] Guide guide;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] RectTransform content;

    public void Load(GuideEntryCode id) {
        GuideEntryData entry = guide.GetEntry(id);
        titleText.SetText(entry.entryName.ToLower());
        descriptionText.SetText(entry.entryText);
        content.SetPositionAndRotation(new Vector3(content.position.x, 0f, content.position.z), Quaternion.identity);
    }

    public void Back() {
        gameObject.SetActive(false);
    }
}
