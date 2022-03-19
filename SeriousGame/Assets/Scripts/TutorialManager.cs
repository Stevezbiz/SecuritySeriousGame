using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour {
    [SerializeField] List<GameObject> tutorialPages;
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI description1;
    [SerializeField] Shop shop;
    [SerializeField] ShopItemDetail shopItemDetail;
    [SerializeField] EmployeeView employeeView;
    [SerializeField] TaskList taskList;
    [SerializeField] SecurityView securityView;
    [SerializeField] MoneyView moneyView;
    [SerializeField] Log log;
    [SerializeField] RectTransform taskListContent;
    [SerializeField] RectTransform employeeChoiceContent;

    public void Load() {
        gameObject.SetActive(true);
        Page1();
    }

    public void EndTutorial() {
        gameManager.EndTutorial();
        Destroy(gameObject);
    }

    void Page1() {
        // welcome message
        tutorialPages[0].SetActive(true);
        description1.SetText("Benvenuto/a " + IOUtils.player + "!");
    }

    public void Page2() {
        tutorialPages[1].SetActive(true);
        tutorialPages[0].SetActive(false);
    }

    public void Page3() {
        tutorialPages[2].SetActive(true);
        tutorialPages[1].SetActive(false);
    }

    public void Page4() {
        tutorialPages[3].SetActive(true);
        tutorialPages[2].SetActive(false);
    }

    public void Page5() {
        tutorialPages[4].SetActive(true);
        tutorialPages[3].SetActive(false);
    }

    public void Page6() {
        tutorialPages[5].SetActive(true);
        tutorialPages[4].SetActive(false);
    }

    public void Page7() {
        tutorialPages[6].SetActive(true);
        tutorialPages[5].SetActive(false);
    }

    public void Page8() {
        // open shop
        shop.OpenShop();
        tutorialPages[7].SetActive(true);
        tutorialPages[6].SetActive(false);
    }

    public void Page9() {
        // open firewall details
        shopItemDetail.Load(ShopItemCode.FIREWALL, null);
        shopItemDetail.gameObject.SetActive(true);
        tutorialPages[8].SetActive(true);
        tutorialPages[7].SetActive(false);
    }

    public void Page10() {
        // purchase firewall
        shopItemDetail.PurchaseItem();
        tutorialPages[9].SetActive(true);
        tutorialPages[8].SetActive(false);
    }

    public void Page11() {
        // close shop
        shop.CloseShop();
        tutorialPages[10].SetActive(true);
        tutorialPages[9].SetActive(false);
    }

    public void Page12() {
        // open employee view
        employeeView.OpenView();
        tutorialPages[11].SetActive(true);
        tutorialPages[10].SetActive(false);
    }

    public void Page13() {
        // open task list (bottom)
        taskList.OpenView();
        taskListContent.localPosition = new Vector3(taskListContent.localPosition.x, 490f, taskListContent.localPosition.z);
        tutorialPages[12].SetActive(true);
        tutorialPages[11].SetActive(false);
    }

    public void Page14() {
        tutorialPages[13].SetActive(true);
        tutorialPages[12].SetActive(false);
    }

    public void Page15() {
        // open employee choice (bottom)
        employeeView.Load(gameManager.GetInstallTask(ShopItemCode.FIREWALL));
        employeeChoiceContent.localPosition = new Vector3(employeeChoiceContent.localPosition.x, 120f, employeeChoiceContent.localPosition.z);
        tutorialPages[14].SetActive(true);
        tutorialPages[13].SetActive(false);
    }

    public void Page16() {
        // select sara
        employeeView.SelectEmployee(EmployeeCode.SARA);
        tutorialPages[15].SetActive(true);
        tutorialPages[14].SetActive(false);
    }

    public void Page17() {
        // assign task and close employee view
        employeeView.Assign();
        employeeView.CloseView();
        tutorialPages[16].SetActive(true);
        tutorialPages[15].SetActive(false);
    }

    public void Page18() {
        // open security view
        securityView.OpenView();
        tutorialPages[17].SetActive(true);
        tutorialPages[16].SetActive(false);
    }

    public void Page19() {
        tutorialPages[18].SetActive(true);
        tutorialPages[17].SetActive(false);
    }

    public void Page20() {
        tutorialPages[19].SetActive(true);
        tutorialPages[18].SetActive(false);
    }

    public void Page21() {
        // close security view
        securityView.CloseView();
        tutorialPages[20].SetActive(true);
        tutorialPages[19].SetActive(false);
    }

    public void Page22() {
        // open money view
        moneyView.OpenView();
        tutorialPages[21].SetActive(true);
        tutorialPages[20].SetActive(false);
    }

    public void Page23() {
        tutorialPages[22].SetActive(true);
        tutorialPages[21].SetActive(false);
    }

    public void Page24() {
        // close money view
        moneyView.CloseView();
        tutorialPages[23].SetActive(true);
        tutorialPages[22].SetActive(false);
    }

    public void Page25() {
        // open log view
        log.OpenLog();
        tutorialPages[24].SetActive(true);
        tutorialPages[23].SetActive(false);
    }

    public void Page26() {
        // close log view
        log.CloseLog();
        tutorialPages[25].SetActive(true);
        tutorialPages[24].SetActive(false);
    }
}
