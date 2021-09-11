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
    [SerializeField] Shop shop;
    [SerializeField] AttacksManager attacksManager;
    [SerializeField] float money;
    [SerializeField] float users;
    [SerializeField] float reputation;

    DateTime dateTime;
    float startTime;
    int updateTime = 1;

    float[,] moneyCoeffs = new float[,] {
        { .5f, 0f },
        { .2f, 28f },
        { .08f, 145f },
        { .03f, 642f },
        { .012f, 2606f },
        { .004f, 10060f },
    };

    float[] usersCoeffs = new float[] {
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

    Dictionary<int, Resistance> resistances = new Dictionary<int, Resistance>();

    float moneyMalus = 5f;
    float usersMalus = 1f;
    float attackUsersMalus = 0f;
    float attackMoneyMalus = 1f;
    float endurance = 1f;
    float miss = 0f;
    int negativeTime = 0;
    int maxNegative = 60;
    int noAttackTime = 0;
    int noAttackStep = 6;
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
            dateTime = dateTime.AddHours(4);

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
        Debug.Log("\n" +
            "moneyMalus\t\t" + moneyMalus + "\n" +
            "usersMalus\t\t" + usersMalus + "\n" +
            "attackMoneyMalus\t\t" + attackMoneyMalus + "\n" +
            "attackUsersMalus\t\t" + attackUsersMalus+ "\n" +
            "");
    }

    public void AddResistance(int id) {
        if (!resistances.ContainsKey(id)) resistances.Add(id, new Resistance(id, 0f, 0f, 0f));
    }

    public void Purchase(int id) {
        ShopItemInfo sii = shop.GetItem(id);
        money -= sii.cost;
        sii.owned = true;
        shop.SetItem(sii);
        EnableShopItem(id);
        Refresh();
    }

    public void EnableShopItem(int id) {
        ShopItemInfo sii = shop.GetItem(id);
        moneyMalus += sii.moneyMalus;
        usersMalus *= 1 - sii.usersMalus;
        sii.on = true;
        shop.SetItem(sii);
        foreach(Resistance r in shop.GetItem(id).resistances) {
            resistances[r.id].miss += r.miss;
            resistances[r.id].duration += r.duration;
            resistances[r.id].endurance += r.endurance;
        }
    }

    public void DisableShopItem(int id) {
        ShopItemInfo sii = shop.GetItem(id);
        moneyMalus -= sii.moneyMalus;
        usersMalus /= 1 - sii.usersMalus;
        sii.on = false;
        shop.SetItem(sii);
        foreach (Resistance r in shop.GetItem(id).resistances) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration -= r.duration;
            resistances[r.id].endurance -= r.endurance;
        }
    }

    public float GetMiss(int id) {
        return miss + resistances[id].miss;
    }

    public float GetDuration(int id) {
        return (1 - resistances[id].duration) * attacksManager.Attack(id).duration + 1;
    }

    public float GetEndurance(int id) {
        return endurance + resistances[id].endurance;
    }

    public void StartAttack(int id) {
        ongoingAttacks++;
        money -= attacksManager.Attack(id).moneyLoss;
        users -= attacksManager.Attack(id).usersLoss;
        attackUsersMalus += attacksManager.Attack(id).usersMalus;
        attackMoneyMalus *= attacksManager.Attack(id).moneyMalus;
        reputation -= attacksManager.Attack(id).reputationMalus;
    }

    public void StopAttack(int id) {
        ongoingAttacks--;
        attackUsersMalus -= attacksManager.Attack(id).usersMalus;
        attackMoneyMalus /= attacksManager.Attack(id).moneyMalus;
    }

    public void MissedAttack() {
        reputation += 0.1f;
    }

    float CalculateMoney(int i) {
        Debug.Log("moneyGain = " + moneyGain[i] + "\n" +
            "moneyMalus = " + moneyMalus + "\n" + 
            "attackMoneyMalus = " + attackMoneyMalus + "\n" +
            "gain = " + (moneyGain[i] * attackMoneyMalus - moneyMalus) + "\n");
        return money + moneyGain[i] * attackMoneyMalus - moneyMalus;
    }

    float CalculateUsers(int i) {
        return users + usersCoeffs[i] * (0.5f * (1 + reputation) * usersMalus - attackUsersMalus) * (float)Mathf.Floor(users);
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
        if(ongoingAttacks == 0) {
            noAttackTime++;
            if (noAttackTime == noAttackStep) {
                noAttackTime = 0;
                rep += 0.01f;
            }
        } else {
            noAttackTime = 0;
        }
        if(rep > 1f) {
            return 1;
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
            Vector3 newPos = new Vector3(0, 0, 0);
            GameObject newWindow = Instantiate(windowPopUp, newPos, Quaternion.identity);
            newWindow.transform.SetParent(gameObject.transform, false);
            newWindow.GetComponent<WindowPopUp>().Message = "GAME OVER";
        }
    }
}
