using Global;
using UnityEngine;

namespace Results {
    public class RetryButtonController : MonoBehaviour {
        public void OnClickToRetry() => GameStateManager.Instance.LoadLastLevel();
    }
}
