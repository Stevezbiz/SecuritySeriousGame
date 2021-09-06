using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackInfo {
    public int id;
    public string name;
    public string description;
    public float moneyLoss;
    public float usersLoss;
    public float moneyMalus;
    public float usersMalus;
    public float reputationMalus;
    public float maxTime;
    public float duration;
}

[System.Serializable]
public class AttacksJSON {
    public AttackInfo[] attacks;
}

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
        }
        StartCoroutine(ExecuteAttack());
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

    IEnumerator ExecuteAttack() {
        // choose first attack
        int nextAttack = Random.Range(0, attacks.Count);
        float maxTime = attacks[nextAttack].maxTime * gui.GetEndurance(nextAttack);
        float nextTime = maxTime - Random.Range(0, 0.5f * maxTime);

        while (true) {
            // wait for the attack
            yield return new WaitForSeconds(nextTime);

            // launch the attack
            attackMonitor.LaunchAttack(nextAttack);

            // choose the next attack
            nextAttack = Random.Range(0, attacks.Count);
            maxTime = attacks[nextAttack].maxTime * gui.GetEndurance(nextAttack);
            nextTime = maxTime - Random.Range(0, 0.5f * maxTime);
        }
    }
}
