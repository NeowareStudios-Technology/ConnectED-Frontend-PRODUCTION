using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class DetailsUpdatesPageSetter : MonoBehaviour {

    //this sets the last page on the details page
    public RawImage organizerImage;
    public Text organizerName;
    public RawImage leaderOnePic;
    public Text leaderOneName;
    public RawImage leaderTwoPic;
    public Text leaderTwoName;
    public RawImage leaderThreePic;
    public Text leaderThreeName;
    public Jsonparser j;
    private Event Event;
    private IEnumerator coroutine;
    private int i = 0;
	private void Start()
	{
		j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
    }
    //this sets this page recursively!!! i controls which part you are at and calls for the organizer, then the leaders
    public void setUpdate(Event e)
    {
        Event = e;
        if(i == 0){
            StartCoroutine(GetProfile(e.e_organizer));
        }
        if (e.leaders == null){
            leaderOneName.transform.parent.gameObject.SetActive(false);
            leaderTwoName.transform.parent.gameObject.SetActive(false);
            leaderThreeName.transform.parent.gameObject.SetActive(false);
			return;
        }
        if(i == 1 && e.leaders[0] != null|| i == 1 && e.leaders[0] != ""){
            leaderOneName.transform.parent.gameObject.SetActive(true);
            startSearch(e.leaders[0]);
            return;
        }
        if (i == 2 && e.leaders[1] != null || i == 2 && e.leaders[1] != "")
        {
            if (e.leaders[1].Length < 3){
                leaderTwoName.transform.parent.gameObject.SetActive(false);
                i = 0;
                return;
			}
            leaderTwoName.transform.parent.gameObject.SetActive(true);
            startSearch(e.leaders[1]);
            return;
        }
        if (i == 3 && e.leaders[2] != null || i == 3 && e.leaders[2] != "")
        {
            if (e.leaders[2].Length < 3)
            {
                leaderThreeName.transform.parent.gameObject.SetActive(false);
                i = 0;
                return;
            }
            leaderThreeName.transform.parent.gameObject.SetActive(true);
            Debug.Log("starting search for 3 as: " + e.leaders[2]);
            startSearch(e.leaders[2]);
            return;
        }
        else{
            Debug.Log("turn off number 3 and possibly 2");
            leaderThreeName.transform.parent.gameObject.SetActive(false);
            if(i == 2){
                Debug.Log("Turning off two");
                leaderTwoName.transform.parent.gameObject.SetActive(false);
                i = 0;
            }
        }

    }
    //we have to use this internally to search for people based off their names
    public void startSearch(string s)
    {
        
        if (s == "" || s == null || s == " ")
            return;
        s = s.Replace("  ", " ");
        s = s.Replace("   ", " ");
        s = s.Replace(" ", "+");

        searchProfiles(s, j.token);



    }
    private string jsonString;
    private string searchProfileURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/search";
    //search for leaders for event
    public void searchProfiles(string s, string t)
    {

        UnityWebRequest www2 = UnityWebRequest.Get(searchProfileURL + "?search_term=" + s);
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = profilePut(www2);
        StartCoroutine(coroutine);
    }

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
                StartCoroutine(GetProfile(pSearch.email[0]));
            }

        }
    }

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
                    profilePopulator();
                }
            }
        };
    }


    //this populates the info about each leader and the organizer
    public void profilePopulator()
    {
        if(i == 3){
            leaderThreeName.text = pSearch.name[0];
            if (pSearch.pic[0].Length > 300)
            {
                Texture2D tex = new Texture2D(200, 200);
                byte[] img = System.Convert.FromBase64String(pSearch.pic[0]);
                Debug.Log(img);
                tex.LoadImage(img, false);

                leaderThreePic.texture = tex;
            }
            i = 0;
            return;
        }
        if(i == 2){
            leaderTwoName.text = pSearch.name[0];
            if (pSearch.pic[0].Length > 300)
            {
                Texture2D tex = new Texture2D(200, 200);
                byte[] img = System.Convert.FromBase64String(pSearch.pic[0]);
                Debug.Log(img);
                tex.LoadImage(img, false);

                leaderTwoPic.texture = tex;
            }

            i++;
            setUpdate(Event);
        }
        if(i == 1){
            leaderOneName.text = pSearch.name[0];
            if (pSearch.pic[0].Length > 300)
            {
                Debug.Log("Setting leader 1 pic");
                Texture2D tex = new Texture2D(200, 200);
                byte[] img = System.Convert.FromBase64String(pSearch.pic[0]);
                Debug.Log(img);
                tex.LoadImage(img, false);

                leaderOnePic.texture = tex;
            }

            i++;
            setUpdate(Event);
        }
        if (i == 0)
        {
            organizerName.text = profile.first_name + " " + profile.last_name;
            if (profile.photo.Length > 300)
            {
                Texture2D tex = new Texture2D(200, 200);
                byte[] img = System.Convert.FromBase64String(profile.photo);
                Debug.Log(img);
                tex.LoadImage(img, false);

                organizerImage.texture = tex;
            }
            i++;
            setUpdate(Event);
        }


    }

    public GameObject updateFeedPrefab;
    public GameObject updateFeedContainer;
    public GameObject updateDotPrefab;
    public GameObject updateDotContainer;
    private UpdateResponse update;
    private string updateURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";
    //this sets the event updates then instantiates them all in a scroll snap rect
    public void getEventUpdates(string origEmail,string origEvent)
    {
        Debug.Log("Getting Updates");
        StartCoroutine(GetUpdates(origEmail, origEvent));

    }
    IEnumerator GetUpdates(string origName, string eventName)
    {

        using (UnityWebRequest www = UnityWebRequest.Get(updateURL + origName+"/"+eventName+"/updates"))
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
                update = JsonUtility.FromJson<UpdateResponse>(jsonString);
                SetUpdates(update);
            }
        };
    }
    //this clears the updates for for a new events updates
    public void ClearUpdates()
    {
        int childKillCount = updateDotContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(updateDotContainer.transform.GetChild(i).gameObject);
        }

        childKillCount = updateFeedContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(updateFeedContainer.transform.GetChild(i).gameObject);
        } 
    }
    //this updates the updates! after clearing them, it loads them all in and enables the scroll snap rect
    public void SetUpdates(UpdateResponse update)
    {

        int childKillCount = updateDotContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(updateDotContainer.transform.GetChild(i).gameObject);
        }

        childKillCount = updateFeedContainer.transform.childCount;
        for (int i = childKillCount - 1; i >= 0; i--)
        {
            Destroy(updateFeedContainer.transform.GetChild(i).gameObject);
        }
        int maxNotifications =10;
        if (update.updates == null)
            return;
        if (update.updates.Length < 10)
            maxNotifications = update.updates.Length;
        for (int x = 0; x < maxNotifications; x++)
        {
            GameObject newUpdateFeed = Instantiate(updateFeedPrefab, updateFeedContainer.transform);
            newUpdateFeed.GetComponent<Text>().text = update.updates[x];
            newUpdateFeed.transform.GetChild(0).GetComponent<Text>().text = update.u_datetime[x];
            Instantiate(updateDotPrefab, updateDotContainer.transform);

        } 

        updateFeedContainer.transform.parent.GetComponent<ScrollSnapRect>().enabled = true;
        updateFeedContainer.transform.parent.GetComponent<ScrollSnapRect>().Refresh();
    }

}
[System.Serializable]
public class UpdateResponse
{
    public string[] u_datetime;
    public string[] updates;
}