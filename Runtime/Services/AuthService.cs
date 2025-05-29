using PlayFab;
using PlayFab.ClientModels;
using PlayFab.AuthenticationModels;
using PlayFabToolkit.Interfaces;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PlayFabToolkit.Services
{
    public class AuthService : IAuthService
    {
        private string displayName;

        public AuthService(string titleId = null)
        {
            if (!string.IsNullOrEmpty(titleId))
                PlayFabSettings.staticSettings.TitleId = titleId;

            PlayFab.Internal.PlayFabWebRequest.SkipCertificateValidation(); // Local dev only
        }

        public void LoginWithEmail(string email, string password, Action<bool, string, string> onComplete)
        {
            if (!ValidateInput(email, password))
            {
                onComplete?.Invoke(false, "Invalid input", null);
                return;
            }

            var request = new LoginWithEmailAddressRequest
            {
                Email = email,
                Password = password,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, result =>
            {
                displayName = result.InfoResultPayload.PlayerProfile?.DisplayName;
                onComplete?.Invoke(true, "Login successful", displayName);
            }, error =>
            {
                onComplete?.Invoke(false, error.GenerateErrorReport(), null);
            });
        }

        public void RegisterWithEmail(string email, string password, string displayname, Action<bool, string, string> onComplete)
        {
            if (!ValidateInput(email, password, displayname))
            {
                onComplete?.Invoke(false, "Invalid input", null);
                return;
            }

            displayName = displayname;

            var request = new RegisterPlayFabUserRequest
            {
                Email = email,
                Password = password,
                DisplayName = displayname,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request, result =>
            {
                onComplete?.Invoke(true, "Registration successful", displayName);
            }, error =>
            {
                onComplete?.Invoke(false, error.GenerateErrorReport(), null);
            });
        }

        public void LoginWithCustomId(string customId, Action<bool, string> onComplete)
        {
            if (string.IsNullOrEmpty(customId))
            {
                onComplete?.Invoke(false, "Custom ID cannot be empty.");
                return;
            }

            var request = new LoginWithCustomIDRequest
            {
                CustomId = customId,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, result =>
            {
                onComplete?.Invoke(true, result.PlayFabId);
            }, error =>
            {
                onComplete?.Invoke(false, error.GenerateErrorReport());
            });
        }

        public void Logout()
        {
            if (PlayFabClientAPI.IsClientLoggedIn())
                PlayFabClientAPI.ForgetAllCredentials();
        }

        public void ResetPassword(string email, Action<bool, string> onComplete)
        {
            if (!ValidateEmail(email))
            {
                onComplete?.Invoke(false, "Invalid email format");
                return;
            }

            var request = new SendAccountRecoveryEmailRequest
            {
                Email = email,
                TitleId = PlayFabSettings.TitleId
            };

            PlayFabClientAPI.SendAccountRecoveryEmail(request, result =>
            {
                onComplete?.Invoke(true, "Password reset email sent!");
            }, error =>
            {
                onComplete?.Invoke(false, error.GenerateErrorReport());
            });
        }

        public void GetEntityToken(Action<string, string> onSuccess, Action<string> onError)
        {
            PlayFabAuthenticationAPI.GetEntityToken(new GetEntityTokenRequest(), result =>
            {
                onSuccess?.Invoke(result.Entity.Id, result.Entity.Type);
            }, error =>
            {
                onError?.Invoke(error.GenerateErrorReport());
            });
        }

        // --- Validation ---
        private bool ValidateInput(string email, string password = null, string displayname = null)
        {
            if (!ValidateEmail(email)) return false;
            if (password != null && !ValidatePassword(password)) return false;
            if (displayname != null && !ValidateDisplayName(displayname)) return false;
            return true;
        }

        private bool ValidateEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email ?? "", emailPattern);
        }

        private bool ValidatePassword(string password)
        {
            string passwordPattern = @"^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{6,}$";
            return Regex.IsMatch(password ?? "", passwordPattern);
        }

        private bool ValidateDisplayName(string name)
        {
            string pattern = @"^[a-zA-Z0-9][a-zA-Z0-9_]{1,18}[a-zA-Z0-9]$";
            return Regex.IsMatch(name ?? "", pattern);
        }
    }
}
