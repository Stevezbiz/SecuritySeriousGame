using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resistance {
    public int id;
    public float duration;
    public float miss;
    public float endurance;

    public Resistance(int id, float duration, float miss, float endurance) {
        this.id = id;
        this.duration = duration;
        this.miss = miss;
        this.endurance = endurance;
    }
}

[System.Serializable]
public class ShopItemInfo {
    public int id;
    public string name;
    public string description;
    public int cost;
    public float moneyMalus;
    public float usersMod;
    public bool owned;
    public bool on;
    public Resistance[] resistances;
}

[System.Serializable]
public class ShopJSON {
    public ShopItemInfo[] powerUps;
}

[System.Serializable]
public class AttackInfo {
    public int id;
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
public class AttacksJSON {
    public AttackInfo[] attacks;
}

[System.Serializable]
public class ShopItemRecap {
    public int id;
    public bool owned;
    public bool on;

    public ShopItemRecap(int id, bool owned, bool on) {
        this.id = id;
        this.owned = owned;
        this.on = on;
    }
}

[System.Serializable]
public class GameSave {
    public GameConfig gc;
    public ShopItemRecap[] sir;
    public LogData logs;

    public GameSave(GameConfig gc, ShopItemRecap[] sir, LogData logs) {
        this.gc = gc;
        this.sir = sir;
        this.logs = logs;
    }
}

[System.Serializable]
public class LogLine {
    public string line;
    public float[] color;

    public LogLine(string line, float[] color) {
        this.line = line;
        this.color = color;
    }
}

[System.Serializable]
public class LogData {
    public LogLine[] lines;
    public int nLines;
    public int nPages;

    public LogData(LogLine[] lines, int nLines, int nPages) {
        this.lines = lines;
        this.nLines = nLines;
        this.nPages = nPages;
    }
}

[System.Serializable]
public class GameConfig {
    public int negativeTime;
    public int maxNegative;
    public int noAttackTime;
    public int noAttackStep;
    public int ongoingAttacks;
    public int userLevel;
    public float money;
    public float users;
    public float reputation;
    public float moneyMalus;
    public float usersMalus;
    public float usersBonus;
    public float attackUsersMalus;
    public float attackMoneyMalus;
    public float endurance;
    public float miss;
    public float[] usersGain;
    public float[] moneyGain;
    public string date;

    public GameConfig(int negativeTime, int maxNegative, int noAttackTime, int noAttackStep, int ongoingAttacks, int userLevel, float money, float users, float reputation, float moneyMalus, float usersMalus, float usersBonus, float attackUsersMalus, float attackMoneyMalus, float endurance, float miss, float[] usersGain, float[] moneyGain, string date) {
        this.negativeTime = negativeTime;
        this.maxNegative = maxNegative;
        this.noAttackTime = noAttackTime;
        this.noAttackStep = noAttackStep;
        this.ongoingAttacks = ongoingAttacks;
        this.userLevel = userLevel;
        this.money = money;
        this.users = users;
        this.reputation = reputation;
        this.moneyMalus = moneyMalus;
        this.usersMalus = usersMalus;
        this.usersBonus = usersBonus;
        this.attackUsersMalus = attackUsersMalus;
        this.attackMoneyMalus = attackMoneyMalus;
        this.endurance = endurance;
        this.miss = miss;
        this.usersGain = usersGain;
        this.moneyGain = moneyGain;
        this.date = date;
    }
}

public class GameConfigJSON {
    public GameConfig gameConfig;
}

public class AttackStats {
    public int id;
    public int n;
    public int hit;
    public int miss;

    public AttackStats(int id, int n, int hit, int miss) {
        this.id = id;
        this.n = n;
        this.hit = hit;
        this.miss = miss;
    }
}

// error codes used in the project
public enum ECode {
    OK, // no error
    GENERIC, // unspecified error
    INSUFFICIENT_MONEY // insufficient money to do a purchase
};


public enum ActionCode {
    PURCHASE,
    ENABLE,
    DISABLE
}

public static class COLOR {
    public static Color LOG_RED = new Color(1f, .25f, .0f, 1f);
    public static Color LOG_BLUE = new Color(.0f, 1f, 1f, 1f);
    public static Color LOG_GREEN = new Color(.0f, 1f, .0f, 1f);
}
