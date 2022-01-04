using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillCode {
    NONE,
    NETWORK,
    ACCESS,
    SOFTWARE,
    ASSET,
    SERVICES
}

public static class BKTModel {
    public static double COGNITIVE_MASTERY = 0.95;     // threshold to reach to achieve mastery
    public static int N_FIRST_EMPIRICAL_TEST = 3;      // N number to verify first empirical test
    public static int M_SECOND_EMPIRICAL_TEST = 10;    // M number to verify second empirical test
    public static double baseTransit = 0.1;
    public static double baseSlip = 0.1;
    public static double baseGuess = 0.3;
    public static double baseLearned = 0.01;

    public static Dictionary<SkillCode, KnowledgeComponent> Init() {
        Dictionary<SkillCode, KnowledgeComponent> kcs = new Dictionary<SkillCode, KnowledgeComponent>();
        kcs.Add(SkillCode.NETWORK, new KnowledgeComponent(SkillCode.NETWORK));
        kcs.Add(SkillCode.ACCESS, new KnowledgeComponent(SkillCode.ACCESS));
        kcs.Add(SkillCode.SOFTWARE, new KnowledgeComponent(SkillCode.SOFTWARE));
        kcs.Add(SkillCode.ASSET, new KnowledgeComponent(SkillCode.ASSET));
        kcs.Add(SkillCode.SERVICES, new KnowledgeComponent(SkillCode.SERVICES));
        return kcs;
    }
}

[System.Serializable]
public class KCRecord {
    public SkillCode id;
    public int transitionPos;
    public bool[] tests;

    public KCRecord(SkillCode id, int transitionPos, bool[] tests) {
        this.id = id;
        this.transitionPos = transitionPos;
        this.tests = tests;
    }
}

public class KnowledgeComponent {
    SkillCode id;                               // the id of the skill
    double transit;                             // p(T)
    double slip;                                // p(S)
    double guess;                               // p(G)
    double learned;                             // p(L)
    int transitionPos;                          // test number corresponding to the estimated transition
    List<bool> tests;                           // test results

    public KnowledgeComponent(SkillCode id) {
        this.id = id;
        transit = BKTModel.baseTransit;
        slip = BKTModel.baseSlip;
        guess = BKTModel.baseGuess;
        learned = BKTModel.baseLearned;
        transitionPos = 0;
        tests = new List<bool>();
    }

    public KnowledgeComponent(SkillCode id, int transitionPos, List<bool> tests) {
        this.id = id;
        transit = BKTModel.baseTransit;
        slip = BKTModel.baseSlip;
        guess = BKTModel.baseGuess;
        learned = BKTModel.baseLearned;
        this.transitionPos = transitionPos;
        this.tests = tests;
    }

    public void AddTestResult(bool result) {
        tests.Add(result);
        FitThisParametersUpdate(result);
    }

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

    bool FirstEmpiricalDegenerationTest() {
        // If a student takes three correct actions in a row but the model’s estimated probability
        // that the student knows the skill is lower than before these three actions, the model failed

        bool degenerated = false;
        double oldLearned = learned;

        for (int i = 0; i < BKTModel.N_FIRST_EMPIRICAL_TEST; i++) UpdateKnownParameter(true);
        if (learned < oldLearned) degenerated = true;
        learned = oldLearned;

        return degenerated;
    }

    bool SecondEmpiricalDegenerationTest() {
        // If a student takes ten correct actions in a row without reaching skill mastery, the model failed

        bool degenerated = false;
        double oldLearned = learned;

        for (int i = 0; i < BKTModel.M_SECOND_EMPIRICAL_TEST; i++) UpdateKnownParameter(true);
        if (learned < BKTModel.COGNITIVE_MASTERY) degenerated = true;
        learned = oldLearned;

        return degenerated;
    }

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

    public double GetTransit() {
        return transit;
    }

    public double GetGuess() {
        return guess;
    }

    public double GetSlip() {
        return slip;
    }

    public double GetLearned() {
        return learned;
    }

    public int GetTransitionPos() {
        return transitionPos;
    }

    public string GetLearnedVector() {
        string str = "";
        for(int i=0; i < tests.Count; i++) {
            if (i < transitionPos) str += "0";
            else str += "1";
        }
        return str;
    }

    public string GetTestsVector() {
        string str = "";
        for (int i = 0; i < tests.Count; i++) {
            if (tests[i]) str += "1";
            else str += "0";
        }
        return str;
    }

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

    public int GetTestN() {
        return tests.Count;
    }
}
