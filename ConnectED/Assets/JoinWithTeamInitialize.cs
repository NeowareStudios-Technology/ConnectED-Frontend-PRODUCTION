using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;

public class JoinWithTeamInitialize : MonoBehaviour
{
    //this loads in all the teams you are in and puts them in the join with team popup so that you can select the team you want to join with
    public string db = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/";
    public profileTeams profileTeams;

    public Jsonparser j;
    public void getTeams()
    {
        j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
        Debug.Log("GetTeams");
        StartCoroutine(teamLister());
    }
    private string jsonString;
    private int retry = 0;

    private Team[] allTeams;
    private string teamURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/";

    IEnumerator teamLister()
    {
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;


        Debug.Log("Prefilling Teams");
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get(db +PlayerPrefs.GetString("email").ToLower()+"/teams"))
        {



            www.SetRequestHeader("Authorization", "Bearer " + j.token);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.responseCode);
                if (www.responseCode == 500)
                {
                }
                Debug.Log(www.url);
                Debug.Log(www.GetRequestHeader("Authorization"));
                Debug.Log(www.GetRequestHeader("Content-Type"));
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
                if (www.responseCode.ToString() == "503" & retry < 3)
                {
                    Debug.Log("Trying again : get prefill");
                    getTeams();
                }
            }
            else
            {
                //success
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                profileTeams = JsonUtility.FromJson<profileTeams>(jsonString);
                //if we have teams that we are registered too
                if(jsonString != "{}" && profileTeams.registered_team_ids != null)
                    StartCoroutine(Populator());
            }
        }
    }
    //populate with teams we are registered for
    IEnumerator Populator()
    {
        allTeams = new Team[profileTeams.registered_team_ids.Length];
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        for (int i = 0; i < profileTeams.registered_team_ids.Length; i++)
        {
            //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
            Debug.Log(profileTeams.registered_team_names[i]);
            using (UnityWebRequest www = UnityWebRequest.Get(teamURL + profileTeams.registered_team_ids[i]))
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
                    if (www.responseCode.ToString() == "500" || www.responseCode.ToString() == "503")
                    {
                        yield return www.SendWebRequest();
                        if (www.isNetworkError || www.isHttpError)
                        {
                            Debug.Log(www.responseCode);
                            Debug.Log(www.url);
                            Debug.Log(www.GetRequestHeader("Authorization"));
                            Debug.Log(www.GetRequestHeader("Content-Type"));
                            Debug.Log(www.error);
                            Debug.Log(www.downloadHandler.text);
                            if (www.responseCode.ToString() == "500" ||www.responseCode.ToString() == "503")
                            {
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
                                    allTeams[i] = JsonUtility.FromJson<Team>(jsonString);
                                    instantiateTeams(allTeams);
                                }
                            }
                            }
                        else
                            {
                                Debug.Log(www.responseCode);
                                byte[] results = www.downloadHandler.data;
                                jsonString = "";
                                jsonString = Encoding.UTF8.GetString(results);
                                Debug.Log(jsonString);
                                allTeams[i] = JsonUtility.FromJson<Team>(jsonString);
                                instantiateTeams(allTeams);

                        }
                    }
                }
                else
                {
                    Debug.Log(www.responseCode);
                    byte[] results = www.downloadHandler.data;
                    jsonString = "";
                    jsonString = Encoding.UTF8.GetString(results);
                    Debug.Log(jsonString);
                    allTeams[i] = JsonUtility.FromJson<Team>(jsonString);
                }
            }
        }
        instantiateTeams(allTeams);
    }
    public GameObject JoinWithTeamPrefab;
    private GameObject newJoinWithTeam;
    public GameObject JoinWithTeamContainer;
    public GameObject JoinWithTeamDotPrefab;
    public GameObject JoinWithTeamDotContainer;
    public ScrollSnapRect scroll;
    //this creates all the teams in the popup
    public void instantiateTeams(Team[] teams){
        for (int i = 0; i < teams.Length; i++)
        {
            newJoinWithTeam = Instantiate(JoinWithTeamPrefab, JoinWithTeamContainer.transform);
            newJoinWithTeam.GetComponent<JoinWithTeamSetter>().setTeam(teams[i]);
            Instantiate(JoinWithTeamDotPrefab, JoinWithTeamDotContainer.transform);
        }
        scroll.enabled = true;
        scroll.Refresh();
    }



}

public class profileTeams
{
    public string[] created_team_ids;
    public string[] created_team_names;
    public string[] leader_team_ids;
    public string[] leader_team_names;
    public string[] pending_team_ids;
    public string[] pending_team_names;
    public string[] registered_team_ids;
    public string[] registered_team_names;

}