using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI usersText;
    [SerializeField] TextMeshProUGUI reputationText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Shop shop;
    [SerializeField] AttacksManager attacksManager;
    [SerializeField] float money;
    [SerializeField] float users;
    [SerializeField] float reputation;
    [SerializeField] float moneyGain;

    DateTime dateTime;
    float startTime;
    int updateTime = 1;

    float[,] coeffs = new float[,] {
        { .5f, 0f },
        { .2f, 28f },
        { .08f, 145f },
        { .03f, 642f },
        { .012f, 2606f },
        { .004f, 10060f },
    };

    float moneyMalus = 0f;
    float usersMalus = 1f;
    float endurance = 1f;

    // Start is called before the first frame update
    void Start() {
        startTime = Time.time;
        Time.timeScale = 1;
        DateTime dt = DateTime.Now.AddMonths(1);
        dateTime = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
        Refresh();
    }

    // Update is called once per frame
    void Update() {
        // update the GUI every second
        if (Time.time - startTime >= updateTime) {
            updateTime++;
            money = CalculateMoney();
            users = CalculateUsers();
            dateTime = dateTime.AddHours(4);
            Refresh();
        }
    }

    void Refresh() {
        moneyText.SetText(Math.Floor(money).ToString());
        usersText.SetText(Math.Floor(users).ToString());
        reputationText.SetText(reputation.ToString());
        dateText.SetText(dateTime.ToString("d MMM yyyy"));
        timeText.SetText(dateTime.ToString("HH:mm"));
    }

    public void Purchase(int id) {
        money -= shop.Item(id).cost;
        Enable(id);
        shop.Item(id).owned = true;
        Refresh();
    }

    public void Enable(int id) {
        moneyMalus += shop.Item(id).moneyMalus;
        usersMalus *= shop.Item(id).usersMalus;
        shop.Item(id).on = true;
    }

    public void Disable(int id) {
        moneyMalus -= shop.Item(id).moneyMalus;
        usersMalus /= shop.Item(id).usersMalus;
        shop.Item(id).on = false;
    }

    public float GetEndurance(int id) {
        return endurance;
    }

    public float GetDuration(int id) {
        return attacksManager.Attack(id).duration;
    }

    float CalculateMoney() {
        int i = 0;
        float m = money;
        while (m > 100) {
            m /= 10;
            i++;
        }
        //Debug.Log("Costs: " + moneyMalus * (coeffs[i, 0] * (float)Math.Floor(users) + coeffs[i, 1]) + "\nDiff: " + (moneyGain * (float)Math.Floor(users) - moneyMalus * (coeffs[i, 0] * (float)Math.Floor(users) + coeffs[i, 1])));
        return money + moneyGain * (float)Math.Floor(users) - moneyMalus * (coeffs[i, 0] * (float)Math.Floor(users) + coeffs[i, 1]);
    }

    float CalculateUsers() {
        return users + 0.01f * reputation * usersMalus * users;
    }
}
