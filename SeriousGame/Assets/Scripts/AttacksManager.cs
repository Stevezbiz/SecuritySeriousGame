using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksManager : MonoBehaviour {
    [SerializeField] TextAsset attacksFileJSON;
    [SerializeField] GUI gui;
    [SerializeField] AttackMonitor attackMonitor;
    [SerializeField] LogManager logManager;

    Dictionary<int, AttackInfo> attacks = new Dictionary<int, AttackInfo>();
    Dictionary<int, Resistance> resistances = new Dictionary<int, Resistance>();

    float endurance = 1f;
    float miss = 0f;

    // Start is called before the first frame update
    void Start() {
        AttacksJSON attacksContent = JsonUtility.FromJson<AttacksJSON>(attacksFileJSON.text);
        foreach (AttackInfo attack in attacksContent.attacks) {
            AddAttack(attack);
            AddResistance(attack.id);
            StartCoroutine(ExecuteAttack(attack.id));
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public AttackInfo Attack(int id) {
        return attacks[id];
    }

    void AddAttack(AttackInfo attack) {
        attacks.Add(attack.id, attack);
    }

    void AddResistance(int id) {
        if (!resistances.ContainsKey(id)) resistances.Add(id, new Resistance(id, 0f, 0f, 0f));
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
        float nextTime = maxTime - Random.Range(0, 0.5f * maxTime);

        while (true) {
            // wait for the attack
            yield return new WaitForSeconds(nextTime);

            // launch the attack if hits
            if (Random.Range(0f, 1f) > GetMiss(id)) {
                // hit
                attackMonitor.LaunchAttack(id, GetDuration(id));
                // log print hit
                logManager.LogPrint(attacks[id].name, true);
            } else {
                // miss
                gui.MissedAttack();
                //Debug.Log("Attack @" + Time.time + " missed");
                // log print miss
                logManager.LogPrint(attacks[id].name, false);
            }

            // choose the time for the next attack
            maxTime = attacks[id].maxTime * GetEndurance(id);
            nextTime = maxTime - Random.Range(0, 0.5f * maxTime);
        }
    }

    public void EnableShopItem(Resistance[] res) {
        foreach (Resistance r in res) {
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
