using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Outline = UnityEngine.UI.Outline;

public class LearningRecord : MonoBehaviour {
    [SerializeField] TextMeshProUGUI skillText;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Outline barOutline;
    [SerializeField] Image barImage;
    [SerializeField] Image arrow1Image;
    [SerializeField] Image arrow2Image;
    [SerializeField] RectTransform marker;
    [SerializeField] RectTransform bar;

    public void Init(KnowledgeComponent kc) {
        skillText.SetText("Competenza: " + kc.name);
        marker.localPosition = new Vector3((float)(-bar.sizeDelta.x * (1 - BKTModel.COGNITIVE_MASTERY)), 0, 0);
    }

    public void Load(KnowledgeComponent kc) {
        valueText.SetText("Livello di abilità: " + (100 * kc.GetLearned()).ToString(".##") + "%");
        if (kc.IsMastered()) SetColor(COLOR.BLUE);
        else SetColor(COLOR.YELLOW);
        barImage.fillAmount = (float)kc.GetLearned();
    }

    void SetColor(Color color) {
        skillText.color = color;
        valueText.color = color;
        barOutline.effectColor = color;
        barImage.color = color;
        arrow1Image.color = color;
        arrow2Image.color = color;
    }
}
