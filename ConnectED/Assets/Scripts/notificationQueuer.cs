using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;

public class notificationQueuer : MonoBehaviour {
    //this script spawns in notifications for events and teams, such as approving a private member
    public string pendingName;
    public string eventName;
    public GameObject notificationPanel;
    public Text notificationText;
    public bool SettingDBa = false;
    public bool SettingDBd = false;
    public RawImage notificationImage;
    public int i = 0;
    public int x = 0;
    public List<Event> events;
    public List<Team> teams;
    public GameObject loading;
    private Profile profile;
    public Jsonparser j;
    private bool gettingProf = false;
	public approveList approve;
    public approveList deny;
    //these are  called when setting event and team tiles if you own the event or team
    public void queueNotifications(Event e){
        events.Add(e);
    }
    public void queueTeamNotifications(Team t)
    {
        teams.Add(t);
    }
    bool eventNoti = false;
    public void setNotification()
    {

        teamMode = false;
        if (gettingProf)
            return;
        if (i < events.Count)
        {
            if (events[i].privacy == "p")
            {
                if (events[i].num_pending_attendees != 0)
                {
                    if (x < events[i].num_pending_attendees)
                    {
                        //this goes through all of the pending attendees recursively
                        notificationPanel.SetActive(true);
                        GetmyProfile(i, x);

                    }
                }
                if (events[i].num_pending_attendees == 0)
                {
                    //this is called when you have no more pending attendees
                    i++;
                    setNotification();
                }
            }
            else
            {
                //if there is no privacy in an event
                Debug.Log("open event");
                i++;
                setNotification();
            }
        }
        else
        {
            if (!SettingDBa && !SettingDBd && notiUp == false)
            {
                //this is if there are no more notifications for events at all
                Debug.Log("no more notifications");
                deny = new approveList();
                approve = new approveList();
				x = 0;
				i = 0;
                eventNoti = true;
                //then it will send notifications for teams
                setTeamNotification();
            }
        }
    }

    public void GetmyProfile(int i1, int x1)
    {
        //let the user know we are loading in the persons information
        loading.SetActive(true);
        gettingProf = true;
        //this adds the persons information to the privacy request
        StartCoroutine(GetProfile(i1,x1));
    }

    private string jsonString;

    IEnumerator GetProfile(int i1, int x1)
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;


