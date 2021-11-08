using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmployeeCode {
    LUIGI,
    ANTONIO,
    SARA,
    GIULIA
}

public enum EmployeeStatus {
    WORK,
    PREVENTION,
    REPAIR,
    UPGRADE
}

[System.Serializable]
public class EmployeeInfo {
    public EmployeeCode id;
    public string name;
    public string description;
    public float moneyGain;
    public bool owned;
    public EmployeeStatus status;
    public EmployeeAbility[] abilities;
}

[System.Serializable]
public class EmployeeRecap {
    public EmployeeCode id;
    public bool owned;
    public EmployeeStatus status;

    public EmployeeRecap(EmployeeCode id, bool owned, EmployeeStatus status) {
        this.id = id;
        this.owned = owned;
        this.status = status;
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

    public static Dictionary<ShopItemCategory, float> GetAbilities(EmployeeAbility[] a) {
        Dictionary<ShopItemCategory, float> abilities = new Dictionary<ShopItemCategory, float>();

        foreach (EmployeeAbility ability in a) {
            abilities.Add(ability.category, ability.level);
        }

        return abilities;
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
            if (e.owned && e.status == EmployeeStatus.WORK) return true;
        }
        return false;
    }

    public static List<EmployeeInfo> GetAvailableEmployees(Dictionary<EmployeeCode, EmployeeInfo> e) {
        List<EmployeeInfo> employees = new List<EmployeeInfo>();

        foreach(EmployeeInfo el in e.Values) {
            if (el.owned && el.status == EmployeeStatus.WORK) employees.Add(el);
        }

        return employees;
    }
}