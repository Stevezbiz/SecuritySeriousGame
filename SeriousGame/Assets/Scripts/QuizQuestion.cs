using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class QuizQuestion : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;
    [SerializeField] RectTransform answers;
    [SerializeField] GameObject quizAlternative;
    [SerializeField] GameObject quizQuestion;
    [SerializeField] GameObject quizEffects;
    [SerializeField] TextMeshProUGUI effectsDescriptionText;
    [SerializeField] TextMeshProUGUI effectsValuesText;

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
        effectsDescriptionText.SetText(q.answers[id].description);
        string effects = "Conseguenze:\n";
        // show the effects of the answer
        foreach (AnswerEffect effect in q.answers[id].effects) {
            switch (effect.target) {
                case Element.REPUTATION:
                    effects += string.Format("    {0,-12} {1,4}%\n", "Reputazione", (100 * effect.modifier).ToString("+0.;-#."));
                    break;
                case Element.MONEY:
                    effects += string.Format("    {0,-12} {1,4}\n", "Fondi", effect.modifier.ToString("+0.;-#."));
                    break;
                case Element.USERS:
                    effects += string.Format("    {0,-12} {1,4}%\n", "Utenti", (100 * effect.modifier).ToString("+0.;-#."));
                    break;
                default:
                    Debug.Log("Error: unexpected Element");
                    return;
            }
        }
        if (q.answers[id].triggeredAttack != AttackCode.NONE && gameManager.AttackIsScheduled(q.answers[id].triggeredAttack)) {
            effects += "    Vulnerabilità x2 all'attacco " + gameManager.GetAttack(q.answers[id].triggeredAttack).name;
        }
        effectsValuesText.SetText(effects);
        quizQuestion.SetActive(false);
        quizEffects.SetActive(true);
    }

    public void Close() {
        TimeManager.Resume();
        gameManager.CloseMessage();
        foreach (GameObject g in toDestroy) Destroy(g);
        toDestroy.Clear();
        quizQuestion.SetActive(true);
        quizEffects.SetActive(false);
        gameObject.SetActive(false);
    }
}
