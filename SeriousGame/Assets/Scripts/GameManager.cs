/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;
using Math = System.Math;

public class GameManager : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] Shop shop;
    [SerializeField] Log logManager;
    [SerializeField] SecurityView securityView;
    [SerializeField] Guide guide;
    [SerializeField] QuizQuestion quizQuestion;
    [SerializeField] LearningReport learningReport;
    [SerializeField] MonitorInterface monitorInterface;
    [SerializeField] NotificationList notificationList;
    [SerializeField] TutorialManager tutorialManager;
    [SerializeField] GameObject message;
    [SerializeField] GameObject personMoving;
    [SerializeField] RectTransform personParent;
    [SerializeField] AudioSettingsMenu audioSettingsMenu;
    [SerializeField] SaveSystem saveSystem;
    [SerializeField] AudioSource effectsSource;
    [SerializeField] AudioClip alarmTone;
    [SerializeField] AudioClip positiveTone;
    [SerializeField] TextAsset gameConfigJSON;
    [SerializeField] TextAsset attacksFileJSON;
    [SerializeField] TextAsset shopFileJSON;
    [SerializeField] TextAsset quizzesFileJSON;
    [SerializeField] TextAsset employeesFileJSON;
    [SerializeField] TextAsset modelFileJSON;
    [SerializeField] List<Sprite> roleIcons;
    [SerializeField] List<Sprite> roleFigures;
    [SerializeField] List<Sprite> categoryIcons;
    [SerializeField] List<Sprite> employeeIcons;

    float startTime;
    int updateTime = 1;
    DateTime dateTime;
    GameConfig gc;
    bool messageShown = false;

    List<LogLine> logs = new List<LogLine>();
    Dictionary<AttackCode, AttackPlan> attackSchedule = new Dictionary<AttackCode, AttackPlan>();
    Dictionary<int, Task> waitingTasks = new Dictionary<int, Task>();
    Dictionary<EmployeeCode, Task> assignedTasks = new Dictionary<EmployeeCode, Task>();
    Dictionary<AttackCode, AttackInfo> attacks = new Dictionary<AttackCode, AttackInfo>();
    Dictionary<AttackCode, Resistance> resistances = new Dictionary<AttackCode, Resistance>();
    Dictionary<AttackCode, AttackStats> attackStats = new Dictionary<AttackCode, AttackStats>();
    Dictionary<ShopItemCode, ShopItemInfo> shopItems = new Dictionary<ShopItemCode, ShopItemInfo>();
    Dictionary<EmployeeCode, EmployeeInfo> employees = new Dictionary<EmployeeCode, EmployeeInfo>();
    Dictionary<int, Quiz> quizzes = new Dictionary<int, Quiz>();
    Dictionary<Role, Person> roleAvatars = new Dictionary<Role, Person>();
    Dictionary<CategoryCode, Category> categories = new Dictionary<CategoryCode, Category>();
    Dictionary<EmployeeCode, Sprite> employeeAvatars = new Dictionary<EmployeeCode, Sprite>();
    Queue<Message> messages = new Queue<Message>();

    // Start is called before the first frame update
    void Start() {
        TimeManager.Pause();
        TimeManager.Pause();
        Load();
        DebugPrint();
        TimeManager.Resume();
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
            CheckMoney();
        }
        UpdateMessages();
    }

    // LOAD-SAVE GAME

    /**
     * <summary>Return the data to be saved to resume correctly the game in the future</summary>
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
        for (int i = 0; i < employeeIcons.Count; i++) {
            employeeAvatars[(EmployeeCode)i] = employeeIcons[i];
        }
        for (int i = 0; i < roleIcons.Count; i++) {
            roleAvatars[(Role)i] = new Person(roleIcons[i].name, roleIcons[i], roleFigures[i]);
        }
        // load the category images
        for (int i = 0; i < categoryIcons.Count; i++) {
            categories[(CategoryCode)i] = new Category(categoryIcons[i].name, categoryIcons[i]);
        }
        // initialize the BKT model
        BKTModel.Init(modelFileJSON);
        if (IOUtils.load) {
            // load the game data of the saved run 
            saveSystem.LoadGame();
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
            // initialize the audio settings
            audioSettingsMenu.Setup(gc);
            // initialize the report structure
            learningReport.Init(BKTModel.kcs);
            // refresh the GUI for the first time
            gui.Refresh(gc.money, gc.users, gc.reputation, dateTime);
            // start the tutorial
            if (IOUtils.tutorial) tutorialManager.Load();
            else TimeManager.Resume();
        }
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
    public void LoadGameData(GameSave gameSave) {
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
        // initialize the audio settings
        audioSettingsMenu.Setup(gc);
        // initialize the report structure
        learningReport.Init(BKTModel.kcs);
        // refresh the GUI for the first time
        gui.Refresh(gc.money, gc.users, gc.reputation, dateTime);
        // set attack trend
        if (gc.actualAttackTrend != AttackCode.NONE) gui.SetNewTrend(attacks[gc.actualAttackTrend].name);
    }

    /**
     * <summary>Reloads the scene and disables the tutorial</summary>
     */
    public void EndTutorial() {
        // restart the scenario
        IOUtils.tutorial = false;
        SceneLoader.ReloadScene();
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
     * <summary>Return all the attacks</summary>
     */
    public List<AttackInfo> GetAttacks() {
        return new List<AttackInfo>(attacks.Values);
    }

    /**
     * <summary>Returns the duration resistance value of an attack</summary>
     */
    float GetActualDurationResistance(AttackCode id) {
        float val = (1 - resistances[id].duration) * gc.duration[gc.actualResistanceMod];
        if (val < 0.99f) return val;
        else return 0.99f;
    }

    /**
     * <summary>Returns the miss resistance value of an attack</summary>
     */
    float GetActualMissResistance(AttackCode id) {
        return resistances[id].miss * gc.miss[gc.actualResistanceMod];
    }

    /**
     * <summary>Returns the endurance resistance value of an attack</summary>
     */
    float GetActualEnduranceResistance(AttackCode id) {
        return resistances[id].endurance * gc.endurance[gc.actualResistanceMod];
    }

    /**
     * <summary>Returns the duration resistance value by applying the actual modifier</summary>
     */
    public float GetActualDurationResistance(float duration) {
        float val = duration * gc.duration[gc.actualResistanceMod];
        if (val < 0.99f) return val;
        else return 0.99f;
    }

    /**
     * <summary>Returns the miss resistance value by applying the actual modifier</summary>
     */
    public float GetActualMissResistance(float miss) {
        return miss * gc.miss[gc.actualResistanceMod];
    }

    /**
     * <summary>Returns the endurance resistance value by applying the actual modifier</summary>
     */
    public float GetActualEnduranceResistance(float endurance) {
        return endurance * gc.endurance[gc.actualResistanceMod];
    }

    /**
     * <summary>Returns the resistances to the specified attack</summary>
     */
    public Resistance GetResistance(AttackCode id) {
        if (!resistances.ContainsKey(id)) return null;
        float duration = GetActualDurationResistance(id);
        float miss = GetActualMissResistance(id);
        float endurance = GetActualEnduranceResistance(id);
        return new Resistance(id, duration, miss, endurance);
    }

    /**
     * <summary>Returns true in the attack is unlocked</summary>
     */
    public bool AttackIsScheduled(AttackCode id) {
        return attackSchedule[id].status != AttackStatus.INACTIVE;
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
            case 24: // day 1
                ScheduleAttack(AttackCode.WORM);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.WORM].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.WORM].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 48: // day 2
                ScheduleAttack(AttackCode.BRUTE_FORCE);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.BRUTE_FORCE].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.BRUTE_FORCE].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 72: // day 3
                ScheduleAttack(AttackCode.MITM);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.MITM].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.MITM].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 144: // day 6
                ScheduleAttack(AttackCode.DOS);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.DOS].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DOS].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 168: // day 7
                SetAttackTrend();
                break;
            case 192: // day 9
                ScheduleAttack(AttackCode.VIRUS);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.VIRUS].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.VIRUS].name + "\nI nostri analisti hanno indicato che tra breve pdiventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 288: // day 12
                ScheduleAttack(AttackCode.SOCIAL_ENGINEERING);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.SOCIAL_ENGINEERING].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SOCIAL_ENGINEERING].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 360: // day 15
                ScheduleAttack(AttackCode.API_VULNERABILITY);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.API_VULNERABILITY].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.API_VULNERABILITY].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 432: // day 18
                ScheduleAttack(AttackCode.DICTIONARY);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.DICTIONARY].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.DICTIONARY].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 504: // day 21
                ScheduleAttack(AttackCode.PHISHING);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.PHISHING].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.PHISHING].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 576: // day 24
                ScheduleAttack(AttackCode.SPYWARE);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.SPYWARE].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.SPYWARE].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 648: // day 27
                ScheduleAttack(AttackCode.RAINBOW_TABLE);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.RAINBOW_TABLE].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RAINBOW_TABLE].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
                break;
            case 720: // day 30
                ScheduleAttack(AttackCode.RANSOMWARE);
                notificationList.AddNotification("NUOVO ATTACCO " + attacks[AttackCode.RANSOMWARE].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
                DisplayMessage("Nuovo attacco: " + attacks[AttackCode.RANSOMWARE].name + "\nI nostri analisti hanno indicato che tra breve diventeremo bersaglio di attacchi di questo tipo.", ActionCode.CONTINUE, Role.SECURITY);
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
        // update graphics
        monitorInterface.EnableAttackIcon(attacks[id].category);
        // generate a message
        notificationList.AddNotification("SUBITO ATTACCO " + attacks[id].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
        DisplayMessage("Individuato attacco " + attacks[id].name + "! " + attacks[id].description + " " + AttackUtils.PrintMotivation(id, false, shopItems), ActionCode.CONTINUE, Role.SECURITY);
        effectsSource.clip = alarmTone;
        effectsSource.Play();
    }

    /**
     * <summary>Removes the effects of the specified attack</summary>
     */
    void StopAttack(AttackCode id) {
        // remove the maluses
        gc.ongoingAttacks--;
        ScheduleAttack(id);
        attackSchedule[id].timer--;
        // update graphics
        monitorInterface.DisableAttackIcon(attacks[id].category);
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
        notificationList.AddNotification("SVENTATO ATTACCO " + attacks[id].name, roleAvatars[Role.SECURITY].name, roleAvatars[Role.SECURITY].icon);
        DisplayMessage("Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name + ". " + AttackUtils.PrintMotivation(id, true, shopItems), ActionCode.CONTINUE, Role.SECURITY);
        effectsSource.clip = positiveTone;
        effectsSource.Play();
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

    /**
     * <summary></summary>
     */
    void SetAttackTrend() {
        List<AttackCode> possibleTrends = new List<AttackCode>();
        foreach (AttackPlan p in attackSchedule.Values) {
            if (p.status != AttackStatus.INACTIVE) possibleTrends.Add(p.id);
        }
        gc.actualAttackTrend = possibleTrends[Random.Range(0, possibleTrends.Count)];
        gc.attackTrendTimer = Random.Range(gc.attackTrendTime, 2 * gc.attackTrendTime);
        DisplayMessage("Attenzione: secondo le nostre analisi gli attacchi di tipo " + attacks[gc.actualAttackTrend].name + " sono in aumento!", ActionCode.CONTINUE, Role.SECURITY);
        gui.SetNewTrend(attacks[gc.actualAttackTrend].name);
    }

    /**
     * <summary></summary>
     */
    void UpdateResistanceAging() {
        if (gc.totalTime % gc.resistanceModStep == 0) gc.actualResistanceMod++;
    }

    // TASK

    /**
     * <summary></summary>
     */
    public List<Task> GetAssignedTasks() {
        return new List<Task>(assignedTasks.Values);
    }

    /**
     * <summary></summary>
     */
    public List<Task> GetWaitingTasks() {
        return new List<Task>(waitingTasks.Values);
    }

    /**
     * <summary></summary>
     */
    public List<Task> GetAvailableTasksByType(TaskType type) {
        List<Task> res = new List<Task>();
        foreach (Task t in waitingTasks.Values) {
            if (t.type == type) res.Add(t);
        }
        return res;
    }

    /**
     * <summary></summary>
     */
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
        foreach (EmployeeCode id in toRemove) {
            assignedTasks.Remove(id);
        }
    }

    /**
     * <summary></summary>
     */
    public void EndTask(Task t) {
        t.progress = 0;
        switch (t.type) {
            case TaskType.INSTALL:
                logManager.LogPrintEndTask(t);
                employees[t.executor].status = TaskType.NONE;
                shopItems[t.shopItem].level = 1;
                EnableShopItem(t.shopItem);
                notificationList.AddNotification("INSTALLAZIONE COMPLETA " + shopItems[t.shopItem].name, employees[t.executor].name, employeeAvatars[t.executor]);
                break;
            case TaskType.REPAIR:
                logManager.LogPrintEndTask(t);
                employees[t.executor].status = TaskType.NONE;
                StopAttack(t.attack);
                notificationList.AddNotification("RIPARAZIONE COMPLETA " + attacks[t.attack].name, employees[t.executor].name, employeeAvatars[t.executor]);
                break;
            case TaskType.UPGRADE:
                logManager.LogPrintEndTask(t);
                employees[t.executor].status = TaskType.NONE;
                shopItems[t.shopItem].level++;
                FinishUpgradeShopItem(t.shopItem);
                notificationList.AddNotification("POTENZIAMENTO COMPLETO " + shopItems[t.shopItem].name, employees[t.executor].name, employeeAvatars[t.executor]);
                break;
            case TaskType.PREVENT:
                logManager.LogPrintEndTask(t);
                employees[t.executor].status = TaskType.NONE;
                waitingTasks.Add(t.id, t);
                monitorInterface.DisableEmployeeIcon(t.category);
                assignedTasks.Remove(t.executor);
                break;
            default:
                Debug.Log("Error: undefined taskType");
                break;
        }
    }

    /**
     * <summary></summary>
     */
    public int GetInstallTaskDuration(EmployeeCode id, ShopItemCode sid) {
        ShopItemInfo sii = shopItems[sid];
        int upgradeTime = sii.upgradeTime[sii.level];
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[sii.category];
        return Mathf.CeilToInt(upgradeTime * (1f - gc.abilityFactor * (abilityLevel - gc.abilityOffset)));
    }

    /**
     * <summary></summary>
     */
    public int GetUpgradeTaskDuration(EmployeeCode id, ShopItemCode sid) {
        ShopItemInfo sii = shopItems[sid];
        int upgradeTime = sii.upgradeTime[sii.level];
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[sii.category];
        return Mathf.CeilToInt(upgradeTime * (1f - gc.abilityFactor * (abilityLevel - gc.abilityOffset)));
    }

    /**
     * <summary></summary>
     */
    public int GetRepairTaskDuration(EmployeeCode id, AttackCode aid) {
        float resMod = GetActualDurationResistance(aid);
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[attacks[aid].category];
        return Mathf.CeilToInt((1f - resMod) * attacks[aid].duration * (1f - gc.abilityFactor * (abilityLevel - gc.abilityOffset)));
    }

    /**
     * <summary></summary>
     */
    public float GetPreventProtection(EmployeeCode id, CategoryCode c) {
        float abilityLevel = EmployeeUtils.GetAbilities(employees[id].abilities)[c];
        return 0.15f + 0.03f * (abilityLevel - gc.abilityOffset);
    }

    /**
     * <summary></summary>
     */
    public float GetTaskProgress(ShopItemCode id) {
        foreach (Task t in assignedTasks.Values) {
            if (t.shopItem == id) return (float)t.progress / (t.duration + 1);
        }
        Debug.Log("Error: no task with the given ShopItemCode");
        return 0f;
    }

    /**
     * <summary></summary>
     */
    public float GetTaskProgress(EmployeeCode id) {
        return (float)assignedTasks[id].progress / (assignedTasks[id].duration + 1);
    }

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    public Task GetRepairTask(AttackCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.REPAIR && t.attack == id) {
                return t;
            }
        }
        return null;
    }

    /**
     * <summary></summary>
     */
    public Task GetInstallTask(ShopItemCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.INSTALL && t.shopItem == id) {
                return t;
            }
        }
        return null;
    }

    /**
     * <summary></summary>
     */
    public Task GetUpgradeTask(ShopItemCode id) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.UPGRADE && t.shopItem == id) {
                return t;
            }
        }
        return null;
    }

    /**
     * <summary></summary>
     */
    public Task GetPreventionTask(CategoryCode c) {
        foreach (Task t in waitingTasks.Values) {
            if (t.type == TaskType.PREVENT && t.category == c) {
                return t;
            }
        }
        return null;
    }

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    public List<ShopItemCode> GetShopItemsByCategory(CategoryCode category) {
        List<ShopItemCode> codes = new List<ShopItemCode>();
        foreach (ShopItemInfo sii in shopItems.Values) {
            if (sii.category == category) codes.Add(sii.id);
        }
        return codes;
    }

    /**
     * <summary></summary>
     */
    public bool IsToUpgrade(ShopItemCode id) {
        foreach (Task t in waitingTasks.Values) {
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

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    public void AddToEmployees(EmployeeInfo e) {
        employees.Add(e.id, e);
    }

    /**
     * <summary></summary>
     */
    public EmployeeInfo GetEmployee(EmployeeCode id) {
        return employees[id];
    }

    /**
     * <summary></summary>
     */
    public bool CheckEmployeeAvailability() {
        return EmployeeUtils.CheckEmployeeAvailability(employees);
    }

    /**
     * <summary></summary>
     */
    public List<EmployeeInfo> GetAvailableEmployees() {
        return EmployeeUtils.GetAvailableEmployees(employees);
    }

    /**
     * <summary></summary>
     */
    public void AssignEmployee(EmployeeCode id, int tid) {
        Task t = waitingTasks[tid];
        employees[id].status = t.type;
        switch (t.type) {
            case TaskType.INSTALL:
                shopItems[t.shopItem].status = ShopItemStatus.INSTALLING;
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetInstallTaskDuration(id, t.shopItem));
                waitingTasks.Remove(tid);
                logManager.LogPrintStartTask(t);
                break;
            case TaskType.REPAIR:
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetRepairTaskDuration(id, t.attack));
                waitingTasks.Remove(tid);
                logManager.LogPrintStartTask(t);
                break;
            case TaskType.UPGRADE:
                shopItems[t.shopItem].status = ShopItemStatus.UPGRADING;
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetUpgradeTaskDuration(id, t.shopItem));
                waitingTasks.Remove(tid);
                logManager.LogPrintStartTask(t);
                break;
            case TaskType.PREVENT:
                assignedTasks.Add(id, t);
                assignedTasks[id].AssignEmployee(id, GetPreventProtection(id, t.category));
                waitingTasks.Remove(tid);
                foreach (Resistance r in resistances.Values) {
                    if (attacks[r.id].category == t.category) {
                        r.miss += t.protection;
                    }
                }
                monitorInterface.EnableEmployeeIcon(t.category, employeeAvatars[t.executor]);
                logManager.LogPrintStartTask(t);
                break;
            default:
                Debug.Log("Error: undefined taskType");
                break;
        }
        // evaluate the choice of the employee
        EvaluateEmployeeManagement(id);
    }

    // MISC

    /**
     * <summary></summary>
     */
    public void SaveAudioSettings(float musicVolume, bool musicMute, float effectsVolume, bool effectsMute) {
        gc.musicVolume = musicVolume;
        gc.musicMute = musicMute;
        gc.effectsVolume = effectsVolume;
        gc.effectsMute = effectsMute;
    }

    /**
     * <summary></summary>
     */
    public Category GetCategory(CategoryCode c) {
        return categories[c];
    }

    /**
     * <summary></summary>
     */
    public Sprite GetCategoryImage(CategoryCode c) {
        return categories[c].sprite;
    }

    /**
     * <summary></summary>
     */
    public Sprite GetEmployeeIcon(EmployeeCode id) {
        return employeeAvatars[id];
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

    /**
     * <summary></summary>
     */
    public List<Resistance> GetShopItemResistances(ShopItemCode id) {
        Dictionary<AttackCode, Resistance> res = new Dictionary<AttackCode, Resistance>();
        foreach (Resistance r in shopItems[id].resArray[shopItems[id].level].resistances) {
            res.Add(r.id, new Resistance(r));
        }
        if (shopItems[id].level > 0) {
            foreach (Resistance r in shopItems[id].resArray[shopItems[id].level - 1].resistances) {
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

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    void UpdateEmployees() {
        if (gc.employeeLevel < gc.employeeGoals.Length && gc.users >= gc.employeeGoals[gc.employeeLevel]) {
            EmployeeCode id = EmployeeUtils.ChooseNewEmployee(employees);
            if (id == EmployeeCode.NONE) {
                Debug.Log("Error: unexpected EmployeeCode");
                return;
            }
            employees[id].owned = true;
            DisplayMessage("Hai raggiunto " + NumUtils.NumToString(gc.employeeGoals[gc.employeeLevel]) + " utenti! Per questo ho deciso di assumere un nuovo dipendente, si chiama " + employees[id].name, ActionCode.CONTINUE, Role.CEO);
            notificationList.AddNotification("OBIETTIVO RAGGIUNTO\nHo assunto " + employees[id].name, roleAvatars[Role.CEO].name, roleAvatars[Role.CEO].icon);
            gc.employeeLevel++;
        }
    }

    /**
     * <summary>Shows the game over</summary>
     */
    void GameOver() {
        Time.timeScale = 0;
        DisplayMessage("Oh no! Il bilancio è dissestato! Non sei stato all'altezza del tuo ruolo, dovrò assumere un nuovo CIO...", ActionCode.GAME_OVER, Role.CEO);
        effectsSource.clip = alarmTone;
        effectsSource.Play();
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
     * <summary></summary>
     */
    void CheckMoney() {
        if (gc.money < 0) {
            if (gc.firstNegative) {
                gc.firstNegative = false;
                DisplayMessage("ATTENZIONE! Il budget è in negativo, se non riesci a riportarlo in positivo in " + gc.maxNegative + " ore dovrò licenziarti!", ActionCode.CONTINUE, Role.CEO);
                notificationList.AddNotification("Il budget è in negativo!\n" + gc.maxNegative + " ore rimaste", roleAvatars[Role.CEO].name, roleAvatars[Role.CEO].icon);
            } else if (gc.negativeTime == 0) {
                notificationList.AddNotification("Il budget è in negativo!\n" + gc.maxNegative + " ore rimaste", roleAvatars[Role.CEO].name, roleAvatars[Role.CEO].icon);
            } else if (gc.maxNegative - gc.negativeTime == 24) {
                notificationList.AddNotification("Il budget è in negativo!\n 24 ore rimaste", roleAvatars[Role.CEO].name, roleAvatars[Role.CEO].icon);
            }
            gc.negativeTime++;
        } else gc.negativeTime = 0;
    }

    /**
     * <summary>Creates a pop-up window message</summary>
     */
    public void DisplayMessage(string message, ActionCode action, Role role) {
        GameObject m = Instantiate(this.message, gameObject.transform, false);
        m.GetComponent<Message>().Load(this, message, action, roleAvatars[role]);
        messages.Enqueue(m.GetComponent<Message>());
    }

    /**
     * <summary></summary>
     */
    public void UpdateMessages() {
        if (messages.Count > 0 && messageShown == false) {
            messageShown = true;
            messages.Dequeue().Show();
        }
    }

    /**
     * <summary></summary>
     */
    public void CloseMessage() {
        messageShown = false;
    }

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    public void DebugPrintAttack(AttackCode id) {
        if (!attacks.ContainsKey(id)) {
            Debug.Log("Unknown code");
            return;
        }
        string res = id + "\n";
        foreach (ShopItemInfo sii in shopItems.Values) {
            foreach (Resistance r in sii.resArray[sii.maxLevel - 1].resistances) {
                if (r.id == id) res += "\t" + sii.id + " | " + "duration: " + r.duration + " | " + "miss: " + r.miss + " | " + "endurance: " + r.endurance + "\n";
            }
        }
        Debug.Log(res);
    }

    /**
     * <summary></summary>
     */
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

    /**
     * <summary></summary>
     */
    void EvaluateSecurityStatus() {
        // every x time evaluate the status of the countermeasures of the active attacks
        Dictionary<CategoryCode, float> scores = new Dictionary<CategoryCode, float>();
        foreach (AttackCode id in attacks.Keys) {
            if (attackSchedule[id].status != AttackStatus.INACTIVE) {
                CategoryCode c = attacks[id].category;
                if (!scores.ContainsKey(c)) scores[c] = 0f;
                scores[c] += (GetActualDurationResistance(id) - BKTModel.GetDurationL(id)) * 0.8f;
                scores[c] += (GetActualMissResistance(id) - BKTModel.GetMissL(id)) * 1f;
                scores[c] += (GetActualEnduranceResistance(id) - BKTModel.GetEnduranceL(id)) * 0.6f;
            }
        }
        foreach (KeyValuePair<CategoryCode, float> s in scores) {
            // Select the proper Knowledge Component
            SkillCode kc;
            switch (s.Key) {
                case CategoryCode.NETWORK:
                    kc = SkillCode.NETWORK;
                    break;
                case CategoryCode.ACCESS:
                    kc = SkillCode.ACCESS;
                    break;
                case CategoryCode.SOFTWARE:
                    kc = SkillCode.SOFTWARE;
                    break;
                case CategoryCode.ASSET:
                    kc = SkillCode.ASSET;
                    break;
                case CategoryCode.SERVICES:
                    kc = SkillCode.SERVICES;
                    break;
                default:
                    Debug.Log("Error: unexpected Category");
                    return;
            }
            // Decide the result of the evaluation
            if (s.Value >= 0) {
                // correct
                BKTModel.kcs[kc].AddTestResult(true);
            } else {
                // wrong
                BKTModel.kcs[kc].AddTestResult(false);
            }
        }
        // auto-save
        saveSystem.SaveGame(SaveGame(), SaveModel());
    }

    /**
     * <summary></summary>
     */
    void EvaluatePurchaseShopItem(ShopItemCode id) {
        // Consider various aspects of the purchase
        float score = 0f;
        ShopItemInfo sii = shopItems[id];
        List<Resistance> res = GetShopItemResistances(id);

        // 1. Are the attacks mitigated by the item active, so that the countermeasure is needed?
        foreach (Resistance r in res) {
            if (attackSchedule[r.id].status == AttackStatus.INACTIVE) score -= (r.duration * 0.8f + r.miss * 1f + r.endurance * 0.6f) * 0.4f;
            else score += (r.duration * 0.8f + r.miss * 1f + r.endurance * 0.6f) * 1f;
            if (r.id == gc.actualAttackTrend) score += (r.duration * 0.8f + r.miss * 1f + r.endurance * 0.6f) * 1f;
        }
        // 2. How much is the impact on the money?
        if (GetActualMoneyGain() - sii.cost[0] > 0) score += 0.5f;
        else score -= 0.5f;
        // 3. Is the countermeasure over-preventing an attack?
        foreach (Resistance r in res) {
            if (GetActualDurationResistance(r.id) > BKTModel.GetDurationH(r.id)) score -= (GetActualDurationResistance(r.id) - BKTModel.GetDurationH(r.id)) * 0.8f;
            if (GetActualMissResistance(r.id) > BKTModel.GetMissH(r.id)) score -= (GetActualMissResistance(r.id) - BKTModel.GetMissH(r.id)) * 1f;
            if (GetActualEnduranceResistance(r.id) > BKTModel.GetEnduranceH(r.id)) score -= (GetActualEnduranceResistance(r.id) - BKTModel.GetEnduranceH(r.id)) * 0.6f;
        }
        // Select the proper Knowledge Component
        SkillCode kc;

        switch (sii.category) {
            case CategoryCode.NETWORK:
                kc = SkillCode.NETWORK;
                break;
            case CategoryCode.ACCESS:
                kc = SkillCode.ACCESS;
                break;
            case CategoryCode.SOFTWARE:
                kc = SkillCode.SOFTWARE;
                break;
            case CategoryCode.ASSET:
                kc = SkillCode.ASSET;
                break;
            case CategoryCode.SERVICES:
                kc = SkillCode.SERVICES;
                break;
            default:
                Debug.Log("Error: unexpected Category");
                return;
        }
        // Decide the result of the evaluation
        if (score >= 0) {
            // correct
            BKTModel.kcs[kc].AddTestResult(true);
        } else {
            // wrong
            BKTModel.kcs[kc].AddTestResult(false);
        }
    }

    /**
     * <summary></summary>
     */
    void EvaluateUpgradeShopItem(ShopItemCode id) {
        // Consider various aspects of the upgrade
        float score = 0f;
        ShopItemInfo sii = shopItems[id];
        List<Resistance> res = GetShopItemResistances(id);

        // 1. Are the attacks mitigated by the item active, so that the upgrade of the countermeasure is needed?
        foreach (Resistance r in res) {
            if (attackSchedule[r.id].status == AttackStatus.INACTIVE) score -= (r.duration * 0.8f + r.miss * 1f + r.endurance * 0.6f) * 0.4f;
            else score += (r.duration * 0.8f + r.miss * 1f + r.endurance * 0.6f) * 1f;
            if (r.id == gc.actualAttackTrend) score += (r.duration * 0.8f + r.miss * 1f + r.endurance * 0.6f) * 1f;
        }
        // 2. How much is the impact on the money?
        if (GetActualMoneyGain() - sii.cost[sii.level] > 0) score += 0.5f;
        else score -= 0.5f;
        // 3. Is the countermeasure over-preventing an attack?
        foreach (Resistance r in res) {
            if (GetActualDurationResistance(r.id) > BKTModel.GetDurationH(r.id)) score -= (GetActualDurationResistance(r.id) - BKTModel.GetDurationH(r.id)) * 0.8f;
            if (GetActualMissResistance(r.id) > BKTModel.GetMissH(r.id)) score -= (GetActualMissResistance(r.id) - BKTModel.GetMissH(r.id)) * 1f;
            if (GetActualEnduranceResistance(r.id) > BKTModel.GetEnduranceH(r.id)) score -= (GetActualEnduranceResistance(r.id) - BKTModel.GetEnduranceH(r.id)) * 0.6f;
        }
        // Select the proper Knowledge Component
        SkillCode kc;

        switch (sii.category) {
            case CategoryCode.NETWORK:
                kc = SkillCode.NETWORK;
                break;
            case CategoryCode.ACCESS:
                kc = SkillCode.ACCESS;
                break;
            case CategoryCode.SOFTWARE:
                kc = SkillCode.SOFTWARE;
                break;
            case CategoryCode.ASSET:
                kc = SkillCode.ASSET;
                break;
            case CategoryCode.SERVICES:
                kc = SkillCode.SERVICES;
                break;
            default:
                Debug.Log("Error: unexpected Category");
                return;
        }
        // Decide the result of the evaluation
        if (score >= 0) {
            // correct
            BKTModel.kcs[kc].AddTestResult(true);
        } else {
            // wrong
            BKTModel.kcs[kc].AddTestResult(false);
        }
    }

    /**
     * <summary></summary>
     */
    public void EvaluateQuiz(int qid, int aid) {
        QuizAnswer qa = quizzes[qid].answers[aid];
        // send the test to the model
        BKTModel.kcs[quizzes[qid].skill].AddTestResult(qa.correct);
        // apply the effects of the answer
        foreach (AnswerEffect effect in qa.effects) {
            switch (effect.target) {
                case Element.REPUTATION:
                    gc.reputation += effect.modifier;
                    break;
                case Element.MONEY:
                    gc.money += effect.modifier;
                    break;
                case Element.USERS:
                    gc.users *= (1f - effect.modifier);
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

    /**
     * <summary></summary>
     */
    void UpdateQuiz() {
        if ((gc.totalTime - 1) % gc.quizTime == 0) {
            // random quiz and time choice
            gc.quizTimer = Random.Range(5, gc.quizTime);
            gc.actualQuiz = Random.Range(0, quizzes.Count);
        }
        // launch quiz
        if (gc.quizTimer-- == 0) Instantiate(personMoving, personParent, false).GetComponent<PersonController>().Load(this, roleAvatars[quizzes[gc.actualQuiz].person]);
    }

    /**
     * <summary></summary>
     */
    public void LaunchQuiz() {
        quizQuestion.Load(quizzes[gc.actualQuiz], roleAvatars[quizzes[gc.actualQuiz].person]);
        messageShown = true;
    }

    /**
     * <summary></summary>
     */
    void EvaluateEmployeeManagement(EmployeeCode id) {
        float score = 0;
        EmployeeInfo employee = employees[id];
        CategoryCode category = assignedTasks[id].category;
        // 1. How good is the selected employee in the category of the task?
        score += EmployeeUtils.GetAbility(employee.abilities, category) * 0.5f;

        foreach (EmployeeInfo e in GetAvailableEmployees()) {
            // 2. Are there better solutions?
            score += (EmployeeUtils.GetAbility(employee.abilities, category) - EmployeeUtils.GetAbility(e.abilities, category)) * 0.3f;
            // 3. How much is the impact on the money?
            score += (employee.moneyGain - e.moneyGain) * 0.1f;
        }
        // Decide the result of the evaluation
        if (score >= 0) {
            // correct
            BKTModel.kcs[SkillCode.MANAGEMENT].AddTestResult(true);
        } else {
            // wrong
            BKTModel.kcs[SkillCode.MANAGEMENT].AddTestResult(false);
        }
    }

    /**
     * <summary></summary>
     */
    public ModelSave SaveModel() {
        List<KCRecord> records = new List<KCRecord>();

        foreach (KnowledgeComponent kc in BKTModel.kcs.Values) {
            records.Add(new KCRecord(kc.id, kc.name, kc.GetTransitionPos(), kc.GetTests()));
        }

        return new ModelSave(records.ToArray(), BKTModel.actualTimeSlot);
    }

    /**
     * <summary></summary>
     */
    public void PrintLearningReport() {
        learningReport.Load(ActionCode.CONTINUE);
    }

    /**
     * <summary></summary>
     */
    public void PrintFinalReport() {
        learningReport.Load(ActionCode.GAME_OVER);
    }
}
