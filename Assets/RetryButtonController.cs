using UnityEngine;

public class RetryButtonController : MonoBehaviour {
    public void OnClickToRetry() => GameStateManager.Instance.LoadLastLevel();
}
