using Assets.Scripts;
using Assets.Scripts.MonsterAI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour, AIEntity
{
    private NavMeshAgent m_Agent;
    private AIStatus _currentStatus;
    public float DammagePoints = 15;
    public float AttackRate = 2f;
    public Transform CarryPosition;

    public LivingThing HealthStatus;

    public MeshRenderer[] Visuals;

    public GameObject DeathRepesentation;
    public GameObject Ressurecting;
    public Rigidbody[] DeathParts;
    public Dictionary<Rigidbody, Vector3> m_ParthsPosition = new Dictionary<Rigidbody, Vector3>();
    public Dictionary<Rigidbody, Vector3> m_PartsRotation = new Dictionary<Rigidbody, Vector3>();

    public Vector3 DeathPosition;
    public Quaternion DeathRotation;

    private AudioSource m_AudioSource;
    private float m_NextSoundStart = 0;


    private void Awake()
    { 
        m_Agent = GetComponent<NavMeshAgent>();
        m_NextSoundStart = Random.Range(0, 30f);
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        HealthStatus.OnDmg += HealthStatus_OnDmg;
        HealthStatus.OnDeath += HealthStatus_OnDeath;
        HealthStatus.OnRessurrect += HealthStatus_OnRessurrect;
    }

    private void OnDisable()
    {
        HealthStatus.OnDmg -= HealthStatus_OnDmg;
        HealthStatus.OnDeath -= HealthStatus_OnDeath;
        HealthStatus.OnRessurrect -= HealthStatus_OnRessurrect;
    }

    private void HealthStatus_OnDmg(float last, float current)
    {
        float stunTime = 3 * (last -current) / HealthStatus.InitialHealth;
        Debug.Log($"Monster Hit {current} stun time {stunTime}");
        _currentStatus.Stun(3 * (current - last) / HealthStatus.InitialHealth);
        // TODO: Add some debris here
    }

    private void HealthStatus_OnDeath()
    {
        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.MonsterDie, Random.Range(0.1f, 0.5f));
        _currentStatus.Stun(100f);
        Ressurecting.SetActive(true);
        Ressurecting.transform.SetParent(null);
        foreach (MeshRenderer visuals in Visuals)
        {
            visuals.enabled = false;
        }

        DeathPosition = transform.position;
        DeathRotation = transform.rotation;

        DeathRepesentation.SetActive(true);
        foreach (Rigidbody rb in DeathParts)
        {
            rb.transform.SetParent(null);
            m_ParthsPosition[rb] = rb.transform.position;
            m_PartsRotation[rb] = rb.transform.rotation.eulerAngles;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddExplosionForce(200f, transform.position, 20f);
        }

        StartCoroutine(DeathCorroutine());
    }

    IEnumerator DeathCorroutine()
    {
        yield return new WaitForSeconds(10);
        float biggestTime = Mathf.NegativeInfinity;
        foreach (Rigidbody rb in DeathParts)
        {
            rb.isKinematic = true;
            rb.useGravity = false;

            float t = 5 + Random.Range(2, 7);
            rb.transform.DOMove(m_ParthsPosition[rb], t).OnComplete(()=> {
                if(gameObject == null)
                {
                    Destroy(rb.gameObject, 1);

                    if (Ressurecting != null)
                    {
                        Ressurecting = null;
                        Destroy(Ressurecting);
                    }
                }

            });
            rb.transform.DORotate(m_PartsRotation[rb], t);
            biggestTime = Mathf.Max(biggestTime, t);
        }

        yield return new WaitForSeconds(biggestTime);

        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.MonsterRessurrect, Random.Range(0.1f, 0.5f));
        HealthStatus.Resurrect();

    }

    private void Start()
    {
        ChangeState(new AIFindingFence(this, m_Agent));
    }

    private void Update()
    {
        _currentStatus.Execute();
    
        if(m_NextSoundStart >= Time.time)
        {
            return;
        }

        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.MonsterWondering, Random.Range(0.1f, 0.3f));
        m_NextSoundStart = Time.time + Random.Range(30, 50);
    }

    public void ChangeState(AIStatus status)
    {
        if(_currentStatus != null)
        {
            _currentStatus.Dispose();
        }

        _currentStatus = status;
        _currentStatus.Setup();
    }

    public void Attack(LivingThing toAttack)
    {
        toAttack.DealDmg(DammagePoints);
    }

    private void HealthStatus_OnRessurrect()
    {

        transform.position = DeathPosition;
        transform.rotation = DeathRotation;
        Ressurecting.transform.SetParent(transform);
        Ressurecting.SetActive(false);
        foreach (Rigidbody rb in DeathParts)
        {
            rb.transform.SetParent(DeathRepesentation.transform);
        }

        foreach (MeshRenderer visuals in Visuals)
        {
            visuals.enabled = true;
        }

        DeathRepesentation.SetActive(false);
        _currentStatus.UnStun();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void PlayMonsterHappy()
    {
        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.MonsterHappy, Random.Range(0.1f, 0.3f));
    }

    public void PlayPumkinStolen()
    {
        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.PumkinStolen, Random.Range(0.1f, 0.3f));
    }

    public void PlayAttack()
    {
        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.MonsterAttack, Random.Range(0.1f, 0.3f));
    }

    public void PlayHit()
    {
        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.ShotgunMonsterHit, Random.Range(0.1f, 0.3f));
    }

    public void PlayFenceHit()
    {
        JukeBox.Instance.PlaySoundAtSource(m_AudioSource, JukeBox.Instance.FenceHit, Random.Range(0.1f, 0.3f));
    }


}
