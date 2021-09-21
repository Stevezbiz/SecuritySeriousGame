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

    public void LaunchAttack(int id, float duration) {
        StartCoroutine(PerformAttack(id, duration));
    }

    IEnumerator PerformAttack(int id, float duration) {
        warningImage.SetActive(true);
        gui.StartAttack(id);
        yield return new WaitForSeconds(duration);
        gui.StopAttack(id);
        warningImage.SetActive(false);
    }
}
