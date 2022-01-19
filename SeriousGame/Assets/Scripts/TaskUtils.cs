using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    NONE,
    INSTALL,
    REPAIR,
    UPGRADE
}

[System.Serializable]
public class Task {
    static int _id = 0;

    public int id;
    public TaskType type;
    public EmployeeCode executor;
    public ShopItemCode shopItem;
    public AttackCode attack;
    public int duration;
    public int progress;

    public Task(Task t) {
        this.id = _id++;
        this.type = t.type;
        this.executor = t.executor;
        this.shopItem = t.shopItem;
        this.attack = t.attack;
        this.duration = t.duration;
        this.progress = t.progress;
    }

    public Task(TaskType type, ShopItemCode shopItem) {
        this.id = _id++;
        this.type = type;
        this.executor = EmployeeCode.NONE;
        this.shopItem = shopItem;
        this.attack = AttackCode.NONE;
        this.duration = 0;
        this.progress = 0;
    }

    public Task(TaskType type, AttackCode attack) {
        this.id = _id++;
        this.type = type;
        this.executor = EmployeeCode.NONE;
        this.shopItem = ShopItemCode.NONE;
        this.attack = attack;
        this.duration = 0;
        this.progress = 0;
    }

    public void AssignEmployee(EmployeeCode executor, int duration) {
        this.executor = executor;
        this.duration = duration;
        this.progress = 0;
    }
}

public static class TaskUtils {
    public static void UpdateTasks(Dictionary<int, Task> waitingTasks, Task[] t1, Dictionary<EmployeeCode, Task> assignedTasks, Task[] t2) {
        foreach (Task t in t1) {
            Task newTask = new Task(t);
            waitingTasks.Add(newTask.id, newTask);
        }
        foreach (Task t in t2) {
            Task newTask = new Task(t);
            assignedTasks.Add(newTask.executor, newTask);
        }
    }
}
