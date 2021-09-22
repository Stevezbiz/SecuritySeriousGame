using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyView : MonoBehaviour {
    [SerializeField] GUI gui;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] TextMeshProUGUI moneyMalusText;
    [SerializeField] TextMeshProUGUI attackMoneyMalusText;
    [SerializeField] TextMeshProUGUI actualMoneyGainText;
    [SerializeField] TextMeshProUGUI usersGainText;
    [SerializeField] TextMeshProUGUI usersModText;
    [SerializeField] TextMeshProUGUI attackUsersMalusText;
    [SerializeField] TextMeshProUGUI actualUsersGainText;

    float oldTimeScale = 1f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void Load() {
        float val;

        moneyGainText.SetText(gui.GetMoneyGain().ToString());
        val = (1 - gui.GetAttackMoneyMalus()) * 100;
        attackMoneyMalusText.SetText("- " + val.ToString("0.##"));
        moneyMalusText.SetText("- " + gui.GetMoneyMalus().ToString());
        val = gui.GetActualMoneyGain();
        if (val > 0) actualMoneyGainText.SetText(val.ToString("0.##"));
        else actualMoneyGainText.SetText("- " + (-val).ToString("0.##"));

        usersGainText.SetText(gui.GetUsersGain().ToString());
        val = (gui.GetUsersMod() -1) * 100;
        if (val > 0) usersModText.SetText(val.ToString("0.##"));
        else usersModText.SetText("- " + (-val).ToString("0.##"));
        attackUsersMalusText.SetText("- " + gui.GetAttackUsersMalus().ToString());
        val = gui.GetActualUsersGain();
        if (val > 0) actualUsersGainText.SetText(val.ToString());
        else actualUsersGainText.SetText("- " + (-val).ToString());
    }

    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Load();
        gameObject.SetActive(true);
    }

    public void CloseView() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
    }
}
