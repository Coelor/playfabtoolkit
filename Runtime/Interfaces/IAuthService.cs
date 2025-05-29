using System;

namespace PlayFabToolkit.Interfaces
{
    public interface IAuthService
    {
        void LoginWithEmail(string email, string password, Action<bool, string, string> onComplete);
        void RegisterWithEmail(string email, string password, string displayName, Action<bool, string, string> onComplete);
        void LoginWithCustomId(string customId, Action<bool, string> onComplete);
        void Logout();
        void ResetPassword(string email, Action<bool, string> onComplete);
        void GetEntityToken(Action<string, string> onSuccess, Action<string> onError);
    }
}
