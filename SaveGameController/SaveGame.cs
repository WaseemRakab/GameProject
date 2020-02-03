using System;
/**
 * SaveGame Storing GameData
 */
[Serializable]
public class SaveGame
{
    public PlayerStats _PlayerStats;
    public Weapon _WeaponStats;
    public Level _PlayerLevel;
    public SpawnObjects _toSpawn;
    public DataUI _DataUI;
    public PlayTime _PlayTime;
    public EnemyStats _EnemyStats;
    public string _DateTime;
    public string _SaveFileName;
    public int WhichSlot;
}