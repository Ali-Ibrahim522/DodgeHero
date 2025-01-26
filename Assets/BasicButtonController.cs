using UnityEngine;

public class BasicButtonController : MonoBehaviour {
        [SerializeField] private GameStateManager.GameState nextState;
        
        public void OnClickToNextState() => GameStateManager.Instance.MoveToState(nextState);
}