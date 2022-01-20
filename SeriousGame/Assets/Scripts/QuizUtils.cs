using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element {
    NONE,
    REPUTATION,
    MONEY,
    USERS
}

[System.Serializable]
public class AnswerEffect {
    public Element target;
    public float modifier;
}

[System.Serializable]
public class QuizAnswer {
    public string text;
    public bool correct;
    public AnswerEffect[] effects;
    public AttackCode triggeredAttack;
}

[System.Serializable]
public class Quiz {
    public int id;
    public SkillCode skill;
    public Role person;
    public string question;
    public QuizAnswer[] answers;
}

[System.Serializable]
public class QuizQuestionsJSON {
    public Quiz[] quizzes;
}

public static class QuizUtils {
    public static Dictionary<int, Quiz> LoadFromFile(TextAsset file) {
        Dictionary<int, Quiz> entries = new Dictionary<int, Quiz>();

        QuizQuestionsJSON quizQuestionsJSON = JsonUtility.FromJson<QuizQuestionsJSON>(file.text);
        foreach (Quiz q in quizQuestionsJSON.quizzes) {
            entries.Add(q.id, q);
        }

        return entries;
    }
}
