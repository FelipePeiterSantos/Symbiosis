using UnityEngine;
using System.Collections;

public class PlayerStatus {

    static string stats;
    static string statEnemies;
    static Transform player;

    public static int distanceToPlayer(Transform myPos) {
        player = (player != null) ? player : GameObject.FindGameObjectWithTag("Player").transform;
        int returnValue = Mathf.FloorToInt(Vector3.Distance(myPos.position, player.position));
        return returnValue;
    }

    public static string playerIsDoing {
        get {
            stats = (stats == null) ? "idle" : stats;
            return stats;
        }
        set {
            stats = value;
        }
    }

    public static string IsSomeoneAttacking {
        get {
            statEnemies = (statEnemies == null) ? "no" : statEnemies;
            return statEnemies;
        }
        set {
            statEnemies = value;
        }
    }

}
