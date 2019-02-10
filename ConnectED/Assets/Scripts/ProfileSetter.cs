using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine.Networking;
using System.Text;
using System;

public class ProfileSetter : MonoBehaviour {
    //this sets your profile page
    public Jsonparser j;
    public getProfileinfo g;
    public RawImage p;
    public Text username;
    public Text Interest1;
    public Text Interest2;
    public Text Interest3;
    public Text hours;
    public Text Skill1;
    public Text Skill2;
    public Text Skill3;
    public Text Education;
    public Text totalHours;
    public Text totalOpportunities;
    public bool set = false;
    //this sets all the values if applicable 
    public void setProfile()
    {
        SetPicture();
        hours.text = j.profile.hours.ToString();
        username.text = j.profile.first_name + " " + j.profile.last_name;
        set = true;

        if (j.profile.interests == null)
        {
            Interest1.text = "None";
            Destroy(Interest2.gameObject);
            Destroy(Interest3.gameObject);
        }
        else
        {
            if (j.profile.interests.Length >= 3)
            {
                Interest1.text = j.profile.interests[0];
                Interest2.text = j.profile.interests[1];
                Interest3.text = j.profile.interests[2];
            }
            if (j.profile.interests.Length == 2)
            {
                Interest1.text = j.profile.interests[0];
                Interest2.text = j.profile.interests[1];
                Destroy(Interest3.gameObject);
            }
            if (j.profile.interests.Length == 1)
            {
                Interest1.text = j.profile.interests[0];
                Destroy(Interest2.gameObject);
                Destroy(Interest3.gameObject);
            }
        }
        if (j.profile.skills == null)
        {
            Skill1.text = "None";
            Destroy(Skill2.gameObject);
            Destroy(Skill3.gameObject);
        }
        else
        {
            if (j.profile.skills.Length >= 3)
            {
                Skill1.text = j.profile.skills[0];
                Skill2.text = j.profile.skills[1];
                Skill3.text = j.profile.skills[2];
            }
            if (j.profile.skills.Length == 2)
            {
                Skill1.text = j.profile.skills[0];
                Skill2.text = j.profile.skills[1];
                Destroy(Skill3.gameObject);
            }
            if (j.profile.skills.Length == 1)
            {
                Skill1.text = j.profile.interests[0];
                Destroy(Skill2.gameObject);
                Destroy(Skill3.gameObject);
            }
        }
            if (j.profile.education != "" && j.profile.education != null)
            {
                Education.text = j.profile.education;
            } 
        else
        {
            Education.text = "Not Specified";
        }
        searchEvents();

    }
    public void SetPicture()
    {
        Texture2D tex = new Texture2D(400, 400);
        if (j.profile.photo != null && j.profile.photo.Length > 300)
        {
            byte[] img = System.Convert.FromBase64String(j.profile.photo);
            tex.LoadImage(img, false);

            p.texture = tex;
        }
    }

    private string jsonString;
    private string searchURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/";
    //this will set all your events
    public void searchEvents()
    {
        searchURL += PlayerPrefs.GetString("email").ToLower() + "/events";

        UnityWebRequest www2 = UnityWebRequest.Get(searchURL);
        www2.SetRequestHeader("Authorization", "Bearer " + j.token);

        StartCoroutine(eventPut(www2));
    }
    private profileHistory hist;
    private IEnumerator eventPut(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.responseCode == 200)
        {
            byte[] results = www.downloadHandler.data;
            jsonString = "";
            jsonString = Encoding.UTF8.GetString(results);
            Debug.Log(jsonString);
            hist = new profileHistory();
            hist = JsonUtility.FromJson<profileHistory>(jsonString);
            if (jsonString != "{}")
            {
                eventPopulator();
            } else{
                gameObject.GetComponent<Image>().raycastTarget = true;
                gameObject.GetComponent<Image>().color = Color.white;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    public GameObject ProfileCreatedEventContainer;
    public GameObject ProfileHistoryEventContainer;
    public GameObject TeamEventPrefab;
    //this populates the events in your profile
    public void eventPopulator()
    {
        GameObject newEvent;
        int childKillCount = ProfileHistoryEventContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(ProfileHistoryEventContainer.transform.GetChild(i).gameObject);
        }
        childKillCount = ProfileCreatedEventContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(ProfileCreatedEventContainer.transform.GetChild(i).gameObject);
        }
        if (hist.created_events == null)
        {
            gameObject.GetComponent<Image>().raycastTarget = true;
            gameObject.GetComponent<Image>().color = Color.white;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < hist.created_events.Length; i++)
            {
                //this populates the created events
                Debug.Log(hist.created_events[i]);
                StartCoroutine(Populator(i, 1, hist.created_events[i]));
                //newEvent = Instantiate(TeamEventPrefab, ProfileCreatedEventContainer.transform);
            }
        }
        if (hist.completed_events != null)
        {
            for (int i = 0; i < hist.completed_events.Length; i++)
            {
                //this populates the completed events
                Debug.Log(hist.completed_events[i]);
                StartCoroutine(Populator(i, 2, hist.completed_events[i]));
                //newEvent = Instantiate(TeamEventPrefab, ProfileCreatedEventContainer.transform);
            }
        }
            




        //if (childKillCount > 7)
        //{
        //    TeamEventHistoryContainer.AddComponent<ContentSizeFitter>();
        //    TeamEventHistoryContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        //}
    }
    private Event Event;
    private string getEventurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";
    //mode 1 for created mode 2 for completed
    IEnumerator Populator(int i, int mode, string s)
    {



        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        Debug.Log(i);
        Debug.Log(s);
        //this parses the string into the email and event
        string str1 = s.Split('_')[0];
        string str2 = s.Split('_')[1];
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        if (s.Length > 4)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(getEventurl + str1 + "/" + str2))
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
                    Debug.Log(www.responseCode);
                    byte[] results = www.downloadHandler.data;
                    jsonString = "";
                    jsonString = Encoding.UTF8.GetString(results);
                    Debug.Log(jsonString);
                    Event = JsonUtility.FromJson<Event>(jsonString);

                    if (mode == 1)
                    {
                        GameObject newobj = Instantiate(TeamEventPrefab, ProfileCreatedEventContainer.transform);
                        newobj.GetComponent<TeamEventInit>().initEvent(Event);
                    }
                    if (mode == 2){
                        GameObject newobj = Instantiate(TeamEventPrefab, ProfileHistoryEventContainer.transform);
                        newobj.GetComponent<TeamEventInit>().initEvent(Event);
                    }
                }
            };
        }
        //after creating those, tidy up
        if (i == hist.created_events.Length - 1)
        {
            totalOpportunities.text = (i+1).ToString();
            gameObject.GetComponent<Image>().raycastTarget = true;
            gameObject.GetComponent<Image>().color = Color.white;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

       
    }




[System.Serializable]
public class profileHistory{
    public string[] completed_events;
    public string[] created_events;
    public string[] registered_events;
}