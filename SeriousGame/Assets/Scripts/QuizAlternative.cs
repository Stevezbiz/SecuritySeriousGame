using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizAlternative : MonoBehaviour {
    [SerializeField] TextMeshProUGUI answerText;

    QuizQuestion quizQuestion;
    int id;

    public void Load(QuizQuestion quizQuestion, int id, string text) {
        this.quizQuestion = quizQuestion;
        this.id = id;
        answerText.SetText(text);
    }

    public void OnClick() {
        quizQuestion.EvaluateAnswer(id);
    }
}
