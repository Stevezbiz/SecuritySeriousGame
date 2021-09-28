using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksManager : MonoBehaviour {
    [SerializeField] GameObject windowPopUp;
    [SerializeField] TextAsset attacksFileJSON;
    [SerializeField] GUI gui;
    [SerializeField] LogManager logManager;

    Dictionary<int, AttackInfo> attacks = new Dictionary<int, AttackInfo>();
    Dictionary<int, Resistance> resistances = new Dictionary<int, Resistance>();
    Dictionary<int, AttackStats> stats = new Dictionary<int, AttackStats>();

    float endurance = 1f;
    float miss = 0f;

    // Start is called before the first frame update
    void Start() {
        AttacksJSON attacksContent = JsonUtility.FromJson<AttacksJSON>(attacksFileJSON.text);
        foreach (AttackInfo attack in attacksContent.attacks) {
            attacks.Add(attack.id, attack);
            if (!resistances.ContainsKey(attack.id)) resistances.Add(attack.id, new Resistance(attack.id, 0f, 0f, 0f));
            stats.Add(attack.id, new AttackStats(attack.id, 0, 0, 0));
            StartCoroutine(ExecuteAttack(attack.id));
        }
    }

    public AttackInfo Attack(int id) {
        return attacks[id];
    }

    float GetMiss(int id) {
        return miss + resistances[id].miss;
    }

    float GetDuration(int id) {
        return (1 - resistances[id].duration) * attacks[id].duration + 1;
    }

    float GetEndurance(int id) {
        return endurance + resistances[id].endurance;
    }

    IEnumerator ExecuteAttack(int id) {
        // choose first attack
        float maxTime = attacks[id].maxTime * GetEndurance(id);
        float nextTime = Random.Range(0.5f * maxTime, maxTime);

        while (true) {
            // wait for the attack
            yield return new WaitForSeconds(nextTime);

            stats[id].n++;
            // launch the attack if hits
            if (Random.Range(0f, 1f) > GetMiss(id)) {
                // hit
                stats[id].hit++;
                LaunchAttack(id, GetDuration(id));
                GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
                newWindow.transform.SetParent(gameObject.transform, false);
                newWindow.GetComponent<WindowPopUp>().Message = "Individuato attacco " + attacks[id].name + "! " + attacks[id].description;
                // log print hit
                logManager.LogPrintAttack(attacks[id].name, true);
            } else {
                // miss
                stats[id].miss++;
                gui.MissedAttack();
                GameObject newWindow = Instantiate(windowPopUp, new Vector3(0, 0, 0), Quaternion.identity);
                newWindow.transform.SetParent(gameObject.transform, false);
                newWindow.GetComponent<WindowPopUp>().Message = "Le nostre difese hanno sventato un tentativo di attacco " + attacks[id].name;
                // log print miss
                logManager.LogPrintAttack(attacks[id].name, false);
            }

            // choose the time for the next attack
            maxTime = attacks[id].maxTime * GetEndurance(id);
            nextTime = Random.Range(0.5f * maxTime, maxTime);
        }
    }

    public void LaunchAttack(int id, float duration) {
        StartCoroutine(PerformAttack(id, duration));
    }

    IEnumerator PerformAttack(int id, float duration) {
        gui.StartAttack(id);
        yield return new WaitForSeconds(duration);
        gui.StopAttack(id);
    }

    public void EnableShopItem(Resistance[] res) {
        foreach (Resistance r in res) {
            if (!resistances.ContainsKey(r.id)) resistances.Add(r.id, new Resistance(r.id, 0f, 0f, 0f));
            resistances[r.id].miss += r.miss;
            resistances[r.id].duration += r.duration;
            resistances[r.id].endurance += r.endurance;
        }
    }

    public void DisableShopItem(Resistance[] res) {
        foreach (Resistance r in res) {
            resistances[r.id].miss -= r.miss;
            resistances[r.id].duration -= r.duration;
            resistances[r.id].endurance -= r.endurance;
        }
    }
}
