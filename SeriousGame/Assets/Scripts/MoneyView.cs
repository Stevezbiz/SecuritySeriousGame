using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyView : MonoBehaviour {
    [SerializeField] GameManager gameManager;
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

        moneyGainText.SetText(gameManager.GetMoneyGain().ToString());
        val = (1 - gameManager.GetAttackMoneyMalus()) * 100;
        attackMoneyMalusText.SetText("- " + val.ToString("0.##"));
        moneyMalusText.SetText("- " + gameManager.GetMoneyMalus().ToString());
        val = gameManager.GetActualMoneyGain();
        if (val > 0) actualMoneyGainText.SetText(val.ToString("0.##"));
        else actualMoneyGainText.SetText("- " + (-val).ToString("0.##"));

        usersGainText.SetText(gameManager.GetUsersGain().ToString());
        val = (gameManager.GetUsersMod() - 1) * 100;
        if (val > 0) usersModText.SetText(val.ToString("0.##"));
        else usersModText.SetText("- " + (-val).ToString("0.##"));
        attackUsersMalusText.SetText("- " + gameManager.GetAttackUsersMalus().ToString());
        val = gameManager.GetActualUsersGain();
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
