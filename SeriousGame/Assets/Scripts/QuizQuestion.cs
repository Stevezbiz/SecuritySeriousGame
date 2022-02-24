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
    List<GameObject> toDestroy = new List<GameObject>();

    public void Load(Quiz q, Person p) {
        this.q = q;
        nameText.SetText(p.name.ToLower());
        image.sprite = p.icon;
        questionText.SetText(q.question);
        for (int i = 0; i < q.answers.Length; i++) {
            toDestroy.Add(Instantiate(quizAlternative, answers, false));
            toDestroy[i].GetComponent<QuizAlternative>().Load(this, i, q.answers[i].text);
        }
        gameObject.SetActive(true);
        TimeManager.Pause();
    }

    public void EvaluateAnswer(int id) {
        gameManager.EvaluateQuiz(q.id, id);
        Close();
    }

    public void Close() {
        TimeManager.Resume();
        foreach (GameObject g in toDestroy) Destroy(g);
        toDestroy.Clear();
        gameObject.SetActive(false);
    }
}
