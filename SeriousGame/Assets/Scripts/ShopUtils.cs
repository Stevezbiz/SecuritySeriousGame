using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopItemCode {
    NONE,
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

public enum ShopItemStatus {
    NOT_OWNED,
    NOT_INSTALLED,
    INSTALLING,
    ACTIVE,
    INACTIVE
}

[System.Serializable]
public class ShopItemInfo {
    public ShopItemCode id;
    public Category category;
    public string name;
    public string description;
    public int cost;
    public float moneyMalus;
    public float usersMod;
    public ShopItemStatus status;
    public bool locked;
    public int upgradeTime;
    public Resistance[] resistances;
    public ShopItemCode[] requirements;
}

[System.Serializable]
public class ShopItemRecap {
    public ShopItemCode id;
    public ShopItemStatus status;
    public bool locked;

    public ShopItemRecap(ShopItemCode id, ShopItemStatus status, bool locked) {
        this.id = id;
        this.status = status;
        this.locked = locked;
    }
}

[System.Serializable]
public class ShopJSON {
    public ShopItemInfo[] powerUps;
}

public static class ShopUtils {
    static ShopJSON shopJSON;

    public static void UpdateShopItems(Dictionary<ShopItemCode, ShopItemInfo> shopItems, ShopItemRecap[] sir) {
        foreach (ShopItemRecap s in sir) {
            shopItems[s.id].status = s.status;
            shopItems[s.id].locked = s.locked;
        }
    }

    public static ShopItemRecap[] GetShopItemRecap(Dictionary<ShopItemCode, ShopItemInfo> shopItems) {
        List<ShopItemRecap> sir = new List<ShopItemRecap>();

        foreach (ShopItemInfo sii in shopItems.Values) {
            sir.Add(new ShopItemRecap(sii.id, sii.status, sii.locked));
        }

        return sir.ToArray();
    }
}
