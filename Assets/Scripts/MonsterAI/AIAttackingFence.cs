using UnityEngine;

namespace Assets.Scripts.MonsterAI
{
    public class AIAttackingFence : AIStatus
    {

        private Fence m_target;

        private float m_NextAttack = 0;
        private Monster m_Monster;

        public AIAttackingFence(Fence target, AIEntity owner, UnityEngine.AI.NavMeshAgent agent) : base(owner, agent)
        {
            m_target = target;
            m_Monster = m_Agent.GetComponent<Monster>();
        }

       
        public override void Setup()
        {

        }


        public override void OnExecute()
        {
            if(m_NextAttack > Time.time)
            {
                return;
            }

            if (!m_target.HealthStatus.IsAlive)
            {
                m_Monster.ChangeState(new AIFindingPumpkin(m_AI, m_Agent));
                return;
            }

            m_Monster.Attack(m_target.HealthStatus);
            m_Monster.PlayFenceHit();
            m_NextAttack = Time.time + m_Monster.AttackRate;
        }

        public override void Dispose()
        {
        }


        public override void OnStunned()
        {
        }

        public override void OnRecoverStun()
        {
        }
    }
}