using Assets.Scripts.MonsterAI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {

        public static GameController Instance;

        public const int NUM_PUMPKINS = 12;

        public int Level = 1;
        public int PumkinsToCollect;
        public int MissingToCollect;
        public int Collected;
        public int Stolen;
        public int Available;

        public int HappyCustomers = 0;

        public static event Action OnHarvestNumbersChanged;

        public GameStatus State;
        public static event Action<GameStatus> OnStatusChanged;

        public bool IsCheating = false;
        public int CheatingLevel = 5;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            HappyCustomers = 0;
            ChangeState(GameStatus.PRESENTATION);
            if (IsCheating)
            {
                Level = CheatingLevel;
            }
            PlayerPrefs.SetInt("Score", 0);
        
        }

        private void OnEnable()
        {
            PumpinBox.OnPumkinCollected += PumpinBox_OnPumkinCollected;
            Portal.OnPumpkinStolen += Portal_OnPumpkinStolen;
            Player.OnShotgunGrabbed += Player_OnShotgunGrabbed;
            Door.OnDoorOpen += Door_OnDoorOpen;
            Cabin.PlayerInCabin += Cabin_PlayerInCabin;
            Door.OnDoorClosed += Door_OnDoorClosed;
        }



        private void OnDisable()
        {
            PumpinBox.OnPumkinCollected -= PumpinBox_OnPumkinCollected;
            Portal.OnPumpkinStolen -= Portal_OnPumpkinStolen;
            Player.OnShotgunGrabbed -= Player_OnShotgunGrabbed;
            Door.OnDoorOpen -= Door_OnDoorOpen;
            Cabin.PlayerInCabin -= Cabin_PlayerInCabin;
            Door.OnDoorClosed -= Door_OnDoorClosed;
        }

        private void PumpinBox_OnPumkinCollected(Pumpkin obj)
        {
            Collected += 1;
            HappyCustomers += 1;
            RecomputeNumbers();

            if(MissingToCollect <= 0)
            {
                ChangeState(GameStatus.ALL_PUMPKINS_COLLECTED);
            }
        }

        private void Portal_OnPumpkinStolen(Portal p, Pumpkin pk)
        {
            Stolen += 1;
            RecomputeNumbers();

            if(MissingToCollect > Available)
            {
                CameraController.Instance.GoToPortal(p, pk);
                ChangeState(GameStatus.PUMKINS_STOLEN);
            }
        }

        private void RecomputeNumbers()
        {
            MissingToCollect = PumkinsToCollect - Collected;
            Available = NUM_PUMPKINS - Collected - Stolen;
            OnHarvestNumbersChanged?.Invoke();
        }


        public void ChangeState(GameStatus newStatus)
        {
            State = newStatus;

            switch (newStatus)
            {
                case GameStatus.NOT_STARTED:
                    PumkinsToCollect = Mathf.Min(2 + Level, NUM_PUMPKINS);
                    Collected = 0;
                    Stolen = 0;
                    RecomputeNumbers();
                    break;
                case GameStatus.SHOTGUN_GRABBED:
                    break;
                case GameStatus.STARTING_GAME:
                    StartCoroutine(StartingGameFeedback());
                    break;
                case GameStatus.IN_GAME:
                    break;
                case GameStatus.ALL_PUMPKINS_COLLECTED:
                    break;
                case GameStatus.CLOSE_THE_DOOR:
                    break;
                case GameStatus.AT_HOME_AFTER_OBJECTIVE:
                    break;
                case GameStatus.SLEEPING:
                    OnSleeping();
                    break;
                case GameStatus.PLAYER_DEAD:
                    StartDeathLogic();
                    break;
                case GameStatus.PUMKINS_STOLEN:
                    StartPumkinsStolenLogic();
                    break;
            }
            OnStatusChanged?.Invoke(newStatus);
        }



        public void OnSleeping()
        {
            // TODO: Repair the map
        }

        public int ComputeMonstersToSpawn()
        {
            if(Level == 1)
            {
                return 0;
            }

            if(Level < 4)
            {
                return Level * 3;
            }
            else
            {
                return 12 + Level;
            }
        }

        private void Door_OnDoorOpen()
        {
            if(State != GameStatus.SHOTGUN_GRABBED)
            {
                return;
            }

            ChangeState(GameStatus.STARTING_GAME);
        }

        IEnumerator StartingGameFeedback()
        {
            MonsterSpawner.Instance.SpawnMonsters(ComputeMonstersToSpawn());
            yield return new WaitForSeconds(2f);
            ChangeState(GameStatus.IN_GAME);
        }

        private void Player_OnShotgunGrabbed()
        {
            ChangeState(GameStatus.SHOTGUN_GRABBED);
        }

        private void Cabin_PlayerInCabin(bool _inCabin)
        {
            if(State != GameStatus.ALL_PUMPKINS_COLLECTED && State != GameStatus.CLOSE_THE_DOOR)
            {
                return;
            }

            if (_inCabin)
            {
                if (Door.Instance.IsOpen)
                {
                    ChangeState(GameStatus.CLOSE_THE_DOOR);
                }
                else
                {
                    ChangeState(GameStatus.AT_HOME_AFTER_OBJECTIVE);
                }
            }
            else
            {
                ChangeState(GameStatus.ALL_PUMPKINS_COLLECTED);
            }
        }

        private void Door_OnDoorClosed()
        {
            if(State != GameStatus.CLOSE_THE_DOOR)
            {
                return;
            }
            ChangeState(GameStatus.AT_HOME_AFTER_OBJECTIVE);
        }

        public void WakeUp()
        {
            Level += 1;
            PlayerPrefs.SetInt("Score", Level - 1);
            UIController.Instance.WakeUpUI(() =>{
                ChangeState(GameStatus.NOT_STARTED);
            });
        }

        private void StartDeathLogic()
        {
            StartCoroutine(PlayerDeathCorroutine());
        }

        private IEnumerator PlayerDeathCorroutine()
        {
            JukeBox.Instance.PlaySound(JukeBox.Instance.GameOver, 0.6f);
            yield return new WaitForSeconds(15);
            SceneManager.LoadScene("GameOver");
            PlayerPrefs.SetString("reason", "You died");
        }


        private void StartPumkinsStolenLogic()
        {
            StartCoroutine(PumkinsStolenRoutine());
        }

        private IEnumerator PumkinsStolenRoutine()
        {
            JukeBox.Instance.PlaySound(JukeBox.Instance.GameOver, 0.6f);
            yield return new WaitForSeconds(15);
            SceneManager.LoadScene("GameOver");
            PlayerPrefs.SetString("reason", "Pumpkins Stolen...");
        }

        private void Update()
        {
            if(State == GameStatus.PRESENTATION)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(GameStatus.NOT_STARTED);
                }
            }


            if(State != GameStatus.PLAYER_DEAD && State != GameStatus.PUMKINS_STOLEN)
            {
                return;
            }
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene("GameOver");
            }
       
        }

        public bool InputEnabled()
        {
            return !(GameController.Instance.State == GameStatus.SLEEPING ||
            GameController.Instance.State == GameStatus.PLAYER_DEAD ||
                        GameController.Instance.State == GameStatus.PRESENTATION ||
            GameController.Instance.State == GameStatus.PUMKINS_STOLEN
            );
        }
    }
}