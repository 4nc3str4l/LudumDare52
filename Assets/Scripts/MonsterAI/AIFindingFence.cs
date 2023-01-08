using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.MonsterAI
{
    public class AIFindingFence : AIStatus
    {

        private Fence _target;

        private Vector3 m_TargetEntry;

        public AIFindingFence(AIEntity owner, NavMeshAgent agent) : base(owner, agent)
        {
        }

        public override void Setup()
        {
            _target = SceneInventory.Instance.GetClosestFence(m_Agent.transform.position);
            m_TargetEntry = _target.GetTargetEntry();

        }

        public override void OnExecute()
        {
            m_Agent.SetDestination(m_TargetEntry);
            if(Vector3.Distance(m_TargetEntry, m_Agent.transform.position) <= 2)
            {
                m_AI.ChangeState(new AIAttackingFence(_target, m_AI, m_Agent));
            }
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