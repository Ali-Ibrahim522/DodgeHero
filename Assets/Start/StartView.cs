using System;
using UnityEngine;

namespace Start
{
    public class StartView : MonoBehaviour
    {
        public void OnPlayButtonPressed() => GameStateManager.Instance.MoveToState(GameStateManager.GameState.Level);
    }
}
