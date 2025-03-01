using Audio;
using Events;
using Global;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ProfilesModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EntityKey = PlayFab.ProfilesModels.EntityKey;
using GetPlayerProfileRequest = PlayFab.ClientModels.GetPlayerProfileRequest;
using GetPlayerProfileResult = PlayFab.ClientModels.GetPlayerProfileResult;
using PlayerProfileViewConstraints = PlayFab.ClientModels.PlayerProfileViewConstraints;
using SendAccountRecoveryEmailRequest = PlayFab.ClientModels.SendAccountRecoveryEmailRequest;
using SendAccountRecoveryEmailResult = PlayFab.ClientModels.SendAccountRecoveryEmailResult;

namespace Login {
    public class LoginView : MonoBehaviour {
        [SerializeField] private Image showPasswordButtonImage; 
        [SerializeField] private Sprite hidePasswordIcon;
        [SerializeField] private Sprite showPasswordIcon;
        [SerializeField] private MusicData loginMusic;
        
        [Header("Field containers")]
        [SerializeField] private GameObject loginContainer;
        [SerializeField] private GameObject emailObjects;
        [SerializeField] private GameObject passwordObjects;
        [SerializeField] private GameObject usernameObjects;
    
        [Header("Input fields")]
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField usernameField;

        [Header("General info text")]
        [SerializeField] private TMP_Text generalInfoText;
    
        [Header("Buttons")]
        [SerializeField] private GameObject loginButton;
        [SerializeField] private GameObject newAccountButton;
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject createAccountButton;
        [SerializeField] private GameObject sendEmailButton;
        [SerializeField] private GameObject forgotPasswordButton;
        [SerializeField] private GameObject continueAsGuestButton;
        [SerializeField] private GameObject returnToResultsButton;
    
        [Header("loading text")]
        [SerializeField] private GameObject loadingText;
        
        private const string UnknownErrorResponse = "There was an error logging you in, Please wait and try again.";
        private const string InvalidLoginResponse = "Entered email and/or password is incorrect.";
        private const string InvalidCreateAccountResponse = "1 or more fields are invalid.";
        private const string UsernameTaken = "Entered username is taken";
        private const string SendEmailResponse = "Password recovery has been sent to this email if connected to an account.";
        
        private bool _showPassword;

        private void OnEnable() {
            _showPassword = false;
            UpdatePasswordVisibility();
            bool isPlayerGuest = PlayerAuthManager.GetPlayerState() == PlayerAuthManager.PlayerState.Guest;
            if (!isPlayerGuest) {
                EventBus<PlayMusicEvent>.Publish(new PlayMusicEvent {
                    MusicData = loginMusic
                });
            }
            continueAsGuestButton.SetActive(!isPlayerGuest);
            returnToResultsButton.SetActive(isPlayerGuest);
            EndLoading();
            SetLoginState();
        }

        public void ChangePasswordVisibility() {
            _showPassword = !_showPassword;
            UpdatePasswordVisibility();
        }

        private void UpdatePasswordVisibility() {
            showPasswordButtonImage.sprite = _showPassword ? hidePasswordIcon : showPasswordIcon;
            passwordField.contentType = _showPassword ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            passwordField.Select();
        }
        
        public void OnNewAccountClicked() => SetNewAccountState();

        public void OnGuestLoginClicked() {
            PlayerAuthManager.LoginAsGuest();
            GameStateManager.Instance.MoveToState(GameStateManager.GameState.Start);
        }
    
        public void OnReturnToResultsClicked() => GameStateManager.Instance.MoveToState(GameStateManager.GameState.Results);

