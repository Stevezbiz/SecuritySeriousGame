using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackCode {
    DOS,
    MITM,
    BRUTE_FORCE,
    DICTIONARY,
    RAINBOW_TABLE,
    API_VULNERABILITY,
    SOCIAL_ENGINEERING,
    PHISHING,
    WORM,
    VIRUS,
    SPYWARE,
    RANSOMWARE
}

public enum AttackStatus {
    INACTIVE,
    PLANNING,
    ACTIVE
}

[System.Serializable]
public class Resistance {
    public AttackCode id;
    public float duration;
    public float miss;
    public float endurance;

    public Resistance(AttackCode id, float duration, float miss, float endurance) {
        this.id = id;
        this.duration = duration;
        this.miss = miss;
        this.endurance = endurance;
    }
}

[System.Serializable]
public class AttackInfo {
    public AttackCode id;
    public Category category;
    public string name;
    public string description;
    public float moneyLoss;
    public float usersLoss;
    public float moneyMalus;
    public float usersMalus;
    public float reputationMalus;
    public float maxTime;
    public int duration;
}

[System.Serializable]
public class AttackPlan {
    public AttackCode id;
    public AttackStatus status;
    public int timer;

    public AttackPlan(AttackCode id, AttackStatus status, int timer) {
        this.id = id;
        this.status = status;
        this.timer = timer;
    }
}

[System.Serializable]
public class AttackStats {
    public AttackCode id;
    public int n;
    public int hit;
    public int miss;

    public AttackStats(AttackCode id, int n, int hit, int miss) {
        this.id = id;
        this.n = n;
        this.hit = hit;
        this.miss = miss;
    }
}

[System.Serializable]
public class AttacksJSON {
    public AttackInfo[] attacks;


}

public static class AttackUtils {
    static AttacksJSON attacksJSON;

    public static Dictionary<AttackCode, AttackInfo> LoadFromFile(TextAsset file) {
        Dictionary<AttackCode, AttackInfo> attacks = new Dictionary<AttackCode, AttackInfo>();

        attacksJSON = JsonUtility.FromJson<AttacksJSON>(file.text);
        foreach (AttackInfo attack in attacksJSON.attacks) {
            attacks.Add(attack.id, attack);
        }

        return attacks;
    }

    public static void SetupAll(Dictionary<AttackCode, AttackInfo> attacks, Dictionary<AttackCode, Resistance> resistances, Dictionary<AttackCode, AttackStats> attackStats, Dictionary<AttackCode, AttackPlan> attackSchedule) {
        foreach (AttackInfo attack in attacks.Values) {
            if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, 0, 0f, 0f));
            attackStats.Add(attack.id, new AttackStats(attack.id, 0, 0, 0));
            attackSchedule.Add(attack.id, new AttackPlan(attack.id, AttackStatus.INACTIVE, 0));
        }
    }

    public static void UpdateAll(Dictionary<AttackCode, Resistance> resistances, Dictionary<AttackCode, AttackStats> attackStats, Dictionary<AttackCode, AttackPlan> attackSchedule, Resistance[] ress, AttackStats[] aStats, AttackPlan[] aSchedules) {
        foreach (Resistance res in ress) {
            resistances.Add(res.id, res);
        }
        foreach (AttackStats aStat in aStats) {
            attackStats.Add(aStat.id, aStat);
        }
        foreach (AttackPlan aSchedule in aSchedules) {
            attackSchedule.Add(aSchedule.id, aSchedule);
        }
    }
}