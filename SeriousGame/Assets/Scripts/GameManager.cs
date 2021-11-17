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
    DateTime dateTime;
    GameConfig gc;

    List<LogLine> logs = new List<LogLine>();
    Dictionary<AttackCode, AttackPlan> attackSchedule = new Dictionary<AttackCode, AttackPlan>();
    Dictionary<int, Task> waitingTasks = new Dictionary<int, Task>();
    Dictionary<EmployeeCode, Task> assignedTasks = new Dictionary<EmployeeCode, Task>();
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
            gc.totalTime++;
            dateTime = dateTime.AddHours(1);
            // schedule new attacks
            ActivateAttacks();
            // update tasks
            UpdateTasks();
            // update attacks
            UpdateAttacks();
            // update values
            gc.userLevel = CalculateUserLevel();
            gc.money += GetActualMoneyGain();
            gc.users += GetActualUsersGain();
            gc.reputation = CalculateReputation();
            gc.availableEmployees = CalculateEmployees();
            // refresh
            gui.Refresh(Math.Round(gc.money).ToString(), Math.Round(gc.users).ToString(), gc.reputation, dateTime);
            // game over check
            CheckGameOver();
            // update the stats for possible game over
            if (gc.money < 0) gc.negativeTime++;
            else gc.negativeTime = 0;
        }
    }

    // LOAD-SAVE GAME

    /**
     * <summary>Return the data to be saved to resume correctly the game in future</summary>
     */
    public GameSave SaveGame() {
        gc.date = dateTime.ToString();
        return new GameSave(gc, ShopUtils.GetShopItemRecap(shopItems), EmployeeUtils.GetEmployeeRecap(employees), new LogData(logs.ToArray(),
            logManager.GetNLines(), logManager.GetNPages()), new List<AttackStats>(attackStats.Values).ToArray(), new List<AttackPlan>(attackSchedule.Values).ToArray(),
            new List<Task>(waitingTasks.Values).ToArray(), new List<Task>(assignedTasks.Values).ToArray(), new List<Resistance>(resistances.Values).ToArray());
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
        attackView.Init();
        if (SaveSystem.load) {
            // load the game data of the saved run from the file 
            LoadGameData(SaveSystem.LoadGame());
        } else {
            // load the game data for a new game
            GameConfigJSON gameConfigContent = JsonUtility.FromJson<GameConfigJSON>(gameConfigJSON.text);
            LoadGameConfig(gameConfigContent.gameConfig);
            // setup attacks, statistics and resistances
            AttackUtils.SetupAll(attacks, resistances, attackStats, attackSchedule);
            gc.userLevel = CalculateUserLevel();
            DateTime dt = DateTime.Now.AddMonths(1);
            dateTime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            UpdateAttacks();
        }
        // generate all the objects in the shop
        shop.Load();
        // refresh the GUI for the first time
        gui.Refresh(Math.Round(gc.money).ToString(), Math.Round(gc.users).ToString(), gc.reputation, dateTime);
    }

    /**
     * <summary>Load the basic configuration of the game</summary>
     */
    void LoadGameConfig(GameConfig gc) {
        this.gc = gc;
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
        AttackUtils.UpdateAll(resistances, attackStats, attackSchedule, gameSave.res, gameSave.aStats, gameSave.aSchedule);
        TaskUtils.UpdateTasks(waitingTasks, gameSave.waitingTasks, assignedTasks, gameSave.assignedTasks);
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

    public List<AttackInfo> GetAttacks() {
        return new List<AttackInfo>(attacks.Values);
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
    void ScheduleAttack(AttackCode id) {
        float maxTime = attacks[id].maxTime * GetAttackEndurance(id);
        float nextTime = Random.Range(0.5f * maxTime, maxTime);
        attackSchedule[id] = new AttackPlan(id, AttackStatus.PLANNING, Mathf.CeilToInt(nextTime));
    }

    /**
     * <summary>Schedule new attacks when the game reaches some checkpoints</summary>
     */
    void ActivateAttacks() {
        switch (gc.totalTime) {
            case 48: // day 2
                ScheduleAttack(AttackCode.DOS);
                ScheduleAttack(AttackCode.BRUTE_FORCE);
                ScheduleAttack(AttackCode.WORM);
                DisplayMessage("Nuovi attacchi: " + attacks[AttackCode.DOS].name + ", " + attacks[AttackCode.BRUTE_FORCE].name + ", " + attacks[AttackCode.WORM].name, ActionCode.CONTINUE);
                break;
            case 120: // day 5
                ScheduleAttack(AttackCode.MITM);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.MITM].name, ActionCode.CONTINUE);
                break;
            case 168: // day 7
                ScheduleAttack(AttackCode.VIRUS);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.VIRUS].name, ActionCode.CONTINUE);
                break;
            case 240: // day 10
                ScheduleAttack(AttackCode.SOCIAL_ENGINEERING);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SOCIAL_ENGINEERING].name, ActionCode.CONTINUE);
                break;
            case 288: // day 12
                ScheduleAttack(AttackCode.API_VULNERABILITY);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.API_VULNERABILITY].name, ActionCode.CONTINUE);
                break;
            case 360: // day 15
                ScheduleAttack(AttackCode.DICTIONARY);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DICTIONARY].name, ActionCode.CONTINUE);
                break;
            case 408: // day 17
                ScheduleAttack(AttackCode.PHISHING);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.PHISHING].name, ActionCode.CONTINUE);
                break;
            case 480: // day 20
                ScheduleAttack(AttackCode.SPYWARE);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SPYWARE].name, ActionCode.CONTINUE);
                break;
            case 528: // day 22
                ScheduleAttack(AttackCode.RAINBOW_TABLE);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RAINBOW_TABLE].name, ActionCode.CONTINUE);
                break;
            case 600: // day 25
                ScheduleAttack(AttackCode.RANSOMWARE);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RANSOMWARE].name, ActionCode.CONTINUE);
                break;
            default:
                break;
        }
    }

    /**
     * <summary>Return the miss ratio for the specified attack</summary>
     */
    float GetAttackMiss(AttackCode id) {
        return gc.miss + resistances[id].miss;
    }

    /**
     * <summary>Return the duration for the specified attack</summary>
     */
    public int GetAttackDuration(EmployeeCode id, AttackCode aid) {
        return Mathf.CeilToInt((1 - resistances[aid].duration) * attacks[aid].duration * (1 - 0.18f * (float)(EmployeeUtils.GetAbilities(employees[id].abilities)[attacks[aid].category] - 5)));
    }

    /**
     * <summary>Return the endurance against the specified attack</summary>
     */
    float GetAttackEndurance(AttackCode id) {
        return gc.endurance + resistances[id].endurance + 0.5f * (1 - gc.reputation);
    }

    /**
     * <summary>Applies the effects of the specified attack</summary>
     */
    void StartAttack(AttackCode id) {
        gc.ongoingAttacks++;
        // apply the maluses
        gc.money -= attacks[id].moneyLoss;
        gc.users -= attacks[id].usersLoss * gc.users;
        gc.reputation -= attacks[id].reputationMalus;
        // generate a message
        DisplayMessage("Individuato attacco " + attacks[id].name + "! " + attacks[id].description, ActionCode.CONTINUE);
    }

    /**
     * <summary>Removes the effects of the specified attack</summary>
     */
    void StopAttack(AttackCode id) {
        // remove the maluses
        gc.ongoingAttacks--;
    }

    /**
     * <summary>Applies the effects of avoiding an attack</summary>
     */
    void MissedAttack(AttackCode id) {
        // increment the reputation
        gc.reputation += 0.02f;
        // generate a message
        DisplayMessage("Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name, ActionCode.CONTINUE);
    }

    /**
     * <summary>Updates the scheduled attacks</summary>
     */
    void UpdateAttacks() {
        foreach (AttackPlan attack in attackSchedule.Values) {
            if (attack.status == AttackStatus.PLANNING) {
                if (attack.timer > 0) {
                    // decrement the timer
                    attack.timer--;
                } else {
                    // start the attack
                    attackStats[attack.id].n++;
                    if (Random.Range(0f, 1f) > GetAttackMiss(attack.id)) {
                        // hit
                        attack.status = AttackStatus.ACTIVE;
                        attackStats[attack.id].hit++;
                        StartAttack(attack.id);
                        gc.miss += 0.1f;
                        // log print hit
                        logManager.LogPrintAttack(attacks[attack.id].name, true);
                        Task newTask = new Task(TaskType.REPAIR, attack.id);
                        waitingTasks.Add(newTask.id, newTask);
                    } else {
                        // miss
                        attackStats[attack.id].miss++;
                        MissedAttack(attack.id);
                        ScheduleAttack(attack.id);
                        gc.miss = 0f;
                        // log print miss
                        logManager.LogPrintAttack(attacks[attack.id].name, false);
                    }
                }
            }
        }
    }

    // TASK
    
    public List<Task> GetTasksByType(TaskType type) {
        List<Task> res = new List<Task>();
        foreach(Task t in waitingTasks.Values) {
            if (t.type == type) res.Add(t);
        }
        return res;
    }

    void UpdateTasks() {
        foreach (Task t in assignedTasks.Values) {
            if (t.progress == t.duration) {
                // end task
                EndTask(t);
            } else {
                t.progress++;
            }
        }
    }

    void EndTask(Task t) {
        switch (t.type) {
            case TaskType.INSTALL:
                employees[t.executor].status = TaskType.NONE;
                EnableShopItem(t.shopItem);
                waitingTasks.Remove(t.id);
                break;
            case TaskType.REPAIR:
                // end the attack
                employees[t.executor].status = TaskType.NONE;
                StopAttack(t.attack);
                ScheduleAttack(t.attack);
                attackSchedule[t.attack].timer--;
                waitingTasks.Remove(t.id);
                break;
            default:
                Debug.Log("Error: undefined taskType");
                break;
        }
    }

    public int GetInstallDuration(EmployeeCode id, ShopItemCode sid) {
        return Mathf.CeilToInt(shopItems[sid].upgradeTime * (1 - 0.18f * (float)(EmployeeUtils.GetAbilities(employees[id].abilities)[shopItems[sid].category] - 5)));
    }

    public float GetTaskProgress(ShopItemCode id) {
        foreach(Task t in waitingTasks.Values) {
            if (t.shopItem == id) return (float)t.progress / (t.duration + 1);
        }
        Debug.Log("Error: no task with the given ShopItemCode");
        return 0f;
    }

    public float GetTaskProgress(EmployeeCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.executor == id) return (float)t.progress / (t.duration + 1);
        }
        Debug.Log("Error: no task assigned to the given employee");
        return 0f;
    }

    public int GetTaskTarget(EmployeeCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.executor == id) {
                switch (t.type) {
                    case TaskType.INSTALL:
                        return (int)t.shopItem;
                    case TaskType.REPAIR:
                        return (int)t.attack;
                }
            }
        }
        Debug.Log("Error: no task assigned to the given employee");
        return 0;
    }

    public Task GetRepairTask(AttackCode id) {
        foreach(Task t in waitingTasks.Values) {
            if(t.type==TaskType.REPAIR && t.attack == id) {
                return t;
            }
        }
        return null;
    }

    public Task GetInstallTask(ShopItemCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.INSTALL && t.shopItem == id) {
                return t;
            }
        }
        return null;
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
        // print in the log
        logManager.LogPrintItem(shopItems[id].name, ActionCode.PURCHASE_ITEM);
        shopItems[id].status = ShopItemStatus.NOT_INSTALLED;
        gc.money -= shopItems[id].cost;
        Task t = new Task(TaskType.INSTALL, id);
        waitingTasks.Add(t.id, t);
        gui.Refresh(Math.Round(gc.money).ToString(), Math.Round(gc.users).ToString(), gc.reputation, dateTime);
    }

    /**
     * <summary>Applies the effects of enabling an item in the shop</summary>
     */
    public void EnableShopItem(ShopItemCode id) {
        // print in the log
        logManager.LogPrintItem(shopItems[id].name, ActionCode.ENABLE_ITEM);
        shopItems[id].status = ShopItemStatus.ACTIVE;
        // update resistances
        foreach (Resistance r in shopItems[id].resistances) {
            if (!resistances.ContainsKey(r.id)) resistances.Add(r.id, new Resistance(r.id, 0, 0f, 0f));
            resistances[r.id].miss += r.miss;
            resistances[r.id].duration += r.duration;
            resistances[r.id].endurance += r.endurance;
        }
    }

    /**
     * <summary>Applies the effects of disabling an item in the shop</summary>
     */
    public void DisableShopItem(ShopItemCode id) {
        // print in the log
        logManager.LogPrintItem(shopItems[id].name, ActionCode.DISABLE_ITEM);
        shopItems[id].status = ShopItemStatus.INACTIVE;
        // update resistances
        foreach (Resistance r in shopItems[id].resistances) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration -= r.duration;
            resistances[r.id].endurance -= r.endurance;
        }
    }

    /**
     * <summary>Returns true if the specified item of the shop is owned</summary>
     */
    public bool ShopItemIsOwned(ShopItemCode id) {
        return shopItems[id].status != ShopItemStatus.NOT_OWNED;
    }

    /**
     * <summary>Unlocks the item of the shop</summary>
     */
    public void ShopItemUnlock(ShopItemCode id) {
        shopItems[id].locked = false;
    }

    /**
     * <summary>Returns true if the specified item of the shop is installed</summary>
     */
    public bool ShopItemIsInstalled(ShopItemCode id) {
        return ShopItemIsOwned(id) && shopItems[id].status != ShopItemStatus.INSTALLING;
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
        gc.hiredEmployees++;
    }

    public bool CheckEmployeeAvailability() {
        return EmployeeUtils.CheckEmployeeAvailability(employees);
    }

    public List<EmployeeInfo> GetAvailableEmployees() {
        return EmployeeUtils.GetAvailableEmployees(employees);
    }

    public void AssignEmployee(EmployeeCode id, int tid) {
        Task t = waitingTasks[tid];
        employees[id].status = t.type;
        switch (t.type) {
            case TaskType.INSTALL:
                shopItems[t.shopItem].status = ShopItemStatus.INSTALLING;
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetInstallDuration(id, t.shopItem));
                waitingTasks.Remove(tid);
                break;
            case TaskType.REPAIR:
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetAttackDuration(id, t.attack));
                waitingTasks.Remove(tid);
                break;
            default:
                Debug.Log("Error: undefined taskType");
                break;
        }
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
        float moneyBonus = 0f;
        float moneyMalus = 0f;
        float attackMoneyMalus = 0f;
        // the revenue from the employees work
        foreach (EmployeeInfo e in employees.Values) {
            if (e.owned) {
                switch (e.status) {
                    case TaskType.NONE:
                        moneyBonus += e.moneyGain;
                        break;
                    case TaskType.INSTALL:
                        moneyBonus += 0.5f * e.moneyGain;
                        break;
                    case TaskType.REPAIR:
                        break;
                    default:
                        Debug.Log("Error: undefined employeeStatus");
                        break;
                }
            }
        }
        // the costs for the active items
        foreach(ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE) {
                if (sii.moneyMalus < 0) moneyBonus -= sii.moneyMalus;
                else moneyMalus += sii.moneyMalus;
            }
        }
        // the malus for the active attacks
        foreach(AttackPlan a in attackSchedule.Values) {
            if (a.status == AttackStatus.ACTIVE) attackMoneyMalus += attacks[a.id].moneyMalus;
        }
        return moneyBonus * (1 - attackMoneyMalus) - moneyMalus;
    }

    /**
     * <summary>Return the gain of money based on the number of users</summary>
     */
    public float GetMoneyGain() {
        float moneyBonus = 0f;
        // the revenue from the employees work
        foreach (EmployeeInfo e in employees.Values) {
            if (e.owned) {
                switch (e.status) {
                    case TaskType.NONE:
                        moneyBonus += e.moneyGain;
                        break;
                    case TaskType.INSTALL:
                        moneyBonus += 0.5f * e.moneyGain;
                        break;
                    case TaskType.REPAIR:
                        break;
                    default:
                        Debug.Log("Error: undefined employeeStatus");
                        break;
                }
            }
        }
        // the gain from the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE && sii.moneyMalus < 0) moneyBonus -= sii.moneyMalus;
        }
        return moneyBonus;
    }

    /**
     * <summary>Return the malus to money caused by the active defences and services</summary>
     */
    public float GetMoneyMalus() {
        float moneyMalus = 0f;
        // the costs for the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE && sii.moneyMalus > 0) {
                moneyMalus += sii.moneyMalus;
            }
        }
        return moneyMalus;
    }

    /**
     * <summary>Return the malus to money caused by the active attacks</summary>
     */
    public float GetAttackMoneyMalus() {
        float attackMoneyMalus = 0f;
        // the malus for the active attacks
        foreach (AttackPlan a in attackSchedule.Values) {
            if (a.status == AttackStatus.ACTIVE) attackMoneyMalus += attacks[a.id].moneyMalus;
        }
        return (float)Math.Round(attackMoneyMalus, 2);
    }

    /**
     * <summary>Return the actual gain of users</summary>
     */
    public float GetActualUsersGain() {
        float usersMalus = 1f;
        float usersBonus = 1f;
        float attackUsersMalus = 0f;
        // the user modifier for the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE) {
                if (sii.usersMod < 1) usersMalus *= 1 - sii.usersMod;
                else usersBonus *= sii.usersMod;
            }
        }
        // the malus for the active attacks
        foreach (AttackPlan a in attackSchedule.Values) {
            if (a.status == AttackStatus.ACTIVE) attackUsersMalus += attacks[a.id].usersMalus;
        }
        return (float)Math.Round(gc.usersGain[gc.userLevel] * (0.5f * (1 + gc.reputation) * usersMalus * usersBonus - attackUsersMalus) * Math.Round(gc.users));
    }

    /**
     * <summary>Return the gain of users without malus and bonus</summary>
     */
    public float GetUsersGain() {
        return (float)Math.Round(gc.usersGain[gc.userLevel] * Math.Round(gc.users) * 0.5f * (1 + gc.reputation));
    }

    /**
     * <summary>Return the modifier of the gain of users caused by defences and services</summary>
     */
    public float GetUsersMod() {
        float usersMalus = 1f;
        float usersBonus = 1f;
        // the user modifier for the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE) {
                if (sii.usersMod < 1) usersMalus *= 1 - sii.usersMod;
                else usersBonus *= sii.usersMod;
            }
        }
        return usersMalus * usersBonus;
    }

    /**
     * <summary>Return the malus to users caused by the active attacks</summary>
     */
    public float GetAttackUsersMalus() {
        float attackUsersMalus = 0f;
        // the malus for the active attacks
        foreach (AttackPlan a in attackSchedule.Values) {
            if (a.status == AttackStatus.ACTIVE) attackUsersMalus += attacks[a.id].usersMalus;
        }
        return (float)Math.Round(gc.usersGain[gc.userLevel] * Math.Round(gc.users) * Math.Round(attackUsersMalus, 2));
    }

    public int GetTotalEmployeesN() {
        return gc.availableEmployees;
    }

    public int GetHiredEmployeesN() {
        return gc.hiredEmployees;
    }

    public List<EmployeeInfo> GetHiredEmployees() {
        return EmployeeUtils.GetHiredEmployees(employees);
    }

    /**
     * <summary>Return the current level of users</summary>
     */
    int CalculateUserLevel() {
        if (gc.userLevel < gc.usersGoals.Length && gc.users > gc.usersGoals[gc.userLevel]) return gc.userLevel + 1;
        if (gc.userLevel > 0 && gc.users < gc.usersGoals[gc.userLevel - 1]) return gc.userLevel - 1;
        return gc.userLevel;
    }

    /**
     * <summary>Return the reputation updated</summary>
     */
    float CalculateReputation() {
        // increment the reputation every step
        float rep = gc.reputation + 0.0005f;
        if (gc.ongoingAttacks == 0) {
            // increment the time without attacks
            gc.noAttackTime++;
            if (gc.noAttackTime == gc.noAttackStep) {
                // increment the reputation for avoiding attacks
                gc.noAttackTime = 0;
                rep += 0.01f;
            }
        } else {
            // reset the time without attacks
            gc.noAttackTime = 0;
        }
        // normalize the reputation in [0, 1]
        if (rep > 1f) return 1f;
        else return rep;
    }

    int CalculateEmployees() {
        if (gc.availableEmployees < gc.employeeGoals.Length + gc.initEmployees && gc.users >= gc.employeeGoals[gc.availableEmployees - gc.initEmployees]) {
            DisplayMessage("Hai raggiunto " + gc.employeeGoals[gc.availableEmployees - gc.initEmployees] + " utenti! Ora puoi assumere un nuovo dipendente", ActionCode.CONTINUE);
            return gc.availableEmployees + 1;
        }
        return gc.availableEmployees;
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
        if (gc.totalTime == gc.endTime) GameOver();
        // game over if the money is negative for too long
        if (gc.negativeTime > gc.maxNegative) GameOver();
        // game over if the reputation reaches 0%
        if (gc.reputation == 0) GameOver();
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
                if (!res.ContainsKey(r.id)) res.Add(r.id, new Resistance(r.id, 0, 0f, 0f));
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
