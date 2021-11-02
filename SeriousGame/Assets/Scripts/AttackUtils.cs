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
    public string name;
    public string description;
    public float moneyLoss;
    public float usersLoss;
    public float moneyMalus;
    public float usersMalus;
    public float reputationMalus;
    public float maxTime;
    public float duration;
}

[System.Serializable]
public class AttackRecap {
    public AttackCode id;
    public int duration;
    public bool active;
    public int timer;

    public AttackRecap(AttackCode id, int duration, bool active, int timer) {
        this.id = id;
        this.duration = duration;
        this.active = active;
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

    public static void SetupAll(Dictionary<AttackCode, AttackInfo> attacks, Dictionary<AttackCode, Resistance> resistances, Dictionary<AttackCode, AttackStats> attackStats) {
        foreach (AttackInfo attack in attacks.Values) {
            if (attack.duration == 0) {
                if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, -1f, 0f, 0f));
            } else {
                if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, 0f, 0f, 0f));
            }
            attackStats.Add(attack.id, new AttackStats(attack.id, 0, 0, 0));
        }
    }

    public static void UpdateAll(Dictionary<AttackCode, Resistance> resistances, Dictionary<AttackCode, AttackStats> attackStats, Resistance[] ress, AttackStats[] aStats) {
        foreach (Resistance res in ress) {
            resistances.Add(res.id, res);
        }
        foreach (AttackStats aStat in aStats) {
            attackStats.Add(aStat.id, aStat);
        }
    }
}