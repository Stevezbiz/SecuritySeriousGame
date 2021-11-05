using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmployeeView : MonoBehaviour {
    [SerializeField] GameObject bottomPanel;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeRecord;
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI nameText;

    float oldTimeScale = 1f;
    EmployeeCode selected;
    List<GameObject> toDestroy;

    public void Load() {
        bool first = true;
        toDestroy = new List<GameObject>();
        foreach(EmployeeInfo e in gameManager.GetHiredEmployees()) {
            AddEmployeeRecord(e);
            if (first) {
                Select(e.id);
                first = false;
            }
        }
    }

    public void AddEmployeeRecord(EmployeeInfo e) {
        GameObject newRecord = Instantiate(employeeRecord, new Vector3(0, 0, 0), Quaternion.identity);
        newRecord.transform.SetParent(content, false);
        newRecord.GetComponent<EmployeeRecord>().Load(e, gameManager, this);
        toDestroy.Add(newRecord);
    }

    public void Select(EmployeeCode id) {
        selected = id;
        nameText.SetText("Assegna " + gameManager.GetEmployee(id).name + " a:");
    }

    public void OpenView() {
        oldTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Load();
        bottomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void CloseView() {
        Time.timeScale = oldTimeScale;
        foreach(GameObject o in toDestroy) {
            Destroy(o);
        }
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
