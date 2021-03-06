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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum SkillCode {
    NONE,
    MANAGEMENT,
    NETWORK,
    ACCESS,
    SOFTWARE,
    ASSET,
    SERVICES
}

[System.Serializable]
public class ResistanceRequirements {
    public AttackCode id;
    public float[] durationL;
    public float[] durationH;
    public float[] missL;
    public float[] missH;
    public float[] enduranceL;
    public float[] enduranceH;
}

[System.Serializable]
public class KCData {
    public SkillCode id;
    public string name;
}

[System.Serializable]
public class ModelConfig {
    public double COGNITIVE_MASTERY;
    public int N_FIRST_EMPIRICAL_TEST;
    public int M_SECOND_EMPIRICAL_TEST;
    public double baseTransit;
    public double baseSlip;
    public double baseGuess;
    public double baseLearned;
    public int actualTimeSlot;
    public int[] timeSlots;
    public KCData[] kcs;
    public ResistanceRequirements[] requirements;
}

[System.Serializable]
public class ModelJSON {
    public ModelConfig modelConfig;
}

[System.Serializable]
public class ModelSave {
    public KCRecord[] records;
    public int actualTimeSlot;

    public ModelSave(KCRecord[] records, int actualTimeSlot) {
        this.records = records;
        this.actualTimeSlot = actualTimeSlot;
    }
}

[System.Serializable]
public class KCRecord {
    public SkillCode id;
    public string name;
    public int transitionPos;
    public bool[] tests;

    public KCRecord(SkillCode id, string name, int transitionPos, bool[] tests) {
        this.id = id;
        this.name = name;
        this.transitionPos = transitionPos;
        this.tests = tests;
    }
}

public static class BKTModel {
    public static double COGNITIVE_MASTERY;         // threshold to reach to achieve mastery
    public static int N_FIRST_EMPIRICAL_TEST;       // N number to verify first empirical test
    public static int M_SECOND_EMPIRICAL_TEST;      // M number to verify second empirical test
    public static double baseTransit;
    public static double baseSlip;
    public static double baseGuess;
    public static double baseLearned;
    public static int actualTimeSlot;
    public static List<int> timeSlots = new List<int>();
    public static Dictionary<SkillCode, KnowledgeComponent> kcs = new Dictionary<SkillCode, KnowledgeComponent>();

    static Dictionary<AttackCode, ResistanceRequirements> resistanceRequirements = new Dictionary<AttackCode, ResistanceRequirements>();

    /**
     * <summary></summary>
     */
    public static void Init(TextAsset file) {
        ModelJSON modelJSON = JsonUtility.FromJson<ModelJSON>(file.text);
        COGNITIVE_MASTERY = modelJSON.modelConfig.COGNITIVE_MASTERY;
        N_FIRST_EMPIRICAL_TEST = modelJSON.modelConfig.N_FIRST_EMPIRICAL_TEST;
        M_SECOND_EMPIRICAL_TEST = modelJSON.modelConfig.M_SECOND_EMPIRICAL_TEST;
        baseTransit = modelJSON.modelConfig.baseTransit;
        baseSlip = modelJSON.modelConfig.baseSlip;
        baseGuess = modelJSON.modelConfig.baseGuess;
        baseLearned = modelJSON.modelConfig.baseLearned;
        actualTimeSlot = modelJSON.modelConfig.actualTimeSlot;
        foreach (KCData kc in modelJSON.modelConfig.kcs) {
            kcs[kc.id] = new KnowledgeComponent(kc.id, kc.name);
        }
        foreach (ResistanceRequirements r in modelJSON.modelConfig.requirements) {
            resistanceRequirements[r.id] = r;
        }
        timeSlots = new List<int>(modelJSON.modelConfig.timeSlots);
    }

    /**
     * <summary></summary>
     */
    public static void LoadModel(ModelSave modelSave) {
        actualTimeSlot = modelSave.actualTimeSlot;
        foreach (KCRecord r in modelSave.records) {
            kcs[r.id] = new KnowledgeComponent(r);
        }
        TimeManager.Resume();
    }

    /**
     * <summary></summary>
     */
    public static void UpdateModel(int time) {
        if (time == timeSlots[actualTimeSlot]) actualTimeSlot++;
    }

    /**
     * <summary></summary>
     */
    public static float GetDurationL(AttackCode id) {
        return resistanceRequirements[id].durationL[actualTimeSlot];
    }

