using Assets.Scripts;
using System;
using UnityEngine;

public class Cabin : MonoBehaviour
{
    public static event Action<bool> PlayerInCabin;

    public GameObject[] GoHomeFeedback;
    public GameObject[] LeaveTheCabin;


    public static Cabin Instance;

    private void Awake()
    {
        Instance = this;
        SetGoHomeFeedback(false);
        SetLeaveTheCabinFeedback(false);
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
        SetGoHomeFeedback(obj == GameStatus.ALL_PUMPKINS_COLLECTED);
        SetLeaveTheCabinFeedback(obj == GameStatus.SHOTGUN_GRABBED || obj == GameStatus.CLOSE_THE_DOOR);
    }

    private void SetGoHomeFeedback(bool enable)
    {
        foreach(GameObject f in GoHomeFeedback)
        {
            f.SetActive(enable);
        }
    }

    private void SetLeaveTheCabinFeedback(bool enable)
    {
        foreach (GameObject f in LeaveTheCabin)
        {
            f.SetActive(enable);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player") { return; }
        PlayerInCabin?.Invoke(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") { return; }
        PlayerInCabin?.Invoke(false);
    }
}
