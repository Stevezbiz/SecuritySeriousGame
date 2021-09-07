using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMonitor : MonoBehaviour {
    [SerializeField] GameObject warningImage;
    [SerializeField] AttacksManager attacksManager;
    [SerializeField] GUI gui;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClick() {

    }

    public void LaunchAttack(int id) {
        StartCoroutine(PerformAttack(id));
    }

    IEnumerator PerformAttack(int id) {
        //Debug.Log("Attack @" + Time.time);
        warningImage.SetActive(true);
        gui.StartAttack(id);
        yield return new WaitForSeconds(gui.GetDuration(id));
        gui.StopAttack(id);
        warningImage.SetActive(false);
    }
}
