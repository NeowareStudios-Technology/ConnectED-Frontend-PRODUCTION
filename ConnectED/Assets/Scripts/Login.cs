using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using System.IO;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    //this script works with many others to log you in
    //this is located in the login object on the login button
    public Animator EventPanel;
    public InputField email;
    public InputField Password;
    public InputField SignupEmail;
    public InputField SignupPassword;
    public Jsonparser j;
    public getProfileinfo info;
    public TeamsGet TeamsGet;
    //this is called to begin logging in
    public void StartLoginProcess()
    {
        //login credentials
        string e;
        string p;
        //determine if user is logging in directly, or logging in automatically after signup
        if (email.text != "" && Password.text != "")
        {
            e = email.text;
            p = Password.text;
        }
        else
        {
            e = SignupEmail.text;
            p = SignupPassword.text;
        }

        Debug.Log(PlayerPrefs.GetString("email"));
        Debug.Log(PlayerPrefs.GetString("password"));
        //this is used for automatically logging you in 
        if (PlayerPrefs.GetString("email","email") != "email" && PlayerPrefs.GetString("password","password") != "password")
        {
            e = PlayerPrefs.GetString("email", "email");
            p = PlayerPrefs.GetString("password", "password");
        }
        else
        {
            //sets your email and password for later
            PlayerPrefs.SetString("email", e);
            PlayerPrefs.SetString("password", p);

        }
        Debug.Log(PlayerPrefs.GetString("email"));
        Debug.Log(PlayerPrefs.GetString("password"));
        //firebase signin
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(e, p).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                PlayerPrefs.DeleteAll();
                j.alreadyin.SetActive(false);
                SceneManager.LoadScene(0);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                PlayerPrefs.DeleteAll();
                j.alreadyin.SetActive(false);
                SceneManager.LoadScene(0);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            j.UserID = newUser.UserId;
            GetToken(auth);

            Debug.Log("Logging in: " + e + " " + p);
            EventPanel.enabled = true;
        });

        //clear signup and login text inputs
        email.text = "";
        Password.text = "";
        SignupEmail.text = "";
        SignupPassword.text = "";



    }
    public InputField resetEmail;
    //this sends you and email if you forgot your password
    public void forgotPassword()
    {
        if (resetEmail.text.Length > 6)
        {
            var auth = FirebaseAuth.DefaultInstance;
            var emailAddress = j.email;
            auth.SendPasswordResetEmailAsync(resetEmail.text);
            // Email sent.
        }
    }
    //this gets your token
    public void GetToken(FirebaseAuth auth)
    {
        FirebaseUser user = auth.CurrentUser;

        user.TokenAsync(true).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("TokenAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("TokenAsync encountered an error: " + task.Exception);
                return;
            }

            j.token = task.Result;
            Debug.Log(j.token);
            loading.SetActive(true);
            pns.gameObject.SetActive(true);
            pns.getNotifications();
            updatePosition();
            JoinWithTeam.getTeams();
          // Send token to your backend via HTTPS
          // ...
      });
    }
    public GameObject loading;
    public string dbProfilePut = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles";
    private IEnumerator coroutine;
    //this changes your location each time you log in
    public void updatePosition()
    {

        Profile profile = new Profile();
        profile.lat = j.lat;
        profile.lon = j.lon;
        string ourProfile = JsonUtility.ToJson(profile);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(ourProfile);
        UnityWebRequest www2 = UnityWebRequest.Put(dbProfilePut, ourProfile);
        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", "Bearer " + j.token);
        www2.SetRequestHeader("Content-Type", "application/json");
        coroutine = Post(www2);
        StartCoroutine(coroutine);
    }


    private IEnumerator Post(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.uploadHandler.data);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.GetRequestHeader("Authorization"));

        if (www.responseCode.ToString() == "200")
        {
            //now that your location is updated continue logging in with these functions
            info.GetmyProfile();

            TeamsGet.getTeams();
        }
    }
    public profileNotificationSetter pns;
    public JoinWithTeamInitialize JoinWithTeam;
    //this gets the initial token for your account
    public void GetInitialToken(FirebaseAuth auth)
    {
        FirebaseUser user = auth.CurrentUser;

        user.TokenAsync(true).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("TokenAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("TokenAsync encountered an error: " + task.Exception);
                return;
            }

            j.token = task.Result;
            Debug.Log(j.token);
            // Send token to your backend via HTTPS
            // ...
            j.SignUp();
        });
    }

    //this script enables the event panel and triggers its animation
    public void Continue()
    {
        if (j.profileSet){
            EventPanel.enabled = true;

        }

    }
}
