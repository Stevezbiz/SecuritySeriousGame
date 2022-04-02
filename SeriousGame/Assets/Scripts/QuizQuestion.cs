/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class QuizQuestion : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioSource effectsSource;
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;
    [SerializeField] RectTransform answers;
    [SerializeField] GameObject quizAlternative;
    [SerializeField] GameObject quizQuestion;
    [SerializeField] GameObject quizEffects;
    [SerializeField] TextMeshProUGUI effectsDescriptionText;
    [SerializeField] TextMeshProUGUI effectsValuesText;
    [SerializeField] AudioClip positiveTone;
    [SerializeField] AudioClip negativeTone;

    Quiz q;
    List<GameObject> toDestroy = new List<GameObject>();

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    public void EvaluateAnswer(int id) {
        if (q.answers[id].correct) effectsSource.clip = positiveTone;
        else effectsSource.clip = negativeTone;
        effectsSource.Play();
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

    /**
     * <summary></summary>
     */
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
