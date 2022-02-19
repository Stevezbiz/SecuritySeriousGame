using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;
using Math = System.Math;
using Image = UnityEngine.UI.Image;

public class GameManager : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] Shop shop;
    [SerializeField] Log logManager;
    [SerializeField] SecurityView securityView;
    [SerializeField] Guide guide;
    [SerializeField] QuizQuestion quizQuestion;
    [SerializeField] LearningReport learningReport;
    [SerializeField] GameObject message;
    [SerializeField] TextAsset gameConfigJSON;
    [SerializeField] TextAsset attacksFileJSON;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] TextAsset quizzesFileJSON;
    [SerializeField] TextAsset employeesFileJSON;
    [SerializeField] TextAsset modelFileJSON;
    [SerializeField] List<Sprite> avatarImages;
    [SerializeField] List<Sprite> categoryImages;

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
    Dictionary<SkillCode, KnowledgeComponent> kcs = new Dictionary<SkillCode, KnowledgeComponent>();
    Dictionary<int, Quiz> quizzes = new Dictionary<int, Quiz>();
    Dictionary<Role, Person> avatars = new Dictionary<Role, Person>();
    Dictionary<Category, Sprite> categories = new Dictionary<Category, Sprite>();

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
            // update model
            BKTModel.UpdateModel(gc.totalTime);
            // game over check
            CheckGameOver();
            // update resistance aging
            UpdateResistanceAging();
            // schedule new attacks
            ActivateAttacks();
            // trigger periodic evaluation
            if (gc.totalTime % gc.evaluationTime == 0) EvaluateSecurityStatus();
            // update quiz timer
            UpdateQuiz();
            // update tasks
            UpdateTasks();
            // update attacks
            UpdateAttacks();
            // update values
            gc.userLevel = CalculateUserLevel();
            gc.money += GetActualMoneyGain();
            gc.users += GetActualUsersGain();
            gc.reputation = CalculateReputation();
            UpdateEmployees();
            // refresh
            gui.Refresh(gc.money, gc.users, gc.reputation, dateTime);
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
        // initialize the shop structure
        shopItems = ShopUtils.LoadFromFile(shopFileJSON);
        shop.Init();
        // load the attacks from the file and initialize the view
        attacks = AttackUtils.LoadFromFile(attacksFileJSON);
        securityView.Init();
        // load the guide structure
        guide.Init();
        // load the quizzes from file
        quizzes = QuizUtils.LoadFromFile(quizzesFileJSON);
        // load the employees from file
        employees = EmployeeUtils.LoadFromFile(employeesFileJSON);
        // load the avatars
        for (int i = 0; i < avatarImages.Count; i++) {
            avatars.Add((Role)i, new Person(avatarImages[i].name, avatarImages[i]));
        }
        // load the category images
        for (int i = 0; i < categoryImages.Count; i++) {
            categories.Add((Category)i, categoryImages[i]);
        }
        if (SaveSystem.load) {
            // load the game data of the saved run from the file 
            LoadGameData(SaveSystem.LoadGame());
            // load the model data saved on file
            kcs = BKTModel.LoadModel();
        } else {
            // load the game data for a new game
            GameConfigJSON gameConfigContent = JsonUtility.FromJson<GameConfigJSON>(gameConfigJSON.text);
            LoadGameConfig(gameConfigContent.gameConfig);
            // setup attacks, statistics and resistances
            AttackUtils.SetupAll(attacks, resistances, attackStats, attackSchedule);
            // setup tasks
            TaskUtils.SetupTasks(waitingTasks);
            gc.userLevel = CalculateUserLevel();
            DateTime dt = DateTime.Now.AddMonths(1);
            dateTime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            UpdateAttacks();
            // initialize the BKT model
            kcs = BKTModel.Init(modelFileJSON);
        }
        // initialize the report structure
        learningReport.Init(kcs);
        // refresh the GUI for the first time
        gui.Refresh(gc.money, gc.users, gc.reputation, dateTime);
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

    float GetActualDurationResistance(AttackCode id) {
        float val = (1 - resistances[id].duration) * gc.duration[gc.actualResistanceMod];
        if (val < 0.99f) return val;
        else return 0.99f;
    }

    float GetActualMissResistance(AttackCode id) {
        return resistances[id].miss * gc.miss[gc.actualResistanceMod];
    }

    float GetActualEnduranceResistance(AttackCode id) {
        return resistances[id].endurance * gc.endurance[gc.actualResistanceMod];
    }

    public float GetActualDurationResistance(float duration) {
        float val = duration * gc.duration[gc.actualResistanceMod];
        if (val < 0.99f) return val;
        else return 0.99f;
    }

    public float GetActualMissResistance(float miss) {
        return miss * gc.miss[gc.actualResistanceMod];
    }

    public float GetActualEnduranceResistance(float endurance) {
        return endurance * gc.endurance[gc.actualResistanceMod];
    }

    /**
     * <summary>Return the resistance to the specified attack</summary>
     */
    public Resistance GetResistance(AttackCode id) {
        if (!resistances.ContainsKey(id)) return null;
        float duration = GetActualDurationResistance(id);
        float miss = GetActualMissResistance(id);
        float endurance = GetActualEnduranceResistance(id);
        return new Resistance(id, duration, miss, endurance);
    }

    /**
     * <summary>Insert an instance of the specified attack among the scheduled ones</summary>
     */
    void ScheduleAttack(AttackCode id) {
        float maxTime = attacks[id].maxTime * GetAttackEndurance(id);
        float nextTime = Random.Range(0.5f * maxTime, maxTime);
        if (gc.actualAttackTrend == id) nextTime /= 2;
        attackSchedule[id] = new AttackPlan(attackSchedule[id], Mathf.CeilToInt(nextTime));
    }

    /**
     * <summary>Schedule new attacks when the game reaches some checkpoints</summary>
     */
    void ActivateAttacks() {
        // all the attacks are introduced at the end of the indicated day 
        switch (gc.totalTime) {
            case 48: // day 2
                ScheduleAttack(AttackCode.MITM);
                ScheduleAttack(AttackCode.BRUTE_FORCE);
                ScheduleAttack(AttackCode.WORM);
                DisplayMessage("Nuovi attacchi: " + attacks[AttackCode.MITM].name + ", " + attacks[AttackCode.BRUTE_FORCE].name + ", " + attacks[AttackCode.WORM].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 120: // day 5
                ScheduleAttack(AttackCode.DOS);
                SetAttackTrend();
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DOS].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 168: // day 7
                ScheduleAttack(AttackCode.VIRUS);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.VIRUS].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 240: // day 10
                ScheduleAttack(AttackCode.SOCIAL_ENGINEERING);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SOCIAL_ENGINEERING].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 288: // day 12
                ScheduleAttack(AttackCode.API_VULNERABILITY);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.API_VULNERABILITY].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 360: // day 15
                ScheduleAttack(AttackCode.DICTIONARY);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DICTIONARY].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 408: // day 17
                ScheduleAttack(AttackCode.PHISHING);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.PHISHING].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 480: // day 20
                ScheduleAttack(AttackCode.SPYWARE);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SPYWARE].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 528: // day 22
                ScheduleAttack(AttackCode.RAINBOW_TABLE);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RAINBOW_TABLE].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 600: // day 25
                ScheduleAttack(AttackCode.RANSOMWARE);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RANSOMWARE].name, ActionCode.CONTINUE, Role.SECURITY);
                break;
            default:
                break;
        }
    }

    /**
     * <summary>Return the miss ratio for the specified attack</summary>
     */
    float GetAttackMiss(AttackCode id) {
        return gc.miss[gc.actualResistanceMod] * resistances[id].miss;
    }

    /**
     * <summary>Return the endurance against the specified attack</summary>
     */
    float GetAttackEndurance(AttackCode id) {
        return gc.endurance[gc.actualResistanceMod] * (1f + resistances[id].endurance);
    }

    /**
     * <summary>Applies the effects of the specified attack</summary>
     */
    void StartAttack(AttackCode id) {
        // update data
        gc.ongoingAttacks++;
        attackSchedule[id].status = AttackStatus.ACTIVE;
        // apply the maluses
        gc.money -= attacks[id].moneyLoss;
        gc.users -= attacks[id].usersLoss * gc.users;
        gc.reputation -= attacks[id].reputationMalus;
        // update statistics
        attackStats[id].n++;
        attackStats[id].hit++;
        // generate a message
        DisplayMessage("Individuato attacco " + attacks[id].name + "! " + attacks[id].description, ActionCode.CONTINUE, Role.SECURITY);
    }

    /**
     * <summary>Removes the effects of the specified attack</summary>
     */
    void StopAttack(AttackCode id) {
        // remove the maluses
        gc.ongoingAttacks--;
        ScheduleAttack(id);
        attackSchedule[id].timer--;
    }

    /**
     * <summary>Applies the effects of avoiding an attack</summary>
     */
    void MissedAttack(AttackCode id) {
        // update data
        // increment the reputation
        gc.reputation += 0.02f;
        // update statistics
        attackStats[id].n++;
        attackStats[id].miss++;
        // re-schedule the attack
        ScheduleAttack(id);
        // generate a message
        DisplayMessage("Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name, ActionCode.CONTINUE, Role.SECURITY);
    }

    /**
     * <summary>Updates the scheduled attacks</summary>
     */
    void UpdateAttacks() {
        // manage the attack trend
        if (gc.actualAttackTrend != AttackCode.NONE) {
            gc.attackTrendTimer--;
            if (gc.attackTrendTimer == 0) SetAttackTrend();
        }
        // manage the the attacks
        foreach (AttackCode id in new List<AttackCode>(attackSchedule.Keys)) {
            if (attackSchedule[id].status == AttackStatus.PLANNING && attackSchedule[id].timer-- == 0) {
                // start the attack
                if (Random.Range(0f, attackSchedule[id].missMod) > GetAttackMiss(attackSchedule[id].id)) {
                    // hit
                    StartAttack(attackSchedule[id].id);
                    logManager.LogPrintAttack(attacks[attackSchedule[id].id].name, true);
                    // generate new repair task
                    Task newTask = new Task(TaskType.REPAIR, attackSchedule[id].id, attacks[attackSchedule[id].id].category);
                    waitingTasks.Add(newTask.id, newTask);
                } else {
                    // miss
                    MissedAttack(attackSchedule[id].id);
                    logManager.LogPrintAttack(attacks[attackSchedule[id].id].name, false);
                }
            }
        }
    }

    void SetAttackTrend() {
        List<AttackCode> possibleTrends = new List<AttackCode>();
        foreach(AttackPlan p in attackSchedule.Values) {
            if (p.status != AttackStatus.INACTIVE) possibleTrends.Add(p.id);
        }
        gc.actualAttackTrend = possibleTrends[Random.Range(0, possibleTrends.Count)];
        gc.attackTrendTimer = Random.Range(gc.attackTrendTime, 2 * gc.attackTrendTime);
        DisplayMessage("Attenzione: secondo le nostre analisi gli attacchi di tipo " + attacks[gc.actualAttackTrend].name + " sono in aumento!", ActionCode.CONTINUE, Role.SECURITY);
    }

    void UpdateResistanceAging() {
        if (gc.totalTime % gc.resistanceModStep == 0) gc.actualResistanceMod++;
    }

    // TASK

    public List<Task> GetAvailableTasksByType(TaskType type) {
        List<Task> res = new List<Task>();
        foreach (Task t in waitingTasks.Values) {
            if (t.type == type) res.Add(t);
        }
        return res;
    }

    void UpdateTasks() {
        List<EmployeeCode> toRemove = new List<EmployeeCode>();
        foreach (Task t in assignedTasks.Values) {
            if (t.progress == t.duration) {
                // end task
                EndTask(t);
                toRemove.Add(t.executor);
            } else {
                t.progress++;
            }
        }
        foreach(EmployeeCode id in toRemove) {
            assignedTasks.Remove(id);
        }
    }

    public void EndTask(Task t) {
        t.progress = 0;
        switch (t.type) {
            case TaskType.INSTALL:
                employees[t.executor].status = TaskType.NONE;
                shopItems[t.shopItem].level = 1;
                EnableShopItem(t.shopItem);
                break;
            case TaskType.REPAIR:
                employees[t.executor].status = TaskType.NONE;
                StopAttack(t.attack);
                break;
            case TaskType.UPGRADE:
                employees[t.executor].status = TaskType.NONE;
                shopItems[t.shopItem].level++;
                FinishUpgradeShopItem(t.shopItem);
                break;
            case TaskType.PREVENT:
                employees[t.executor].status = TaskType.NONE;
                waitingTasks.Add(t.id, t);
                assignedTasks.Remove(t.executor);
                break;
            default:
                Debug.Log("Error: undefined taskType");
                break;
        }
    }

    public int GetInstallTaskDuration(EmployeeCode id, ShopItemCode sid) {
        ShopItemInfo sii = shopItems[sid];
        int upgradeTime = sii.upgradeTime[sii.level];
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[sii.category];
        return Mathf.CeilToInt(upgradeTime * (1f - gc.abilityFactor * (abilityLevel - gc.abilityOffset)));
    }

    public int GetUpgradeTaskDuration(EmployeeCode id, ShopItemCode sid) {
        ShopItemInfo sii = shopItems[sid];
        int upgradeTime = sii.upgradeTime[sii.level];
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[sii.category];
        return Mathf.CeilToInt(upgradeTime * (1f - gc.abilityFactor * (abilityLevel - gc.abilityOffset)));
    }

    public int GetRepairTaskDuration(EmployeeCode id, AttackCode aid) {
        float resMod = GetActualDurationResistance(aid);
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[attacks[aid].category];
        return Mathf.CeilToInt((1f - resMod) * attacks[aid].duration * (1f - gc.abilityFactor * (abilityLevel - gc.abilityOffset)));
    }

    public float GetPreventProtection(EmployeeCode id, Category c) {
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[c];
        return 0.2f + 0.04f * (abilityLevel - gc.abilityOffset);
    }

    public float GetTaskProgress(ShopItemCode id) {
        foreach (Task t in assignedTasks.Values) {
            if (t.shopItem == id) return (float)t.progress / (t.duration + 1);
        }
        Debug.Log("Error: no task with the given ShopItemCode");
        return 0f;
    }

    public float GetTaskProgress(EmployeeCode id) {
        return (float)assignedTasks[id].progress / (assignedTasks[id].duration + 1);
    }

    public int GetTaskTarget(EmployeeCode id) {
        Task t = assignedTasks[id];
        switch (t.type) {
            case TaskType.INSTALL:
                return (int)t.shopItem;
            case TaskType.REPAIR:
                return (int)t.attack;
            case TaskType.UPGRADE:
                return (int)t.shopItem;
            case TaskType.PREVENT:
                return (int)t.category;
        }
        Debug.Log("Error: no task assigned to the given employee");
        return 0;
    }

    public Task GetRepairTask(AttackCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.REPAIR && t.attack == id) {
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

    public Task GetUpgradeTask(ShopItemCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.UPGRADE && t.shopItem == id) {
                return t;
            }
        }
        return null;
    }

    public Task GetPreventionTask(Category c) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.PREVENT && t.category == c) {
                return t;
            }
        }
        return null;
    }

    public Task GetPreventionTask(EmployeeCode id) {
        return assignedTasks[id];
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

    public List<ShopItemCode> GetShopItemsByCategory(Category category) {
        List<ShopItemCode> codes = new List<ShopItemCode>();
        foreach(ShopItemInfo sii in shopItems.Values) {
            if (sii.category == category) codes.Add(sii.id);
        }
        return codes;
    }

    public bool IsToUpgrade(ShopItemCode id) {
        foreach(Task t in waitingTasks.Values) {
            if (t.type == TaskType.UPGRADE && t.shopItem == id) return true;
        }
        return false;
    }

    /**
     * <summary>Applies the effects of buying an item in the shop</summary>
     */
    public void PurchaseShopItem(ShopItemCode id) {
        // print in the log
        logManager.LogPrintItem(shopItems[id].name, ActionCode.PURCHASE_ITEM);
        // evaluate the purchase
        EvaluatePurchaseShopItem(id);
        // apply the effects
        shopItems[id].status = ShopItemStatus.NOT_INSTALLED;
        gc.money -= shopItems[id].cost[shopItems[id].level];
        Task t = new Task(TaskType.INSTALL, id, shopItems[id].category);
        waitingTasks.Add(t.id, t);
        gui.Refresh(gc.money, gc.users, gc.reputation, dateTime);
    }

    public void StartUpgradeShopItem(ShopItemCode id) {
        // print in the log
        logManager.LogPrintItem(shopItems[id].name, ActionCode.UPGRADE_ITEM);
        // evaluate the purchase
        EvaluateUpgradeShopItem(id);
        // apply the effects
        gc.money -= shopItems[id].cost[shopItems[id].level];
        Task t = new Task(TaskType.UPGRADE, id, shopItems[id].category);
        waitingTasks.Add(t.id, t);
        gui.Refresh(gc.money, gc.users, gc.reputation, dateTime);
    }

    void FinishUpgradeShopItem(ShopItemCode id) {
        foreach (Resistance r in shopItems[id].resArray[shopItems[id].level - 2].resistances) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration /= (1 - r.duration);
            resistances[r.id].endurance -= r.endurance;
        }
        EnableShopItem(id);
    }

    /**
     * <summary>Applies the effects of enabling an item in the shop</summary>
     */
    public void EnableShopItem(ShopItemCode id) {
        // print in the log
        logManager.LogPrintItem(shopItems[id].name, ActionCode.ENABLE_ITEM);
        shopItems[id].status = ShopItemStatus.ACTIVE;
        // update resistances
        foreach (Resistance r in shopItems[id].resArray[shopItems[id].level - 1].resistances) {
            if (!resistances.ContainsKey(r.id)) resistances.Add(r.id, new Resistance(r.id, 1f, 0f, 0f));
            resistances[r.id].miss += r.miss;
            resistances[r.id].duration *= (1 - r.duration);
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
        foreach (Resistance r in shopItems[id].resArray[shopItems[id].level - 1].resistances) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration /= (1 - r.duration);
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
        shopItems[id].locked[shopItems[id].level] = false;
    }

    /**
     * <summary>Returns true if the specified item of the shop is installed</summary>
     */
    public bool RequirementIsSatisfied(Requirement r) {
        return shopItems[r.id].level >= r.level;
    }

    // EMPLOYEES

    public void AddToEmployees(EmployeeInfo e) {
        employees.Add(e.id, e);
    }

    public EmployeeInfo GetEmployee(EmployeeCode id) {
        return employees[id];
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
                assignedTasks[id].AssignEmployee(id, GetInstallTaskDuration(id, t.shopItem));
                waitingTasks.Remove(tid);
                break;
            case TaskType.REPAIR:
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetRepairTaskDuration(id, t.attack));
                waitingTasks.Remove(tid);
                break;
            case TaskType.UPGRADE:
                shopItems[t.shopItem].status = ShopItemStatus.UPGRADING;
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetUpgradeTaskDuration(id, t.shopItem));
                waitingTasks.Remove(tid);
                break;
            case TaskType.PREVENT:
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetPreventProtection(id, t.category));
                waitingTasks.Remove(tid);
                List<Resistance> res = new List<Resistance>();
                foreach(Resistance r in resistances.Values) {
                    if (attacks[r.id].category == t.category) {
                        r.miss += t.protection;
                    }
                }
                break;
            default:
                Debug.Log("Error: undefined taskType");
                break;
        }
        // evaluate the choice of the employee
        EvaluateEmployeeManagement(id);
    }

    // MISC

    public Sprite GetCategoryImage(Category c) {
        return categories[c];
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
        float moneyBonus = 0f;
        float moneyMalus = 0f;
        float attackMoneyMalus = 0f;
        // the revenue from the employees work
        foreach (EmployeeInfo e in employees.Values) {
            if (e.owned) moneyBonus += e.GetMoneyGain();
        }
        // the costs for the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE || sii.status == ShopItemStatus.UPGRADING) {
                if (sii.moneyMalus[sii.level - 1] < 0) moneyBonus -= sii.moneyMalus[sii.level - 1];
                else moneyMalus += sii.moneyMalus[sii.level - 1];
            }
        }
        // the malus for the active attacks
        foreach (AttackPlan a in attackSchedule.Values) {
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
            if (e.owned) moneyBonus += e.GetMoneyGain();
        }
        // the gain from the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if ((sii.status == ShopItemStatus.ACTIVE || sii.status == ShopItemStatus.UPGRADING) && sii.moneyMalus[sii.level - 1] < 0) moneyBonus -= sii.moneyMalus[sii.level - 1];
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
            if ((sii.status == ShopItemStatus.ACTIVE || sii.status == ShopItemStatus.UPGRADING) && sii.moneyMalus[sii.level - 1] > 0) {
                moneyMalus += sii.moneyMalus[sii.level - 1];
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
        float usersMod = 1f;
        float attackUsersMalus = 0f;
        // the user modifier for the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE) {
                if (sii.usersMod[sii.level - 1] > 0) usersMod *= 1 - sii.usersMod[sii.level - 1];
                else usersMod *= 1 - sii.usersMod[sii.level - 1];
            }
        }
        // the malus for the active attacks
        foreach (AttackPlan a in attackSchedule.Values) {
            if (a.status == AttackStatus.ACTIVE) attackUsersMalus += attacks[a.id].usersMalus;
        }
        return (float)Math.Round(gc.usersGain[gc.userLevel] * (0.5f * (1 + gc.reputation) * usersMod - attackUsersMalus) * Math.Round(gc.users));
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
        float usersMod = 1f;
        // the user modifier for the active items
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.status == ShopItemStatus.ACTIVE || sii.status == ShopItemStatus.UPGRADING) {
                if (sii.usersMod[sii.level - 1] > 0) usersMod *= 1 - sii.usersMod[sii.level - 1];
                else usersMod *= 1 - sii.usersMod[sii.level - 1];
            }
        }
        return usersMod;
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

    public List<Resistance> GetShopItemResistances(ShopItemCode id) {
        Dictionary<AttackCode, Resistance> res = new Dictionary<AttackCode, Resistance>();
        foreach (Resistance r in shopItems[id].resArray[shopItems[id].level].resistances) {
            res.Add(r.id, new Resistance(r));
        }
        if (shopItems[id].level > 0) {
            foreach(Resistance r in shopItems[id].resArray[shopItems[id].level - 1].resistances) {
                if (res.ContainsKey(r.id)) {
                    res[r.id].duration -= r.duration;
                    res[r.id].miss -= r.miss;
                    res[r.id].endurance -= r.endurance;
                    if (res[r.id].duration == 0 && res[r.id].miss == 0 && res[r.id].endurance == 0) res.Remove(r.id);
                }
            }
        }
        foreach (AttackCode k in res.Keys) {
            res[k].duration *= gc.duration[gc.actualResistanceMod];
            res[k].miss *= gc.duration[gc.actualResistanceMod];
            res[k].endurance *= gc.duration[gc.actualResistanceMod];
        }
        return new List<Resistance>(res.Values);
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

    void UpdateEmployees() {
        if (gc.employeeLevel < gc.employeeGoals.Length && gc.users >= gc.employeeGoals[gc.employeeLevel]) {
            EmployeeCode id = EmployeeUtils.ChooseNewEmployee(employees);
            if (id == EmployeeCode.NONE) {
                Debug.Log("Error: unexpected EmployeeCode");
                return;
            }
            employees[id].owned = true;
            DisplayMessage("Hai raggiunto " + NumUtils.NumToString(gc.employeeGoals[gc.employeeLevel]) + " utenti! Per questo ho deciso di assumere un nuovo dipendente, si chiama " + employees[id].name, ActionCode.CONTINUE, Role.CEO);
            gc.employeeLevel++;
        }
    }

    /**
     * <summary>Shows the game over</summary>
     */
    void GameOver() {
        Time.timeScale = 0;
        DisplayMessage("Oh no! Il bilancio è dissestato! Non sei stato all'altezza del tuo ruolo, dovrò assumere un nuovo CIO", ActionCode.CONTINUE, Role.CEO);
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
    public void DisplayMessage(string message, ActionCode action, Role role) {
        Instantiate(this.message, gameObject.transform, false).GetComponent<Message>().Load(message, action, avatars[role]);
    }

    void DebugPrint() {
        Dictionary<AttackCode, Resistance> res = new Dictionary<AttackCode, Resistance>();
        float totalMoney = 0f;
        foreach (ShopItemInfo sii in shopItems.Values) {
            foreach (Resistance r in sii.resArray[sii.maxLevel - 1].resistances) {
                if (!res.ContainsKey(r.id)) res.Add(r.id, new Resistance(r.id, 1f, 0f, 0f));
                res[r.id].duration *= (1 - r.duration);
                res[r.id].miss += r.miss;
                res[r.id].endurance += r.endurance;
            }
            totalMoney += sii.moneyMalus[sii.maxLevel - 1];
        }
        foreach (Resistance r in res.Values) {
            Debug.Log(r.id + " | " + "duration: " + r.duration + " | " + "miss: " + r.miss + " | " + "endurance: " + r.endurance);
        }
        Debug.Log("Total moneyMalus: " + totalMoney);
    }

    public void DebugPrintAttack(AttackCode id) {
        if (!attacks.ContainsKey(id)) {
            Debug.Log("Unknown code");
            return;
        }
        string res = id + "\n";
        foreach(ShopItemInfo sii in shopItems.Values) {
            foreach(Resistance r in sii.resArray[sii.maxLevel - 1].resistances) {
                if (r.id == id) res += "\t" + sii.id + " | " + "duration: " + r.duration + " | " + "miss: " + r.miss + " | " + "endurance: " + r.endurance + "\n";
            }
        }
        Debug.Log(res);
    }

    public void DebugPrintShopItem(ShopItemCode id) {
        if (!shopItems.ContainsKey(id)) {
            Debug.Log("Unknown code");
            return;
        }
        string res = id + "\n";
        ShopItemInfo sii = shopItems[id];
        foreach (Resistance r in sii.resArray[sii.maxLevel - 1].resistances) {
            res += "\t" + r.id + " | " + "duration: " + r.duration + " | " + "miss: " + r.miss + " | " + "endurance: " + r.endurance + "\n";
        }
        Debug.Log(res);
    }

    void EvaluateSecurityStatus() {
        // every x time evaluate the status of the countermeasures of the active attacks
        Dictionary<Category, int> scores = new Dictionary<Category, int>();
        foreach(AttackCode id in attacks.Keys) {
            if(attackSchedule[id].status != AttackStatus.INACTIVE) {
                Category c = attacks[id].category;
                if (GetActualDurationResistance(id) >= BKTModel.GetDurationL(id)) scores[c]++;
                else scores[c]--;
                if (GetActualMissResistance(id) >= BKTModel.GetMissL(id)) scores[c]++;
                else scores[c]--;
                if (GetActualEnduranceResistance(id) >= BKTModel.GetEnduranceL(id)) scores[c]++;
                else scores[c]--;
            }
        }
        foreach(KeyValuePair<Category, int> s in scores) {
            // Select the proper Knowledge Component
            SkillCode kc;
            switch (s.Key) {
                case Category.NETWORK:
                    kc = SkillCode.NETWORK;
                    break;
                case Category.ACCESS:
                    kc = SkillCode.ACCESS;
                    break;
                case Category.SOFTWARE:
                    kc = SkillCode.SOFTWARE;
                    break;
                case Category.ASSET:
                    kc = SkillCode.ASSET;
                    break;
                case Category.SERVICES:
                    kc = SkillCode.SERVICES;
                    break;
                default:
                    Debug.Log("Error: unexpected Category");
                    return;
            }
            // Decide the result of the evaluation
            if (s.Value >= 0) {
                // correct
                kcs[kc].AddTestResult(true);
            } else {
                // wrong
                kcs[kc].AddTestResult(false);
            }
        }
    }
    
    void EvaluatePurchaseShopItem(ShopItemCode id) {
        // Consider various aspects of the purchase
        int score = 0;
        ShopItemInfo sii = shopItems[id];
        List<Resistance> res = GetShopItemResistances(id);
        
        // 1. Are the attacks mitigated by the item active, so that the countermeasure is needed?
        foreach(Resistance r in res) {
            if (attackSchedule[r.id].status == AttackStatus.INACTIVE) score--;
            else score++;
            if (r.id == gc.actualAttackTrend) score++;
        }
        // 2. How much is the impact on the money?
        if (GetActualMoneyGain() - sii.cost[0] > 0) score++;
        else score--;
        // 3. Is the countermeasure over-preventing an attack?
        foreach(Resistance r in res) {
            if (GetActualDurationResistance(r.id) > BKTModel.GetDurationH(r.id)) score--;
            if (GetActualMissResistance(r.id) > BKTModel.GetMissH(r.id)) score--;
            if (GetActualEnduranceResistance(r.id) > BKTModel.GetEnduranceH(r.id)) score--;
        }
        // Select the proper Knowledge Component
        SkillCode kc;

        switch (sii.category) {
            case Category.NETWORK:
                kc = SkillCode.NETWORK;
                break;
            case Category.ACCESS:
                kc = SkillCode.ACCESS;
                break;
            case Category.SOFTWARE:
                kc = SkillCode.SOFTWARE;
                break;
            case Category.ASSET:
                kc = SkillCode.ASSET;
                break;
            case Category.SERVICES:
                kc = SkillCode.SERVICES;
                break;
            default:
                Debug.Log("Error: unexpected Category");
                return;
        }
        // Decide the result of the evaluation
        if (score >= 0) {
            // correct
            kcs[kc].AddTestResult(true);
        } else {
            // wrong
            kcs[kc].AddTestResult(false);
        }
    }

    void EvaluateUpgradeShopItem(ShopItemCode id) {
        // Consider various aspects of the upgrade
        int score = 0;
        ShopItemInfo sii = shopItems[id];
        List<Resistance> res = GetShopItemResistances(id);

        // 1. Are the attacks mitigated by the item active, so that the upgrade of the countermeasure is needed?
        foreach (Resistance r in res) {
            if (attackSchedule[r.id].status == AttackStatus.INACTIVE) score--;
            else score++;
            if (r.id == gc.actualAttackTrend) score++;
        }
        // 2. How much is the impact on the money?
        if (GetActualMoneyGain() - sii.cost[sii.level] > 0) score++;
        else score--;
        // 3. Is the countermeasure over-preventing an attack?
        foreach (Resistance r in res) {
            if (GetActualDurationResistance(r.id) > BKTModel.GetDurationH(r.id)) score--;
            if (GetActualMissResistance(r.id) > BKTModel.GetMissH(r.id)) score--;
            if (GetActualEnduranceResistance(r.id) > BKTModel.GetEnduranceH(r.id)) score--;
        }
        // Select the proper Knowledge Component
        SkillCode kc;

        switch (sii.category) {
            case Category.NETWORK:
                kc = SkillCode.NETWORK;
                break;
            case Category.ACCESS:
                kc = SkillCode.ACCESS;
                break;
            case Category.SOFTWARE:
                kc = SkillCode.SOFTWARE;
                break;
            case Category.ASSET:
                kc = SkillCode.ASSET;
                break;
            case Category.SERVICES:
                kc = SkillCode.SERVICES;
                break;
            default:
                Debug.Log("Error: unexpected Category");
                return;
        }
        // Decide the result of the evaluation
        if (score >= 0) {
            // correct
            kcs[kc].AddTestResult(true);
        } else {
            // wrong
            kcs[kc].AddTestResult(false);
        }
    }

    public void EvaluateQuiz(int qid, int aid) {
        QuizAnswer qa = quizzes[qid].answers[aid];
        // send the test to the model
        kcs[quizzes[qid].skill].AddTestResult(qa.correct);
        // apply the effects of the answer
        foreach(AnswerEffect effect in qa.effects) {
            switch (effect.target) {
                case Element.REPUTATION:
                    gc.reputation += effect.modifier;
                    break;
                case Element.MONEY:
                    gc.money += effect.modifier;
                    break;
                case Element.USERS:
                    gc.users *= effect.modifier;
                    break;
                default:
                    Debug.Log("Error: unexpected Element");
                    return;
            }
        }
        // eventually launch an attack
        if (qa.triggeredAttack != AttackCode.NONE && attackSchedule[qa.triggeredAttack].status != AttackStatus.INACTIVE) {
            attackSchedule[qa.triggeredAttack].missMod = 2f;
        }
    }

    void UpdateQuiz() {
        if ((gc.totalTime - 1) % gc.quizTime == 0) {
            // random quiz and time choice
            gc.quizTimer = Random.Range(1, gc.quizTime);
            gc.actualQuiz = Random.Range(0, quizzes.Count);
        }
        // launch quiz
        if (gc.quizTimer-- == 0) quizQuestion.Load(quizzes[gc.actualQuiz], avatars[quizzes[gc.actualQuiz].person]);
    }

    void EvaluateEmployeeManagement(EmployeeCode id) {
        int score = 0;
        EmployeeInfo employee = employees[id];
        Category category = assignedTasks[id].category;
        // 1. How good is the selected employee in the category of the task?
        score += EmployeeUtils.GetAbility(employee.abilities, category);
        
        bool good1 = true;
        bool good2 = true;
        foreach(EmployeeInfo e in GetAvailableEmployees()) {
            // 2. Are there better solutions?
            if (EmployeeUtils.GetAbility(employee.abilities, category) < EmployeeUtils.GetAbility(e.abilities, category)) {
                good1 = false;
                score--;
            }
            // 3. How much is the impact on the money?
            if (employee.moneyGain > e.moneyGain) {
                good2 = false;
                score--;
            }
        }
        if (good1) score++;
        if (good2) score++;
        // Decide the result of the evaluation
        if (score >= 0) {
            // correct
            kcs[SkillCode.MANAGEMENT].AddTestResult(true);
        } else {
            // wrong
            kcs[SkillCode.MANAGEMENT].AddTestResult(false);
        }
    }

    public ModelSave SaveModel() {
        List<KCRecord> records = new List<KCRecord>();

        foreach(KnowledgeComponent kc in kcs.Values) {
            records.Add(new KCRecord(kc.id, kc.name, kc.GetTransitionPos(), kc.GetTests()));
        }

        return new ModelSave(records.ToArray(), BKTModel.actualTimeSlot);
    }

    public void PrintLearningReport() {
        learningReport.Load(ActionCode.CONTINUE);
    }

    public void PrintFinalReport() {
        learningReport.Load(ActionCode.GAME_OVER);
    }
}
