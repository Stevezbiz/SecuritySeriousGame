using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    NONE,
    INSTALL,
    REPAIR
}

[System.Serializable]
public class Task {
    public TaskType type;
    public bool assigned;
    public EmployeeCode executor;
    public ShopItemCode shopItem;
    public AttackCode attack;
    public int duration;
    public int progress;

    public Task(TaskType type, ShopItemCode shopItem) {
        this.type = type;
        this.assigned = false;
        this.shopItem = shopItem;
    }

    public Task(TaskType type, AttackCode attack) {
        this.type = type;
        this.assigned = false;
        this.attack = attack;
    }

    public void AssignEmployee(EmployeeCode executor, int duration) {
        this.assigned = true;
        this.executor = executor;
        this.duration = duration;
        this.progress = 0;
    }
}

public static class TaskUtils {

}
