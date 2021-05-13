using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Alice : MonoBehaviour {
    [SerializeField] GameObject packet;

    LVL1_Network network;

    List<string> messages = new List<string> {
        "ecco il codice di accesso che mi hai chiesto: 18632745",
        "la nuova password è: 5up3r_53cur3",
        "concordo, Mallory è molto antipatico",
        "il file che cerchi è superSecret.pdf. Per scaricarlo il codice è 87a6jhy6",
        "devo scegliere la nuova password: uso sempre la stessa in realtà, così non la dimentico",
        "12345678 è una password sicura?",
        "Mallory prima ha cercato di leggere i miei messaggi. Che ficcanaso!",
    };

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(collision.gameObject);
    }

    IEnumerator SendToBob() {
        while (true) {
            LVL1_Packet newPacket = Instantiate(packet).GetComponent<LVL1_Packet>();
            newPacket.SetSourceIP("alice");
            newPacket.SetDestIP("bob");
            newPacket.SetSourceMAC("alice");
            newPacket.SetDestMAC("bob");
            newPacket.SetSourcePos(network.GetPos("alice"));
            newPacket.SetDestPos(network.GetPos("switch"));
            newPacket.SetBody(messages[Random.Range(0, messages.Count)]);
            yield return new WaitForSeconds(20f);
        }

    }

    public void BeginLevel() {
        StartCoroutine("SendToBob");
    }
}
