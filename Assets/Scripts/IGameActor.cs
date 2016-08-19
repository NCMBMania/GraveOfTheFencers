public interface IGameActor {

    void RecieveAnimationState(string name, bool enabled);
    void OnPause();
    void OnResume();
    void OnDead();

    void OnAttackHit(UnityEngine.Collider col);
}
