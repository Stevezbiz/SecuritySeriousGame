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

public enum EmployeeCode {
    NONE,
    LUIGI,
    ANTONIO,
    SARA,
    GIULIA,
    MIRKO,
    MELISSA,
    AGATA,
    BRUNO
}

[System.Serializable]
public class EmployeeInfo {
    public EmployeeCode id;
    public string name;
    public string description;
    public float moneyGain;
    public bool owned;
    public TaskType status;
    public EmployeeAbility[] abilities;

    public float GetMoneyGain() {
        switch (status) {
            case TaskType.NONE:
                return moneyGain;
            case TaskType.INSTALL:
                return 0.5f * moneyGain;
            case TaskType.REPAIR:
                return 0f;
            case TaskType.UPGRADE:
                return 0.5f * moneyGain;
            case TaskType.PREVENT:
                return 0.5f * moneyGain;
            default:
                Debug.Log("Error: undefined employeeStatus");
                return 0f;
        }
    }
}

[System.Serializable]
public class EmployeeRecap {
    public EmployeeCode id;
    public bool owned;
    public TaskType status;

    public EmployeeRecap(EmployeeCode id, bool owned, TaskType status) {
        this.id = id;
        this.owned = owned;
        this.status = status;
    }
}

[System.Serializable]
public class EmployeeAbility {
    public CategoryCode category;
    public int level;
}

[System.Serializable]
public class EmployeesJSON {
    public EmployeeInfo[] employees;
}

public static class EmployeeUtils {
    public static Dictionary<EmployeeCode, EmployeeInfo> LoadFromFile(TextAsset file) {
        Dictionary<EmployeeCode, EmployeeInfo> employees = new Dictionary<EmployeeCode, EmployeeInfo>();
        EmployeesJSON empployeesJSON = JsonUtility.FromJson<EmployeesJSON>(file.text);
        foreach (EmployeeInfo e in empployeesJSON.employees) {
            employees.Add(e.id, e);
        }
        return employees;
    }

    public static void UpdateEmployees(Dictionary<EmployeeCode, EmployeeInfo> employees, EmployeeRecap[] er) {
        foreach (EmployeeRecap e in er) {
            employees[e.id].owned = e.owned;
            employees[e.id].status = e.status;
        }
    }

    public static EmployeeRecap[] GetEmployeeRecap(Dictionary<EmployeeCode, EmployeeInfo> employees) {
        List<EmployeeRecap> er = new List<EmployeeRecap>();

        foreach (EmployeeInfo e in employees.Values) {
            er.Add(new EmployeeRecap(e.id, e.owned, e.status));
        }

        return er.ToArray();
    }

    public static Dictionary<CategoryCode, float> GetAbilities(EmployeeAbility[] a) {
        Dictionary<CategoryCode, float> abilities = new Dictionary<CategoryCode, float>();

        foreach (EmployeeAbility ability in a) {
            abilities.Add(ability.category, ability.level);
        }

        return abilities;
    }

    public static int GetAbility(EmployeeAbility[] abilities, CategoryCode c) {
        foreach (EmployeeAbility a in abilities) {
            if (a.category == c) return a.level;
        }
        Debug.Log("Error: unexpected Category");
        return -1;
    }

    public static List<EmployeeInfo> GetHiredEmployees(Dictionary<EmployeeCode, EmployeeInfo> e) {
        List<EmployeeInfo> employees = new List<EmployeeInfo>();

        foreach (EmployeeInfo el in e.Values) {
            if (el.owned) employees.Add(el);
        }
        return employees;
    }

    public static bool CheckEmployeeAvailability(Dictionary<EmployeeCode, EmployeeInfo> employees) {
        foreach (EmployeeInfo e in employees.Values) {
            if (e.owned && e.status == TaskType.NONE) return true;
        }
        return false;
    }

    public static List<EmployeeInfo> GetAvailableEmployees(Dictionary<EmployeeCode, EmployeeInfo> e) {
        List<EmployeeInfo> employees = new List<EmployeeInfo>();

        foreach (EmployeeInfo el in e.Values) {
            if (el.owned && el.status == TaskType.NONE) employees.Add(el);
        }

        return employees;
    }

    public static EmployeeCode ChooseNewEmployee(Dictionary<EmployeeCode, EmployeeInfo> employees) {
        List<EmployeeCode> candidates = new List<EmployeeCode>();
        foreach (EmployeeInfo e in employees.Values) {
            if (!e.owned) candidates.Add(e.id);
        }
        if (candidates.Count == 0) return EmployeeCode.NONE;
        return candidates[Random.Range(0, candidates.Count)];
    }
}