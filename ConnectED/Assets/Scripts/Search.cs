using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
//this is the search script 
public class Search : MonoBehaviour {
    public InputField search;
    public Jsonparser j;

    public int mode = 0;

    //set mode, 0 is event, 1 is profile search, 2 is team search
    //when you set the mode you want to clear out the last search
    public void setMode(int i)
    {
        search.text = "";
        int childKillCount = eventSearchContainer.transform.childCount;
        for (int y = childKillCount - 1; y >= 0; y--)
        {
            Destroy(eventSearchContainer.transform.GetChild(y).gameObject);
        }
        mode = i;
    }

    private Searcher s;
    //this formats what you typed to be a good search
    public void startSearch()
    {
        if (search.text == "" || search.text == null || search.text == " ")
            return;
        search.text = search.text.Replace("  ", " ");
        search.text = search.text.Replace("   ", " ");
        search.text = search.text.Replace(" ", "+");
        if(mode == 0)
        {
            searchEvents(search.text , j.token );
		}
        if(mode == 1)
            searchProfiles(search.text, j.token);
        if (mode == 2)
            searchTeams(search.text, j.token);


    }

    private string searchURL ="https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/search";
    private IEnumerator coroutine;
    //this searches based on what you typed, we pass the text s and the token t
    public void searchEvents(string s, string t)
    {

        UnityWebRequest www2 = UnityWebRequest.Get(searchURL+"?search_term="+ search.text);
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = eventPut(www2);
        StartCoroutine(coroutine);
    }
    private string jsonString;
    public GameObject eventSearchPrefab;
    public GameObject eventSearchContainer;
    private EventSearch eSearch;
    //this gets a list of events 
    private IEnumerator eventPut(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        Debug.Log(www.downloadHandler.data);
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if(www.responseCode == 200){
            byte[] results = www.downloadHandler.data;
            jsonString = "";
            jsonString = Encoding.UTF8.GetString(results);
            Debug.Log(jsonString);
            eSearch = JsonUtility.FromJson<EventSearch>(jsonString);
            if (jsonString != "{}")
            {
                //if the list is not empty
                eventPopulator();
            }
        }
    }
    public void eventPopulator()
    {
        GameObject newEvent;
        int childKillCount = eventSearchContainer.transform.childCount;
        //this clears out the old events 
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(eventSearchContainer.transform.GetChild(i).gameObject);
        }
        //this adds in the new events
        for (int i = 0; i < eSearch.event_ids.Length; i++)
        {
            newEvent = Instantiate(eventSearchPrefab, eventSearchContainer.transform);
            newEvent.GetComponent<searchEventPrefab>().setSearchEvent(eSearch, i);
        }
        //this formats the size of the container so you can scroll properly
        if (childKillCount > 7)
        {
            eventSearchContainer.AddComponent<ContentSizeFitter>();
            eventSearchContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private string searchProfileURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/search";
    //this is called if you are on profile mode
    public void searchProfiles(string s, string t)
    {

        UnityWebRequest www2 = UnityWebRequest.Get(searchProfileURL + "?search_term=" + search.text);
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = profilePut(www2);
        StartCoroutine(coroutine);
    }
    public GameObject profileSearchPrefab;
    private ProfileSearch pSearch;
    private IEnumerator profilePut(UnityWebRequest www)
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
            pSearch = JsonUtility.FromJson<ProfileSearch>(jsonString);
            if (jsonString != "{}")
            {
                //this is the same as the other search
                profilePopulator();
            }
        }
    }
	
    public void profilePopulator()
    {
        GameObject newProfile;
        ProfileSearch a = new ProfileSearch();
        int childKillCount = eventSearchContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(eventSearchContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < pSearch.email.Length; i++)
        {
            newProfile = Instantiate(profileSearchPrefab, eventSearchContainer.transform);

            newProfile.GetComponent<profileSearchInitializer>().setSearch(this);
            newProfile.GetComponent<profileSearchInitializer>().setPic(pSearch.pic[i]);
            newProfile.GetComponent<profileSearchInitializer>().setNameEmail(pSearch.name[i],pSearch.email[i]);
        }

        if (childKillCount > 7)
        {
            eventSearchContainer.AddComponent<ContentSizeFitter>();
            eventSearchContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
	}

    private string searchTeamURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/teams/search";
   
    public void searchTeams(string s, string t)
    {

        UnityWebRequest www2 = UnityWebRequest.Get(searchTeamURL + "?search_term=" + search.text);
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = TeamPut(www2);
        StartCoroutine(coroutine);
    }
    public GameObject teamSearchPrefab;
    private TeamSearch tSearch;
    private IEnumerator TeamPut(UnityWebRequest www)
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
            tSearch = JsonUtility.FromJson<TeamSearch>(jsonString);
            if (jsonString != "{}")
            {
                teamPopulator();
            }
        }
    }
    public GameObject teamPage;
    public void teamPopulator()
    {
        GameObject newEvent;
        EventSearch a = new EventSearch();
        int childKillCount = eventSearchContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(eventSearchContainer.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < tSearch.name.Length; i++)
        {
            newEvent = Instantiate(teamSearchPrefab, eventSearchContainer.transform);
            newEvent.GetComponent<searchTeamInitializer>().setSearchTeam(tSearch, i, teamPage,this);
        }

        if (childKillCount > 7)
        {
            eventSearchContainer.AddComponent<ContentSizeFitter>();
            eventSearchContainer.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }





    public InputField leader;
    //this is used to set leaders for events
    public void setLeader(string s)
    {
        leader.text = s;

        GetComponent<Animator>().ResetTrigger("Searching");
        GetComponent<Animator>().SetTrigger("SearchingCancel");
    }
    }


[System.Serializable]
public class Searcher
{
    public string search_term;
}
public class EventSearch
{
    public float[] distances;
    public string[] event_dates;
    public string[] event_ids;
    public string[] event_pics;
}
public class ProfileSearch
{
    public string[] email;
    public string[] name;
    public string[] pic;
}
public class TeamSearch
{
    public float[] distance;
    public string[] name;
    public string[] pic;
    public string[] t_id;
}