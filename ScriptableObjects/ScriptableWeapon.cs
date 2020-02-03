using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableWeapon", menuName = "ScriptableObjects/ScriptableWeapon", order = 3)]
[Serializable]
/**
 * Storing WeaponStats in AllScenes
 */
public class ScriptableWeapon : ScriptableObject
{
    public Weapon _Weapon;
}