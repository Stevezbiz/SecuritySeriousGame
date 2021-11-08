using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    UPGRADE
}

[SerializeField]
public class Task {
    public TaskType type;
    public bool assigned;
    public EmployeeCode executor;
    public ShopItemCode shopItem;
    public int duration;

    public Task(TaskType type, ShopItemCode shopItem) {
        this.type = type;
        this.assigned = false;
        this.shopItem = shopItem;
    }

    public void AssignEmployee(EmployeeCode executor, int duration) {
        this.assigned = true;
        this.executor = executor;
        this.duration = duration;
    }
}

public static class TaskUtils {

}
