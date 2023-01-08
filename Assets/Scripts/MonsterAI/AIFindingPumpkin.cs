using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.MonsterAI
{
    public class AIFindingPumpkin : AIStatus
    {

        private Pumpkin _target;
        private Monster _monster;

        private float m_TimeToReconsider;

        public AIFindingPumpkin(AIEntity owner, NavMeshAgent agent) : base(owner, agent)
        {
            _monster = m_Agent.GetComponent<Monster>();
            m_TimeToReconsider = Time.time + 15 + Random.Range(0, 20);
        }


        public override void Setup()
        {
            _target = SceneInventory.Instance.GetClosestFreePumpkin(m_Agent.transform.position);
        }

        public override void OnExecute()
        {
            if(_target == null)
            {
                _monster.ChangeState(new AIAttackPlayer(m_AI, m_Agent));
                return;
            }

            if (m_TimeToReconsider < Time.time)
            {
                _monster.ChangeState(new AIFindingPumpkin(m_AI, m_Agent));
                return;
            }
                
            if (_target.Carrier == _monster.gameObject)
            {
                m_AI.ChangeState(new AIStealingPumpkin(_target, m_AI, m_Agent));
                return;
            }

            if (!_target.ShouldBeSeekedByMonster())
            {
                _target = SceneInventory.Instance.GetClosestFreePumpkin(m_Agent.transform.position);
                
                if(_target == null)
                {
                    m_AI.ChangeState(new AIAttackPlayer(m_AI, m_Agent));
                }
                return;
            }

            m_Agent.SetDestination(_target.transform.position);

            if(Vector3.Distance(_target.transform.position, m_Agent.transform.position) < 2.5)
            {
                _target.MosterCarry(_monster);
            }
        }

        public override void Dispose()
        {
        }

        public override void OnStunned()
        {
            if (_target != null && _target.Carrier == _monster.gameObject)
            {
                _target.MonsterRelease();
            }
        }

        public override void OnRecoverStun()
        {
            if(Random.Range(0.0f, 1.0f) > 0.9f)
            {
                Setup();
            }
            else
            {
                m_AI.ChangeState(new AIAttackPlayer(m_AI, m_Agent));
            }
        }
    }
}