using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopItemCode {
    NONE,
    NETWORK_SECURITY,
    FIREWALL,
    DNS_SECURITY,
    PASSWORD_SECURITY,
    MFA,
    HASH,
    IDS,
    SYSTEM_SECURITY,
    ANTIVIRUS,
    USER_AWARENESS,
    USER_SUPPORT,
    SYSTEM_PERFORMANCE,
    SECURITY_TRAINING,
    BACK_UP
}

public enum ShopItemStatus {
    NOT_OWNED,
    NOT_INSTALLED,
    INSTALLING,
    UPGRADING,
    ACTIVE,
    INACTIVE
}

[System.Serializable]
public class ShopItemInfo {
    public ShopItemCode id;
    public Category category;
    public string name;
    public string[] description;
    public int[] cost;
    public float[] moneyMalus;
    public float[] usersMod;
    public ShopItemStatus status;
    public bool locked;
    public int[] upgradeTime;
    public ResistanceArray[] resistances;
    public ShopItemCode[] requirements;
    public int maxLevel;
    public int level;
}

[System.Serializable]
public class ShopItemRecap {
    public ShopItemCode id;
    public ShopItemStatus status;
    public bool locked;
    public int level;

    public ShopItemRecap(ShopItemCode id, ShopItemStatus status, bool locked, int level) {
        this.id = id;
        this.status = status;
        this.locked = locked;
        this.level = level;
    }
}

[System.Serializable]
public class ShopJSON {
    public ShopItemInfo[] shopItems;
}

public static class ShopUtils {
    public static Dictionary<ShopItemCode, ShopItemInfo> LoadFromFile(TextAsset file) {
        Dictionary<ShopItemCode, ShopItemInfo> shopItems = new Dictionary<ShopItemCode, ShopItemInfo>();
        ShopJSON shopContent = JsonUtility.FromJson<ShopJSON>(file.text);
        foreach (ShopItemInfo item in shopContent.shopItems) {
            shopItems.Add(item.id, item);
        }
        return shopItems;
    }
    
    public static void UpdateShopItems(Dictionary<ShopItemCode, ShopItemInfo> shopItems, ShopItemRecap[] sir) {
        foreach (ShopItemRecap s in sir) {
            shopItems[s.id].status = s.status;
            shopItems[s.id].locked = s.locked;
        }
    }

    public static ShopItemRecap[] GetShopItemRecap(Dictionary<ShopItemCode, ShopItemInfo> shopItems) {
        List<ShopItemRecap> sir = new List<ShopItemRecap>();

        foreach (ShopItemInfo sii in shopItems.Values) {
            sir.Add(new ShopItemRecap(sii.id, sii.status, sii.locked, sii.level));
        }

        return sir.ToArray();
    }
}
