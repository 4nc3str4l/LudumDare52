
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIStatus
{
        
    protected AIEntity m_AI;
    protected NavMeshAgent m_Agent;
    protected bool m_IsStunned = false;
    private float m_StopStunnedTime = 0f;

    public AIStatus(AIEntity owner, NavMeshAgent agent)
    {
        m_AI = owner;
        m_Agent = agent;
    }

    public abstract void Setup();
    public abstract void OnExecute();

    public void Execute()
    {
        if (m_IsStunned)
        {
            return;
        }

        OnExecute();
    }
    public void Stun(float duration)
    {
        if(m_StopStunnedTime < Time.time)
        {
            m_StopStunnedTime = Time.time + duration;
            m_Agent.GetComponent<Monster>().StartCoroutine(StunEffect(duration));
        }
        else
        {
            m_StopStunnedTime += duration;
            Debug.Log($"Stunned Untill {m_StopStunnedTime}");
        }

    }

    public void UnStun()
    {
        if (!m_IsStunned)
        {
            return;
        }
        m_StopStunnedTime = Time.time;
    }

    private IEnumerator StunEffect(float duration)
    {
        m_IsStunned = true;
        m_Agent.isStopped = true;
        OnStunned();
        while (m_StopStunnedTime >= Time.time)
        {
            yield return null;
        }
        OnRecoverStun();
        m_Agent.isStopped = false;
        m_IsStunned = false;
    }


    public abstract void Dispose();

    public abstract void OnStunned();
    public abstract void OnRecoverStun();
}
