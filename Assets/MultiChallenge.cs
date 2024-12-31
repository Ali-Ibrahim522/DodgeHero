using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [System.Serializable]
    public class MultiChallenge : MonoBehaviour, IChallenge
    {
        public List<IChallenge> Challenges;
        public delegate void Handler(List<IChallenge> challenges);
        public Handler challengeHandler;

        public MultiChallenge(List<IChallenge> challenges)
        {
            Challenges = challenges;
        }
        
        public void UpdateChallenge()
        {
            challengeHandler?.Invoke(Challenges);
        }
    }
}