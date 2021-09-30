using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;

public class GameManager : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] ShopGUI shop;
    [SerializeField] Log logManager;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextAsset gameConfigJSON;
    [SerializeField] TextAsset attacksFileJSON;

    float startTime;
    int updateTime = 1;
    int negativeTime;
    int maxNegative;
    int noAttackTime;
    int noAttackStep;
    int ongoingAttacks;
    int userLevel;
    float money;
    float users;
    float reputation;
    float moneyMalus;
    float usersMalus;
    float usersBonus;
    float attackUsersMalus;
    float attackMoneyMalus;
    float endurance;
    float miss;
    float[] usersGain;
    float[] moneyGain;
    DateTime dateTime;

    List<LogLine> logs = new List<LogLine>();
    Dictionary<int, AttackInfo> attacks = new Dictionary<int, AttackInfo>();
    Dictionary<int, Resistance> resistances = new Dictionary<int, Resistance>();
    Dictionary<int, AttackStats> attackStats = new Dictionary<int, AttackStats>();
    Dictionary<int, ShopItemInfo> shopItems = new Dictionary<int, ShopItemInfo>();

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    // Update is called once per frame
    void Update() {
        // update the GUI every second
        if (Time.time - startTime >= updateTime) {
            updateTime++;

            userLevel = CalculateUserLevel();
            // money
            money = CalculateMoney();
            // users
            users = CalculateUsers();
            // reputation
            reputation = CalculateReputation();
            // date
            dateTime = dateTime.AddHours(1);

            // game over check
            CheckGameOver();

            // refresh
            gui.Refresh(Mathf.Floor(money).ToString(), Mathf.Floor(users).ToString(), reputation, dateTime);
        }
    }

    public string GetDateTime() {
        return dateTime.ToString("dd-MMM-yyyy-HH:mm | ");
    }

    public float GetActualMoneyGain() {
        return moneyGain[userLevel] * attackMoneyMalus - moneyMalus;
    }

    public float GetMoneyGain() {
        return moneyGain[userLevel];
    }

    public float GetMoneyMalus() {
        return moneyMalus;
    }

    public float GetAttackMoneyMalus() {
        return attackMoneyMalus;
    }

    public float GetActualUsersGain() {
        return (float)Mathf.Floor(usersGain[userLevel] * (0.5f * (1 + reputation) * usersMalus * usersBonus - attackUsersMalus) * (float)Mathf.Floor(users));
    }

    public float GetUsersGain() {
        return (float)Mathf.Floor(usersGain[userLevel] * (float)Mathf.Floor(users) * 0.5f * (1 + reputation));
    }

    public float GetUsersMod() {
        return usersMalus * usersBonus;
    }

    public float GetAttackUsersMalus() {
        return (float)Mathf.Floor(usersGain[userLevel] * (float)Mathf.Floor(users) * attackUsersMalus);
    }

    public GameSave SaveGame() {
        return new GameSave(new GameConfig(negativeTime, maxNegative, noAttackTime, noAttackStep, ongoingAttacks, userLevel, money, users, reputation, moneyMalus, usersMalus, usersBonus, attackUsersMalus, attackMoneyMalus, endurance, miss, usersGain, moneyGain, dateTime.ToString()), GetShopItemRecap(), new LogData(logs.ToArray(), logManager.GetNLines(), logManager.GetNPages()));
    }

    void Init() {
        startTime = Time.time;
        Time.timeScale = 0;
        shop.Init();

        if (SaveSystem.load) {
            LoadGameData(SaveSystem.LoadGame());
        } else {
            GameConfigJSON gameConfigContent = JsonUtility.FromJson<GameConfigJSON>(gameConfigJSON.text);
            LoadGameConfig(gameConfigContent.gameConfig);
            userLevel = CalculateUserLevel();
            DateTime dt = DateTime.Now.AddMonths(1);
            dateTime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
        }

        shop.Load();
        AttacksJSON attacksContent = JsonUtility.FromJson<AttacksJSON>(attacksFileJSON.text);
        foreach (AttackInfo attack in attacksContent.attacks) {
            attacks.Add(attack.id, attack);
            if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, 0f, 0f, 0f));
            attackStats.Add(attack.id, new AttackStats(attack.id, 0, 0, 0));
            StartCoroutine(ExecuteAttack(attack.id));
        }

        gui.Refresh(Mathf.Floor(money).ToString(), Mathf.Floor(users).ToString(), reputation, dateTime);
    }

    float CalculateMoney() {
        return money + moneyGain[userLevel] * attackMoneyMalus - moneyMalus;
    }

    float CalculateUsers() {
        return users + usersGain[userLevel] * (0.5f * (1 + reputation) * usersMalus * usersBonus - attackUsersMalus) * (float)Mathf.Floor(users);
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
    
    // LOG

    public void AddToLogs(LogLine line) {
        logs.Add(line);
    }

    public LogLine GetLog(int i) {
        return logs[i];
    }

    // ATTACKS

    public AttackInfo GetAttack(int id) {
        return attacks[id];
    }

    IEnumerator ExecuteAttack(int id) {
        // choose first attack
        float maxTime = attacks[id].maxTime * GetEndurance(id);
        float nextTime = Random.Range(0.5f * maxTime, maxTime);

        while (true) {
            // wait for the attack
            yield return new WaitForSeconds(nextTime);

            attackStats[id].n++;
            // launch the attack if hits
            if (Random.Range(0f, 1f) > GetMiss(id)) {
                // hit
                attackStats[id].hit++;
                LaunchAttack(id, GetDuration(id));
                GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
                newWindow.transform.SetParent(gameObject.transform, false);
                newWindow.GetComponent<WindowPopUp>().Message = "Individuato attacco " + attacks[id].name + "! " + attacks[id].description;
                // log print hit
                logManager.LogPrintAttack(attacks[id].name, true);
            } else {
                // miss
                attackStats[id].miss++;
                MissedAttack();
                GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
                newWindow.transform.SetParent(gameObject.transform, false);
                newWindow.GetComponent<WindowPopUp>().Message = "Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name;
                // log print miss
                logManager.LogPrintAttack(attacks[id].name, false);
            }

            // choose the time for the next attack
            maxTime = attacks[id].maxTime * GetEndurance(id);
            nextTime = Random.Range(0.5f * maxTime, maxTime);
        }
    }

    float GetMiss(int id) {
        return miss + resistances[id].miss;
    }

    float GetDuration(int id) {
        return (1 - resistances[id].duration) * attacks[id].duration + 1;
    }

    float GetEndurance(int id) {
        return endurance + resistances[id].endurance;
    }

    void LaunchAttack(int id, float duration) {
        StartCoroutine(PerformAttack(id, duration));
    }

    IEnumerator PerformAttack(int id, float duration) {
        StartAttack(id);
        yield return new WaitForSeconds(duration);
        StopAttack(id);
    }

    void StartAttack(int id) {
        ongoingAttacks++;
        money -= attacks[id].moneyLoss;
        users -= attacks[id].usersLoss;
        attackUsersMalus += attacks[id].usersMalus;
        attackMoneyMalus *= attacks[id].moneyMalus;
        reputation -= attacks[id].reputationMalus;
        if (reputation <= 0) {
            reputation = 0;
            // game over
        }
    }

    void StopAttack(int id) {
        ongoingAttacks--;
        attackUsersMalus -= attacks[id].usersMalus;
        attackMoneyMalus /= attacks[id].moneyMalus;
    }

    void MissedAttack() {
        reputation += 0.01f;
        if (reputation > 1f) reputation = 1f;
    }

    // SHOP

    public void AddToShopItems(ShopItemInfo sii) {
        if (shopItems.ContainsKey(sii.id)) shopItems[sii.id] = sii;
        else shopItems.Add(sii.id, sii);
    }

    public ShopItemInfo GetShopItem(int id) {
        return shopItems[id];
    }

    ShopItemRecap[] GetShopItemRecap() {
        List<ShopItemRecap> sir = new List<ShopItemRecap>();

        foreach (ShopItemInfo sii in shopItems.Values) {
            sir.Add(new ShopItemRecap(sii.id, sii.owned, sii.on));
        }

        return sir.ToArray();
    }

    void LoadGameConfig(GameConfig gc) {
        negativeTime = gc.negativeTime;
        maxNegative = gc.maxNegative;
        noAttackTime = gc.noAttackTime;
        noAttackStep = gc.noAttackStep;
        ongoingAttacks = gc.ongoingAttacks;
        userLevel = gc.userLevel;
        money = gc.money;
        users = gc.users;
        reputation = gc.reputation;
        moneyMalus = gc.moneyMalus;
        usersMalus = gc.usersMalus;
        usersBonus = gc.usersBonus;
        attackUsersMalus = gc.attackUsersMalus;
        attackMoneyMalus = gc.attackMoneyMalus;
        endurance = gc.endurance;
        miss = gc.miss;
        usersGain = gc.usersGain;
        moneyGain = gc.moneyGain;
        dateTime = DateTime.Parse(gc.date);
    }

    void LoadGameData(GameSave gameSave) {
        LoadGameConfig(gameSave.gc);

        foreach (ShopItemRecap s in gameSave.sir) {
            if (s.owned) {
                shopItems[s.id].owned = true;
                if (s.on) {
                    shopItems[s.id].on = true;
                }
            }
        }

        logs = new List<LogLine>(gameSave.logs.lines);
        logManager.LoadGameData(gameSave.logs.nLines, gameSave.logs.nPages);
    }

    public void Purchase(int id) {
        shopItems[id].owned = true;
        money -= shopItems[id].cost;
        gui.Refresh(Mathf.Floor(money).ToString(), Mathf.Floor(users).ToString(), reputation, dateTime);
    }

    public void EnableShopItem(int id) {
        shopItems[id].on = true;
        
        ShopItemInfo sii = shopItems[id];

        moneyMalus += sii.moneyMalus;
        if (sii.usersMod < 1) usersMalus *= 1 - sii.usersMod;
        else usersBonus *= sii.usersMod;

        foreach (Resistance r in sii.resistances) {
            if (!resistances.ContainsKey(r.id)) resistances.Add(r.id, new Resistance(r.id, 0f, 0f, 0f));
            resistances[r.id].miss += r.miss;
            resistances[r.id].duration += r.duration;
            resistances[r.id].endurance += r.endurance;
        }
    }

    public void DisableShopItem(int id) {
        shopItems[id].on = false;
        
        ShopItemInfo sii = shopItems[id];

        moneyMalus -= sii.moneyMalus;
        if (sii.usersMod < 1) usersMalus /= 1 - sii.usersMod;
        else usersBonus /= sii.usersMod;

        foreach (Resistance r in sii.resistances) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration -= r.duration;
            resistances[r.id].endurance -= r.endurance;
        }
    }
}
