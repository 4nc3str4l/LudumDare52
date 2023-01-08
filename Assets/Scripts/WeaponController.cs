using Assets.Scripts;
using Assets.Scripts.MonsterAI;
using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{

    public Weapon Shotgun;
    private Weapon m_CurrentWeapon;

    public TMP_Text AmmoIndicator;

    private void Start()
    {
        m_CurrentWeapon = Shotgun;
        Shotgun.UnEquip();
    }


    private void OnEnable()
    {
        Player.OnCarringPumpkin += Player_OnCarringPumpkin;
        Player.OnShotgunGrabbed += Player_OnShotgunGrabbed;
        GameController.OnStatusChanged += GameController_OnStatusChanged;
    }

    private void GameController_OnStatusChanged(GameStatus obj)
    {
        if (obj == GameStatus.NOT_STARTED || obj == GameStatus.SLEEPING || obj == GameStatus.PLAYER_DEAD || obj == GameStatus.PUMKINS_STOLEN)
        {
            m_CurrentWeapon.UnEquip();
        }
    }

    private void OnDisable()
    {
        Player.OnCarringPumpkin -= Player_OnCarringPumpkin;
        Player.OnShotgunGrabbed -= Player_OnShotgunGrabbed;
        GameController.OnStatusChanged -= GameController_OnStatusChanged;
    }


    private void Player_OnCarringPumpkin(bool carry)
    {
        if (carry)
        {
            m_CurrentWeapon.UnEquip();
        }
        else
        {
            m_CurrentWeapon.Equip();
        }
    }


    private void Update()
    {

        UpdateUI();

        if (!GameController.Instance.InputEnabled())
        {
            return;
        }


        if (!m_CurrentWeapon.IsActive)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            m_CurrentWeapon.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            m_CurrentWeapon.Reload();
        }

    }


    public void UpdateUI()
    {
        if (m_CurrentWeapon == null || !m_CurrentWeapon.IsActive)
        {
            AmmoIndicator.gameObject.SetActive(false);
            return;
        }
        AmmoIndicator.gameObject.SetActive(true);
        if (m_CurrentWeapon.HasAmmo)
        {
            AmmoIndicator.text = $"Ammo: {m_CurrentWeapon.Current} / {m_CurrentWeapon.Capacity}";
        }
        else
        {
            AmmoIndicator.text = "[R] to reload!";
        }
    }

    private void Player_OnShotgunGrabbed()
    {
        m_CurrentWeapon = Shotgun;
        m_CurrentWeapon.Equip();
    }

}
