using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI usersText;
    [SerializeField] TextMeshProUGUI reputationText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] float money;
    [SerializeField] float users;
    [SerializeField] float reputation;
    [SerializeField] float moneyGain;
    [SerializeField] float usersGain;

    DateTime dateTime;

    int updateTime = 1;

    // Start is called before the first frame update
    void Start() {
        DateTime dt = DateTime.Now.AddDays(1);
        dateTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0, DateTimeKind.Local);
        Refresh();
    }

    // Update is called once per frame
    void Update() {
        // update the GUI every second
        if (Time.time >= updateTime) {
            updateTime++;
            money += moneyGain * (float)Math.Floor(users);
            users += usersGain;
            dateTime = dateTime.AddHours(4);
            usersGain = 0.01f * reputation * users;
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

    public ECode Purchase(int cost) {
        if (money >= cost) {
            money -= cost;
            Refresh();
            return ECode.OK;
        }
        return ECode.INSUFFICIENT_MONEY;
    }
}
