using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DateTime = System.DateTime;
using DateTimeKind = System.DateTimeKind;
using Image = UnityEngine.UI.Image;

public class GUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] TextMeshProUGUI usersText;
    [SerializeField] TextMeshProUGUI reputationText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Image reputationBar;
    [SerializeField] GameObject windowPopUp;
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

    Dictionary<int, float> resistances = new Dictionary<int, float>();

    float moneyMalus = 0f;
    float usersMalus = 1f;
    float attackMalus = 0f;
    float endurance = 1f;
    int negativeTime = 0;
    int maxNegative = 60;

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
            if (money < 0) {
                negativeTime++;
            } else {
                negativeTime = 0;
            }
            if(negativeTime > maxNegative) {
                // end the game
                Time.timeScale = 0;
                Vector3 newPos = new Vector3(0, 0, 0);
                GameObject newWindow = Instantiate(windowPopUp, newPos, Quaternion.identity);
                newWindow.transform.SetParent(gameObject.transform, false);
                newWindow.GetComponent<WindowPopUp>().Message = "GAME OVER";
            } else {
                Refresh();
            }
            
        }
    }

    void Refresh() {
        moneyText.SetText(Mathf.Floor(money).ToString());
        usersText.SetText(Mathf.Floor(users).ToString());
        reputationText.SetText(Mathf.FloorToInt(reputation * 100).ToString() + "%");
        reputationBar.fillAmount = reputation;
        dateText.SetText(dateTime.ToString("d MMM yyyy"));
        timeText.SetText(dateTime.ToString("HH:mm"));
    }

    public void Purchase(int id) {
        money -= shop.Item(id).cost;
        EnableShopItem(id);
        shop.Item(id).owned = true;
        Refresh();
    }

    public void EnableShopItem(int id) {
        moneyMalus += shop.Item(id).moneyMalus;
        usersMalus *= shop.Item(id).usersMalus;
        shop.Item(id).on = true;
    }

    public void DisableShopItem(int id) {
        moneyMalus -= shop.Item(id).moneyMalus;
        usersMalus /= shop.Item(id).usersMalus;
        shop.Item(id).on = false;
    }

    public float GetEndurance(int id) {
        return endurance;
    }

    public float GetDuration(int id) {
        return attacksManager.Attack(id).duration + 1;
    }

    public void StartAttack(int id) {
        money -= attacksManager.Attack(id).moneyLoss;
        users -= attacksManager.Attack(id).usersLoss;
        attackMalus += attacksManager.Attack(id).usersMalus;
        moneyMalus += attacksManager.Attack(id).moneyMalus;
    }

    public void StopAttack(int id) {
        attackMalus -= attacksManager.Attack(id).usersMalus;
        moneyMalus -= attacksManager.Attack(id).moneyMalus;
    }

    float CalculateMoney() {
        int i = 0;
        float m = money;
        while (m > 100) {
            m /= 10;
            i++;
        }
        //Debug.Log("Costs: " + moneyMalus * (coeffs[i, 0] * (float)Math.Floor(users) + coeffs[i, 1]) + "\nDiff: " + (moneyGain * (float)Math.Floor(users) - moneyMalus * (coeffs[i, 0] * (float)Math.Floor(users) + coeffs[i, 1])));
        return money + moneyGain * (float)Mathf.Floor(users) - moneyMalus * (coeffs[i, 0] * (float)Mathf.Floor(users) + coeffs[i, 1]);
    }

    float CalculateUsers() {
        return users + 0.01f * (reputation * usersMalus - attackMalus) * (float)Mathf.Floor(users);
    }
}
