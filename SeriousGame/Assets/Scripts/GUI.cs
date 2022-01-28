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
    [SerializeField] Image reputationBar;

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
    }
}
