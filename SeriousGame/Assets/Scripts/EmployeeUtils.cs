using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmployeeCode {
    GIGI,
    ANTONIO,
    SARA,
    GIULIA
}

[System.Serializable]
public class EmployeeInfo {
    public EmployeeCode id;
    public string name;
    public string description;
    public int cost;
    public bool owned;
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
}