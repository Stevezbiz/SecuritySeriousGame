using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;

public class QuizQuestion : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;
    [SerializeField] RectTransform answers;
    [SerializeField] GameObject quizAlternative;

    Quiz q;

    public void Load(Quiz q, Person p) {
        this.q = q;
        nameText.SetText(p.name.ToLower());
        image.sprite = p.sprite;
        questionText.SetText(q.question);
        for (int i = 0; i < q.answers.Length; i++) {
            Instantiate(quizAlternative, answers, false).GetComponent<QuizAlternative>().Load(this, i, q.answers[i].text);
        }
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void EvaluateAnswer(int id) {
        gameManager.EvaluateQuiz(q.id, id);
        Close();
    }

    public void Close() {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
