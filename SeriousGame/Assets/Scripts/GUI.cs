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
using DateTime = System.DateTime;
using Image = UnityEngine.UI.Image;
using Math = System.Math;

public class GUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI usersText;
    [SerializeField] TextMeshProUGUI reputationText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI clockText;
    [SerializeField] Image reputationBar;
    [SerializeField] TextMeshProUGUI attackTrendText;

    /**
     * <summary>Displays the current (updated) values in the GUI</summary>
     */
    public void Refresh(float money, float users, float reputation, DateTime dateTime) {
        moneyText.SetText(Math.Round(money).ToString());
        usersText.SetText(NumUtils.NumToString(Math.Round(users)));
        reputationText.SetText(Mathf.FloorToInt(reputation * 100).ToString() + "%");
        reputationBar.fillAmount = reputation;
        dateText.SetText(dateTime.ToString("d MMM yyyy"));
        timeText.SetText(dateTime.ToString("HH:mm"));
        clockText.SetText(dateTime.ToString("HH:mm"));
    }

    /**
     * <summary></summary>
     */
    public void SetNewTrend(string attack) {
        attackTrendText.SetText(attack);
    }
}
