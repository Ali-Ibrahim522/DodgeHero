using UnityEngine;

namespace Global {
        public class BasicButtonController : MonoBehaviour {
                [SerializeField] private GameStateManager.GameState nextState;
                
                public void OnClickToNextState() => GameStateManager.Instance.MoveToState(nextState);
        }
}