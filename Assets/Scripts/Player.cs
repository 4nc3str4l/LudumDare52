using System;
using UnityEngine;

namespace Assets.Scripts.MonsterAI
{
    public class Player : MonoBehaviour
    {

        public static Player Instance;
        public LivingThing HealthStatus;


        public static event Action<bool> OnPointingToPumpkin;
        public static event Action<Door, bool> OnPointingToDoor;
        public static event Action<bool> OnCarringPumpkin;
        public static event Action OnShotgunGrabbed;
        public static event Action<bool> OnPointingShotgunStand;
        public static event Action<bool> OnBedPointed;


        public Pumpkin RaycastedPumpkin = null;
        public ShotgunStand RaycastedStand = null;
        public Bed RaycastedBed = null;


        public PlayerStatus CurrentStatus { get; private set; } = PlayerStatus.Weapon;
        public Door RaycastedDoor { get; private set; }

        private Vector3 m_InitialPosition;
        private Quaternion m_InitialRotatiom;

        private void Awake()
        {
            Instance = this;
            m_InitialPosition = transform.position;
            m_InitialRotatiom = transform.rotation;
        }

        private void OnEnable()
        {
            GameController.OnStatusChanged += GameController_OnStatusChanged;
            HealthStatus.OnDmg += HealthStatus_OnDmg;
            HealthStatus.OnDeath += HealthStatus_OnDeath;
        }


        private void OnDisable()
        {
            GameController.OnStatusChanged -= GameController_OnStatusChanged;
            HealthStatus.OnDmg -= HealthStatus_OnDmg;
            HealthStatus.OnDeath -= HealthStatus_OnDeath;
        }

        private void GameController_OnStatusChanged(GameStatus obj)
        {
            if(obj != GameStatus.NOT_STARTED)
            {
                return;
            }

            transform.position = m_InitialPosition;
            transform.rotation = m_InitialRotatiom;
            HealthStatus.Resurrect();
            HealthStatus.DealDmg(0);
        }


        private void Update()
        {
            switch (GameController.Instance.State)
            {
                case GameStatus.NOT_STARTED:
                    UpdateGameNotStarted();
                    break;
                case GameStatus.SHOTGUN_GRABBED:
                    UpdateOnShotgunGrabbed();
                    break;
                case GameStatus.STARTING_GAME:
                    break;
                case GameStatus.IN_GAME:
                    UpdateInGameState();
                    break;
                case GameStatus.ALL_PUMPKINS_COLLECTED:
                    UpdateAllPumpkinsCollected();
                    break;
                case GameStatus.CLOSE_THE_DOOR:
                    UpdateAllPumpkinsCollected();
                    break;
                case GameStatus.AT_HOME_AFTER_OBJECTIVE:
                    UpdateAtHomeAfterObjective();
                    break;
                case GameStatus.PLAYER_DEAD:
                    break;
                case GameStatus.PUMKINS_STOLEN:
                    break;
            }
        }



