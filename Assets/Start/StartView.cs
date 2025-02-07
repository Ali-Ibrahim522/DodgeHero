using Global;
using TMPro;
using UnityEngine;

namespace Start
{
    public class StartView : MonoBehaviour {
        [SerializeField] private TMP_Text displayNameTxt;
        private void OnEnable() {
            displayNameTxt.text = PlayerAuthManager.GetDisplayName();
        }
    }
}
