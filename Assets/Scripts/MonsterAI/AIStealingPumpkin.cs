using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.MonsterAI
{
    public class AIStealingPumpkin : AIStatus
    {

        private Portal _target;
        private Pumpkin _stolen;

        public AIStealingPumpkin(Pumpkin stolen, AIEntity owner, NavMeshAgent agent) : base(owner, agent)
        {
            _stolen = stolen;
        }

        public override void Setup()
        {
            _target = SceneInventory.Instance.GetPortal(m_Agent.transform.position);
            m_Agent.GetComponent<Monster>().PlayMonsterHappy();
 
        }

        public override void OnExecute()
        {
            if(GameController.Instance.State != GameStatus.IN_GAME)
            {
                if(_stolen != null)
                {
                    _stolen.MonsterRelease();
                    _stolen = null;
                }
                return;
            }

            m_Agent.SetDestination(_target.transform.position);
            if(Vector3.Distance(_target.transform.position, m_Agent.transform.position) <= 2)
            {
                _stolen.ReleaseInPortal(_target);
                m_Agent.GetComponent<Monster>().PlayPumkinStolen();
                m_AI.ChangeState(new AIFindingPumpkin(m_AI, m_Agent));
            }
        }

        public override void Dispose()
        {
        }

        public override void OnStunned()
        {
            if(_stolen != null)
            {
                _stolen.MonsterRelease();
            }
        }

        public override void OnRecoverStun()
        {
            if (Random.Range(0.0f, 1.0f) > 0.9f)
            {
                m_AI.ChangeState(new AIFindingPumpkin(m_AI, m_Agent));
            }
            else
            {
                m_AI.ChangeState(new AIAttackPlayer(m_AI, m_Agent));
            }
        }
    }
}