        public void OnLoginClicked() {
            StartLoading();
            PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest {
                Email = emailField.text,
                Password = passwordField.text
            }, OnLoginSuccess, OnLoginError);
        }
        private void OnLoginError(PlayFabError err) {
            Debug.Log(err.GenerateErrorReport());
            generalInfoText.text = err.Error switch {
                PlayFabErrorCode.InvalidParams => InvalidLoginResponse,
                PlayFabErrorCode.InvalidEmailOrPassword => InvalidLoginResponse,
                PlayFabErrorCode.AccountDeleted => InvalidLoginResponse,
                _ => UnknownErrorResponse
            };
            EndLoading();
        }
        private void OnLoginSuccess(LoginResult suc) {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest {
                PlayFabId = suc.PlayFabId,
                ProfileConstraints = new PlayerProfileViewConstraints {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true
                }
            }, OnGetPlayerProfileSuccess, OnGetPlayerProfileError);
        }
        private void OnGetPlayerProfileError(PlayFabError err) {
            generalInfoText.text = UnknownErrorResponse;
            EndLoading();
        }
        private void OnGetPlayerProfileSuccess(GetPlayerProfileResult suc) {
            PlayerAuthManager.PlayerState fromState = PlayerAuthManager.GetPlayerState();
            PlayerAuthManager.Login(suc.PlayerProfile);
            PlayerDataManager.Instance.RetrievePlayerData();
            GameStateManager.Instance.MoveToState(fromState == PlayerAuthManager.PlayerState.Guest
                ? GameStateManager.GameState.Results
                : GameStateManager.GameState.Start);
        }

        public void OnForgotPasswordClicked() => SetForgotPasswordState();

        public void OnSendEmailClicked() {
            StartLoading();
            PlayFabClientAPI.SendAccountRecoveryEmail(new SendAccountRecoveryEmailRequest {
                TitleId = PlayFabSettings.TitleId,
                Email = emailField.text,
            }, OnSendEmailSuccess, OnSendEmailError);
        }
        private void OnSendEmailError(PlayFabError err) {
            generalInfoText.text = SendEmailResponse;
            EndLoading();
        }
        private void OnSendEmailSuccess(SendAccountRecoveryEmailResult suc) {
            generalInfoText.text = SendEmailResponse;
            EndLoading();
        } 
        public void OnBackClicked() => SetLoginState();

        public void OnCreateAccountClicked() {
            StartLoading();
            PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest {
                DisplayName = usernameField.text,
                Email = emailField.text,
                Password = passwordField.text,
                RequireBothUsernameAndEmail = false
            }, OnCreateAccountSuccess, OnCreateAccountError);
        }
        private void OnCreateAccountError(PlayFabError err) {
            Debug.Log(err.GenerateErrorReport());
            generalInfoText.text = err.Error switch {
                PlayFabErrorCode.NameNotAvailable => UsernameTaken,
                PlayFabErrorCode.InvalidParams => InvalidCreateAccountResponse,
                PlayFabErrorCode.EmailAddressNotAvailable => InvalidCreateAccountResponse,
                _ => UnknownErrorResponse
            };
            EndLoading();
        }
        private void OnCreateAccountSuccess(RegisterPlayFabUserResult suc) {
            PlayFabProfilesAPI.SetDisplayName(new SetDisplayNameRequest {
                DisplayName = usernameField.text,
                Entity = new EntityKey {
                    Id = suc.EntityToken.Entity.Id,
                    Type = suc.EntityToken.Entity.Type
                }
            }, null, null);
        
            PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest {ImageUrl = "defaultPfp"}, null, null);
        
            generalInfoText.text = "Account successfully created, Login to proceed";
            SetLoginState(true);
            EndLoading();
        }

        private void SetNewAccountState() {
            emailObjects.SetActive(true);
            passwordObjects.SetActive(true);
            usernameObjects.SetActive(true);

            emailField.text = "";
            emailField.caretPosition = 0;
            passwordField.text = "";
            passwordField.caretPosition = 0;
            usernameField.text = "";
            usernameField.caretPosition = 0;
        
            generalInfoText.text = "";
        
            loginButton.SetActive(false);
            newAccountButton.SetActive(false);
            forgotPasswordButton.SetActive(true);
            createAccountButton.SetActive(true);
            sendEmailButton.SetActive(false);
            backButton.SetActive(true);
        }

        private void SetForgotPasswordState() {
            emailObjects.SetActive(true);
            passwordObjects.SetActive(false);
            usernameObjects.SetActive(false);

            emailField.text = "";
            emailField.caretPosition = 0;
        
            generalInfoText.text = "";
        
            loginButton.SetActive(false);
            newAccountButton.SetActive(false);
            forgotPasswordButton.SetActive(false);
            createAccountButton.SetActive(false);
            sendEmailButton.SetActive(true);
            backButton.SetActive(true);
        }

        private void SetLoginState(bool preserveInputs = false) {
            emailObjects.SetActive(true);
            passwordObjects.SetActive(true);
            usernameObjects.SetActive(false);
        
            if (!preserveInputs) {
                emailField.text = "";
                emailField.caretPosition = 0;
                passwordField.text = "";
                passwordField.caretPosition = 0;
            
                generalInfoText.text = "";
            }
        
            loginButton.SetActive(true);
            newAccountButton.SetActive(true);
            forgotPasswordButton.SetActive(true);
            createAccountButton.SetActive(false);
            sendEmailButton.SetActive(false);
            backButton.SetActive(false);
        }

        private void StartLoading() {
            loginContainer.SetActive(false);
            loadingText.SetActive(true);
        }

        private void EndLoading() {
            loginContainer.SetActive(true);
            loadingText.SetActive(false);
        }
    }
}
