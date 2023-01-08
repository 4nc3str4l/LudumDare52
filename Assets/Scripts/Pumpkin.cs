
using Assets.Scripts;
using Assets.Scripts.MonsterAI;
using DG.Tweening;
using System;
using UnityEngine;

public class Pumpkin : MonoBehaviour
{
    public PumpkinState CurrentState { get; private set; } =  PumpkinState.OnTheGround;

    public event Action<PumpkinState> OnStateChanged;

    public GameObject Carrier;

    private Rigidbody m_RigidBody;
    private Collider m_Collider;

    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;

    public Transform ToFollow;

    public Arrow MyArrow;

    private void Awake()
    {
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
        m_RigidBody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
    }

    public void ChangeState(PumpkinState newState)
    {
        CurrentState = newState;
    }

    public void MosterCarry(Monster monster)
    {
        ChangeState(PumpkinState.OnMonster);
        DisablePhysics();

        transform.DOMove(monster.CarryPosition.position, 0.5f).OnComplete(() => {
            ToFollow = monster.CarryPosition;
        });

        Carrier = monster.gameObject;
        UIStealingIndicator.Instance.Register(this);

    }

    public void MonsterRelease()
    {
        ToFollow = null;
        ChangeState(PumpkinState.OnTheGround);
        EnablePhysics();
        Carrier = null;
        m_RigidBody.AddForce(Vector3.up * 100);
        UIStealingIndicator.Instance.Unregister(this);
    }

    public void ReleaseInPortal(Portal _p)
    {
        ToFollow = null;
        DisablePhysics();
        ChangeState(PumpkinState.Stolen);
        _p.FeedbackOnPumpkinReleased(this);
    }


    public bool ShouldBeSeekedByMonster()
    {
        return CurrentState == PumpkinState.OnTheGround;
    }

    public void PlayerCarry()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.PumpkinGather, 0.4f);
        DisablePhysics();
        ChangeState(PumpkinState.OnPlayer);
    }

    public void PlayerRelease()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.PumkinRelease, 0.4f);
        ToFollow = null;
        EnablePhysics();

        ChangeState(PumpkinState.OnTheGround);
        m_RigidBody.velocity = FPSController.Instance.characterController.velocity;
    }

    private void DisablePhysics()
    {
        m_RigidBody.isKinematic = true;
        m_RigidBody.useGravity = false;
        m_Collider.enabled = false;
    }

    private void EnablePhysics()
    {
        m_RigidBody.isKinematic = false;
        m_RigidBody.useGravity = true;
        m_Collider.enabled = true;
    }

    public void OnCollected()
    {
        JukeBox.Instance.PlaySound(JukeBox.Instance.PumkinCollected, 0.4f);
        ChangeState(PumpkinState.Harvested);
        DisablePhysics();
        Hide();
    }

    private void Update()
    {
        if(CurrentState == PumpkinState.Stolen || CurrentState == PumpkinState.Harvested)
        {
            MyArrow.SetVisible(false);
            return;
        }

        if(GameController.Instance.State != GameStatus.IN_GAME)
        {
            if(MyArrow != null)
            {
                MyArrow.SetVisible(false);
            }
            return;
        }

        if (MyArrow != null)
        {
            MyArrow.SetVisible(Player.Instance.CurrentStatus != PlayerStatus.CarryingPumpkin);
        }
    }

    private void LateUpdate()
    {
        if(CurrentState == PumpkinState.OnPlayer)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.2f;
        }
        else if(ToFollow != null && Carrier != null && Carrier.gameObject != null)
        {
            transform.position = ToFollow.transform.position;
        }
    }

    private void Show()
    {
        foreach (MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = true;
        }
    }

    private void Hide()
    {
        foreach(MeshRenderer r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }
    }

    public void Reset()
    {
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
        EnablePhysics();
        Carrier = null;
        CurrentState = PumpkinState.OnTheGround;
        Show();
    }
}
