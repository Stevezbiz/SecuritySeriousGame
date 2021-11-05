using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public enum ShopItemCategory {
    NETWORK,
    ACCESS,
    SOFTWARE,
    ASSET,
    SERVICES
}

[System.Serializable]
public class ShopItemInfo {
    public ShopItemCode id;
    public ShopItemCategory category;
    public string name;
    public string description;
    public int cost;
    public float moneyMalus;
    public float usersMod;
    public bool owned;
    public bool on;
    public bool locked;
    public int upgradeTime;
    public Resistance[] resistances;
    public ShopItemCode[] requirements;
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
public class ShopJSON {
    public ShopItemInfo[] powerUps;
}

public static class ShopUtils {
    static ShopJSON shopJSON;

    public static void UpdateShopItems(Dictionary<ShopItemCode, ShopItemInfo> shopItems, ShopItemRecap[] sir) {
        foreach (ShopItemRecap s in sir) {
            shopItems[s.id].owned = s.owned;
            shopItems[s.id].on = s.on;
            shopItems[s.id].locked = s.locked;
        }
    }

    public static ShopItemRecap[] GetShopItemRecap(Dictionary<ShopItemCode, ShopItemInfo> shopItems) {
        List<ShopItemRecap> sir = new List<ShopItemRecap>();

        foreach (ShopItemInfo sii in shopItems.Values) {
            sir.Add(new ShopItemRecap(sii.id, sii.owned, sii.on, sii.locked));
        }

        return sir.ToArray();
    }
}
