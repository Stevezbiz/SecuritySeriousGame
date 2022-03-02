using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLearningRecord : MonoBehaviour {
    [SerializeField] TextMeshProUGUI skillText;
    [SerializeField] TextMeshProUGUI learnedText;
    [SerializeField] TextMeshProUGUI posText;
    [SerializeField] TextMeshProUGUI testText;

    public void Init(KnowledgeComponent kc) {
        skillText.SetText(kc.name);
    }

    public void Load(KnowledgeComponent kc) {
        learnedText.SetText("p(L) = " + kc.GetLearned().ToString("0.###"));
        posText.SetText("pos(T) = " + kc.GetTransitionPos());
        testText.SetText("tests:\t" +
            kc.GetTestsVector() + "\n" +
            "learned:\t" +
            kc.GetLearnedVector());
    }
}
