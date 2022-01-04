using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BKTTest : MonoBehaviour {
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI answers;
    [SerializeField] TextMeshProUGUI learned;

    KnowledgeComponent kc;

    // Start is called before the first frame update
    void Start() {
        // Instantiate and initialize the data structures of BKT model
        ResetButton();
    }

    public void CorrectAnswer() {
        // Apply the observation to the model
        kc.AddTestResult(true);
        Refresh();
    }

    public void WrongAnswer() {
        // Apply the observation to the model
        kc.AddTestResult(false);
        Refresh();
    }

    public void ResetButton() {
        kc = new KnowledgeComponent(SkillCode.NONE);
        answers.SetText(kc.GetTestsVector());
        learned.SetText(kc.GetLearnedVector());
        for (int i = 0; i < content.childCount; i++) {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    void Refresh() {
        // Print the parameters of the model
        GameObject newRecord = Instantiate(new GameObject());
        newRecord.transform.SetParent(content, false);
        newRecord.AddComponent<CanvasRenderer>();
        TextMeshProUGUI text = newRecord.AddComponent<TextMeshProUGUI>();
        newRecord.GetComponent<RectTransform>().sizeDelta = new Vector2(1800, 50);
        text.color = Color.black;
        text.fontSize = 30;
        text.SetText("test" + kc.GetTestN() + "\tp(L) = " + kc.GetLearned() + "\tp(T) = " + kc.GetTransit() + "\tp(G) = " + kc.GetGuess() +
            "\tp(S) = " + kc.GetSlip() + "\ttransition = " + kc.GetTransitionPos() + "\tacc = " + (int)(kc.GetAccuracy() * 100) + "%");
        answers.SetText(kc.GetTestsVector());
        learned.SetText(kc.GetLearnedVector());
    }
}
