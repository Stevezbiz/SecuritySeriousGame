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
    [SerializeField] GameObject bottomPanel;

    float oldTimeScale = 1f;

    /**
     * <summary>Populate the view with the values to show</summary>
     */
    void Load() {
        float val;
        // set the money part
        moneyGainText.SetText(gameManager.GetMoneyGain().ToString());
        val = gameManager.GetAttackMoneyMalus() * 100;
        attackMoneyMalusText.SetText("- " + val.ToString("0."));
        moneyMalusText.SetText("- " + gameManager.GetMoneyMalus().ToString());
        val = gameManager.GetActualMoneyGain();
        if (val >= 0) actualMoneyGainText.SetText(val.ToString("0."));
        else actualMoneyGainText.SetText("- " + (-val).ToString("0."));
        // set the users part
        usersGainText.SetText(NumUtils.NumToString(gameManager.GetUsersGain()));
        val = (gameManager.GetUsersMod() - 1) * 100;
        if (val >= 0) usersModText.SetText("+ " + NumUtils.NumToString(val));
        else usersModText.SetText("- " + NumUtils.NumToString(-val));
        attackUsersMalusText.SetText("- " + gameManager.GetAttackUsersMalus().ToString());
        val = gameManager.GetActualUsersGain();
        if (val >= 0) actualUsersGainText.SetText(NumUtils.NumToString(val));
        else actualUsersGainText.SetText("- " + NumUtils.NumToString(-val));
    }

    /**
     * <summary>Open the view</summary>
     */
    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0;
        Load();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        Time.timeScale = oldTimeScale;
        gameObject.SetActive(false);
        bottomPanel.SetActive(true);
    }

    /**
     * <summary>Open a window with a brief explanation of the value</summary>
     */
    public void MoneyGainButton() {
        gameManager.DisplayMessage("Guadagno utenti: numero di fondi guadagnati in base al numero di utenti che utilizzano il servizio", ActionCode.CONTINUE, Role.CEO);
    }

    /**
     * <summary>Open a window with a brief explanation of the value</summary>
     */
    public void MoneyMalusButton() {
        gameManager.DisplayMessage("Costi: fondi spesi per mantenere attivi servizi e difese", ActionCode.CONTINUE, Role.CEO);
    }

    /**
     * <summary>Open a window with a brief explanation of the value</summary>
     */
    public void AttackMoneyMalusButton() {
        gameManager.DisplayMessage("Malus attacchi: riduzione applicata al guadagno utenti, dovuta agli attacchi in corso", ActionCode.CONTINUE, Role.CEO);
    }

    /**
         * <summary>Open a window with a brief explanation of the value</summary>
         */
    public void UsersGainButton() {
        gameManager.DisplayMessage("Nuovi utenti: numero di nuovi utenti che si iscrivono al servizio", ActionCode.CONTINUE, Role.CEO);
    }

    /**
         * <summary>Open a window with a brief explanation of the value</summary>
         */
    public void UsersModButton() {
        gameManager.DisplayMessage("Modificatore usabilità: modificatore applicato al numero di nuovi utenti. Indica il grado di semplicità/difficoltà che gli utenti incontrano nell'utilizzare il servizio", ActionCode.CONTINUE, Role.CEO);
    }

    /**
         * <summary>Open a window with a brief explanation of the value</summary>
         */
    public void AttackUsersMalusButton() {
        gameManager.DisplayMessage("Malus attacchi: riduzione applicata al numero di nuovi utenti, dovuta agli attacchi in corso", ActionCode.CONTINUE, Role.CEO);
    }
}
