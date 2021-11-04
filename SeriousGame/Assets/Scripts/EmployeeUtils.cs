using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmployeeCode {
    LUIGI,
    ANTONIO,
    SARA,
    GIULIA
}

[System.Serializable]
public class EmployeeInfo {
    public EmployeeCode id;
    public string name;
    public string description;
    public float moneyGain;
    public bool owned;
    public EmployeeAbility[] abilities;
}

[System.Serializable]
public class EmployeeRecap {
    public EmployeeCode id;
    public bool owned;

    public EmployeeRecap(EmployeeCode id, bool owned) {
        this.id = id;
        this.owned = owned;
    }
}

[System.Serializable]
public class EmployeeAbility {
    public ShopItemCategory category;
    public int level;
}

[System.Serializable]
public class EmployeesJSON {
    public EmployeeInfo[] employees;
}

public static class EmployeeUtils {
    public static void UpdateEmployees(Dictionary<EmployeeCode, EmployeeInfo> employees, EmployeeRecap[] er) {
        foreach (EmployeeRecap e in er) {
            employees[e.id].owned = e.owned;
        }
    }

    public static EmployeeRecap[] GetEmployeeRecap(Dictionary<EmployeeCode, EmployeeInfo> employees) {
        List<EmployeeRecap> er = new List<EmployeeRecap>();

        foreach (EmployeeInfo e in employees.Values) {
            er.Add(new EmployeeRecap(e.id, e.owned));
        }

        return er.ToArray();
    }

    public static Dictionary<ShopItemCategory, float> GetAbilities(EmployeeAbility[] a) {
        Dictionary<ShopItemCategory, float> abilities = new Dictionary<ShopItemCategory, float>();

        foreach (EmployeeAbility ability in a) {
            abilities.Add(ability.category, ability.level);
        }

        return abilities;
    }
}