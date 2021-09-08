using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksManager : MonoBehaviour {
    [SerializeField] TextAsset attacksFileJSON;
    [SerializeField] GUI gui;
    [SerializeField] AttackMonitor attackMonitor;

    Dictionary<int, AttackInfo> attacks = new Dictionary<int, AttackInfo>();

    // Start is called before the first frame update
    void Start() {
        AttacksJSON attacksContent = JsonUtility.FromJson<AttacksJSON>(attacksFileJSON.text);
        foreach (AttackInfo attack in attacksContent.attacks) {
            AddAttack(attack);
            gui.AddResistance(attack.id);
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

    IEnumerator ExecuteAttack(int id) {
        // choose first attack
        float maxTime = attacks[id].maxTime * gui.GetEndurance(id);
        float nextTime = maxTime - Random.Range(0, 0.5f * maxTime);

        while (true) {
            // wait for the attack
            yield return new WaitForSeconds(nextTime);

            // launch the attack if hits
            if(Random.Range(0f,1f) > gui.GetMiss(id)) {
                // hit
                attackMonitor.LaunchAttack(id);
            } else {
                // miss
                gui.MissedAttack();
                Debug.Log("Attack @" + Time.time + " missed");
            }

            // choose the time for the next attack
            maxTime = attacks[id].maxTime * gui.GetEndurance(id);
            nextTime = maxTime - Random.Range(0, 0.5f * maxTime);
        }
    }
}
