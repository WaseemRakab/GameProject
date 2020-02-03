using System;
/**
* In-Game PlayerStats Storing
*/
[Serializable]
public class PlayerStats
{
    public string objectName = "PlayerStats";

    public int MyCash;
    public int Lives = 3;
    public int Health = 100;
    public int ShieldCapacity = 0;
    public int Shield = 0;
    public int TotalKills = 0;
    public int TotalEnemyKills = 0;
    public int TotalTurretKills = 0;
    public bool HasWeapon = false;

    public Vector PlayerPosition = new Vector(-40f, -8f, 0f);

    public PreviousPlayerStats PreviousStats = new PreviousPlayerStats();
    public PlayerStats(PreviousPlayerStats prevPlayerStats)
    {
        MyCash = prevPlayerStats.MyCash;
        Lives = prevPlayerStats.Lives;
        Health = prevPlayerStats.Health;
        ShieldCapacity = prevPlayerStats.ShieldCapacity;
        Shield = prevPlayerStats.Shield;
        TotalKills = prevPlayerStats.TotalKills;
        TotalEnemyKills = prevPlayerStats.TotalEnemyKills;
        TotalTurretKills = prevPlayerStats.TotalTurretKills;
        HasWeapon = prevPlayerStats.HasWeapon;
        PreviousStats = new PreviousPlayerStats(this);
    }
    public PlayerStats() { }
    /**
     * Inner Class To Store The Previous PlayerStats When Game Restarted,NextLevel,Etcs
     */
    [Serializable]
    public class PreviousPlayerStats
    {
        public int MyCash = 0;
        public int Lives = 3;
        public int Health = 100;
        public int ShieldCapacity = 0;
        public int Shield = 0;
        public int TotalKills = 0;
        public int TotalEnemyKills = 0;
        public int TotalTurretKills = 0;
        public bool HasWeapon = false;

        public PreviousPlayerStats(PlayerStats playerStats)
        {
            MyCash = playerStats.MyCash;
            Lives = playerStats.Lives;
            Health = playerStats.Health;
            ShieldCapacity = playerStats.ShieldCapacity;
            Shield = playerStats.Shield;
            TotalKills = playerStats.TotalKills;
            TotalEnemyKills = playerStats.TotalEnemyKills;
            TotalTurretKills = playerStats.TotalTurretKills;
            HasWeapon = playerStats.HasWeapon;
        }
        public PreviousPlayerStats() { }
    }
}