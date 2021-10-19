using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;

public class GameManager : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] ShopGUI shop;
    [SerializeField] Log logManager;
    [SerializeField] AttackView attackView;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextAsset gameConfigJSON;
    [SerializeField] TextAsset attacksFileJSON;

    float startTime;
    int updateTime = 1;
    int totalTime;
    int endTime;
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
    float moneyBonus;
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
    Dictionary<ShopItemCode, ShopItemInfo> shopItems = new Dictionary<ShopItemCode, ShopItemInfo>();

    // Start is called before the first frame update
    void Start() {
        Load();
        DebugPrint();
    }

    // Update is called once per frame
    void Update() {
        // update the GUI every second
        if (Time.time - startTime >= updateTime) {
            // step up the time
            updateTime++;
            totalTime++;
            dateTime = dateTime.AddHours(1);
            // schedule new attacks
            ActivateAttacks();
            // update attacks
            UpdateAttacks();
            // update values
            userLevel = CalculateUserLevel();
            money = CalculateMoney();
            users = CalculateUsers();
            reputation = CalculateReputation();
            // refresh
            gui.Refresh(Math.Round(money).ToString(), Math.Round(users).ToString(), reputation, dateTime);
            // game over check
            CheckGameOver();
            // update the stats for possible game over
            if (money < 0) negativeTime++;
            else negativeTime = 0;
        }
    }

    /**
     * <summary>Return a string containing the date in the format 'dd-MMM-yyyy-HH:mm'</summary>
     */
    public string GetDateTime() {
        return dateTime.ToString("dd-MMM-yyyy-HH:mm");
    }

    /**
     * <summary>Return the actual gain of money</summary>
     */
    public float GetActualMoneyGain() {
        return (moneyGain[userLevel] + moneyBonus) * (1 - attackMoneyMalus) - moneyMalus;
    }

    /**
     * <summary>Return the gain of money based on the number of users</summary>
     */
    public float GetMoneyGain() {
        return moneyGain[userLevel] + moneyBonus;
    }

    /**
     * <summary>Return the malus to money caused by the active defences and services</summary>
     */
    public float GetMoneyMalus() {
        return moneyMalus;
    }

    /**
     * <summary>Return the malus to money caused by the active attacks</summary>
     */
    public float GetAttackMoneyMalus() {
        return (float)Math.Round(attackMoneyMalus, 2);
    }

    /**
     * <summary>Return the actual gain of users</summary>
     */
    public float GetActualUsersGain() {
        return (float)Math.Round(usersGain[userLevel] * (0.5f * (1 + reputation) * usersMalus * usersBonus - attackUsersMalus) * Math.Round(users));
    }

    /**
     * <summary>Return the gain of users without malus and bonus</summary>
     */
    public float GetUsersGain() {
        return (float)Math.Round(usersGain[userLevel] * Math.Round(users) * 0.5f * (1 + reputation));
    }

    /**
     * <summary>Return the modifier of the gain of users caused by defences and services</summary>
     */
    public float GetUsersMod() {
        return usersMalus * usersBonus;
    }

    /**
     * <summary>Return the malus to users caused by the active attacks</summary>
     */
    public float GetAttackUsersMalus() {
        return (float)Math.Round(usersGain[userLevel] * Math.Round(users) * Math.Round(attackUsersMalus, 2));
    }

    /**
     * <summary>Return the data to be saved to resume correctly the game in future</summary>
     */
    public GameSave SaveGame() {
        return new GameSave(new GameConfig(totalTime, endTime, negativeTime, maxNegative, noAttackTime, noAttackStep, ongoingAttacks, userLevel,
            money, users, reputation, moneyMalus, moneyBonus, usersMalus, usersBonus, attackUsersMalus, attackMoneyMalus, endurance, miss,
            usersGain, moneyGain, dateTime.ToString()), GetShopItemRecap(), new LogData(logs.ToArray(), logManager.GetNLines(), logManager.GetNPages()),
            new List<AttackStats>(attackStats.Values).ToArray(), attackSchedule.ToArray(), new List<Resistance>(resistances.Values).ToArray());
    }

    /**
     * <summary>Initialize all the main data structures of the game</summary>
     */
    void Load() {
        // initialize the data structures and the settings
        startTime = Time.time;
        Time.timeScale = 0;
        shop.Init();
        // load the attacks from the file and initialize the view
        AttacksJSON attacksContent = JsonUtility.FromJson<AttacksJSON>(attacksFileJSON.text);
        foreach (AttackInfo attack in attacksContent.attacks) {
            attacks.Add(attack.id, attack);
        }
        attackView.Init(attacks.Count);
        if (SaveSystem.load) {
            // load the game data of the saved run from the file 
            LoadGameData(SaveSystem.LoadGame());
        } else {
            // load the game data for a new game
            GameConfigJSON gameConfigContent = JsonUtility.FromJson<GameConfigJSON>(gameConfigJSON.text);
            LoadGameConfig(gameConfigContent.gameConfig);
            // setup attacks, statistics and resistances
            foreach (AttackInfo attack in attacks.Values) {
                if (attack.duration == 0) {
                    if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, -1f, 0f, 0f));
                } else {
                    if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, 0f, 0f, 0f));
                }
                attackStats.Add(attack.id, new AttackStats(attack.id, 0, 0, 0));
            }
            ScheduleAttack((int)AttackCode.DOS, attackSchedule.Count);
            DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.DOS].name, ActionCode.CONTINUE);
            ScheduleAttack((int)AttackCode.BRUTE_FORCE, attackSchedule.Count);
            DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.BRUTE_FORCE].name, ActionCode.CONTINUE);
            ScheduleAttack((int)AttackCode.WORM, attackSchedule.Count);
            DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.WORM].name, ActionCode.CONTINUE);
            userLevel = CalculateUserLevel();
            DateTime dt = DateTime.Now.AddMonths(1);
            dateTime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            UpdateAttacks();
        }
        // generate all the objects in the shop
        shop.Load();
        // refresh the GUI for the first time
        gui.Refresh(Math.Round(money).ToString(), Math.Round(users).ToString(), reputation, dateTime);
    }

    /**
     * <summary>Return the money updated</summary>
     */
    float CalculateMoney() {
        return money + (moneyGain[userLevel] + moneyBonus) * (1 - attackMoneyMalus) - moneyMalus;
    }

    /**
     * <summary>Return the number of users updated</summary>
     */
    float CalculateUsers() {
        return users + usersGain[userLevel] * (0.5f * (1 + reputation) * usersMalus * usersBonus - attackUsersMalus) * (float)Math.Round(users);
    }

    /**
     * <summary>Return the current level of users</summary>
     */
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

    /**
     * <summary>Return the reputation updated</summary>
     */
    float CalculateReputation() {
        // increment the reputation every step
        float rep = reputation + 0.0005f;
        if (ongoingAttacks == 0) {
            // increment the time without attacks
            noAttackTime++;
            if (noAttackTime == noAttackStep) {
                // increment the reputation for avoiding attacks
                noAttackTime = 0;
                rep += 0.01f;
            }
        } else {
            // reset the time without attacks
            noAttackTime = 0;
        }
        // normalize the reputation in [0, 1]
        if (rep > 1f) return 1f;
        else return rep;
    }

    /**
     * <summary>Shows the game over</summary>
     */
    void GameOver() {
        Time.timeScale = 0;
        DisplayMessage("GAME OVER", ActionCode.GAME_OVER);
    }

    /**
     * <summary>Checks if the game is over</summary>
     */
    void CheckGameOver() {
        // game over if the time reaches the end
        if (totalTime == endTime) GameOver();
        // game over if the money is negative for too long
        if (negativeTime > maxNegative) GameOver();
        // game over if the reputation reaches 0%
        if (reputation == 0) GameOver();
    }

    // LOG

    /**
     * <summary>Insert a line in the logs</summary>
     */
    public void AddToLogs(LogLine line) {
        logs.Add(line);
    }

    /**
     * <summary>Return the specified line of the log</summary>
     */
    public LogLine GetLog(int i) {
        return logs[i];
    }

    // ATTACKS

    /**
     * <summary>Return the stats of the specified attack</summary>
     */
    public AttackStats GetAttackStats(int id) {
        if (!attackStats.ContainsKey(id)) return null;
        return attackStats[id];
    }

    /**
     * <summary>Return the stats for all the attacks</summary>
     */
    public AttackStats GetAttackStatsTotal() {
        AttackStats stats = new AttackStats(0, 0, 0, 0);
        foreach(AttackStats s in attackStats.Values) {
            stats.n += s.n;
            stats.hit += s.hit;
            stats.miss += s.miss;
        }
        return stats;
    }

    /**
     * <summary>Return the specified attack</summary>
     */
    public AttackInfo GetAttack(int id) {
        if (!attacks.ContainsKey(id)) return null;
        return attacks[id];
    }

    /**
     * <summary>Return the resistance to the specified attack</summary>
     */
    public Resistance GetResistance(int id) {
        if (!resistances.ContainsKey(id)) return null;
        return resistances[id];
    }

    /**
     * <summary>Insert an instance of the specified attack among the scheduled ones</summary>
     */
    void ScheduleAttack(int id, int i) {
        float maxTime = attacks[id].maxTime * GetEndurance(id);
        float nextTime = Random.Range(0.5f * maxTime, maxTime);
        if (i != attackSchedule.Count) attackSchedule[i] = new AttackRecap(id, Mathf.CeilToInt(GetDuration(id)), false, Mathf.CeilToInt(nextTime));
        else attackSchedule.Insert(i, new AttackRecap(id, Mathf.CeilToInt(GetDuration(id)), false, Mathf.CeilToInt(nextTime)));
    }

    /**
     * <summary>Schedule new attacks when the game reaches some checkpoints</summary>
     */
    void ActivateAttacks() {
        switch (totalTime) {
            case 120: // day 5
                ScheduleAttack((int)AttackCode.MITM, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.MITM].name, ActionCode.CONTINUE);
                break;
            case 168: // day 7
                ScheduleAttack((int)AttackCode.VIRUS, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.VIRUS].name, ActionCode.CONTINUE);
                break;
            case 240: // day 10
                ScheduleAttack((int)AttackCode.SOCIAL_ENGINEERING, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.SOCIAL_ENGINEERING].name, ActionCode.CONTINUE);
                break;
            case 288: // day 12
                ScheduleAttack((int)AttackCode.API_VULNERABILITY, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.API_VULNERABILITY].name, ActionCode.CONTINUE);
                break;
            case 360: // day 15
                ScheduleAttack((int)AttackCode.DICTIONARY, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.DICTIONARY].name, ActionCode.CONTINUE);
                break;
            case 408: // day 17
                ScheduleAttack((int)AttackCode.PHISHING, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.PHISHING].name, ActionCode.CONTINUE);
                break;
            case 480: // day 20
                ScheduleAttack((int)AttackCode.SPYWARE, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.SPYWARE].name, ActionCode.CONTINUE);
                break;
            case 528: // day 22
                ScheduleAttack((int)AttackCode.RAINBOW_TABLE, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.RAINBOW_TABLE].name, ActionCode.CONTINUE);
                break;
            case 600: // day 25
                ScheduleAttack((int)AttackCode.RANSOMWARE, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[(int)AttackCode.RANSOMWARE].name, ActionCode.CONTINUE);
                break;
            default:
                break;
        }
    }

    /**
     * <summary>Return the miss ratio for the specified attack</summary>
     */
    float GetMiss(int id) {
        return miss + resistances[id].miss;
    }

    /**
     * <summary>Return the duration for the specified attack</summary>
     */
    float GetDuration(int id) {
        return (1 - resistances[id].duration) * attacks[id].duration;
    }

    /**
     * <summary>Return the endurance against the specified attack</summary>
     */
    float GetEndurance(int id) {
        return endurance + resistances[id].endurance + 0.5f * (1 - reputation);
    }

    /**
     * <summary>Applies the effects of the specified attack</summary>
     */
    void StartAttack(int id) {
        ongoingAttacks++;
        // apply the maluses
        money -= attacks[id].moneyLoss;
        users -= attacks[id].usersLoss * users;
        attackUsersMalus += attacks[id].usersMalus;
        attackMoneyMalus += attacks[id].moneyMalus;
        reputation -= attacks[id].reputationMalus;
        // generate a message
        DisplayMessage("Individuato attacco " + attacks[id].name + "! " + attacks[id].description, ActionCode.CONTINUE);
    }

    /**
     * <summary>Removes the effects of the specified attack</summary>
     */
    void StopAttack(int id) {
        // remove the maluses
        ongoingAttacks--;
        attackUsersMalus -= attacks[id].usersMalus;
        attackMoneyMalus -= attacks[id].moneyMalus;
    }

    /**
     * <summary>Applies the effects of avoiding an attack</summary>
     */
    void MissedAttack(int id) {
        // increment the reputation
        reputation += 0.02f;
        // generate a message
        DisplayMessage("Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name, ActionCode.CONTINUE);
    }

    /**
     * <summary>Updates the scheduled attacks</summary>
     */
    void UpdateAttacks() {
        for(int i = 0; i < attackSchedule.Count; i++) {
            // check the attack status
            if (attackSchedule[i].timer > 0) {
                // decrement the timer
                attackSchedule[i].timer--;
            } else {
                if (!attackSchedule[i].active) {
                    // start the attack
                    attackStats[attackSchedule[i].id].n++;
                    if (Random.Range(0f, 1f) > GetMiss(attackSchedule[i].id)) {
                        // hit
                        attackStats[attackSchedule[i].id].hit++;
                        attackSchedule[i].active = true;
                        StartAttack(attackSchedule[i].id);
                        miss += 0.1f;
                        // log print hit
                        logManager.LogPrintAttack(attacks[attackSchedule[i].id].name, true);
                    } else {
                        // miss
                        attackStats[attackSchedule[i].id].miss++;
                        MissedAttack(attackSchedule[i].id);
                        ScheduleAttack(attackSchedule[i].id, i);
                        miss = 0f;
                        // log print miss
                        logManager.LogPrintAttack(attacks[attackSchedule[i].id].name, false);
                    }
                }
                if (attackSchedule[i].active) {
                    if (attackSchedule[i].duration == 0) {
                        // end the attack
                        StopAttack(attackSchedule[i].id);
                        ScheduleAttack(attackSchedule[i].id, i);
                        attackSchedule[i].timer--;
                    } else {
                        // update the attack
                        attackSchedule[i].duration--;
                    }
                }
            }
        }
    }

    // SHOP

    /**
     * <summary>Inserts an item of the shop in the collection</summary>
     */
    public void AddToShopItems(ShopItemInfo sii) {
        if (shopItems.ContainsKey(sii.id)) shopItems[sii.id] = sii;
        else shopItems.Add(sii.id, sii);
    }

    /**
     * <summary>Return the specified item of the shop</summary>
     */
    public ShopItemInfo GetShopItem(ShopItemCode id) {
        return shopItems[id];
    }

    /**
     * <summary>Return a recap of the status of all the items in the shop</summary>
     */
    ShopItemRecap[] GetShopItemRecap() {
        List<ShopItemRecap> sir = new List<ShopItemRecap>();

        foreach (ShopItemInfo sii in shopItems.Values) {
            sir.Add(new ShopItemRecap(sii.id, sii.owned, sii.on, sii.locked));
        }

        return sir.ToArray();
    }

    /**
     * <summary>Load the basic configuration of the game</summary>
     */
    void LoadGameConfig(GameConfig gc) {
        totalTime = gc.totalTime;
        endTime = gc.endTime;
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
        moneyBonus = gc.moneyBonus;
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

    /**
     * <summary>Load the game data needed to resume a previously saved run</summary>
     */
    void LoadGameData(GameSave gameSave) {
        // load the basic config
        LoadGameConfig(gameSave.gc);
        // update the status of the items in the shop
        foreach (ShopItemRecap s in gameSave.sir) {
            shopItems[s.id].owned = s.owned;
            shopItems[s.id].on = s.on;
            shopItems[s.id].locked = s.locked;
        }
        // load the logs
        logs = new List<LogLine>(gameSave.logs.lines);
        logManager.LoadGameData(gameSave.logs.nLines, gameSave.logs.nPages);
        // load the data structures
        foreach(AttackStats aStat in gameSave.aStats) attackStats.Add(aStat.id, aStat);
        foreach(Resistance res in gameSave.res) resistances.Add(res.id, res);
        attackSchedule = new List<AttackRecap>(gameSave.aSchedule);
    }

    /**
     * <summary>Applies the effects of buying an item in the shop</summary>
     */
    public void Purchase(ShopItemCode id) {
        shopItems[id].owned = true;
        money -= shopItems[id].cost;
        gui.Refresh(Math.Round(money).ToString(), Math.Round(users).ToString(), reputation, dateTime);
    }

    /**
     * <summary>Applies the effects of enabling an item in the shop</summary>
     */
    public void EnableShopItem(ShopItemCode id) {
        shopItems[id].on = true;
        ShopItemInfo sii = shopItems[id];
        if (sii.moneyMalus >= 0) moneyMalus += sii.moneyMalus;
        else moneyBonus -= sii.moneyMalus;
        if (sii.usersMod < 1) usersMalus *= 1 - sii.usersMod;
        else usersBonus *= sii.usersMod;
        // update resistances
        foreach (Resistance r in sii.resistances) {
            if (!resistances.ContainsKey(r.id)) resistances.Add(r.id, new Resistance(r.id, 0f, 0f, 0f));
            resistances[r.id].miss += r.miss;
            resistances[r.id].duration += r.duration;
            resistances[r.id].endurance += r.endurance;
        }
    }

    /**
     * <summary>Applies the effects of disabling an item in the shop</summary>
     */
    public void DisableShopItem(ShopItemCode id) {
        shopItems[id].on = false;
        ShopItemInfo sii = shopItems[id];
        if (sii.moneyMalus >= 0) moneyMalus -= sii.moneyMalus;
        else moneyBonus += sii.moneyMalus;
        if (sii.usersMod < 1) usersMalus /= 1 - sii.usersMod;
        else usersBonus /= sii.usersMod;
        // update resistances
        foreach (Resistance r in sii.resistances) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration -= r.duration;
            resistances[r.id].endurance -= r.endurance;
        }
    }

    /**
     * <summary>Returns true if the specified item of the shop is owned</summary>
     */
    public bool ShopItemIsOwned(ShopItemCode id) {
        return shopItems[id].owned;
    }

    /**
     * <summary>Unlocks the item of the shop</summary>
     */
    public void ShopItemUnlock(ShopItemCode id) {
        shopItems[id].locked = false;
    }

    void DisplayMessage(string message, ActionCode action) {
        GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
        newWindow.transform.SetParent(gameObject.transform, false);
        newWindow.GetComponent<WindowPopUp>().Load(message, action);
    }

    void DebugPrint() {
        Dictionary<int, Resistance> res = new Dictionary<int, Resistance>();
        foreach (ShopItemInfo sii in shopItems.Values) {
            foreach(Resistance r in sii.resistances) {
                if (!res.ContainsKey(r.id)) res.Add(r.id, new Resistance(r.id, 0f, 0f, 0f));
                res[r.id].miss += r.miss;
                res[r.id].duration += r.duration;
                res[r.id].endurance += r.endurance;
            }
        }
        foreach(Resistance r in res.Values) {
            Debug.Log("attack: " + attacks[r.id].name + "\n" +
            "tot duration: " + r.duration + "\n" +
            "tot miss: " + r.miss + "\n" +
            "tot endurance: " + r.endurance + "\n" +
            "max time: " + attacks[r.id].maxTime);
        }
    }
}
