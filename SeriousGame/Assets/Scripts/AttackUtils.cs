/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackCode {
    NONE,
    DOS,
    MITM,
    BRUTE_FORCE,
    DICTIONARY,
    RAINBOW_TABLE,
    API_VULNERABILITY,
    SOCIAL_ENGINEERING,
    PHISHING,
    WORM,
    VIRUS,
    SPYWARE,
    RANSOMWARE
}

public enum AttackStatus {
    INACTIVE,
    PLANNING,
    ACTIVE
}

[System.Serializable]
public class Resistance {
    public AttackCode id;
    public float duration;
    public float miss;
    public float endurance;

    public Resistance(AttackCode id, float duration, float miss, float endurance) {
        this.id = id;
        this.duration = duration;
        this.miss = miss;
        this.endurance = endurance;
    }

    public Resistance(Resistance r) {
        this.id = r.id;
        this.duration = r.duration;
        this.miss = r.miss;
        this.endurance = r.endurance;
    }
}

[System.Serializable]
public class ResistanceArray {
    public Resistance[] resistances;
}

[System.Serializable]
public class AttackInfo {
    public AttackCode id;
    public CategoryCode category;
    public string name;
    public string description;
    public float moneyLoss;
    public float usersLoss;
    public float moneyMalus;
    public float usersMalus;
    public float reputationMalus;
    public float maxTime;
    public int duration;
}

[System.Serializable]
public class AttackPlan {
    public AttackCode id;
    public AttackStatus status;
    public int timer;
    public float missMod;

    public AttackPlan(AttackCode id) {
        this.id = id;
        this.status = AttackStatus.INACTIVE;
        this.timer = 0;
        this.missMod = 1f;
    }

    public AttackPlan(AttackPlan ap, int timer) {
        this.id = ap.id;
        this.status = AttackStatus.PLANNING;
        this.timer = timer;
        this.missMod = 1f;
    }
}

[System.Serializable]
public class AttackStats {
    public AttackCode id;
    public int n;
    public int hit;
    public int miss;

    public AttackStats(AttackCode id, int n, int hit, int miss) {
        this.id = id;
        this.n = n;
        this.hit = hit;
        this.miss = miss;
    }
}

[System.Serializable]
public class AttacksJSON {
    public AttackInfo[] attacks;


}

public static class AttackUtils {
    public static Dictionary<AttackCode, AttackInfo> LoadFromFile(TextAsset file) {
        Dictionary<AttackCode, AttackInfo> attacks = new Dictionary<AttackCode, AttackInfo>();

        AttacksJSON attacksJSON = JsonUtility.FromJson<AttacksJSON>(file.text);
        foreach (AttackInfo attack in attacksJSON.attacks) {
            attacks.Add(attack.id, attack);
        }

        return attacks;
    }

    public static void SetupAll(Dictionary<AttackCode, AttackInfo> attacks, Dictionary<AttackCode, Resistance> resistances, Dictionary<AttackCode, AttackStats> attackStats, Dictionary<AttackCode, AttackPlan> attackSchedule) {
        foreach (AttackInfo attack in attacks.Values) {
            if (!resistances.ContainsKey(attack.id)) resistances[attack.id] = new Resistance(attack.id, 1f, 0f, 0f);
            attackStats[attack.id] = new AttackStats(attack.id, 0, 0, 0);
            attackSchedule[attack.id] = new AttackPlan(attack.id);
        }
    }

    public static void UpdateAll(Dictionary<AttackCode, Resistance> resistances, Dictionary<AttackCode, AttackStats> attackStats, Dictionary<AttackCode, AttackPlan> attackSchedule, Resistance[] ress, AttackStats[] aStats, AttackPlan[] aSchedules) {
        foreach (Resistance res in ress) {
            resistances.Add(res.id, res);
        }
        foreach (AttackStats aStat in aStats) {
            attackStats.Add(aStat.id, aStat);
        }
        foreach (AttackPlan aSchedule in aSchedules) {
            attackSchedule.Add(aSchedule.id, aSchedule);
        }
    }

