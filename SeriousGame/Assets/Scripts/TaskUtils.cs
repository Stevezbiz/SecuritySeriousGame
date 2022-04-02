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

public enum TaskType {
    NONE,
    INSTALL,
    REPAIR,
    UPGRADE,
    PREVENT
}

[System.Serializable]
public class Task {
    static int _id = 0;

    public int id;
    public TaskType type;
    public EmployeeCode executor;
    public ShopItemCode shopItem;
    public AttackCode attack;
    public CategoryCode category;
    public int duration;
    public int progress;
    public float protection;

    public Task(Task t) {
        this.id = _id++;
        this.type = t.type;
        this.executor = t.executor;
        this.shopItem = t.shopItem;
        this.attack = t.attack;
        this.category = t.category;
        this.duration = t.duration;
        this.progress = t.progress;
        this.protection = t.protection;
    }

    public Task(TaskType type, ShopItemCode shopItem, CategoryCode category) {
        this.id = _id++;
        this.type = type;
        this.executor = EmployeeCode.NONE;
        this.shopItem = shopItem;
        this.attack = AttackCode.NONE;
        this.category = category;
        this.duration = 0;
        this.progress = 0;
        this.protection = 0f;
    }

    public Task(TaskType type, AttackCode attack, CategoryCode category) {
        this.id = _id++;
        this.type = type;
        this.executor = EmployeeCode.NONE;
        this.shopItem = ShopItemCode.NONE;
        this.attack = attack;
        this.category = category;
        this.duration = 0;
        this.progress = 0;
        this.protection = 0f;
    }

    public Task(TaskType type, CategoryCode category) {
        this.id = _id++;
        this.type = type;
        this.executor = EmployeeCode.NONE;
        this.shopItem = ShopItemCode.NONE;
        this.attack = AttackCode.NONE;
        this.category = category;
        this.duration = 0;
        this.progress = 0;
        this.protection = 0f;
    }

    public void AssignEmployee(EmployeeCode executor, int duration) {
        this.executor = executor;
        this.duration = duration;
        this.progress = 0;
    }

    public void AssignEmployee(EmployeeCode executor, float protection) {
        this.executor = executor;
        this.protection = protection;
        this.progress = 0;
        this.duration = -1;
    }
}

public static class TaskUtils {
    public static void SetupTasks(Dictionary<int, Task> waitingTasks) {
        foreach (CategoryCode c in typeof(CategoryCode).GetEnumValues()) {
            Task t = new Task(TaskType.PREVENT, c);
            waitingTasks[t.id] = t;
        }
    }

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
