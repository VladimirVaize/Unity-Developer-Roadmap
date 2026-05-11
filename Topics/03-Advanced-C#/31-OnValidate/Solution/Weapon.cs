using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _range;
    [SerializeField] private float _fireRate;
    [SerializeField] private int _ammo;
    [SerializeField] private int _maxAmmo;

    private float _reloadTime;

    private void OnValidate()
    {
        _damage = Mathf.Clamp(_damage, 1, 100);
        _range = Mathf.Clamp(_range, 0.5f, 50);
        _fireRate = Mathf.Clamp(_fireRate, 1, 20);

        _ammo = Mathf.Max(_ammo, 0);
        if (_ammo > _maxAmmo)
            _ammo = _maxAmmo;

        _reloadTime = 1 / _fireRate;

        Debug.Log($"Weapon validated: Dmg={_damage}, Range={_range}, Rate={_fireRate}, Ammo={_ammo}/{_maxAmmo}, ReloadTime={_reloadTime}");
    }
}
