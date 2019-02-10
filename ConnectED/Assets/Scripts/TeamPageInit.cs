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

public class TeamPageInit : MonoBehaviour {

    private Team team;
    public Text TeamName;
    public RawImage teamPhoto;
    public Text TeamDesc;
    public GameObject[] teamRoster;
    public int totalHours;
    public int totalOpportunities;
    public GameObject[] eventHistory;
    public GameObject TeamProfilePrefab;
    public GameObject TeamProfileContainer;
    public GameObject TeamEventPrefab;
    public GameObject TeamEventHistoryContainer;
    public Text Hours;
    public Text Opportunities;
    public Button Join;
    private Jsonparser j;

    private string joinTeamURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/";
    //this initializes the team page
    public void setTeamPage(Team t)
    {
        team = t;
        TeamName.text = t.t_name;
        TeamDesc.text = t.t_desc;
        Hours.text = t.t_hours.ToString();

        j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
        if (t.t_photo != null && t.t_photo.Length > 300)
        {
            Texture2D tex = new Texture2D(200, 200);


            byte[] img = System.Convert.FromBase64String(t.t_photo);
            tex.LoadImage(img, false);

            teamPhoto.texture = tex;

        }



        //this clears the team page, then sets it with the correct information
        int childKillCount = TeamProfileContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(TeamProfileContainer.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < t.t_member_num; i++)
        {
            StartCoroutine(GetProfile(t.t_members[i]));
        }
        bool deregister = false;

        if (t.t_members == null) { }
        else
        {
            for (int i = 0; i < t.t_members.Length; i++)
            {
                //this checks to see if you are already in the team, then changes the button to reflect your status
                if (t.t_members[i] == PlayerPrefs.GetString("email").ToLower())
                {
                    Debug.Log("Team matches email");
                    Join.onClick.RemoveAllListeners();


                    Debug.Log(t.t_orig_name);
                    Join.onClick.AddListener(() => Deregister(t.t_orig_name, j.token));
                    Join.onClick.AddListener(() => j.changeTeamRegisterStatus(0));
                    Join.transform.GetChild(0).GetComponent<Text>().text = "Leave Team";
                    deregister = true;

                }

            }
            if (t.t_pending_members == null) { }
            else
            {
                for (int i = 0; i < t.t_pending_members.Length; i++)
                {
                    //this happens if you are pending to get in the team
                    if (t.t_pending_members[i] == PlayerPrefs.GetString("email").ToLower())
                    {
                        Debug.Log("Team matches email pending");
                        Join.onClick.RemoveAllListeners();


                        Debug.Log(t.t_orig_name);
                        Join.onClick.AddListener(() => j.changeTeamRegisterStatus(-1));
                        Join.transform.GetChild(0).GetComponent<Text>().text = "Pending...";
                        deregister = true;

                    }

                }
            }
        }
        //otherwise its the default join team
            if (!deregister)
            {
                Debug.Log("No matching email");
                Join.onClick.RemoveAllListeners();
                Debug.Log(t.t_orig_name);
                privacy = t.t_privacy;
                Join.onClick.AddListener(() => Register(t.t_orig_name, j.token));
                Join.onClick.AddListener(() => j.changeTeamRegisterStatus(1));
                Join.transform.GetChild(0).GetComponent<Text>().text = "Join Team";
            }
        

		StartCoroutine(GetHistory(t.t_name));
    }
    private IEnumerator coroutine;
    private string privacy;

