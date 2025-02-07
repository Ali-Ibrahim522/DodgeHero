using System.Collections.Generic;
using Events;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelect {
    public class LevelSelectView : MonoBehaviour {
        [SerializeField] private TMP_Text detailHeader;
        [SerializeField] private TMP_Text detailBody;
        [SerializeField] private Image selectedLevelPreview;
        [SerializeField] private GameObject settingsBtn;
        [SerializeField] private Button nextChallengeBtn;
        [SerializeField] private Button prevChallengeBtn;
        [SerializeField] private GameObject nextChallengeBtnObj;
        [SerializeField] private GameObject prevChallengeBtnObj;
        [SerializeField] private Button detailsBtn;
        
        [Header("player info")]
        [SerializeField] private RawImage playerPfp;
        [SerializeField] private TMP_Text displayName;

        [SerializeField] private GameObject detailsContainer;
        [SerializeField] private GameObject leaderboardContainer;
        [SerializeField] private Button leaderboardBtn;
        [SerializeField] private GameObject leaderboardPage1;
        [SerializeField] private GameObject leaderboardPage2;

        private GameStateManager.GameState _selectedGameState;
        private List<LevelDiffChallengeInfo.ChallengeInfo> _challengeDetails;
        private int _challengeIndex;
        private bool _isGuest;

        private EventProcessor<LevelSelectDiffChangeEvent> _onLevelSelectDiffChangeEventProcessor;
        private EventProcessor<ReportLeaderboardSizeEvent> _onReportLeaderboardSizeEventProcessor;

        private void Awake() {
            _onLevelSelectDiffChangeEventProcessor = new EventProcessor<LevelSelectDiffChangeEvent>(SetSelectedLevel);
            _onReportLeaderboardSizeEventProcessor = new EventProcessor<ReportLeaderboardSizeEvent>(RetrieveLeaderboardSize);
        }

        private void OnEnable() {
            LevelDiffChallengeInfo.LoadChallengeInfoMap();
            _isGuest = PlayerAuthManager.GetPlayerState() == PlayerAuthManager.PlayerState.Guest;
            settingsBtn.SetActive(!_isGuest);
            playerPfp.texture = PlayerAuthManager.GetPfp();
            displayName.text = PlayerAuthManager.GetDisplayName();
            EventBus<LevelSelectDiffChangeEvent>.Subscribe(_onLevelSelectDiffChangeEventProcessor);
            EventBus<ReportLeaderboardSizeEvent>.Subscribe(_onReportLeaderboardSizeEventProcessor);
        }
        private void OnDisable() {
            EventBus<LevelSelectDiffChangeEvent>.Unsubscribe(_onLevelSelectDiffChangeEventProcessor);
            EventBus<ReportLeaderboardSizeEvent>.Unsubscribe(_onReportLeaderboardSizeEventProcessor);
        }

        private void RetrieveLeaderboardSize(ReportLeaderboardSizeEvent reportLeaderboardSizeEventProps) {
            detailsBtn.interactable = true;
            nextChallengeBtn.interactable = reportLeaderboardSizeEventProps.LeaderboardSize > 5;
        }

        public void OnDetailsClicked() {
            _challengeIndex = 0;
            nextChallengeBtnObj.SetActive(true);
            prevChallengeBtnObj.SetActive(true);
            UpdateButtonInteractable();
            detailsContainer.SetActive(true);
            leaderboardContainer.SetActive(false);
        }

        public void OnLeaderboardClicked() {
            if (leaderboardContainer.activeSelf) return;
            detailsContainer.SetActive(false);
            leaderboardContainer.SetActive(true);
            leaderboardPage1.SetActive(true);
            leaderboardPage2.SetActive(false);
            nextChallengeBtnObj.SetActive(!_isGuest);
            prevChallengeBtnObj.SetActive(!_isGuest);
            prevChallengeBtn.interactable = false;
            nextChallengeBtn.interactable = true;
        }
        
        public void OnNextClicked() {
            if (detailsContainer.activeSelf) {
                _challengeIndex++;
                SetChallengeDetails();
                UpdateButtonInteractable();
            } else {
                leaderboardPage1.SetActive(false);
                leaderboardPage2.SetActive(true);
                prevChallengeBtn.interactable = true;
                nextChallengeBtn.interactable = false;
            }
        }

        public void OnPrevClicked() {
            if (detailsContainer.activeSelf) {
                _challengeIndex--;
                SetChallengeDetails();
                UpdateButtonInteractable();
            } else {
                leaderboardPage2.SetActive(false);
                leaderboardPage1.SetActive(true);
                nextChallengeBtn.interactable = true;
                prevChallengeBtn.interactable = false;
            }
        }

        private void SetChallengeDetails() {
            detailHeader.text = _challengeDetails[_challengeIndex].ChallengeHeader;
            detailBody.text = _challengeDetails[_challengeIndex].ChallengeBody;
        }

        private void UpdateButtonInteractable() {
            prevChallengeBtn.interactable = _challengeIndex > 0;
            nextChallengeBtn.interactable = _challengeIndex < _challengeDetails.Count - 1;
        }

        private void SetSelectedLevel(LevelSelectDiffChangeEvent levelSelectDiffChangeEventProps) {
            selectedLevelPreview.sprite = levelSelectDiffChangeEventProps.SelectedLevelPreview;
            _selectedGameState = levelSelectDiffChangeEventProps.SelectedGameState;
            _challengeDetails = LevelDiffChallengeInfo.ChallengeInfoMap[_selectedGameState];
            if (_selectedGameState == GameStateManager.GameState.LevelSelect) leaderboardBtn.interactable = false;
            else {
                leaderboardBtn.interactable = true;
                GameStateManager.Instance.StoreLastLevelState(_selectedGameState);
            }
            OnDetailsClicked();
            SetChallengeDetails();
        }
        
        public void OnPlayButton() {
            if (_selectedGameState != GameStateManager.GameState.LevelSelect) GameStateManager.Instance.MoveToState(_selectedGameState);
        }
    }
}
