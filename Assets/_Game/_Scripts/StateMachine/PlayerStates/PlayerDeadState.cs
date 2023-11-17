namespace _Game._Scripts.StateMachine.PlayerStates
{
    using _Game._Scripts.Character.Hero;
    using _Game._Scripts.StateMachine.Base;
    using _Game._Scripts.UI.Hud;
    using UniRx;
    using UnityEngine;

    public class PlayerDeadState : IAIState
    {
        private readonly HeroAIStateMachine _stateMachine;
        private readonly IHeroFacade _hero;


        public PlayerDeadState( HeroAIStateMachine stateMachine, IHeroFacade hero )
        {
            _stateMachine = stateMachine;
            _hero = hero;
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