    public static string PrintMotivation(AttackCode id, bool missed, Dictionary<ShopItemCode, ShopItemInfo> shopItems) {
        if (!missed) {
            switch (id) {
                case AttackCode.DOS:
                    if (shopItems[ShopItemCode.FIREWALL].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.FIREWALL].status != ShopItemStatus.UPGRADING) {
                        return "Dovremmo dotarci di un firewall al più presto.";
                    } else if (shopItems[ShopItemCode.NETWORK_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.NETWORK_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "L'attacco ha sfruttato alcune nostre lacune nella sicurezza di rete.";
                    } else if (shopItems[ShopItemCode.SYSTEM_PERFORMANCE].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.SYSTEM_PERFORMANCE].status != ShopItemStatus.UPGRADING) {
                        return "Investire maggiormente nelle prestazioni del sistema ci avrebbe permesso di mitigarne gli effetti.";
                    }
                    return "";
                case AttackCode.MITM:
                    if (shopItems[ShopItemCode.MFA].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.MFA].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di una strategia di Autenticazione Multi-Fattore ha agevolato l'attacco.";
                    } else if (shopItems[ShopItemCode.HASH].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.HASH].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di un meccanismo di hash per le password ha agevolato l'attacco.";
                    }
                    return "";
                case AttackCode.BRUTE_FORCE:
                    if (shopItems[ShopItemCode.PASSWORD_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.PASSWORD_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Le nostre password sono troppo semplici.";
                    } else if (shopItems[ShopItemCode.MFA].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.MFA].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di una strategia di Autenticazione Multi-Fattore ha agevolato l'attacco.";
                    }
                    return "";
                case AttackCode.DICTIONARY:
                    if (shopItems[ShopItemCode.PASSWORD_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.PASSWORD_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Le nostre password sono troppo semplici.";
                    } else if (shopItems[ShopItemCode.MFA].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.MFA].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di una strategia di Autenticazione Multi-Fattore ha agevolato l'attacco.";
                    }
                    return "";
                case AttackCode.RAINBOW_TABLE:
                    if (shopItems[ShopItemCode.PASSWORD_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.PASSWORD_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Le nostre password sono troppo semplici.";
                    } else if (shopItems[ShopItemCode.MFA].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.MFA].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di una strategia di Autenticazione Multi-Fattore ha agevolato l'attacco.";
                    }
                    return "";
                case AttackCode.API_VULNERABILITY:
                    if (shopItems[ShopItemCode.IDS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.IDS].status != ShopItemStatus.UPGRADING) {
                        return "Data la mancanza di un IDS non siamo riusciti ad individuare in tempo il pattern di attacco.";
                    }
                    return "";
                case AttackCode.SOCIAL_ENGINEERING:
                    if (shopItems[ShopItemCode.USER_AWARENESS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.USER_AWARENESS].status != ShopItemStatus.UPGRADING) {
                        return "I clienti non sono ancora abbastanza informati dei rischi che corrono online.";
                    } else if (shopItems[ShopItemCode.DNS_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.DNS_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Non avendo un buon livello di sicurezza per il DNS non riusciamo a limitare l'impatto di questi attacchi.";
                    }
                    return "";
                case AttackCode.PHISHING:
                    if (shopItems[ShopItemCode.SECURITY_TRAINING].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.SECURITY_TRAINING].status != ShopItemStatus.UPGRADING) {
                        return "Gli impiegati non sono ancora abbastanza informati sull'importanza della sicurezza informatica nelle proprie abitudini lavorative.";
                    } else if (shopItems[ShopItemCode.DNS_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.DNS_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Non avendo un buon livello di sicurezza per il DNS non riusciamo a limitare l'impatto di questi attacchi.";
                    } else if (shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.UPGRADING) {
                        return "Con un antivirus avremmo potuto identificare l'allegato malevolo prima che facesse danni.";
                    }
                    return "";
                case AttackCode.WORM:
                    if (shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.UPGRADING) {
                        return "Con un antivirus avremmo potuto neutralizzare la minaccia.";
                    } else if (shopItems[ShopItemCode.IDS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.IDS].status != ShopItemStatus.UPGRADING) {
                        return "Non avendo un IDS attivo non siamo riusciti ad identificare il pattern di diffusione del malware in tempo utile.";
                    } else if (shopItems[ShopItemCode.SYSTEM_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.SYSTEM_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Dobbiamo migliorare le nostre politiche per la sicurezza del sistema per il futuro.";
                    }
                    return "";
                case AttackCode.VIRUS:
                    if (shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.UPGRADING) {
                        return "Con un antivirus avremmo potuto neutralizzare la minaccia.";
                    } else if (shopItems[ShopItemCode.IDS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.IDS].status != ShopItemStatus.UPGRADING) {
                        return "Non avendo un IDS attivo non siamo riusciti ad identificare il pattern di diffusione del malware in tempo utile.";
                    } else if (shopItems[ShopItemCode.BACK_UP].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.BACK_UP].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di un backup non ci permetterà di mitigare i danni al sistema.";
                    }
                    return "";
                case AttackCode.SPYWARE:
                    if (shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.UPGRADING) {
                        return "Con un antivirus avremmo potuto neutralizzare la minaccia.";
                    } else if (shopItems[ShopItemCode.IDS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.IDS].status != ShopItemStatus.UPGRADING) {
                        return "Non avendo un IDS attivo non siamo riusciti ad identificare il pattern di diffusione del malware in tempo utile.";
                    } else if (shopItems[ShopItemCode.SYSTEM_SECURITY].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.SYSTEM_SECURITY].status != ShopItemStatus.UPGRADING) {
                        return "Dobbiamo migliorare le nostre politiche per la sicurezza del sistema per il futuro.";
                    }
                    return "";
                case AttackCode.RANSOMWARE:
                    if (shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.ANTIVIRUS].status != ShopItemStatus.UPGRADING) {
                        return "Con un antivirus avremmo potuto neutralizzare la minaccia.";
                    } else if (shopItems[ShopItemCode.IDS].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.IDS].status != ShopItemStatus.UPGRADING) {
                        return "Non avendo un IDS attivo non siamo riusciti ad identificare il pattern di diffusione del malware in tempo utile.";
                    } else if (shopItems[ShopItemCode.BACK_UP].status != ShopItemStatus.ACTIVE && shopItems[ShopItemCode.BACK_UP].status != ShopItemStatus.UPGRADING) {
                        return "La mancanza di un backup non ci permetterà di mitigare i danni al sistema.";
                    }
                    return "";
                default:
                    Debug.Log("Error: unexpected AttackCode");
                    return "";
            }
        } else {
            ShopItemCode selected;
            switch (id) {
                case AttackCode.DOS:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.FIREWALL, ShopItemCode.NETWORK_SECURITY, ShopItemCode.SYSTEM_PERFORMANCE }, shopItems);
                    switch (selected) {
                        case ShopItemCode.FIREWALL:
                            return "Siamo riusciti ad identificare la fonte dell'attacco e a neutralizzarla con l'aiuto del firewall.";
                        case ShopItemCode.NETWORK_SECURITY:
                            return "Grazie alla configurazione di rete l'attacco non è riuscito ad insinuarsi nel nostro sistema.";
                        case ShopItemCode.SYSTEM_PERFORMANCE:
                            return "Grazie agli investimenti effettuati i server sono stati in grado di sopportare il carico dell'attacco senza inconvenienti.";
                        default:
                            return "";
                    }
                case AttackCode.MITM:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.MFA, ShopItemCode.DNS_SECURITY }, shopItems);
                    switch (selected) {
                        case ShopItemCode.MFA:
                            return "Il malintenzionato non è riuscito a superare i controlli dell'Autenticazione Multi-Fattore.";
                        case ShopItemCode.DNS_SECURITY:
                            return "L'utilizzo di misure di sicurezza per il DNS ha impedito all'attaccante di modificare con successo le risposte DNS.";
                        default:
                            return "";
                    }
                case AttackCode.BRUTE_FORCE:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.MFA, ShopItemCode.PASSWORD_SECURITY }, shopItems);
                    switch (selected) {
                        case ShopItemCode.MFA:
                            return "Il malintenzionato non è riuscito a superare i controlli dell'Autenticazione Multi-Fattore.";
                        case ShopItemCode.PASSWORD_SECURITY:
                            return "L'utilizzo di password complesse ha impedito all'attaccante di individuare quella corretta e ci ha permesso di reagire in tempo all'attacco.";
                        default:
                            return "";
                    }
                case AttackCode.DICTIONARY:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.MFA, ShopItemCode.PASSWORD_SECURITY }, shopItems);
                    switch (selected) {
                        case ShopItemCode.MFA:
                            return "Il malintenzionato non è riuscito a superare i controlli dell'Autenticazione Multi-Fattore.";
                        case ShopItemCode.PASSWORD_SECURITY:
                            return "L'utilizzo di password complesse ha impedito all'attaccante di individuare quella corretta e ci ha permesso di reagire in tempo all'attacco.";
                        default:
                            return "";
                    }
                case AttackCode.RAINBOW_TABLE:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.MFA, ShopItemCode.PASSWORD_SECURITY }, shopItems);
                    switch (selected) {
                        case ShopItemCode.MFA:
                            return "Il malintenzionato non è riuscito a superare i controlli dell'Autenticazione Multi-Fattore.";
                        case ShopItemCode.PASSWORD_SECURITY:
                            return "L'utilizzo di password complesse ha impedito all'attaccante di individuare quella corretta e ci ha permesso di reagire in tempo all'attacco.";
                        default:
                            return "";
                    }
                case AttackCode.API_VULNERABILITY:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.IDS }, shopItems);
                    switch (selected) {
                        case ShopItemCode.IDS:
                            return "Il nostro sistema DNS ha individuato e segnalato il pattern del tentativo di accesso non autorizzato e così siamo riusciti a bloccarlo in tempo.";
                        default:
                            return "";
                    }
                case AttackCode.SOCIAL_ENGINEERING:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.USER_AWARENESS, ShopItemCode.DNS_SECURITY }, shopItems);
                    switch (selected) {
                        case ShopItemCode.USER_AWARENESS:
                            return "Alcuni clienti ci hanno segnalato un tentativo di social engineering via SMS e così siamo riusciti ad allertare la nostra utenza del pericolo.";
                        case ShopItemCode.DNS_SECURITY:
                            return "L'utilizzo di misure di sicurezza per il DNS ha impedito all'attaccante di modificare con successo le risposte DNS e ci ha permesso di bloccare la truffa prima che potesse colpire i nostri clienti.";
                        default:
                            return "";
                    }
                case AttackCode.PHISHING:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.SECURITY_TRAINING, ShopItemCode.DNS_SECURITY, ShopItemCode.ANTIVIRUS }, shopItems);
                    switch (selected) {
                        case ShopItemCode.SECURITY_TRAINING:
                            return "Grazie al training ricevuto un dipendente ci ha segnalato un tentativo di phishing via mail prima che riuscisse raggirare altri colleghi.";
                        case ShopItemCode.DNS_SECURITY:
                            return "L'utilizzo di misure di sicurezza per il DNS ha impedito all'attaccante di modificare con successo le risposte DNS e ci ha permesso di bloccare l'accesso ad un sito malevolo tramite link contenuto in una mail.";
                        case ShopItemCode.ANTIVIRUS:
                            return "Analizzando il contenuto dell'allegato di una mail sospetta l'antivirus ha sventato un attacco malware.";
                        default:
                            return "";
                    }
                case AttackCode.WORM:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.IDS, ShopItemCode.SYSTEM_SECURITY, ShopItemCode.ANTIVIRUS }, shopItems);
                    switch (selected) {
                        case ShopItemCode.IDS:
                            return "Il sistema IDS ha individuato il worm che tentava di proliferare nei nostri sistemi e siamo riusciti a bloccare l'infezione in tempo.";
                        case ShopItemCode.SYSTEM_SECURITY:
                            return "L'aggiornamento tempestivo di alcuni software affetti da gravi vulnerabilità ci ha permesso di evitare un attacco che ha colpito alcuni dei nostri competitors.";
                        case ShopItemCode.ANTIVIRUS:
                            return "L'antivirus ha individuato un worm che si nascondeva all'interno di un programma legittimo, evitandone la proliferazione.";
                        default:
                            return "";
                    }
                case AttackCode.VIRUS:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.IDS, ShopItemCode.BACK_UP, ShopItemCode.ANTIVIRUS }, shopItems);
                    switch (selected) {
                        case ShopItemCode.IDS:
                            return "Il sistema IDS ha individuato il virus che tentava di raccogliere informazioni sulla nostra rete e così siamo riusciti a neutralizzarlo.";
                        case ShopItemCode.BACK_UP:
                            return "Grazie al backup del sistema abbiamo quasi azzerato i danni che un virus ha inflitto a parte del nostro sistema.";
                        case ShopItemCode.ANTIVIRUS:
                            return "L'antivirus ha individuato un virus che si nascondeva all'interno di un programma legittimo e lo ha neutralizzato.";
                        default:
                            return "";
                    }
                case AttackCode.SPYWARE:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.IDS, ShopItemCode.SYSTEM_SECURITY, ShopItemCode.ANTIVIRUS }, shopItems);
                    switch (selected) {
                        case ShopItemCode.IDS:
                            return "Il sistema IDS ha individuato uno spyware che tentava di comunicare dati sensibili all'esterno della nostra rete e così siamo riusciti a neutralizzarlo in tempo.";
                        case ShopItemCode.SYSTEM_SECURITY:
                            return "L'aggiornamento tempestivo di alcuni software affetti da gravi vulnerabilità ci ha permesso di evitare un attacco che ha colpito alcuni dei nostri competitors.";
                        case ShopItemCode.ANTIVIRUS:
                            return "L'antivirus ha individuato uno spyware che si nascondeva all'interno di un programma legittimo e lo ha neutralizzato.";
                        default:
                            return "";
                    }
                case AttackCode.RANSOMWARE:
                    selected = SelectShopItem(new ShopItemCode[] { ShopItemCode.IDS, ShopItemCode.BACK_UP, ShopItemCode.ANTIVIRUS }, shopItems);
                    switch (selected) {
                        case ShopItemCode.IDS:
                            return "Il sistema IDS ha individuato il ransomware che tentava di accedere ai dati del nostro sistema e così siamo riusciti a neutralizzarlo.";
                        case ShopItemCode.BACK_UP:
                            return "Grazie al backup del sistema abbiamo quasi azzerato i danni che un ransomware ha inflitto a parte del nostro sistema.";
                        case ShopItemCode.ANTIVIRUS:
                            return "L'antivirus ha individuato un ransomware che si nascondeva all'interno di un programma legittimo e lo ha neutralizzato.";
                        default:
                            return "";
                    }
                default:
                    Debug.Log("Error: unexpected AttackCode");
                    return "";
            }
        }
    }

    static ShopItemCode SelectShopItem(ShopItemCode[] codes, Dictionary<ShopItemCode, ShopItemInfo> shopItems) {
        Dictionary<int, ShopItemCode> intervals = new Dictionary<int, ShopItemCode>();
        int tot = 0;
        foreach (ShopItemCode id in codes) {
            if (shopItems[id].status == ShopItemStatus.ACTIVE || shopItems[id].status == ShopItemStatus.UPGRADING) intervals[tot++] = id;
        }
        if (tot > 0) {
            return intervals[Random.Range(0, tot)];
        } else {
            return ShopItemCode.NONE;
        }
    }
}