        private void UpdateInGameState()
        {
            switch (CurrentStatus)
            {
                case PlayerStatus.Weapon:
                    if (RaycastedPumpkin != null)
                    {
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            RaycastedPumpkin.PlayerCarry();
                            CurrentStatus = PlayerStatus.CarryingPumpkin;
                            OnCarringPumpkin?.Invoke(true);
                        }
                    }

                    break;
                case PlayerStatus.CarryingPumpkin:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        RaycastedPumpkin.PlayerRelease();
                        CurrentStatus = PlayerStatus.Weapon;
                        OnCarringPumpkin?.Invoke(false);
                    }
                    break;
                case PlayerStatus.Dead:
                    break;
                case PlayerStatus.AtHome:
                    break;
            }
        }

       private void UpdateAllPumpkinsCollected()
        {
            switch (CurrentStatus)
            {
                case PlayerStatus.Weapon:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if(RaycastedDoor == null)
                        {
                            return;
                        }
                        if (RaycastedDoor.IsOpen)
                        {
                            RaycastedDoor.Close();
                        }
                        else
                        {
                            RaycastedDoor.Open();
                        }
                    }
                    break;
                case PlayerStatus.CarryingPumpkin:
                    break;
                case PlayerStatus.Dead:
                    break;
                case PlayerStatus.AtHome:
                    break;
            }
        }

        private void UpdateAtHomeAfterObjective()
        {
            switch (CurrentStatus)
            {
                case PlayerStatus.Weapon:
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (RaycastedBed == null)
                        {
                            return;
                        }
                        RaycastedBed.GoToSleep();
                    }
                    break;
                case PlayerStatus.CarryingPumpkin:
                    break;
                case PlayerStatus.Dead:
                    break;
                case PlayerStatus.AtHome:
                    break;
            }
        }

        private void UpdateGameNotStarted()
        {
            if (RaycastedStand != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    RaycastedStand.GrabShotgun();
                    OnShotgunGrabbed?.Invoke();
                }
            }
        }

        private void UpdateOnShotgunGrabbed()
        {
            if (RaycastedDoor != null)
            {
                if (!RaycastedDoor.IsOpen)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (RaycastedDoor.IsOpen)
                        {
                            RaycastedDoor.Close();
                        }
                        else
                        {
                            RaycastedDoor.Open();
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {

            switch (GameController.Instance.State)
            {
                case GameStatus.NOT_STARTED:
                    FixedUpdateGameNotStarted();
                    break;
                case GameStatus.SHOTGUN_GRABBED:
                    RaycastDoor();
                    break;
                case GameStatus.STARTING_GAME:
                    break;
                case GameStatus.IN_GAME:
                    FixedUpdateInGameState();
                    break;
                case GameStatus.ALL_PUMPKINS_COLLECTED:
                    FixedUpdateInGameState();
                    break;
                case GameStatus.CLOSE_THE_DOOR:
                    FixedUpdateInGameState();
                    break;
                case GameStatus.AT_HOME_AFTER_OBJECTIVE:
                    FixedUpdateAtHomeAfterObjective();
                    break;
                case GameStatus.PLAYER_DEAD:
                    break;
                case GameStatus.PUMKINS_STOLEN:
                    break;
            }
        }



        private void FixedUpdateInGameState()
        {
            if (CurrentStatus == PlayerStatus.Weapon)
            {
                RaycastPumpkins();
                RaycastDoor();
            }
        }

        private void FixedUpdateGameNotStarted()
        {
            RaycastGrabShotgun();
        }

        private void RaycastPumpkins()
        {
            // Create a ray from the transform's position in the direction of the forward vector.
            Ray ray = new Ray(CameraController.Instance.transform.position, CameraController.Instance.transform.forward);

            // Set the layer mask to only hit objects in the "pumpkin" layer.
            int layerMask = 1 << LayerMask.NameToLayer("Pumpkin");

            // Perform the raycast using the layer mask.
            if (Physics.Raycast(ray, out RaycastHit hit, 2.5f, layerMask))
            {
                // The raycast hit an object in the "pumpkin" layer.
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                RaycastedPumpkin = hit.collider.GetComponent<Pumpkin>();
                OnPointingToPumpkin?.Invoke(true);
            }
            else
            {
                if(RaycastedPumpkin != null)
                {
                    RaycastedPumpkin = null;
                    OnPointingToPumpkin?.Invoke(false);
                }
            }
        }

        private void RaycastDoor()
        {
            // Create a ray from the transform's position in the direction of the forward vector.
            Ray ray = new Ray(CameraController.Instance.transform.position, CameraController.Instance.transform.forward);

            // Set the layer mask to only hit objects in the "pumpkin" layer.
            int layerMask = 1 << LayerMask.NameToLayer("Door");

            // Perform the raycast using the layer mask.
            if (Physics.Raycast(ray, out RaycastHit hit, 2.5f, layerMask))
            {
                // The raycast hit an object in the "pumpkin" layer.
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                RaycastedDoor = hit.collider.GetComponent<Door>();
                OnPointingToDoor?.Invoke(RaycastedDoor, true);
            }
            else
            {
                if (RaycastedDoor != null)
                {
                    RaycastedDoor = null;
                    OnPointingToDoor?.Invoke(null, false);
                }
            }
        }

        private void RaycastGrabShotgun()
        {
            Ray ray = new Ray(CameraController.Instance.transform.position, CameraController.Instance.transform.forward);

            int layerMask = 1 << LayerMask.NameToLayer("ShotgunStand");

            if (Physics.Raycast(ray, out RaycastHit hit, 2.5f, layerMask))
            {
     
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                RaycastedStand = hit.collider.GetComponent<ShotgunStand>();
                OnPointingShotgunStand?.Invoke(true);
            }
            else
            {
                if (RaycastedStand != null)
                {
                    RaycastedStand = null;
                    OnPointingShotgunStand?.Invoke(false);
                }
            }
        }


        private void RaycastBed()
        {
            Ray ray = new Ray(CameraController.Instance.transform.position, CameraController.Instance.transform.forward);

            int layerMask = 1 << LayerMask.NameToLayer("Bed");

            if (Physics.Raycast(ray, out RaycastHit hit, 2.5f, layerMask))
            {

                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                RaycastedBed = hit.collider.GetComponent<Bed>();
                OnBedPointed?.Invoke(true);
            }
            else
            {
                if (RaycastedBed != null)
                {
                    RaycastedBed = null;
                    OnBedPointed?.Invoke(false);
                }
            }
        }

        private void FixedUpdateAtHomeAfterObjective()
        {
            RaycastBed();
        }


        private void HealthStatus_OnDmg(float arg1, float arg2)
        {
            if (Mathf.Abs(arg1 - arg2) > 0)
            {
                JukeBox.Instance.PlaySound(JukeBox.Instance.PlayerHurt, UnityEngine.Random.Range(0.2f, 0.5f));
                CameraController.Instance.Shake();
            }
        }

        private void HealthStatus_OnDeath()
        {
            JukeBox.Instance.PlaySound(JukeBox.Instance.PlayerDeath, 0.6f);
            GameController.Instance.ChangeState(GameStatus.PLAYER_DEAD);
        }
    }
}