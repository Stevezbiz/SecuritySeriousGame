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
    public string description;
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
