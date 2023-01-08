using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.MonsterAI
{
    public class AIAttackPlayer : AIStatus
    {
        private Monster _monster;
        private float m_NextAttack = 0;

        private float m_TimeAttacking = 0;

        public AIAttackPlayer(AIEntity owner, NavMeshAgent agent) : base(owner, agent)
        {
            _monster = m_Agent.GetComponent<Monster>();
            m_TimeAttacking = Time.time + 15 + Random.Range(0, 20);
        }


        public override void Setup()
        {

        }

        public override void OnExecute()
        {
            if(m_TimeAttacking < Time.time)
            {
                m_Agent.isStopped = false;
                _monster.ChangeState(new AIFindingPumpkin(m_AI, m_Agent));
                return;
            }

            m_Agent.SetDestination(Player.Instance.transform.position);


            if (Vector3.Distance(m_Agent.transform.position, Player.Instance.transform.position) < 2f)
            {
                m_Agent.isStopped = true;
            }
            else
            {
                m_Agent.isStopped = false;
            }

            if (m_NextAttack > Time.time)
            {
                return;
            }

            if (Vector3.Distance(m_Agent.transform.position, Player.Instance.transform.position) < 2.5f)
            {
                _monster.Attack(Player.Instance.HealthStatus);
                m_NextAttack = Time.time + _monster.AttackRate;
                m_TimeAttacking = Time.time + 15 + Random.Range(0, 20);
                _monster.PlayAttack();
            }
        }

        public override void Dispose()
        {
        }

        public override void OnStunned()         
        {
            m_TimeAttacking = Time.time + 15 + Random.Range(0, 20);
        }

        public override void OnRecoverStun()
        {
            m_TimeAttacking = Time.time + 15 + Random.Range(0, 20);
        }
    }
}