    /**
     * <summary></summary>
     */
    public static float GetDurationH(AttackCode id) {
        return resistanceRequirements[id].durationH[actualTimeSlot];
    }

    /**
     * <summary></summary>
     */
    public static float GetMissL(AttackCode id) {
        return resistanceRequirements[id].missL[actualTimeSlot];
    }

    /**
     * <summary></summary>
     */
    public static float GetMissH(AttackCode id) {
        return resistanceRequirements[id].missH[actualTimeSlot];
    }

    /**
     * <summary></summary>
     */
    public static float GetEnduranceL(AttackCode id) {
        return resistanceRequirements[id].enduranceL[actualTimeSlot];
    }

    /**
     * <summary></summary>
     */
    public static float GetEnduranceH(AttackCode id) {
        return resistanceRequirements[id].enduranceH[actualTimeSlot];
    }
}

public class KnowledgeComponent {
    public SkillCode id;
    public string name;
    double transit;                             // p(T)
    double slip;                                // p(S)
    double guess;                               // p(G)
    double learned;                             // p(L)
    int transitionPos;                          // test number corresponding to the estimated transition
    List<bool> tests;                           // test results

    /**
     * <summary></summary>
     */
    public KnowledgeComponent(SkillCode id, string name) {
        this.id = id;
        this.name = name;
        transit = BKTModel.baseTransit;
        slip = BKTModel.baseSlip;
        guess = BKTModel.baseGuess;
        learned = BKTModel.baseLearned;
        transitionPos = 0;
        tests = new List<bool>();
    }

    /**
     * <summary></summary>
     */
    public KnowledgeComponent(KCRecord r) {
        id = r.id;
        name = r.name;
        transit = BKTModel.baseTransit;
        slip = BKTModel.baseSlip;
        guess = BKTModel.baseGuess;
        learned = BKTModel.baseLearned;
        transitionPos = r.transitionPos;
        tests = new List<bool>(r.tests);
    }

    /**
     * <summary></summary>
     */
    public void AddTestResult(bool result) {
        tests.Add(result);
        FitThisParametersUpdate(result);
    }

    /**
     * <summary></summary>
     */
    void EstimateEmpiricalProbabilityPosition() {
        double score;
        double max_score = -1;
        int bestTransitionPos = -1;

        // finding the best position for the transition
        for (int pos = 0; pos < tests.Count; pos++) {
            score = 0;
            for (int i = 0; i < tests.Count; i++) {
                if (i < pos) {
                    // not learned
                    if (!tests[i]) score++;
                } else {
                    // learned
                    if (tests[i]) score++;
                }
            }
            if (score > max_score) {
                max_score = score;
                bestTransitionPos = pos;
            }
        }

        // edge case: skill not learned yet
        score = 0;
        for (int i = 0; i < tests.Count; i++) {
            // not learned
            if (!tests[i]) score++;
        }
        if (score > max_score) {
            max_score = score;
            bestTransitionPos = tests.Count;
        }

        // If transitionPos = 0 we can consider it also as an edge case: skill learned before start
        transitionPos = bestTransitionPos;
    }

    /**
     * <summary></summary>
     */
    void UpdateTransitionParameter() {
        // p(T) = p(Ln+1=1|Ln=0) = p(Ln+1=1 && Ln=0)/p(Ln=0)
        double num = 0, den = 0;
        for (int i = 0; i < tests.Count - 1; i++) {
            if (i < transitionPos) {
                if (i + 1 >= transitionPos) {
                    num++;
                    den++;
                }
                den++;
            }
        }
        if (den != 0) transit = num / den;
    }

    /**
     * <summary></summary>
     */
    void UpdateGuessParameter() {
        // p(G) = p(correct|Ln=0) = p(correct && Ln=0)/p(Ln=0)
        double num = 0, den = 0;
        for (int i = 0; i < tests.Count; i++) {
            if (i < transitionPos) {
                if (tests[i]) num++;
                den++;
            }
        }
        if (den != 0) guess = num / den;
        //if (guess > 0.3) guess = 0.3;
    }

    /**
     * <summary></summary>
     */
    void UpdateSlipParameter() {
        // p(S) = p(wrong|Ln=1) = p(wrong && Ln=1)/p(Ln=1)
        double num = 0, den = 0;
        for (int i = 0; i < tests.Count; i++) {
            if (i >= transitionPos) {
                if (!tests[i]) num++;
                den++;
            }
        }
        if (den != 0) slip = num / den;
        //if (slip > 0.1) slip = 0.1;
    }