        Debug.Log("Getting profile with " + events[i1].pending_attendees[x1]);
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/" + events[i1].pending_attendees[x1].ToLower()))
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
                //on success set the information on the notification
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                profile = JsonUtility.FromJson<Profile>(jsonString);
                loading.SetActive(false);
                initializeNotification(i1);
                notificationPanel.SetActive(true);
            }
        };
    }
    //set the notification
    public void initializeNotification(int i1)
    {
        notificationText.text = "Would you like to allow " + profile.first_name + " " + profile.last_name + " to come to your private event " + events[i1].e_title + "?";
        Texture2D tex = new Texture2D(200, 200);
        byte[] img = System.Convert.FromBase64String(profile.photo);
        Debug.Log(img);
        tex.LoadImage(img, false);
        notificationImage.texture = tex;
        gettingProf = false;
    }
    public bool teamMode;
    public int pendingIndex = 0;
    //this is called if you accept someone
    public void accept()
    {
        if (teamMode)
        {
            //if you accept someone onto your team
            acceptTeam();
            return;
        }
        else
        {
            //if you accept someone into an event it changes around the lists associated with the event and approves the people you approved and denys the people you deny
            if (approve.approve_list.Length != events[i].pending_attendees.Length)
                approve.approve_list = new string[events[i].pending_attendees.Length];
            approve.approve_list[x] = events[i].pending_attendees[x];
            Debug.Log(events[i].pending_attendees[x]);
            if (x < events[i].pending_attendees.Length - 1)
            {
                Debug.Log("Adding " + profile.email);
                x++;
                setNotification();
            }
            else
            {
                //this decided to initiate a deny or approve action
                if (approve.approve_list.Length != 0 || approve.approve_list == null)
                    StartCoroutine(acceptProfile());
                if (deny.approve_list.Length != 0 || deny.approve_list == null)
                    StartCoroutine(denyProfile());
            }

        }
    }

    public void denial()
    {
        if (teamMode)
        {
            denyTeam();
            return;
        }
        else
        {
            if (deny.approve_list.Length != events[i].pending_attendees.Length)
                deny.approve_list = new string[events[i].pending_attendees.Length];
            deny.approve_list[x] = events[i].pending_attendees[x];
            Debug.Log(events[i].pending_attendees[x]);
            if (x < events[i].pending_attendees.Length - 1)
            {
                Debug.Log("Denying " + profile.email);
                x++;
                setNotification();
            }
            else
            {
                if (approve.approve_list.Length != 0 || approve.approve_list == null)
                    StartCoroutine(acceptProfile());
                if (deny.approve_list.Length != 0 || deny.approve_list == null)
                    StartCoroutine(denyProfile());
            }
        }
    }

    IEnumerator acceptProfile()
    {
        loading.SetActive(true);
        SettingDBa = true;
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        string newApprove = JsonUtility.ToJson(approve);
        Debug.Log(approve.approve_list[0]);
        Debug.Log(newApprove);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(newApprove);
        Debug.Log("Accepting profile with for "+events[i].e_title );
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Put("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/" + PlayerPrefs.GetString("email", "email").ToLower() +"/"+ events[i].e_orig_title +"/approve", bodyRaw))
        {



            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
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
            //get ready to set approvals and denys for the next event
            i++;
            x = 0;
            SettingDBa = false;
            setNotification();
            loading.SetActive(false);
        };
    }

    IEnumerator denyProfile()
    {

        loading.SetActive(true);
        SettingDBd = true;
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        string newApprove = JsonUtility.ToJson(deny);
        Debug.Log(deny.approve_list[0]);
        Debug.Log(newApprove);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(newApprove);
        Debug.Log("Denying profile with for " + events[i].e_title);
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Put("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/" + PlayerPrefs.GetString("email", "email").ToLower() + "/" + events[i].e_orig_title + "/deny", bodyRaw))
        {



            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
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
            i++;
            x = 0;
            SettingDBd = false;
            setNotification();
            loading.SetActive(false);
        };
    }
    //this is where team notifications are handled, it is almost the exact same as the event notifications
    public void setTeamNotification()
    {
        teamMode = true;

        if (gettingProf)
            return;
        if (i < teams.Count)
        {
            if (teams[i].t_privacy == "p")
            {
                if (teams[i].t_pending_members.Length != 0)
                {
                    if (x < teams[i].t_pending_members.Length)
                    {
                        notificationPanel.SetActive(true);
                        GetmyTeam(i, x);

                    }
                }
                if (teams[i].t_pending_members.Length == 0)
                {
                    i++;
                    setTeamNotification();
                }
            }
            else
            {
                Debug.Log("open team");
                i++;
                setTeamNotification();
            }
        }
        else
        {
            if (!SettingDBa && !SettingDBd && notiUp == false)
            {
                teamNoti = true;
                Debug.Log("no more notifications");
                teamMode = false;
                deny = new approveList();
                approve = new approveList();
                this.gameObject.SetActive(false);
            }
        }
    }
    //i is the current team and x is the current person on that team for getting their profile
    public void GetmyTeam(int i1, int x1)
    {
        teamMode = true;
        loading.SetActive(true);
        gettingProf = true;
        StartCoroutine(GetTeam(i1, x1));
    }

    //this gets the profile for the person you are going to approve or deny for teams
    IEnumerator GetTeam(int i1, int x1)
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;


        Debug.Log("Getting team with " + teams[i1].t_pending_members[x1]);
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/" + teams[i1].t_pending_members[x1].ToLower()))
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
                //on success display notification and wait for user to deny or approve
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                profile = JsonUtility.FromJson<Profile>(jsonString);
                loading.SetActive(false);
                //initialize the team notification
                initializeTeamNotification(i1);
                notificationPanel.SetActive(true);
            }
        };
    }
    public bool notiUp;
    public void initializeTeamNotification(int i1)
    {
        teamMode = true;
        notiUp = true;
        notificationText.text = "Would you like to allow " + profile.first_name + " " + profile.last_name + " to join your private team " + teams[i1].t_name + "?";
        if (profile.photo == null) { }
        else
        {
            Texture2D tex = new Texture2D(200, 200);
            byte[] img = System.Convert.FromBase64String(profile.photo);
            Debug.Log(img);
            tex.LoadImage(img, false);
            notificationImage.texture = tex;
        }
        gettingProf = false;
    }

    public void acceptTeam()
    {
        Debug.Log(teams[i].t_pending_members.Length);
        approve.approve_list = new string[teams[i].t_pending_members.Length];
        approve.approve_list[x] = teams[i].t_pending_members[x];
        Debug.Log(teams[i].t_pending_members[x]);
        //if there are more team members to approve keep recursively doing that
        if (x < teams[i].t_pending_members.Length - 1)
        {
            Debug.Log("Adding " + profile.email);
            x++;
            setTeamNotification();
        }
        //or start the accept or deny process for the private people on this event
        else
        {
            if (approve.approve_list != null && approve.approve_list.Length != 0 )
                StartCoroutine(acceptingTeam());
            if ( deny.approve_list != null && deny.approve_list.Length != 0 )
                StartCoroutine(denyTeam());
        }

    }

    public void denialTeam()
    {
        
        deny.approve_list = new string[teams[i].t_pending_members.Length];
        deny.approve_list[x] = teams[i].t_pending_members[x];
        Debug.Log(teams[i].t_pending_members[x]);
        if (x < teams[i].t_pending_members.Length - 1)
        {
            Debug.Log("Denying " + profile.email);
            x++;
            setTeamNotification();
        }
        else
        {
            if (approve.approve_list.Length != 0 || approve.approve_list == null)
                StartCoroutine(acceptingTeam());
            if (deny.approve_list.Length != 0 || deny.approve_list == null)
                StartCoroutine(denyTeam());
        }
    }

    IEnumerator acceptingTeam()
    {
        loading.SetActive(true);
        SettingDBa = true;
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        string newApprove = JsonUtility.ToJson(approve);
        Debug.Log(approve.approve_list[0]);
        Debug.Log(newApprove);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(newApprove);
        Debug.Log("Accepting profile with for " + teams[i].t_name);
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Put("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/" + teams[i].t_orig_name +"/approve",bodyRaw ))
        {



            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
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
                Debug.Log(www.downloadHandler.data);
            }
            Debug.Log(www.responseCode);
            //now that we have approved or denied the people for this team, move onto the next one on the list if there is one
            i++;
            x = 0;
            SettingDBa = false;
			loading.SetActive(false);
            notificationPanel.SetActive(false);
            notiUp = false;
            teamNoti = true;
            setTeamNotification();
        };
    }
    public bool teamNoti = false;
    IEnumerator denyTeam()
    {

        loading.SetActive(true);
        SettingDBd = true;
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        string newApprove = JsonUtility.ToJson(deny);
        Debug.Log(deny.approve_list[0]);
        Debug.Log(newApprove);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(newApprove);
        Debug.Log("Denying profile with for " + events[i].e_title);
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Put("https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/" + teams[i].t_orig_name +"/deny", bodyRaw))
        {



            www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
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
            i++;
            x = 0;
            SettingDBd = false;
            setTeamNotification();
            loading.SetActive(false);
            notiUp = false;
        };
    }




}
[System.Serializable]
public class approveList 
{
    public string[] approve_list;
}