    public void Register(string s, string t)
    {

        string newEvent = "";
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newEvent);
        UnityWebRequest www2 = UnityWebRequest.Get(joinTeamURL + s + "/registration");
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = Put(www2,true, s);
        StartCoroutine(coroutine);
    }

    public void Deregister(string s, string t)
    {

        string newEvent = "";
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newEvent);
        UnityWebRequest www2 = UnityWebRequest.Delete(joinTeamURL + s + "/registration");
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = Put(www2 , false , s);
        StartCoroutine(coroutine);
    }

    //this handles registering for teams based on the register bool and the privacy status of the team
    private IEnumerator Put(UnityWebRequest www , bool register, string orig)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));

        if(!register){
            Debug.Log("Unregistered");
            Join.onClick.RemoveAllListeners();
            Join.onClick.AddListener(() => Register(orig , j.token));
            Join.transform.GetChild(0).GetComponent<Text>().text = "Join Team";
            privacy = team.t_privacy;
            Debug.Log(orig);
        } else{
            if (privacy == "p")
            {
                Join.onClick.RemoveAllListeners();
                Join.transform.GetChild(0).GetComponent<Text>().text = "Pending...";
            }
            else
            {
                Debug.Log("Registered");
                Join.onClick.RemoveAllListeners();
                Join.onClick.AddListener(() => Deregister(orig, j.token));
                Join.transform.GetChild(0).GetComponent<Text>().text = "Leave Team";
            }
            Debug.Log(orig);

        }

    }
    private string jsonString;
    private Profile profile;
    IEnumerator GetProfile(string other)
    {


        using (UnityWebRequest www = UnityWebRequest.Get("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/" + other))
        {
            www.SetRequestHeader("Authorization", "Bearer " + j.token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.url);
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = System.Text.Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                if (jsonString != "{}")
                {
                    profile = JsonUtility.FromJson<Profile>(jsonString);
                    GameObject newTeamProfile = Instantiate(TeamProfilePrefab, TeamProfileContainer.transform);
                    newTeamProfile.GetComponent<TeamProfileInit>().setTeamProfile(profile, team.t_orig_name);
                }
            }
        };
    }
    private teamHistory hist;
    IEnumerator GetHistory(string other)
    {


        using (UnityWebRequest www = UnityWebRequest.Get("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/" + other+"/history"))
        {
            www.SetRequestHeader("Authorization", "Bearer " + j.token);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.url);
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                hist = new teamHistory();
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = System.Text.Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                if (jsonString == "{}")
                {  
                    int childKillCount = TeamEventHistoryContainer.transform.childCount;
                    for (int i = childKillCount - 1; i >= 0; i--)
                    {
                        Destroy(TeamEventHistoryContainer.transform.GetChild(i).gameObject);
                    }
                }
                else
                {
                    hist = JsonUtility.FromJson<teamHistory>(jsonString);
                    HistoryConverter();
                }
            }
        };
    }
    public bool set = false;
    public void HistoryConverter(){
        Opportunities.text = hist.event_ids.Length.ToString();
        for (int i = 0; i < hist.event_ids.Length;i++){
            if (hist.event_names != null && hist.event_names[i] != null)
            {
                searchEvents(hist.event_names[i]);
                set = true;
            }

        }

    }


    private string searchURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/search";
   
    public void searchEvents(string s)
    {

        UnityWebRequest www2 = UnityWebRequest.Get(searchURL + "?search_term=" + s);
        www2.SetRequestHeader("Authorization", "Bearer " + j.token);

        StartCoroutine(eventPut(www2));
    }
    private EventSearch eSearch;
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
            eSearch = JsonUtility.FromJson<EventSearch>(jsonString);
            if (jsonString != "{}")
            {
                eventPopulator();
            }
        }
    }

    public void eventPopulator()
    {
        GameObject newEvent;
        int childKillCount = TeamEventHistoryContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(TeamEventHistoryContainer.transform.GetChild(i).gameObject);
        }

            newEvent = Instantiate(TeamEventPrefab, TeamEventHistoryContainer.transform);
       
            newEvent.GetComponent<TeamEventInit>().getEvent(eSearch.event_ids[0]);


        if (childKillCount > 7)
        {
            TeamEventHistoryContainer.AddComponent<ContentSizeFitter>();
            TeamEventHistoryContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

}
[System.Serializable]
public class teamHistory{
    public string[] event_ids;
    public string[] event_names;
}