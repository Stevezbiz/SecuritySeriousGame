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
    public float usersMalus;
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

// error codes used in the project
public enum ECode {
    OK, // no error
    GENERIC, // unspecified error
    INSUFFICIENT_MONEY // insufficient money to do a purchase
};
