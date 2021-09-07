using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int[] attacks;
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
