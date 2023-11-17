namespace _Game._Scripts.StateMachine.Base
{
    public interface IAIState
    {
        string Name { get; }
    
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }
}
