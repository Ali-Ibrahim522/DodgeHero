using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScoreComponents : MonoBehaviour {
    [SerializeField] public GameObject container;
    [SerializeField] public RawImage pfp;
    [SerializeField] public TMP_Text displayName;
    [SerializeField] public TMP_Text score;
    [SerializeField] public TMP_Text rank;
}