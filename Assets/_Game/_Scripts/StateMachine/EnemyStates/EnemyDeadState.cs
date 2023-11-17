using _Game._Scripts.StateMachine.Base;

namespace _Game._Scripts.StateMachine.EnemyStates
{
    using UnityEngine.Scripting;

    public class EnemyDeadState : IAIState
    {
        [Preserve]
        public EnemyDeadState( )
        {
        }

        public string Name => "Dead";

        public void Enter()
        {
        }

        public void Exit()
        {
        
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }
    }
}