    /**
     * <summary></summary>
     */
    void UpdateKnownParameter(bool LastPerformance) {
        double pLearnedBefore;
        if (LastPerformance == true) {
            // probability the student knew it if his action is correct
            // p(Ln-1|correct) = p(Ln-1) * (1 - p(S)) / (p(Ln-1) * (1 - p(S)) + (1 - p(Ln-1)) * p(G))
            pLearnedBefore = learned * (1 - slip) / (learned * (1 - slip) + (1 - learned) * guess);
        } else {
            // probability the student knew it if his action is wrong
            // p(Ln-1|wrong) = p(Ln-1) * p(S) / (p(Ln-1) * p(S) + (1 - p(Ln-1)) * (1 - p(G)))
            pLearnedBefore = learned * slip / (learned * slip + (1 - learned) * (1 - guess));
        }
        // p(Ln) = p(Ln-1|result) + (1 - p(Ln-1|result)) * p(T)
        learned = pLearnedBefore + (1 - pLearnedBefore) * transit;
    }

    /**
     * <summary></summary>
     */
    bool FirstEmpiricalDegenerationTest() {
        // If a student takes three correct actions in a row but the model?s estimated probability
        // that the student knows the skill is lower than before these three actions, the model failed

        bool degenerated = false;
        double oldLearned = learned;

        for (int i = 0; i < BKTModel.N_FIRST_EMPIRICAL_TEST; i++) UpdateKnownParameter(true);
        if (learned < oldLearned) degenerated = true;
        learned = oldLearned;

        return degenerated;
    }

    /**
     * <summary></summary>
     */
    bool SecondEmpiricalDegenerationTest() {
        // If a student takes ten correct actions in a row without reaching skill mastery, the model failed

        bool degenerated = false;
        double oldLearned = learned;

        for (int i = 0; i < BKTModel.M_SECOND_EMPIRICAL_TEST; i++) UpdateKnownParameter(true);
        if (learned < BKTModel.COGNITIVE_MASTERY) degenerated = true;
        learned = oldLearned;

        return degenerated;
    }

    /**
     * <summary></summary>
     */
    void FitThisParametersUpdate(bool result) {
        double oldTransit = transit;
        double oldGuess = guess;
        double oldSlip = slip;
        double oldLearned = learned;
        int oldTransitionPos = transitionPos;

        EstimateEmpiricalProbabilityPosition();
        UpdateKnownParameter(result);
        if (FirstEmpiricalDegenerationTest() || SecondEmpiricalDegenerationTest()) {
            // model is degenerated
            transit = oldTransit;
            guess = oldGuess;
            slip = oldSlip;
            learned = oldLearned;
            transitionPos = oldTransitionPos;
            Debug.Log("Model degenerated!");
        }
    }

    /**
     * <summary></summary>
     */
    public double GetTransit() {
        return transit;
    }

    /**
     * <summary></summary>
     */
    public double GetGuess() {
        return guess;
    }

    /**
     * <summary></summary>
     */
    public double GetSlip() {
        return slip;
    }

    /**
     * <summary></summary>
     */
    public double GetLearned() {
        return learned;
    }

    /**
     * <summary></summary>
     */
    public int GetTransitionPos() {
        return transitionPos;
    }

    /**
     * <summary></summary>
     */
    public string GetLearnedVector() {
        string str = "";
        for (int i = 0; i < tests.Count; i++) {
            if (i < transitionPos) str += "0";
            else str += "1";
        }
        return str;
    }

    /**
     * <summary></summary>
     */
    public string GetTestsVector() {
        string str = "";
        for (int i = 0; i < tests.Count; i++) {
            if (tests[i]) str += "1";
            else str += "0";
        }
        return str;
    }

    /**
     * <summary></summary>
     */
    public double GetAccuracy() {
        double score = 0;
        for (int i = 0; i < tests.Count; i++) {
            if (i < transitionPos) {
                // not learned
                if (!tests[i]) score++;
            } else {
                // learned
                if (tests[i]) score++;
            }
        }
        return score / tests.Count;
    }

    /**
     * <summary></summary>
     */
    public int GetTestN() {
        return tests.Count;
    }

    /**
     * <summary></summary>
     */
    public bool[] GetTests() {
        return tests.ToArray();
    }

    /**
     * <summary></summary>
     */
    public bool IsMastered() {
        return learned >= 0.95;
    }
}
