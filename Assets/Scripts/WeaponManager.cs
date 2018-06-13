using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private Transform weaponHolder;

    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    private void Start()
    {
        //equip weapon
        EquipWeapon(primaryWeapon);

    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    void EquipWeapon (PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        GameObject _weaponIns = (GameObject)Instantiate(_weapon.WeaponGFX, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();

        if (currentGraphics == null)
            Debug.LogError("No WeapongGraphics component on the weapon object: " + _weaponIns.name);

        if (isLocalPlayer)
            Utility.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }

}
