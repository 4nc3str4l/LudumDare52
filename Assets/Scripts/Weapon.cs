using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    public int Capacity;
    public int Current;
    public float ReloadTime;

    public float ShootRate;

    public bool IsVisible = true;
    public bool IsActive = true;
    public bool IsReloading = false;

    public float ProjectileDmg;

    private float m_NextShoot = 0;

    public bool HasAmmo { get { return Current > 0; } }

    public Transform Barrel;

    public void Shoot()
    {
        if (IsReloading)
        {
            return;
        }

        if(Current <= 0)
        {
            OnShootEmpty();
            return;
        }

        if(m_NextShoot > Time.time)
        {
            return;
        }

        OnShot();
        Current--;
    }

    public void Reload()
    {
        StartCoroutine(ReloadLogic());
    }

    IEnumerator ReloadLogic()
    {
        IsReloading = true;
        OnReload();
        yield return new WaitForSeconds(ReloadTime);
        Current = Capacity;
        IsReloading = false;
    }

    public void Equip()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.EquipShotgunSound, 0.5f);

        IsActive = true;
        IsVisible = true;
        OnShow();
    }

    public void UnEquip()
    {
        IsActive = false;
        IsVisible = false;
        OnHide();
    }

    public abstract void OnShot();
    public abstract void OnReload();
    public abstract void OnHide();
    public abstract void OnShow();
    public abstract void OnShootEmpty();
 
}