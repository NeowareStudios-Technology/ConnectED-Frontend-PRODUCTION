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

public class DetailChanger : MonoBehaviour
{
    //this script changes what is on the details page
    public Text Date;
    public Text Title;
    public RawImage pic;
    public Text Description;
    public Text Capacity;
    public Text Time;
    public Text Location;
    public Text Status;
    public GameObject Tag1;
    public GameObject Tag2;
    public GameObject Tag3;
    private Event a;
    public GameObject loader;
    public Jsonparser j;
    public DetailsUpdatesPageSetter detailsUpdatesPageSetter;
    public Button Volunteer;
    public GameObject JoinWithTeam;
    private string registerURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";

    public string privacy()
    {
        if (a.privacy == "o")
            return "Open";
        else
            return "Private";
    }

    public void registerWithTeam()
    {

        Register(a.e_organizer + "/" + a.e_title, j.token);
    }

    public DetailsTeamInstantiator teamInstantiator;
    public void processTeams(Event e)
    {
        if (e.teams == null) { }
        else
        {
            string[] teams = e.teams;
            for (int i = 0; i < e.teams.Length; i++)
            {
                Debug.Log(e.teams[i]);
                searchTeams(e.teams[i], j.token);
            }
        }
    }
    public int eventCapacity;
    //this function gets an event and fills out the details page based on that event
    public void setDetails(Event e)
    {
        //these set all of the information on the page
        eventCapacity = e.capacity;
        detailsUpdatesPageSetter.setUpdate(e);
        detailsUpdatesPageSetter.getEventUpdates(e.e_organizer, e.e_orig_title);
        JoinWithTeam.SetActive(false);
        a = new Event();
        a = e;
        processTeams(e);
        Date.text = GetMonth(e.date[0]) + " " + GetDay(e.date[0]);
        Location.text = (e.street + " " + e.city + " " + e.state + " " + e.zip_code);
        Title.text = e.e_title;
        Time.text = e.start[0] + " -" + e.end[0];

        //this sets all of the buttons on the page
        joinWithTeamButton.onClick.RemoveAllListeners();
        joinWithoutButton.onClick.RemoveAllListeners();
        joinWithTeamButton.onClick.AddListener(() => RegisterWithTeam(e.e_organizer + "/" + e.e_title, j.token));
        joinWithoutButton.onClick.AddListener(() => Register(e.e_organizer + "/" + e.e_title, j.token));
        Description.text = e.e_desc;
        Capacity.text = e.num_attendees + " / " + e.capacity;
        //this sets the required skills of the details page
        Status.text = privacy() + " / Required Skills: ";
        if (e.req_skills == null)
        {

        }
        else
        {
            if (e.req_skills.Length == 1)
                Status.text += e.req_skills[0];
            if (e.req_skills.Length == 2)
                Status.text += e.req_skills[0] + ", " + e.req_skills[1];
            if (e.req_skills.Length >= 3)
                Status.text += e.req_skills[0] + ", " + e.req_skills[1] + ", " + e.req_skills[2];
        }
        //this sets the volunteer button correctly
        Volunteer.onClick.RemoveAllListeners();
        if (e.is_registered == 0)
        {
            //Volunteer.onClick.AddListener(() => Register(e.e_organizer + "/" + e.e_title, j.token));
            Volunteer.onClick.AddListener(() => JoinWithTeam.SetActive(true));
            Volunteer.onClick.AddListener(() => joinWithAction.SetActive(true));
            Volunteer.transform.GetChild(0).GetComponent<Text>().text = "VOLUNTEER";
        }
        if (e.is_registered == 1)
        {
            Volunteer.onClick.AddListener(() => Deregister(e.e_organizer + "/" + e.e_title, j.token));
            Volunteer.transform.GetChild(0).GetComponent<Text>().text = "DEREGISTER";
            // is already registered
        }
        if (e.is_registered == -1)
        {
            Volunteer.transform.GetChild(0).GetComponent<Text>().text = "PENDING...";
            // is private event
        }
        //this sets the interests of the page
        if (e.interests == null)
        {
            Tag1.SetActive(false);
            Tag2.SetActive(false);
            Tag3.SetActive(false);
        }
        else
        {
            if (e.interests.Length == 1)
            {
                Tag1.SetActive(true);
                Tag1.GetComponent<Text>().text = e.interests[0];
                Tag2.SetActive(false);
                Tag3.SetActive(false);
            }
            if (e.interests.Length == 2)
            {
                Tag1.SetActive(true);
                Tag1.GetComponent<Text>().text = e.interests[0];
                Tag2.SetActive(true);
                Tag2.GetComponent<Text>().text = e.interests[1];
                Tag3.SetActive(false);
            }
            if (e.interests.Length >= 3)
            {
                Tag1.SetActive(true);
                Tag1.GetComponent<Text>().text = e.interests[0];
                Tag2.SetActive(true);
                Tag2.GetComponent<Text>().text = e.interests[1];
                Tag3.SetActive(true);
                Tag3.GetComponent<Text>().text = e.interests[2];
            }
        }
        Texture2D tex = new Texture2D(400, 400);
        if (e.e_photo != null && e.e_photo.Length > 300)
        {
            byte[] img = System.Convert.FromBase64String(e.e_photo);
            tex.LoadImage(img, false);

            pic.texture = tex;
        }
    }





