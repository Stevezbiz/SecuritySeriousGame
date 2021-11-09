using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class GameSave {
    public GameConfig gc;
    public ShopItemRecap[] sir;
    public EmployeeRecap[] er;
    public LogData logs;
    public AttackStats[] aStats;
    public AttackRecap[] aSchedule;
    public Task[] tasks;
    public Resistance[] res;

    public GameSave(GameConfig gc, ShopItemRecap[] sir, EmployeeRecap[] er, LogData logs, AttackStats[] aStats, AttackRecap[] aSchedule, Task[] tasks, Resistance[] res) {
        this.gc = gc;
        this.sir = sir;
        this.er = er;
        this.logs = logs;
        this.aStats = aStats;
        this.aSchedule = aSchedule;
        this.tasks = tasks;
        this.res = res;
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
    public int initEmployees;
    public int availableEmployees;
    public int hiredEmployees;
    public float money;
    public float users;
    public float reputation;
    public float endurance;
    public float miss;
    public float[] usersGain;
    public float[] usersGoals;
    public float[] employeeGoals;
    public string date;

    public GameConfig(int totalTime, int endTime, int negativeTime, int maxNegative, int noAttackTime, int noAttackStep, int ongoingAttacks,
        int userLevel, int initEmployees, int availableEmployees, int hiredEmployees, float money, float users, float reputation, float endurance, float miss,
        float[] usersGain, float[] usersGoals, float[] employeeGoals, string date) {
        this.totalTime = totalTime;
        this.endTime = endTime;
        this.negativeTime = negativeTime;
        this.maxNegative = maxNegative;
        this.noAttackTime = noAttackTime;
        this.noAttackStep = noAttackStep;
        this.ongoingAttacks = ongoingAttacks;
        this.userLevel = userLevel;
        this.initEmployees = initEmployees;
        this.availableEmployees = availableEmployees;
        this.hiredEmployees = hiredEmployees;
        this.money = money;
        this.users = users;
        this.reputation = reputation;
        this.endurance = endurance;
        this.miss = miss;
        this.usersGain = usersGain;
        this.usersGoals = usersGoals;
        this.employeeGoals = employeeGoals;
        this.date = date;
    }
}

[System.Serializable]
public class GameConfigJSON {
    public GameConfig gameConfig;
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

public static class COLOR {
    public static Color RED = new Color(1f, .25f, .0f, 1f);
    public static Color BLUE = new Color(.0f, 1f, 1f, 1f);
    public static Color GREEN = new Color(.0f, 1f, .0f, 1f);
    public static Color YELLOW = new Color(1f, .8f, .0f, 1f);
    public static Color GREEN_DISABLED = new Color(.0f, .4f, .0f, 1f);
}
