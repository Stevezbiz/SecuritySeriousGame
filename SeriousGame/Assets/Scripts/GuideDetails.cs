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

    public void Load(GuideEntryCode id) {
        GuideEntryData entry = guide.GetEntry(id);
        titleText.SetText(entry.entryName.ToLower());
        descriptionText.SetText(entry.entryText);
        this.link = entry.link;
        content.SetPositionAndRotation(new Vector3(content.position.x, 0f, content.position.z), Quaternion.identity);
    }

    public void Back() {
        gameObject.SetActive(false);
    }

    public void OpenLink() {
        Application.OpenURL(link);
    }
}
