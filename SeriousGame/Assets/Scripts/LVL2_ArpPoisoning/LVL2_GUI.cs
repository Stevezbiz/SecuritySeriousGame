using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2_GUI : MonoBehaviour {
    LVL2_Network network;
    [SerializeField] GameObject messageList;
    [SerializeField] GameObject notes;
    [SerializeField] GameObject objective;

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL2_Network>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void MessageButton() {
        Time.timeScale = 0;
        gameObject.SetActive(false);
        messageList.SetActive(true);
    }

    public void NotesButton() {
        Time.timeScale = 0;
        gameObject.SetActive(false);
        notes.SetActive(true);
    }

    public void ObjectiveButton() {
        Time.timeScale = 0;
        gameObject.SetActive(false);
        objective.SetActive(true);
    }

    public void ResumeGame(GameObject toHide) {
        Time.timeScale = 1;
        gameObject.SetActive(true);
        toHide.SetActive(false);
    }
}
