using Assets.Scripts;
using UnityEngine;

public class ShotgunStand : MonoBehaviour
{
    public GameObject Shotgun;
    public GameObject Arrow;

    private BoxCollider m_Collider;

    public bool ShotgunGrabbed = false;

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        GameController.OnStatusChanged += GameController_OnStatusChanged;
    }

    private void OnDisable()
    {
        GameController.OnStatusChanged -= GameController_OnStatusChanged;
    }


    private void GameController_OnStatusChanged(GameStatus obj)
    {
        if(obj == GameStatus.NOT_STARTED || obj == GameStatus.SLEEPING)
        {
            ReleaseShotgun();
        }
        else
        {
            GrabShotgun();
        }
    }


    public void GrabShotgun()
    {
        if (ShotgunGrabbed)
        {
            return;
        }
        Shotgun.SetActive(false);
        Arrow.SetActive(false);
        m_Collider.enabled = false;
    }

    public void ReleaseShotgun()
    {
        if (ShotgunGrabbed)
        {
            return;
        }
        Shotgun.SetActive(true);
        Arrow.SetActive(true);
        m_Collider.enabled = true;
    }

}
