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
    public AttackPlan[] aSchedule;
    public Task[] waitingTasks;
    public Task[] assignedTasks;
    public Resistance[] res;

    public GameSave(GameConfig gc, ShopItemRecap[] sir, EmployeeRecap[] er, LogData logs, AttackStats[] aStats,
        AttackPlan[] aSchedule, Task[] waitingTasks, Task[] assignedTasks, Resistance[] res) {
        this.gc = gc;
        this.sir = sir;
        this.er = er;
        this.logs = logs;
        this.aStats = aStats;
        this.aSchedule = aSchedule;
        this.waitingTasks = waitingTasks;
        this.assignedTasks = assignedTasks;
        this.res = res;
    }
}

[System.Serializable]
public class GameConfig {
    public int totalTime;
    public int endTime;
    public int negativeTime;
    public int maxNegative;
    public int evaluationTime;
    public int quizTime;
    public int actualQuiz;
    public int quizTimer;
    public int attackTrendTime;
    public AttackCode actualAttackTrend;
    public int attackTrendTimer;
    public int resistanceModStep;
    public int actualResistanceMod;
    public int noAttackTime;
    public int noAttackStep;
    public int ongoingAttacks;
    public int userLevel;
    public int employeeLevel;
    public float abilityOffset;
    public float abilityFactor;
    public float money;
    public float users;
    public float reputation;
    public float[] duration;
    public float[] endurance;
    public float[] miss;
    public float[] usersGain;
    public float[] usersGoals;
    public float[] employeeGoals;
    public string date;
    public float musicVolume;
    public bool musicMute;
    public float effectsVolume;
    public bool effectsMute;
}

[System.Serializable]
public class GameConfigJSON {
    public GameConfig gameConfig;
}

public class Person {
    public string name;
    public Sprite icon;
    public Sprite figure;

    public Person(string name, Sprite icon, Sprite figure) {
        this.name = name;
        this.icon = icon;
        this.figure = figure;
    }
}

public class Category {
    public string name;
    public Sprite sprite;

    public Category(string name, Sprite sprite) {
        this.name = name;
        this.sprite = sprite;
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
    BYPASS_LOADING,
    PURCHASE_ITEM,
    UPGRADE_ITEM,
    ENABLE_ITEM,
    DISABLE_ITEM
}

public enum CategoryCode {
    NETWORK,
    ACCESS,
    SOFTWARE,
    ASSET,
    SERVICES
}

public enum Role {
    CEO,
    SECURITY,
    SOFTWARE,
    MARKETING,
    ASSISTANCE
}

public static class COLOR {
    public static Color RED = new Color(1f, .25f, .0f, 1f);
    public static Color BLUE = new Color(.0f, 1f, 1f, 1f);
    public static Color GREEN = new Color(.0f, 1f, .0f, 1f);
    public static Color YELLOW = new Color(1f, .8f, .0f, 1f);
    public static Color GREEN_DISABLED = new Color(.0f, .4f, .0f, 1f);
    public static Color DISABLED = new Color(.0f, .0f, .0f, .0f);
}

public static class NumUtils {
    public static string NumToString(double val) {
        string text;
        if (val > 1000000000) text = (val / 1000000000).ToString(".#") + "B";
        else if (val > 1000000) text = (val / 1000000).ToString(".#") + "M";
        else if (val > 1000) text = (val / 1000).ToString(".#") + "K";
        else text = val.ToString("0.");
        return text;
    }
}
