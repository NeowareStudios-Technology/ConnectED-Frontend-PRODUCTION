using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class getProfileinfo : MonoBehaviour {
    //this gets your profile
    private Profile profile;
    public Profile otherProfile;
    private string jsonString;
    private Coroutine getOtherProfile;
    public Jsonparser j;
    public Login l;
    public string idToken;

    public void GetmyProfile()
    {
        StartCoroutine(GetProfile());
    }
    

    IEnumerator GetProfile()
    {
		FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;


        Debug.Log("Getting profile with "+PlayerPrefs.GetString("email"));
		//using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/" + PlayerPrefs.GetString("email").ToLower()))
        {
            


			www.SetRequestHeader("Authorization", "Bearer " + j.token);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.responseCode);
                Debug.Log(www.url);
                Debug.Log(www.GetRequestHeader("Authorization"));
                Debug.Log(www.GetRequestHeader("Content-Type"));
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                //not error
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                profile = JsonUtility.FromJson<Profile>(jsonString);
                //sets your profile and continues logging in
                j.SetProfile(profile);
                if(l)
                    l.Continue();
            }
        };
    }


   //this handles the input of pictures, not sure why it is here
    public void SetPicture()
    {
        Texture2D tex = new Texture2D(200, 200);
        byte[] img = System.Convert.FromBase64String(j.profile.photo);
        Debug.Log(img);
        tex.LoadImage(img, false);

        this.gameObject.GetComponent<RawImage>().texture = tex;
    }
}
