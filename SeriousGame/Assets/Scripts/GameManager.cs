using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;
using Math = System.Math;

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
    int totalEmployees;
    int hiredEmployees;
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
    float[] usersGoals;
    float[] employeeGoals;
    DateTime dateTime;

    List<LogLine> logs = new List<LogLine>();
    List<AttackRecap> attackSchedule = new List<AttackRecap>();
    Dictionary<AttackCode, AttackInfo> attacks = new Dictionary<AttackCode, AttackInfo>();
    Dictionary<AttackCode, Resistance> resistances = new Dictionary<AttackCode, Resistance>();
    Dictionary<AttackCode, AttackStats> attackStats = new Dictionary<AttackCode, AttackStats>();
    Dictionary<ShopItemCode, ShopItemInfo> shopItems = new Dictionary<ShopItemCode, ShopItemInfo>();
    Dictionary<EmployeeCode, EmployeeInfo> employees = new Dictionary<EmployeeCode, EmployeeInfo>();

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
            totalEmployees = CalculateEmployees();
            // refresh
            gui.Refresh(Math.Round(money).ToString(), Math.Round(users).ToString(), reputation, dateTime);
            // game over check
            CheckGameOver();
            // update the stats for possible game over
            if (money < 0) negativeTime++;
            else negativeTime = 0;
        }
    }

    // LOAD-SAVE GAME

    /**
     * <summary>Return the data to be saved to resume correctly the game in future</summary>
     */
    public GameSave SaveGame() {
        return new GameSave(new GameConfig(totalTime, endTime, negativeTime, maxNegative, noAttackTime, noAttackStep, ongoingAttacks, userLevel,
            totalEmployees, hiredEmployees, money, users, reputation, moneyMalus, moneyBonus, usersMalus, usersBonus, attackUsersMalus,
            attackMoneyMalus, endurance, miss, usersGain, usersGoals, employeeGoals, dateTime.ToString()), ShopUtils.GetShopItemRecap(shopItems),
            EmployeeUtils.GetEmployeeRecap(employees), new LogData(logs.ToArray(), logManager.GetNLines(), logManager.GetNPages()),
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
        attacks = AttackUtils.LoadFromFile(attacksFileJSON);
        attackView.Init(attacks.Count);
        if (SaveSystem.load) {
            // load the game data of the saved run from the file 
            LoadGameData(SaveSystem.LoadGame());
        } else {
            // load the game data for a new game
            GameConfigJSON gameConfigContent = JsonUtility.FromJson<GameConfigJSON>(gameConfigJSON.text);
            LoadGameConfig(gameConfigContent.gameConfig);
            // setup attacks, statistics and resistances
            AttackUtils.SetupAll(attacks, resistances, attackStats);
            ScheduleAttack(AttackCode.DOS, attackSchedule.Count);
            DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DOS].name, ActionCode.CONTINUE);
            ScheduleAttack(AttackCode.BRUTE_FORCE, attackSchedule.Count);
            DisplayMessage("Nuovo attacco: " + attacks[AttackCode.BRUTE_FORCE].name, ActionCode.CONTINUE);
            ScheduleAttack(AttackCode.WORM, attackSchedule.Count);
            DisplayMessage("Nuovo attacco: " + attacks[AttackCode.WORM].name, ActionCode.CONTINUE);
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
        totalEmployees = gc.totalEmployees;
        hiredEmployees = gc.hiredEmployees;
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
        usersGoals = gc.usersGoals;
        employeeGoals = gc.employeeGoals;
        dateTime = DateTime.Parse(gc.date);
    }

    /**
     * <summary>Load the game data needed to resume a previously saved run</summary>
     */
    void LoadGameData(GameSave gameSave) {
        // load the basic config
        LoadGameConfig(gameSave.gc);
        // update the status of the items in the shop
        ShopUtils.UpdateShopItems(shopItems, gameSave.sir);
        EmployeeUtils.UpdateEmployees(employees, gameSave.er);
        // load the logs
        logs = new List<LogLine>(gameSave.logs.lines);
        logManager.LoadGameData(gameSave.logs.nLines, gameSave.logs.nPages);
        // load the data structures
        AttackUtils.UpdateAll(resistances, attackStats, gameSave.res, gameSave.aStats);
        attackSchedule = new List<AttackRecap>(gameSave.aSchedule);
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
    public AttackStats GetAttackStats(AttackCode id) {
        if (!attackStats.ContainsKey(id)) return null;
        return attackStats[id];
    }

    /**
     * <summary>Return the stats for all the attacks</summary>
     */
    public AttackStats GetAttackStatsTotal() {
        AttackStats stats = new AttackStats(0, 0, 0, 0);
        foreach (AttackStats s in attackStats.Values) {
            stats.n += s.n;
            stats.hit += s.hit;
            stats.miss += s.miss;
        }
        return stats;
    }

    /**
     * <summary>Return the specified attack</summary>
     */
    public AttackInfo GetAttack(AttackCode id) {
        if (!attacks.ContainsKey(id)) return null;
        return attacks[id];
    }

    /**
     * <summary>Return the resistance to the specified attack</summary>
     */
    public Resistance GetResistance(AttackCode id) {
        if (!resistances.ContainsKey(id)) return null;
        return resistances[id];
    }

    /**
     * <summary>Insert an instance of the specified attack among the scheduled ones</summary>
     */
    void ScheduleAttack(AttackCode id, int i) {
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
                ScheduleAttack(AttackCode.MITM, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.MITM].name, ActionCode.CONTINUE);
                break;
            case 168: // day 7
                ScheduleAttack(AttackCode.VIRUS, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.VIRUS].name, ActionCode.CONTINUE);
                break;
            case 240: // day 10
                ScheduleAttack(AttackCode.SOCIAL_ENGINEERING, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SOCIAL_ENGINEERING].name, ActionCode.CONTINUE);
                break;
            case 288: // day 12
                ScheduleAttack(AttackCode.API_VULNERABILITY, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.API_VULNERABILITY].name, ActionCode.CONTINUE);
                break;
            case 360: // day 15
                ScheduleAttack(AttackCode.DICTIONARY, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DICTIONARY].name, ActionCode.CONTINUE);
                break;
            case 408: // day 17
                ScheduleAttack(AttackCode.PHISHING, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.PHISHING].name, ActionCode.CONTINUE);
                break;
            case 480: // day 20
                ScheduleAttack(AttackCode.SPYWARE, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SPYWARE].name, ActionCode.CONTINUE);
                break;
            case 528: // day 22
                ScheduleAttack(AttackCode.RAINBOW_TABLE, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RAINBOW_TABLE].name, ActionCode.CONTINUE);
                break;
            case 600: // day 25
                ScheduleAttack(AttackCode.RANSOMWARE, attackSchedule.Count);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RANSOMWARE].name, ActionCode.CONTINUE);
                break;
            default:
                break;
        }
    }

    /**
     * <summary>Return the miss ratio for the specified attack</summary>
     */
    float GetMiss(AttackCode id) {
        return miss + resistances[id].miss;
    }

    /**
     * <summary>Return the duration for the specified attack</summary>
     */
    float GetDuration(AttackCode id) {
        return (1 - resistances[id].duration) * attacks[id].duration;
    }

    /**
     * <summary>Return the endurance against the specified attack</summary>
     */
    float GetEndurance(AttackCode id) {
        return endurance + resistances[id].endurance + 0.5f * (1 - reputation);
    }

    /**
     * <summary>Applies the effects of the specified attack</summary>
     */
    void StartAttack(AttackCode id) {
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
    void StopAttack(AttackCode id) {
        // remove the maluses
        ongoingAttacks--;
        attackUsersMalus -= attacks[id].usersMalus;
        attackMoneyMalus -= attacks[id].moneyMalus;
    }

    /**
     * <summary>Applies the effects of avoiding an attack</summary>
     */
    void MissedAttack(AttackCode id) {
        // increment the reputation
        reputation += 0.02f;
        // generate a message
        DisplayMessage("Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name, ActionCode.CONTINUE);
    }

    /**
     * <summary>Updates the scheduled attacks</summary>
     */
    void UpdateAttacks() {
        for (int i = 0; i < attackSchedule.Count; i++) {
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
     * <summary>Applies the effects of buying an item in the shop</summary>
     */
    public void PurchaseShopItem(ShopItemCode id) {
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

    // EMPLOYEES

    public void AddToEmployees(EmployeeInfo e) {
        employees.Add(e.id, e);
    }

    public EmployeeInfo GetEmployee(EmployeeCode id) {
        return employees[id];
    }

    /**
     * <summary>Applies the effects of hiring an employee</summary>
     */
    public void HireEmployee(EmployeeCode id) {
        employees[id].owned = true;
        moneyBonus += employees[id].moneyGain;
        hiredEmployees++;
    }

    // MISC

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
        return moneyBonus * (1 - attackMoneyMalus) - moneyMalus;
    }

    /**
     * <summary>Return the gain of money based on the number of users</summary>
     */
    public float GetMoneyGain() {
        return moneyBonus;
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

    public int GetTotalEmployees() {
        return totalEmployees;
    }

    public int GetHiredEmployees() {
        return hiredEmployees;
    }

    /**
     * <summary>Return the money updated</summary>
     */
    float CalculateMoney() {
        return money + moneyBonus * (1 - attackMoneyMalus) - moneyMalus;
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
        if (userLevel < usersGoals.Length && users > usersGoals[userLevel]) return userLevel + 1;
        if (userLevel > 0 && users < usersGoals[userLevel - 1]) return userLevel - 1;
        return userLevel;
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

    int CalculateEmployees() {
        if (totalEmployees < employeeGoals.Length && employeeGoals[totalEmployees - 2] >= users) {
            DisplayMessage("Hai raggiunto " + employeeGoals[totalEmployees] + " utenti! Ora puoi assumere un nuovo dipendente", ActionCode.CONTINUE);
            return totalEmployees + 1;
        }
        return totalEmployees;
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

    /**
     * <summary>Creates a pop-up window message</summary>
     */
    void DisplayMessage(string message, ActionCode action) {
        GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
        newWindow.transform.SetParent(gameObject.transform, false);
        newWindow.GetComponent<WindowPopUp>().Load(message, action);
    }

    void DebugPrint() {
        Dictionary<AttackCode, Resistance> res = new Dictionary<AttackCode, Resistance>();
        foreach (ShopItemInfo sii in shopItems.Values) {
            foreach (Resistance r in sii.resistances) {
                if (!res.ContainsKey(r.id)) res.Add(r.id, new Resistance(r.id, 0f, 0f, 0f));
                res[r.id].miss += r.miss;
                res[r.id].duration += r.duration;
                res[r.id].endurance += r.endurance;
            }
        }
        foreach (Resistance r in res.Values) {
            Debug.Log("attack: " + attacks[r.id].name + "\n" +
            "tot duration: " + r.duration + "\n" +
            "tot miss: " + r.miss + "\n" +
            "tot endurance: " + r.endurance + "\n" +
            "max time: " + attacks[r.id].maxTime);
        }
    }
}
