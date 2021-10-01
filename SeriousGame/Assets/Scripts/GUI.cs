using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Image = UnityEngine.UI.Image;
using DateTime = System.DateTime;

public class GUI : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI usersText;
    [SerializeField] TextMeshProUGUI reputationText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Image reputationBar;

    /**
     * <summary>Displays the current (updated) values in the GUI</summary>
     */
    public void Refresh(string money, string users, float reputation, DateTime dateTime) {
        moneyText.SetText(money);
        usersText.SetText(users);
        reputationText.SetText(Mathf.FloorToInt(reputation * 100).ToString() + "%");
        reputationBar.fillAmount = reputation;
        dateText.SetText(dateTime.ToString("d MMM yyyy"));
        timeText.SetText(dateTime.ToString("HH:mm"));
    }
}
