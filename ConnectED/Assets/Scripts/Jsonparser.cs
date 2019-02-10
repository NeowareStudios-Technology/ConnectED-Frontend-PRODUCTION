using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class Jsonparser : MonoBehaviour {
    //this is the main script in the app as it holds the token, which is referenced frequently
    //Also it holds the users profile information
    public Text email;
    public string UserID;
    public string token;
    public InputField password;
    public Text firstname;
    public Text lastname;
    public float lat = -81.191458f;
    public float lon = 28.590012f;
    public GameObject mon;
    public GameObject tue;
    public GameObject wed;
    public GameObject thu;
    public GameObject fri;
    public GameObject sat;
    public GameObject sun;
    public timeOfDay timeDay;
    public returnPressed education;
    public returnPressed interests;
    public returnPressed skills;
    public RawImage profilePic;
    public Profile profile;
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    public bool profileSet = false;
    public Animator a;
    public Login l;
    public ProfileSetter setter;
    public GameObject alreadyin;
    private string path;
    private string jsonString;
    private string dbprofiles = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles";
    //Post
    private IEnumerator coroutine;
    //this is how you get the users location


    private IEnumerator StartLocationService()
    {
        Debug.Log("LOCATION START");
        if (!Input.location.isEnabledByUser){
 
            Debug.Log("LOCATION FAILED");
            yield break;
        }
        //this is the process described in the tutorial
        Input.location.Start();
        int maxWait = 20;
        Debug.Log("BEOFRE WAIT LOOOP");
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log("IN WAIT LOOOP");
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if(maxWait <= 0){
            Debug.Log("DOING LOGIN LOGOUT STUFF");
            Debug.Log("Timed out");
            if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
            {
                alreadyin.SetActive(true);
                l.StartLoginProcess();
            }
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
           
            lat = -81.191458f;
            lon = 28.590012f;
            Debug.Log("Unable to determine device location");
            if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
            {
                alreadyin.SetActive(true);
                l.StartLoginProcess();
            }
            yield break;
        }

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;
        Debug.Log("lat: " + lat + " lon: " + lon);
        if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
        {
            alreadyin.SetActive(true);
            l.StartLoginProcess();
        }
        yield break;
    }

    void Awake()
    {
        StartCoroutine("StartLocationService");
    }

    /* private void StartLocationService()
    {
        if (!Input.location.isEnabledByUser){
 
            return;//yield break;
        }
        //this is the process described in the tutorial
        Input.location.Start();
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if(maxWait <= 0){
            Debug.Log("Timed out");
            if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
            {
                alreadyin.SetActive(true);
                l.StartLoginProcess();
            }
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
           
            lat = -81.191458f;
            lon = 28.590012f;
            Debug.Log("Unable to determine device location");
            if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
            {
                alreadyin.SetActive(true);
                l.StartLoginProcess();
            }
            yield break;
        }

        lat = Input.location.lastData.latitude;
        lon = Input.location.lastData.longitude;
        Debug.Log("lat: " + lat + " lon: " + lon);
        if (PlayerPrefs.GetString("email", "email") != "email" && PlayerPrefs.GetString("password", "password") != "password")
        {
            alreadyin.SetActive(true);
            l.StartLoginProcess();
        }
        yield break;
    }*/
    // Use this for initialization

    void Start() {

        //this is where the app flow mostly begins
        System.DateTime DateTime = DateTime.Now;
        Debug.Log(DateTime.DayOfWeek);
        //firebase init
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("Firebase OK!");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
			}
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://connected-dev-214119.firebaseio.com/");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Debug.Log("start finished");
       
		});
    }
    //creates firebase account
	public void CreateProfile()
    {
        
        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
        
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            UserID = newUser.UserId;
            PlayerPrefs.SetString("email", email.text.ToLower());
            PlayerPrefs.SetString("password", password.text);
            Debug.Log("PASSWORD "+password.text);
            Debug.Log("EMAIL "+ email.text.ToLower());

            l.GetInitialToken(auth);
        });


        //create a profile

    }

    public GameObject loading;


    //this script creates your profile based on the signup page
    public void SignUp()
    {
        loading.SetActive(true);
        path = Application.streamingAssetsPath + "/Profile.json";
        jsonString = File.ReadAllText(path);
        profile = JsonUtility.FromJson<Profile>(jsonString);
        Debug.Log("signing up");

        profile.first_name = firstname.text;
        profile.last_name = lastname.text;
        profile.email = email.text;
        PlayerPrefs.SetString("email", email.text);
        PlayerPrefs.SetString("password", password.text);
        profile.time_day = timeDay.setTime();
        profile.education = education.Check();
        profile.interests = interests.CheckScrollVersion();
        profile.skills = skills.CheckScrollVersion();
        profile.mon = mon.GetComponent<spriteSwitcher>().pressed;
        profile.tue = tue.GetComponent<spriteSwitcher>().pressed;
        profile.wed = wed.GetComponent<spriteSwitcher>().pressed;
        profile.thu = thu.GetComponent<spriteSwitcher>().pressed;
        profile.fri = fri.GetComponent<spriteSwitcher>().pressed;
        profile.sat = sat.GetComponent<spriteSwitcher>().pressed;
        profile.sun = sun.GetComponent<spriteSwitcher>().pressed;
        profile.lat = lat;
        profile.lon = lon;
        //this is how you convert a texture to a jpg
        if (profilePic.color.a == 1)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(profilePic.texture.width, profilePic.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(profilePic.texture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(profilePic.texture.width, profilePic.texture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            //https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
            profile.photo = Convert.ToBase64String(myTexture2D.EncodeToJPG());
            Debug.Log(profile.photo);
            //read in with texture2d.loadimage(bytedata);
        }

        string t = "Bearer " + token;

        string ourProfile = JsonUtility.ToJson(profile);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(ourProfile);
        UnityWebRequest www2 = UnityWebRequest.Post(dbprofiles, ourProfile);
        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", t);
        www2.SetRequestHeader("Content-Type", "application/json");
        coroutine = Post(www2);
        StartCoroutine(coroutine);
    }


    private IEnumerator Post(UnityWebRequest www){
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.uploadHandler.data);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if(www.error == null)
        {
            
            l.StartLoginProcess();
            Debug.Log("JUST BEFORE LOGIN PROCESS");
        }
    }
    public EventSpawner eventSpawner;
    //this is how you set the profile, when you log in 
    public void SetProfile(Profile p)
    {
        profile = p;
        setter.setProfile();
        profileSet = true;
        eventSpawner.populateEvents();

    }
    //this is a reference to the active explore tile, so that we can keep track of what we are looking at
    public void setExploreTile(GameObject o)
    {
        exploreTile = o;
    }

    //functions to help the Detail changer
    public GameObject exploreTile;
    //this changes the registration status of the event tiles so if you back out of an event and go back in it will have the correct registration status
    public void changeExploreRegisterStatus(int i)
    {
        exploreTile.GetComponent<EventInitializer>().setRegistration(i);
    }
    public GameObject TeamButton;
    //this keeps track of what team button you pressed
    public void setTeamButton(GameObject o)
    {
        TeamButton = o;
    }

    //functions to help the Detail changer
    public void changeTeamRegisterStatus(int priv)
    {
        
        TeamButton.GetComponent<teamInitializer>().setRegistration(priv);
    }
}


[System.Serializable]
public class Profile
{
    public string first_name;
    public string last_name;
    public float hours;
    public string email;
    public string env_pref;
    public string education;
    public string[] interests;
    public string[] skills;
    public float lat;
    public float lon;
    public bool mon;
    public bool tue;
    public bool wed;
    public bool thu;
    public bool fri;
    public bool sat;
    public bool sun;
    public int search_rad;
    public string time_day;
    public string photo;
}


