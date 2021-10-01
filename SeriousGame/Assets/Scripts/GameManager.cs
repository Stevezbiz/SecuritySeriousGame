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
    List<AttackRecap> attackSchedule = new List<AttackRecap>();
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

            // update attacks
            PerformAttacks();
            
            // update values
            userLevel = CalculateUserLevel();
            money = CalculateMoney();
            users = CalculateUsers();
            reputation = CalculateReputation();
            dateTime = dateTime.AddHours(1);

            // refresh
            gui.Refresh(Mathf.Floor(money).ToString(), Mathf.Floor(users).ToString(), reputation, dateTime);

            // game over check
            CheckGameOver();
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
        return Mathf.Floor(usersGain[userLevel] * (0.5f * (1 + reputation) * usersMalus * usersBonus - attackUsersMalus) * Mathf.Floor(users));
    }

    public float GetUsersGain() {
        return Mathf.Floor(usersGain[userLevel] * Mathf.Floor(users) * 0.5f * (1 + reputation));
    }

    public float GetUsersMod() {
        return usersMalus * usersBonus;
    }

    public float GetAttackUsersMalus() {
        return Mathf.Floor(usersGain[userLevel] * Mathf.Floor(users) * attackUsersMalus);
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
            ScheduleAttack(attack.id, attackSchedule.Count, false);
        }

        gui.Refresh(Mathf.Floor(money).ToString(), Mathf.Floor(users).ToString(), reputation, dateTime);
    }

    float CalculateMoney() {
        return money + moneyGain[userLevel] * attackMoneyMalus - moneyMalus;
    }

    float CalculateUsers() {
        return users + usersGain[userLevel] * (0.5f * (1 + reputation) * usersMalus * usersBonus - attackUsersMalus) * Mathf.Floor(users);
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
        if (rep > 1f) return 1f;
        else return rep;
    }

    void GameOver() {
        Time.timeScale = 0;
        GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
        newWindow.transform.SetParent(gameObject.transform, false);
        newWindow.GetComponent<WindowPopUp>().Load("GAME OVER");
    }

    void CheckGameOver() {
        if (negativeTime > maxNegative) GameOver();
        if (reputation == 0) GameOver();

        if (money < 0) negativeTime++;
        else negativeTime = 0;
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

    void ScheduleAttack(int id, int i, bool replace) {
        float maxTime = attacks[id].maxTime * GetEndurance(id);
        float nextTime = Random.Range(0.5f * maxTime, maxTime);
        if (replace) attackSchedule[i] = new AttackRecap(id, Mathf.CeilToInt(GetDuration(id)), false, Mathf.CeilToInt(nextTime));
        else attackSchedule.Insert(i, new AttackRecap(id, Mathf.CeilToInt(GetDuration(id)), false, Mathf.CeilToInt(nextTime)));
    }

    float GetMiss(int id) {
        return miss + resistances[id].miss;
    }

    float GetDuration(int id) {
        return (1 - resistances[id].duration) * attacks[id].duration;
    }

    float GetEndurance(int id) {
        return endurance + resistances[id].endurance;
    }

    void StartAttack(int id) {
        ongoingAttacks++;
        money -= attacks[id].moneyLoss;
        users -= attacks[id].usersLoss;
        attackUsersMalus += attacks[id].usersMalus;
        attackMoneyMalus *= attacks[id].moneyMalus;
        reputation -= attacks[id].reputationMalus;
        if (reputation <= 0) reputation = 0;

        GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
        newWindow.transform.SetParent(gameObject.transform, false);
        newWindow.GetComponent<WindowPopUp>().Load("Individuato attacco " + attacks[id].name + "! " + attacks[id].description);
    }

    void StopAttack(int id) {
        ongoingAttacks--;
        attackUsersMalus -= attacks[id].usersMalus;
        attackMoneyMalus /= attacks[id].moneyMalus;
    }

    void MissedAttack(int id) {
        reputation += 0.01f;
        if (reputation > 1f) reputation = 1f;

        GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
        newWindow.transform.SetParent(gameObject.transform, false);
        newWindow.GetComponent<WindowPopUp>().Load("Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name);
    }

    void PerformAttacks() {
        for(int i = 0; i < attackSchedule.Count; i++) {
            if (attackSchedule[i].timer > 0) {
                attackSchedule[i].timer--;
            } else {
                // start the attack eventually
                if (!attackSchedule[i].active) {
                    if (Random.Range(0f, 1f) > GetMiss(attackSchedule[i].id)) {
                        // hit
                        attackStats[attackSchedule[i].id].hit++;
                        attackSchedule[i].active = true;
                        StartAttack(attackSchedule[i].id);

                        // log print hit
                        logManager.LogPrintAttack(attacks[attackSchedule[i].id].name, true);
                    } else {
                        // miss
                        attackStats[attackSchedule[i].id].miss++;
                        MissedAttack(attackSchedule[i].id);
                        ScheduleAttack(attackSchedule[i].id, i, true);

                        // log print miss
                        logManager.LogPrintAttack(attacks[attackSchedule[i].id].name, false);
                    }
                    attackSchedule[i].active = true;
                }
                // end the attack eventually
                if (attackSchedule[i].duration == 0) {
                    StopAttack(attackSchedule[i].id);
                    ScheduleAttack(attackSchedule[i].id, i, true);

                } else {
                    attackSchedule[i].duration--;
                }
            }
        }
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
            shopItems[s.id].owned = s.owned;
            shopItems[s.id].on = s.on;
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
