using System;

/**
 * In-Game Weapon Storing
 */
[Serializable]
public class Weapon
{
    public string objectName = "Weapon";

    public float _Damage = 15f;
    public float FireRate = 1.2f;
    public float ClipSize = 10f;
    public int Ammo = 10;
    public int TotalWeaponFired = 0;

    public int _UpgradeDamageLevel = 0;
    public int _MaxUpgradeDamageLevel = 5;
    public int[] UpgradeDamagePrices = { 300, 600, 900, 1600, 2000 };
    public int UpgradeFireRateLevel = 0;
    public int _MaxUpgradeFireRateLevel = 4;
    public int[] UpgradeFireRatePrices = { 600, 900, 1300, 2000 };
    public int _UpgradeClipSizeLevel = 0;
    public int _MaxUpgradeClipSizeLevel = 5;
    public int[] UpgradeClipSizePrices = { 200, 800, 1600, 2400, 3000 };

    public PreviousWeaponStats PreviousStats = new PreviousWeaponStats();
    public Weapon(PreviousWeaponStats prevWeaponStats)
    {
        _Damage = prevWeaponStats._Damage;
        FireRate = prevWeaponStats.FireRate;
        ClipSize = prevWeaponStats.ClipSize;
        Ammo = prevWeaponStats.Ammo;
        TotalWeaponFired = prevWeaponStats.TotalWeaponFired;
        _UpgradeDamageLevel = prevWeaponStats._UpgradeDamageLevel;
        UpgradeFireRateLevel = prevWeaponStats.UpgradeFireRateLevel;
        _UpgradeClipSizeLevel = prevWeaponStats._UpgradeClipSizeLevel;
        PreviousStats = new PreviousWeaponStats(this);
    }
    public Weapon() { }
    /**
     * Inner Class To Store The Previous WeaponStats When Game Restarted,NextLevel,Etcs
     */
    [Serializable]
    public class PreviousWeaponStats
    {
        public float _Damage = 15f;
        public float FireRate = 1.2f;
        public float ClipSize = 10f;
        public int Ammo = 10;
        public int TotalWeaponFired = 0;
        public int _UpgradeDamageLevel = 0;
        public int UpgradeFireRateLevel = 0;
        public int _UpgradeClipSizeLevel = 0;
        public PreviousWeaponStats(Weapon weaponStats)
        {
            _Damage = weaponStats._Damage;
            FireRate = weaponStats.FireRate;
            ClipSize = weaponStats.ClipSize;
            Ammo = weaponStats.Ammo;
            TotalWeaponFired = weaponStats.TotalWeaponFired;
            _UpgradeDamageLevel = weaponStats._UpgradeDamageLevel;
            UpgradeFireRateLevel = weaponStats.UpgradeFireRateLevel;
            _UpgradeClipSizeLevel = weaponStats._UpgradeClipSizeLevel;
        }
        public PreviousWeaponStats() { }
    }
}