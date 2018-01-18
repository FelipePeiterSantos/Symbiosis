using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class XMLvaluesSetup : MonoBehaviour {
    void Awake() {
        setupValues.LoadXMLValues();
    }
}

public static class setupValues  {
    public static int player_initialCombo = 0;
    public static int player_continueCombo = 0;
    public static int player_dash = 0;
    public static int player_longAttack = 0;

    public static float player_OrbMaxDamage = 0;
    public static float player_DroneBullet = 0;
    public static float player_enemyAttack = 0;
    public static float player_bossJumpAttack = 0;
    public static float player_bossRightAttack = 0;
    public static float player_bossLeftAttack = 0;

    public static float hardHit = 0;
    public static float hit = 0;

    public static int spawn1_sameTime = 0;
    public static int spawn1_quantityEnemies = 0;
    public static int spawn2_sameTime = 0;
    public static int spawn2_quantityEnemies = 0;
    public static int spawn3_sameTime = 0;
    public static int spawn3_quantityEnemies = 0;
    public static int spawn4_sameTime = 0;

    public static float enemy_tickReact = 0;
    public static float enemy_OrbMaxDamage = 0;
    public static float enemy_playerAttack = 0;
    public static float enemy_droneAttack = 0;

    public static float drone_tickReact = 0;
    public static float drone_OrbMaxDamage = 0;
    public static float drone_playerAttack = 0;

    public static float boss_tickReact = 0;
    public static float boss_OrbMaxDamage = 0;
    public static float boss_playerAttack = 0;
    public static float boss_jumpChance = 0;
    public static float boss_droneAttack = 0;

    public static int energyOrb_speed = 0;
    public static int energyOrb_cooldown = 0;

    public static void LoadXMLValues() {
        TextAsset textAsset = (TextAsset) Resources.Load("XMLValues");  
        XmlDocument xmldoc = new XmlDocument ();

        xmldoc.LoadXml(textAsset.text);
        XmlNode node = xmldoc.SelectSingleNode("/XML/PlayerStaminaConsume");
        player_initialCombo = (node == null) ? 10 : int.Parse(node.SelectSingleNode("InitiateCombo").FirstChild.Value);
        player_continueCombo = (node == null) ? 15 : int.Parse(node.SelectSingleNode("ContinueCombo").FirstChild.Value);
        player_dash = (node == null) ? 25 : int.Parse(node.SelectSingleNode("Roll").FirstChild.Value);
        player_longAttack = (node == null) ? 30 : int.Parse(node.SelectSingleNode("LongAttack").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/PlayerDamageTaken");
        player_OrbMaxDamage = (node == null) ? 1000 : int.Parse(node.SelectSingleNode("EnergyOrbMaxDamage").FirstChild.Value);
        player_DroneBullet = (node == null) ? 250 : int.Parse(node.SelectSingleNode("DroneBullet").FirstChild.Value);
        player_enemyAttack = (node == null) ? 100 : int.Parse(node.SelectSingleNode("EnemyAttack").FirstChild.Value);
        player_bossJumpAttack = (node == null) ? 700 : int.Parse(node.SelectSingleNode("BossJumpAttack").FirstChild.Value);
        player_bossRightAttack = (node == null) ? 500 : int.Parse(node.SelectSingleNode("BossRightHandAttack").FirstChild.Value);
        player_bossLeftAttack = (node == null) ? 250 : int.Parse(node.SelectSingleNode("BossLeftHandAttack").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/HitDetectCooldown");
        hardHit = (node == null) ? 4f : float.Parse(node.SelectSingleNode("hardHit").FirstChild.Value);
        hit = (node == null) ? 0.3f : float.Parse(node.SelectSingleNode("hit").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/Spawner");
        spawn1_sameTime = (node == null) ? 1 : int.Parse(node.SelectSingleNode("Spawn1/SameTime").FirstChild.Value);
        spawn1_quantityEnemies = (node == null) ? 4 : int.Parse(node.SelectSingleNode("Spawn1/Quantity").FirstChild.Value);
        spawn2_sameTime = (node == null) ? 2 : int.Parse(node.SelectSingleNode("Spawn2/SameTime").FirstChild.Value);
        spawn2_quantityEnemies = (node == null) ? 8 : int.Parse(node.SelectSingleNode("Spawn2/Quantity").FirstChild.Value);
        spawn3_sameTime = (node == null) ? 3 : int.Parse(node.SelectSingleNode("Spawn3/SameTime").FirstChild.Value);
        spawn3_quantityEnemies = (node == null) ? 12 : int.Parse(node.SelectSingleNode("Spawn3/Quantity").FirstChild.Value);
        spawn4_sameTime = (node == null) ? 3 : int.Parse(node.SelectSingleNode("Spawn4/SameTime").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/Enemy");
        enemy_tickReact = (node == null) ? 0.5f : float.Parse(node.SelectSingleNode("TickReactionTime").FirstChild.Value);
        enemy_OrbMaxDamage = (node == null) ? 3.5f : float.Parse(node.SelectSingleNode("EnergyOrbMaxDamage").FirstChild.Value);
        enemy_playerAttack = (node == null) ? 0.5f : float.Parse(node.SelectSingleNode("PlayerAttack").FirstChild.Value);
        enemy_droneAttack = (node == null) ? 0.5f : float.Parse(node.SelectSingleNode("DroneAttack").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/Drone");
        drone_tickReact = (node == null) ? 0.5f : float.Parse(node.SelectSingleNode("TickReactionTime").FirstChild.Value);
        drone_OrbMaxDamage = (node == null) ? 4f : float.Parse(node.SelectSingleNode("EnergyOrbMaxDamage").FirstChild.Value);
        drone_playerAttack = (node == null) ? 1f : float.Parse(node.SelectSingleNode("PlayerAttack").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/Boss");
        boss_tickReact = (node == null) ? 1f : float.Parse(node.SelectSingleNode("TickReactionTime").FirstChild.Value);
        boss_OrbMaxDamage = (node == null) ? 0.6f : float.Parse(node.SelectSingleNode("EnergyOrbMaxDamage").FirstChild.Value);
        boss_playerAttack = (node == null) ? 0.1f : float.Parse(node.SelectSingleNode("PlayerAttack").FirstChild.Value);
        boss_jumpChance = (node == null) ? 0.8f : float.Parse(node.SelectSingleNode("JumpChance").FirstChild.Value);
        boss_droneAttack = (node == null) ? 0.2f : float.Parse(node.SelectSingleNode("DroneAttack").FirstChild.Value);

        node = xmldoc.SelectSingleNode("/XML/EnergyOrb");
        energyOrb_speed = (node == null) ? 5 : int.Parse(node.SelectSingleNode("Speed").FirstChild.Value);
        energyOrb_cooldown = (node == null) ? 30 : int.Parse(node.SelectSingleNode("Cooldown").FirstChild.Value);
    }
}
