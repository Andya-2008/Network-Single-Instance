using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper 
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;
    public static async Task<AuthState> DoAuth(int maxRetries = 5) {
        if (AuthState == AuthState.Authenticated)
        {
            return AuthState;
        }

        if (AuthState == AuthState.Authenticating) {
            Debug.LogWarning("Already Authenticating!");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxRetries);
        Debug.Log(AuthState);
        return AuthState;
    }

    private static async Task<AuthState> Authenticating() {

        while (AuthState == AuthState.Authenticated || AuthState == AuthState.NotAuthenticated) {
            await Task.Delay(200);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries) {
        
        AuthState = AuthState.Authenticating;
        int tries = 0;
        while (AuthState == AuthState.Authenticating && tries < maxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authEx)
            {
                Debug.LogError(authEx);
                AuthState = AuthState.Error;

            }
            catch (RequestFailedException reqEx) {
                Debug.LogError(reqEx);
                AuthState = AuthState.Error;
            }
            
            tries++;
            await Task.Delay(1000);  //wait 1 second
        }
        if (AuthState != AuthState.Authenticated) {
            Debug.LogWarning($"Player was not signed in successfully after {tries} tries");
            AuthState = AuthState.TimeOut;
        }
    }

}

public enum AuthState { 
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}