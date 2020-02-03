using UnityEngine;

/**
 * Getting Ammo Crate on Boss Levels Behaviour
 */
public class AmmoCrate : MonoBehaviour
{
    /*give the player ammo when clicking on intracting button in the player*/
    private Player _CurrentPlayer;
    private UIManager _UIManager;

    public AudioManager _AudioManager;
    void Awake()
    {
        _CurrentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _UIManager = GameObject.FindGameObjectWithTag("UiSystem").GetComponent<UIManager>();
        if (_AudioManager == null)
            GameObject.FindGameObjectWithTag("AudioManager").TryGetComponent(out _AudioManager);
    }

    public void GetAmmoKit()
    {
        _AudioManager.PlayGotAmmoBossSound(GetComponent<AudioSource>());
        _CurrentPlayer._MyWeaponData._Weapon.Ammo = Mathf.FloorToInt(_CurrentPlayer._MyWeaponData._Weapon.ClipSize);
        _UIManager.SetAmmoBulletsInUI(_CurrentPlayer._MyWeaponData._Weapon.Ammo);
    }
}