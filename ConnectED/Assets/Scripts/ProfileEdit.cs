using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class ProfileEdit : MonoBehaviour {
    //this script is how you edit your profile
    public Jsonparser j;
    public InputField fname;
    public InputField lname;
    public InputField Email;
    private Profile profile;
    public string[] skills;
    public RawImage editImage;
    public string[] interests;
    public bool skillCheck = false;
    public bool interestsCheck = false;
    public bool fnameCheck = false;
	public bool lnameCheck = false;
    public bool EmailCheck = false;
    public bool scheduleCheck = false;
    public bool mon;
    public bool tue;
    public bool wed;
    public bool thu;
    public bool fri;
    public bool sat;
    public bool sun;
    public string timeday;
    public returnPressedFields r;
    public returnPressedFields s;
    private IEnumerator coroutine;
    public string dbProfilePut = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles";
    protected Firebase.Auth.FirebaseAuth auth;
    //change your name
    public void GetProfile()
    {
        fname.text = j.profile.first_name;
        lname.text = j.profile.last_name;
        //we do not allow the changing of emails
        //Email.text = PlayerPrefs.GetString("email","email");
    }
    //edit your skills or other things
    public void editProfile()
    {
        profile = new Profile();
        if(fnameCheck)
            profile.first_name = fname.text;
        if(lnameCheck)
            profile.last_name = lname.text;
        //if(EmailCheck)
        //    profile.email = Email.text;
        if(skillCheck)
            profile.skills = skills;
        if(interestsCheck)
            profile.interests = interests;
        //change your profile photo
        if (editImage.color.a != 0)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(editImage.texture.width, editImage.texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(editImage.texture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(editImage.texture.width, editImage.texture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            //https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
            profile.photo = Convert.ToBase64String(myTexture2D.EncodeToJPG());
            Debug.Log(profile.photo);
            //read in with texture2d.loadimage(bytedata);
        }
            
        if(scheduleCheck)
        {
            profile.mon = mon;
            profile.tue = tue;
            profile.wed = wed;
            profile.thu = thu;
            profile.fri = fri;
            profile.sat = sat;
            profile.sun = sun;
            profile.time_day = timeday;
        }

        string t = "Bearer " + j.token;

        string ourProfile = JsonUtility.ToJson(profile);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(ourProfile);
        UnityWebRequest www2 = UnityWebRequest.Put(dbProfilePut, ourProfile);
        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", t);
        www2.SetRequestHeader("Content-Type", "application/json");
        coroutine = Post(www2);
        StartCoroutine(coroutine);
    }

    //post the new stuff to the db
    private IEnumerator Post(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.uploadHandler.data);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if(www.responseCode.ToString() == "200")
       {
            //on success exit the edit menu
            this.gameObject.GetComponent<Animator>().SetTrigger("EditExit");
            this.gameObject.GetComponent<Animator>().ResetTrigger("Edit");
           //if (EmailCheck && Email.text != j.profile.email)
           //    changeEmail();
       }
    }

    //public void changeEmail()
    //{
    //    auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    //    auth.CurrentUser.UpdateEmailAsync(Email.text);
    //}
    //changing your password
    public GameObject passwordPage;
    public InputField pass;
    public GameObject newPassPage;
    public InputField newPass;
    public GameObject newRePassPage;
    public InputField newRePass;
    public Animator editAnimator;

    public void passwordNext()
    {
        if (newRePassPage.activeSelf)
        {
            if (newRePass.text == newPass.text)
            {
                changePassword();
                editAnimator.SetTrigger("EditExit");
                Debug.Log("passwords match");
                return;
            }
        }

        if (newPassPage.activeSelf)
        {
            if (newPass.text.Length >= 6)
            {
                Debug.Log("new password is good");
                newRePassPage.SetActive(true);
                return;
            }
        }

        if (passwordPage.activeSelf)
        {
            if (pass.text == PlayerPrefs.GetString("password"))
            {
                Debug.Log("password is good");
                newPassPage.SetActive(true);
                return;
            }
        } 
        Debug.Log(PlayerPrefs.GetString("password"));
        Debug.Log("password is not good");
    }

    public void changePassword()
    {
  		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.CurrentUser.UpdatePasswordAsync(newRePass.text);
    }



    public void getFields()
    {
        interestsCheck = true;
        interests = r.returnFields();
    }

    public void getSkills()
    {
        skillCheck = true;
        skills = s.returnFields();
    }

    public void fnameChange(){
        fnameCheck = true;
    }

    public void lnameChange()
    {
        lnameCheck = true;
    }

    //public void EmailChange()
    //{
    //    EmailCheck = true;
    //}
}
