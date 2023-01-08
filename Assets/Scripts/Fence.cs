using Assets.Scripts;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fence : MonoBehaviour
{

    public LivingThing HealthStatus;
    private Vector3 m_InitialScale;
    public Transform[] FenceEntryes;

    public Dictionary<Rigidbody, Vector3> m_Positions = new Dictionary<Rigidbody, Vector3>();
    public Dictionary<Rigidbody, Quaternion> m_Rotations = new Dictionary<Rigidbody, Quaternion>();
    public AudioSource Source;


    private void Awake()
    {
        m_InitialScale = transform.localScale;
    }

    private void OnEnable()
    {
        HealthStatus.OnDmg += HealthStatus_OnDmg;
        HealthStatus.OnDeath += HealthStatus_OnDeath;
    }

    private void OnDisable()
    {
        HealthStatus.OnDmg -= HealthStatus_OnDmg;
        HealthStatus.OnDeath -= HealthStatus_OnDeath;
    }

    private void HealthStatus_OnDmg(float arg1, float arg2)
    {
        transform.DOShakeScale(1).OnComplete(() =>
        {
            transform.localScale = m_InitialScale;
        });
    }

    private void HealthStatus_OnDeath()
    {
        JukeBox.Instance.PlaySoundAtSource(Source, JukeBox.Instance.FenceDestroyed, 0.2f);
        GetComponent<NavMeshObstacle>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            m_Positions[rb] = rb.transform.position;
            m_Rotations[rb] = rb.transform.localRotation;
            rb.isKinematic = false;
        }
    }

    public Vector3 GetTargetEntry()
    {
        return FenceEntryes[Random.Range(0, FenceEntryes.Length)].transform.position;
    }

    public void Repair()
    {
        GetComponent<NavMeshObstacle>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;

        foreach(var k in m_Positions.Keys)
        {
            k.transform.DOMove(m_Positions[k], 3.5f);
            k.transform.localRotation = m_Rotations[k];
            k.isKinematic = true;
        }

        HealthStatus.Resurrect();
    }
}
