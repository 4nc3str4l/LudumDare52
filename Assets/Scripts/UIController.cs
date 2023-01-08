using Assets.Scripts.MonsterAI;
using UnityEngine;
using TMPro;
using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{

    public TMP_Text LblPressToGrab;
    public TMP_Text LblPressToRelease;
    public TMP_Text LblHarvestStats;
    public TMP_Text LblGoal;
    public TMP_Text LblSurived;
    public TMP_Text LblHappyCustomers;


    public TMP_Text LblPressToOpenDoor;
    public TMP_Text LblPressToCloseDoor;

    public TMP_Text LblPressToGrabShotgun;
    public TMP_Text LblOnBedPointed;

    public CanvasGroup PanelSleeping;
    public Button ButtonWakeUp;
    public TMP_Text TxtSleeping;

    public CanvasGroup OnDeath;
    public CanvasGroup OnStolen;
    public CanvasGroup OnPresentation;

    public static UIController Instance;


    Vector3 m_LblGoalScale;

    private void Awake()
    {
        m_LblGoalScale = LblGoal.transform.localScale;
        Instance = this;
        DisableAllInteractionLabels();
    }

    private void DisableAllInteractionLabels()
    {
        LblPressToGrab.enabled = false;
        LblPressToRelease.enabled = false;
        LblPressToOpenDoor.enabled = false;
        LblPressToCloseDoor.enabled = false;
        LblPressToGrabShotgun.enabled = false;
        LblOnBedPointed.enabled = false;
    }

    private void OnEnable()
    {
        Player.OnPointingToPumpkin += Player_OnPointingToPumpkin;
        Player.OnPointingToDoor += Player_OnPointingToDoor;
        Player.OnCarringPumpkin += Player_OnCarringPumpkin;
        GameController.OnHarvestNumbersChanged += GameController_OnHarvestNumbersChanged;
        Player.OnPointingShotgunStand += Player_OnPointingShotgunStand;
        Player.OnShotgunGrabbed += Player_OnShotgunGrabbed;

        GameController.OnStatusChanged += GameController_OnStatusChanged;
        Player.OnBedPointed += Player_OnBedPointed;
    }



    private void OnDisable()
    {
        Player.OnPointingToPumpkin -= Player_OnPointingToPumpkin;
        Player.OnPointingToDoor -= Player_OnPointingToDoor;
        Player.OnCarringPumpkin -= Player_OnCarringPumpkin;
        GameController.OnHarvestNumbersChanged -= GameController_OnHarvestNumbersChanged;
        Player.OnPointingShotgunStand -= Player_OnPointingShotgunStand;
        Player.OnShotgunGrabbed -= Player_OnShotgunGrabbed;
        GameController.OnStatusChanged -= GameController_OnStatusChanged;
        Player.OnBedPointed -= Player_OnBedPointed;
    }



    private void Player_OnPointingToDoor(Door door, bool pointing)
    {
        DisableAllInteractionLabels();
        if (pointing)
        {
            if (door.IsOpen)
            {
                LblPressToCloseDoor.enabled = true;
            }
            else
            {
                LblPressToOpenDoor.enabled = true;
            }
        }
    }

    private void Player_OnPointingToPumpkin(bool pointing)
    {
        DisableAllInteractionLabels();
        LblPressToGrab.enabled = pointing;
    }

    private void Player_OnCarringPumpkin(bool carrying)
    {
        DisableAllInteractionLabels();
        LblPressToRelease.enabled = carrying;
    }

    private void GameController_OnHarvestNumbersChanged()
    {
        LblHarvestStats.text = $"Collected: {GameController.Instance.Collected}\n" +
                                $"Stolen: {GameController.Instance.Stolen}\n" +
                                $"Available: {GameController.Instance.Available}\n";

        UpdateGoal();
    }

    private void UpdateGoal()
    {
        switch (GameController.Instance.State)
        {
            case GameStatus.PRESENTATION:
                LblGoal.text = "Welcome Back!";
                OnPresentation.alpha = 1;
                break;
            case GameStatus.NOT_STARTED:
                OnPresentation.alpha = 0;
                LblGoal.text = $"Goal: Grab your shotgun";
                break;
            case GameStatus.SHOTGUN_GRABBED:
                LblGoal.text = $"Goal: Leave the cabin";
                break;
            case GameStatus.STARTING_GAME:
                LblGoal.text = $"Starting Level...";
                break;
            case GameStatus.IN_GAME:
                LblGoal.text = $"Goal: Harvest and store {GameController.Instance.MissingToCollect}\npumpkins in the delivery boxes";
                break;
            case GameStatus.ALL_PUMPKINS_COLLECTED:
                LblGoal.text = $"Goal: Go Back Home";
                break;
            case GameStatus.CLOSE_THE_DOOR:
                LblGoal.text = $"Goal: Close the door!";
                break;
            case GameStatus.AT_HOME_AFTER_OBJECTIVE:
                DisableAllInteractionLabels();
                LblGoal.text = $"Well done, now you can go to bed untill next year...!";
                break;
            case GameStatus.SLEEPING:
                LblSurived.text = GameController.Instance.Level.ToString();
                LblHappyCustomers.text = GameController.Instance.HappyCustomers.ToString();
                DisableAllInteractionLabels();
                SleepUI();
                break;
            case GameStatus.PLAYER_DEAD:
                DisableAllInteractionLabels();
                OnDeath.alpha = 1;
                break;
            case GameStatus.PUMKINS_STOLEN:
                DisableAllInteractionLabels();
                OnStolen.alpha = 1;
                break;
        }



        LblGoal.transform.DOShakeScale(1).OnComplete(() =>
        {
            LblGoal.transform.localScale = m_LblGoalScale;
        });
    }


    private void Player_OnPointingShotgunStand(bool isPointing)
    {
        DisableAllInteractionLabels();
        LblPressToGrabShotgun.enabled = isPointing;
    }

    private void Player_OnShotgunGrabbed()
    {
        DisableAllInteractionLabels();
    }

    private void GameController_OnStatusChanged(GameStatus obj)
    {
        UpdateGoal();
    }

    private void Player_OnBedPointed(bool obj)
    {
        DisableAllInteractionLabels();
        LblOnBedPointed.enabled = obj;
    }


    private void SleepUI()
    {
        StartCoroutine(FadeInSleeping());
    }

    private IEnumerator FadeInSleeping()
    {
        TxtSleeping.gameObject.SetActive(false);
        ButtonWakeUp.gameObject.SetActive(false);
        PanelSleeping.interactable = false;
        PanelSleeping.blocksRaycasts = false;
        float fadeInTime = 2;
        float TimeToFadein = Time.realtimeSinceStartup + fadeInTime;
        while(Time.realtimeSinceStartupAsDouble < TimeToFadein)
        {
            // Lerp the color of the image between itself and transparent.
            float alphaChange = Time.deltaTime / fadeInTime;
            PanelSleeping.alpha += alphaChange;
            yield return null;
        }

        if (GameController.Instance.Level == 3)
        {
            LblGoal.text = "Wake up... Neo?";
        }
        if (GameController.Instance.Level == 5)
        {
            LblGoal.text = "Wake up moth****fuk**r!...";
        }
        else
        {
            LblGoal.text = "Wake up...";
        }

        TxtSleeping.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        ButtonWakeUp.gameObject.SetActive(true);
        PanelSleeping.interactable = true;
        PanelSleeping.blocksRaycasts = true;
    }

    public void WakeUpUI(Action _onComplete)
    {
        StartCoroutine(FadeOutSleeping(_onComplete));
    }

    private IEnumerator FadeOutSleeping(Action _onComplete)
    {
        TxtSleeping.gameObject.SetActive(false);
        ButtonWakeUp.gameObject.SetActive(false);
        PanelSleeping.interactable = false;
        PanelSleeping.blocksRaycasts = false;
        float fadeOutTime = 2;
        float TimeToFadeout = Time.realtimeSinceStartup + fadeOutTime;
        while (Time.realtimeSinceStartupAsDouble < TimeToFadeout)
        {
            // Lerp the color of the image between itself and transparent.
            float alphaChange = Time.deltaTime / fadeOutTime;
            PanelSleeping.alpha -= alphaChange;
            yield return null;
        }
        _onComplete?.Invoke();
    }

}
