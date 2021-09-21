using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;
using Image = UnityEngine.UI.Image;

public class GUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI usersText;
    [SerializeField] TextMeshProUGUI reputationText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Image reputationBar;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] ShopGUI shop;
    [SerializeField] AttacksManager attacksManager;
    [SerializeField] LogManager logManager;
    [SerializeField] float money;
    [SerializeField] float users;
    [SerializeField] float reputation;

    DateTime dateTime;
    float startTime;
    int updateTime = 1;

    float[] usersGain = new float[] {
        0.1f,
        0.05f,
        0.03f,
        0.02f,
        0.01f,
        0.005f
    };

    float[] moneyGain = new float[] {
        10f,
        20f,
        30f,
        40f,
        50f,
        60f
    };


    float moneyMalus = 5f;
    float usersMalus = 1f;
    float attackUsersMalus = 0f;
    float attackMoneyMalus = 1f;
    int negativeTime = 0;
    int maxNegative = 60;
    int noAttackTime = 0;
    int noAttackStep = 10;
    int ongoingAttacks = 0;
    int userLevel = 0;

    // Start is called before the first frame update
    void Start() {
        startTime = Time.time;
        Time.timeScale = 1;
        DateTime dt = DateTime.Now.AddMonths(1);
        dateTime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
        Refresh();
    }

    // Update is called once per frame
    void Update() {
        // update the GUI every second
        if (Time.time - startTime >= updateTime) {
            updateTime++;

            userLevel = CalculateUserLevel();
            // money
            money = CalculateMoney(userLevel);
            // users
            users = CalculateUsers(userLevel);
            // reputation
            reputation = CalculateReputation();
            // date
            dateTime = dateTime.AddHours(1);

            // game over check
            CheckGameOver();

            // refresh
            Refresh();
        }
    }

    void Refresh() {
        moneyText.SetText(Mathf.Floor(money).ToString());
        usersText.SetText(Mathf.Floor(users).ToString());
        reputationText.SetText(Mathf.FloorToInt(reputation * 100).ToString() + "%");
        reputationBar.fillAmount = reputation;
        dateText.SetText(dateTime.ToString("d MMM yyyy"));
        timeText.SetText(dateTime.ToString("HH:mm"));
    }

    public void Purchase(int id) {
        ShopItemInfo sii = shop.GetItem(id);
        string name = sii.name;
        money -= sii.cost;
        sii.owned = true;
        shop.AddItem(sii);
        logManager.LogPrintItem(name, ActionCode.PURCHASE);
        EnableShopItem(id);
        Refresh();
    }

    public void EnableShopItem(int id) {
        ShopItemInfo sii = shop.GetItem(id);
        string name = sii.name;
        moneyMalus += sii.moneyMalus;
        usersMalus *= 1 - sii.usersMalus;
        sii.on = true;
        shop.AddItem(sii);
        attacksManager.EnableShopItem(sii.resistances);
        logManager.LogPrintItem(name, ActionCode.ENABLE);
    }

    public void DisableShopItem(int id) {
        ShopItemInfo sii = shop.GetItem(id);
        string name = sii.name;
        moneyMalus -= sii.moneyMalus;
        usersMalus /= 1 - sii.usersMalus;
        sii.on = false;
        shop.AddItem(sii);
        attacksManager.DisableShopItem(sii.resistances);
        logManager.LogPrintItem(name, ActionCode.DISABLE);
    }

    public void StartAttack(int id) {
        ongoingAttacks++;
        money -= attacksManager.Attack(id).moneyLoss;
        users -= attacksManager.Attack(id).usersLoss;
        attackUsersMalus += attacksManager.Attack(id).usersMalus;
        attackMoneyMalus *= attacksManager.Attack(id).moneyMalus;
        reputation -= attacksManager.Attack(id).reputationMalus;
        if (reputation <= 0) {
            reputation = 0;
            // game over
        }
    }

    public void StopAttack(int id) {
        ongoingAttacks--;
        attackUsersMalus -= attacksManager.Attack(id).usersMalus;
        attackMoneyMalus /= attacksManager.Attack(id).moneyMalus;
    }

    public void MissedAttack() {
        reputation += 0.01f;
        if (reputation > 1f) reputation = 1f;
    }

    public string GetDateTime() {
        return dateTime.ToString("dd-MMM-yyyy-HH:mm | ");
    }

    float CalculateMoney(int i) {
        return money + moneyGain[i] * attackMoneyMalus - moneyMalus;
    }

    float CalculateUsers(int i) {
        return users + usersGain[i] * (0.5f * (1 + reputation) * usersMalus - attackUsersMalus) * (float)Mathf.Floor(users);
    }

    int CalculateUserLevel() {
        int i;
        if (users < 100) i = 0;
        else if (users < 1000) i = 1;
        else if (users < 10000) i = 2;
        else if (users < 100000) i = 3;
        else if (users < 1000000) i = 4;
        else i = 5;
        return i;
    }

    float CalculateReputation() {
        float rep = reputation + 0.0005f;
        if (ongoingAttacks == 0) {
            noAttackTime++;
            if (noAttackTime == noAttackStep) {
                noAttackTime = 0;
                rep += 0.01f;
            }
        } else {
            noAttackTime = 0;
        }
        if (rep > 1f) {
            return 1f;
        } else {
            return rep;
        }
    }

    void CheckGameOver() {
        if (money < 0) {
            negativeTime++;
        } else {
            negativeTime = 0;
        }
        if (negativeTime > maxNegative) {
            // end the game
            Time.timeScale = 0;
            GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
            newWindow.transform.SetParent(gameObject.transform, false);
            newWindow.GetComponent<WindowPopUp>().Message = "GAME OVER";
        }
    }
}
