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
    public ShopItemCode id;
    public string name;
    public string description;
    public int cost;
    public float moneyMalus;
    public float usersMod;
    public bool owned;
    public bool on;
    public bool locked;
    public Resistance[] resistances;
    public ShopItemCode[] requirements;
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
    public ShopItemCode id;
    public bool owned;
    public bool on;
    public bool locked;

    public ShopItemRecap(ShopItemCode id, bool owned, bool on, bool locked) {
        this.id = id;
        this.owned = owned;
        this.on = on;
        this.locked = locked;
    }
}

[System.Serializable]
public class GameSave {
    public GameConfig gc;
    public ShopItemRecap[] sir;
    public LogData logs;
    public AttackStats[] aStats;
    public AttackRecap[] aSchedule;
    public Resistance[] res;

    public GameSave(GameConfig gc, ShopItemRecap[] sir, LogData logs, AttackStats[] aStats, AttackRecap[] aSchedule, Resistance[] res) {
        this.gc = gc;
        this.sir = sir;
        this.logs = logs;
        this.aStats = aStats;
        this.aSchedule = aSchedule;
        this.res = res;
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
    public int totalTime;
    public int endTime;
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
    public float moneyBonus;
    public float usersMalus;
    public float usersBonus;
    public float attackUsersMalus;
    public float attackMoneyMalus;
    public float endurance;
    public float miss;
    public float[] usersGain;
    public float[] moneyGain;
    public string date;

    public GameConfig(int totalTime, int endTime, int negativeTime, int maxNegative, int noAttackTime, int noAttackStep, int ongoingAttacks, int userLevel, float money, float users, float reputation, float moneyMalus, float moneyBonus, float usersMalus, float usersBonus, float attackUsersMalus, float attackMoneyMalus, float endurance, float miss, float[] usersGain, float[] moneyGain, string date) {
        this.totalTime = totalTime;
        this.endTime = endTime;
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
        this.moneyBonus = moneyBonus;
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

[System.Serializable]
public class GameConfigJSON {
    public GameConfig gameConfig;
}

[System.Serializable]
public class AttackRecap {
    public int id;
    public int duration;
    public bool active;
    public int timer;

    public AttackRecap(int id, int duration, bool active, int timer) {
        this.id = id;
        this.duration = duration;
        this.active = active;
        this.timer = timer;
    }
}

[System.Serializable]
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
    GAME_OVER,
    CONTINUE,
    PURCHASE_ITEM,
    ENABLE_ITEM,
    DISABLE_ITEM
}

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

public enum ShopItemCode {
    CONN_TIME_RED,
    CONN_QUEUE_CIRC,
    FIREWALL,
    DNSSEC,
    ERR_PASS_LIMIT,
    ERR_PASS_WAIT,
    CHANGE_PASS,
    COMPLEX_PASS,
    TWO_F_A,
    HASH,
    CAPTCHA,
    IDS,
    SOFTWARE_UPDATES,
    ANTIVIRUS,
    AUTH_SOFTWARE,
    USER_AWARENESS,
    USER_SUPPORT,
    SERVER_UPGRADE,
    SEC_TRAINING,
    BACK_UP
}

public static class COLOR {
    public static Color LOG_RED = new Color(1f, .25f, .0f, 1f);
    public static Color LOG_BLUE = new Color(.0f, 1f, 1f, 1f);
    public static Color LOG_GREEN = new Color(.0f, 1f, .0f, 1f);
}
