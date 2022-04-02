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
        TimeManager.Pause();
        Load();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the view</summary>
     */
    public void CloseView() {
        TimeManager.Resume();
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