    public string GetMonth(string s)
    {
        string m = s.Substring(0, 2);
        switch (m)
        {
            case "01":
                return "January";
            case "02":
                return "February";
            case "03":
                return "March";
            case "04":
                return "April";
            case "05":
                return "May";
            case "06":
                return "June";
            case "07":
                return "July";
            case "08":
                return "August";
            case "09":
                return "September";
            case "10":
                return "October";
            case "11":
                return "November";
            case "12":
                return "December";
            default:
                return "???";
        }
    }
    public string GetDay(string s)
    {
        string m = s.Substring(3, 2);
        if (m[0] == '0')
            return m[1].ToString();
        else
            return m;
    }

    private IEnumerator coroutine;
    public SignUpWithTeam team;
    public Button joinWithTeamButton;
    public Button joinWithoutButton;
    public GameObject joinWithTeamContainer;
    public GameObject joinWithAction;
    public ScrollSnapRect scroll;
    public String action = "both";

    public void setTeamAction(int i){
        if(i == 0){
            action = "Walking";

        }
        if(i == 1){
            action = "Fundraising";
        }
        if(i == 2){
            action = "Both";
        }
    }
    //this starts the regestering process and takes in a string s and a team t
    public void RegisterWithTeam(string s, string t)
    {
        loader.SetActive(true);
        team.team = joinWithTeamContainer.transform.GetChild(scroll.getCurrentPage()).GetComponent<JoinWithTeamSetter>().teamName.text;
        team.user_action = action;
        string newEvent = JsonUtility.ToJson(team);
        Debug.Log(newEvent);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newEvent);
        UnityWebRequest www2 = UnityWebRequest.Put(registerURL + s + "/" + "registration", newEvent);

        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = Put(www2);
        StartCoroutine(coroutine);
    }
    public void Register(string s, string t)
    {
        loader.SetActive(true);
        string newEvent = JsonUtility.ToJson(team);
        Debug.Log(newEvent);
        byte[] bodyRaw2 = Encoding.UTF8.GetBytes(newEvent);
        UnityWebRequest www2 = UnityWebRequest.Put(registerURL + s + "/" + "registration", newEvent);

        www2.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw2);
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = Put(www2);
        StartCoroutine(coroutine);
    }
    //this is where we make the call and handle the results changing the page as necessary
    private IEnumerator Put(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.responseCode.ToString() == "200")
        {
            if (a.privacy == "o")
            {
                Volunteer.onClick.RemoveAllListeners();
                Volunteer.onClick.AddListener(() => Deregister(a.e_organizer + "/" + a.e_title, j.token));
                Volunteer.transform.GetChild(0).GetComponent<Text>().text = "DEREGISTER";
                JoinWithTeam.SetActive(false);
                j.changeExploreRegisterStatus(1);
                loader.SetActive(false);
            }
            else
            {
                Volunteer.onClick.RemoveAllListeners();
                Volunteer.transform.GetChild(0).GetComponent<Text>().text = "PENDING...";
                JoinWithTeam.SetActive(false);
                j.changeExploreRegisterStatus(-1);
                loader.SetActive(false);
            }
        }

    }
    //these are the deregistering process
    public void Deregister(string s, string t)
    {
        loader.SetActive(true);
        UnityWebRequest www2 = UnityWebRequest.Delete(registerURL + s + "/" + "registration");

        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = Delete(www2);
        StartCoroutine(coroutine);
    }

    private IEnumerator Delete(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.responseCode.ToString() == "200")
        {
            Volunteer.onClick.RemoveAllListeners();
            Volunteer.onClick.AddListener(() => JoinWithTeam.SetActive(true));
            Volunteer.onClick.AddListener(() => joinWithAction.SetActive(true));
            Volunteer.transform.GetChild(0).GetComponent<Text>().text = "VOLUNTEER";
            j.changeExploreRegisterStatus(0);
            loader.SetActive(false);
        }

    }



    public MyDictionary[] eventTeamCounter;
    private string jsonString;
    private string searchTeamURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/search";
    private string token;


    //this searches teams associated with an event
    public void searchTeams(string s, string t)
    {
        token = t;
        loader.SetActive(true);
        eventTeamCounter = new MyDictionary[s.Length];

            for (int q = 0; q < eventTeamCounter.Length; q++)
            {
                Debug.Log(s);

                if (eventTeamCounter[q] == null)
                {
                    eventTeamCounter[q] = new MyDictionary();
                    eventTeamCounter[q].s = s;
                    eventTeamCounter[q].i = 1;
                    Debug.Log(s +" added");
                    q = eventTeamCounter.Length - 1;

                }
                else
                {
                    if (eventTeamCounter[q].s == s)
                    {
                        eventTeamCounter[q].i++;
                    }
                }
            }

		Debug.Log(eventTeamCounter);
        for (int i = 0; i < eventTeamCounter.Length; i++)
        {
            //if (eventTeamCounter[i] != null && eventTeamCounter[i].s != "")
            //{
            //    UnityWebRequest www2 = UnityWebRequest.Get(searchTeamURL + "?search_term=" + eventTeamCounter[i].s.Replace(" ", "+"));
            //    www2.SetRequestHeader("Authorization", "Bearer " + t);
            //    coroutine = TeamPut(www2);
            //    StartCoroutine(coroutine);
            //}
            if (eventTeamCounter[i] != null && eventTeamCounter[i].s != "")
            {
                UnityWebRequest www2 = UnityWebRequest.Get(teamURL +  eventTeamCounter[i].s.Replace(" ", "+"));
                www2.SetRequestHeader("Authorization", "Bearer " + t);
                coroutine = TeamPut(www2);
                StartCoroutine(coroutine);
            }
        }
    }


    public GameObject teamSearchPrefab;
    public GameObject teamSearchContainer;
    private TeamSearch tSearch;
    private int retry = 0;


    private void teamPutRetry(UnityWebRequest www){
        www.SetRequestHeader("Authorization", "Bearer " + token);
        StartCoroutine(TeamPut(www));
    }


    private IEnumerator TeamPut(UnityWebRequest www)
    {
        allTeams = new Team();
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.responseCode == 503 && retry < 3)
        {
            teamPutRetry(www);

        }
        else
        {
            if (www.responseCode == 200)
            {
                retry = 0;
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                Team newTeam = JsonUtility.FromJson<Team>(jsonString);
                if (jsonString != "{}")
                {
                    int numberOfV = 1;
                    for (int y = 0; y < eventTeamCounter.Length; y++)
                    {
                        if (eventTeamCounter[y] == null) { }
                        else
                        {
                            if (allTeams.t_name == eventTeamCounter[y].s)
                                numberOfV = eventTeamCounter[y].i;
                        }
                    }
                    GameObject newsearchEventPrefab = Instantiate(teamSearchPrefab, teamSearchContainer.transform);
                    newsearchEventPrefab.GetComponent<DetailsTeamInstantiator>().finishSet(newTeam, teamPage, numberOfV, eventCapacity);
                }
            }
        }
    }

    public GameObject teamPage;

    //this populates the team page for the event on the details page
    public void teamPopulator()
    {
        GameObject newEvent;
        EventSearch a = new EventSearch();
        int childKillCount = teamSearchContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(teamSearchContainer.transform.GetChild(i).gameObject);
        }

            //newEvent = Instantiate(teamSearchPrefab, teamSearchContainer.transform);
            setTeamButton(tSearch.t_id[0],teamPage);

        loader.SetActive(false);


    }

    private string teamURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/";

    public void setTeamButton(string t_id, GameObject _teamPage)
    {
        j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
        StartCoroutine(Populator(t_id));
        teamPage = _teamPage;
    }
    private Team allTeams;


    public void populatorRetry(string i){
        StartCoroutine(Populator(i));
    }


    IEnumerator Populator(string t_id)
    {
        allTeams = new Team();
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        Debug.Log(t_id);
        using (UnityWebRequest www = UnityWebRequest.Get(teamURL + t_id))
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
                if (www.responseCode.ToString() == "500")
                {
                    populatorRetry(t_id);
                }
            }
            else
            {
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                allTeams = JsonUtility.FromJson<Team>(jsonString);
                int numberOfV = 1;
                for (int y = 0; y < eventTeamCounter.Length;y++){
                    if (allTeams.t_name == eventTeamCounter[y].s)
                        numberOfV = eventTeamCounter[y].i;
                }
                GameObject newsearchEventPrefab = Instantiate(teamSearchPrefab, teamSearchContainer.transform);
                newsearchEventPrefab.GetComponent<DetailsTeamInstantiator>().finishSet(allTeams, teamPage,numberOfV,eventCapacity);

            }
        }

    }

    public void clearTeams(){
        for (int i = teamSearchContainer.transform.childCount-1; i >= 0;i--)
            Destroy(teamSearchContainer.transform.GetChild(i).gameObject);
    }

}

[System.Serializable]
public class SignUpWithTeam{
    public string team;
    public string user_action;
}
public class MyDictionary{
    public string s;
    public int i;
}