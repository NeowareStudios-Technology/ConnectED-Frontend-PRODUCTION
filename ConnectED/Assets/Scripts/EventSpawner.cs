using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using UnityEngine.Networking;

public class EventSpawner : MonoBehaviour {
    //this script spawns in all the events into the explore page
    public GameObject container;
    public GameObject dotContainer;
    public GameObject prefabEvent;
    public GameObject dotPrefab;
    private string prefillurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/prefill";
    private string getEventurl = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";
    public string jsonString;
    public ScrollSnapRect scroll;
    public GameObject Details;
    private prefill prefill;
    private Event Event;
    private Event[] allEvents;
    public Jsonparser j;
    public GameObject Loading;
    public calendarPopulator calPop;
    public GameObject EventHolder;

    //lead into prefilllister webcall
    public void populateEvents()
    {
        StartCoroutine(prefillLister());
    }

    public int retry = 0;
    //this webcall gets all of the events
    IEnumerator prefillLister()
    {

         foreach (Transform child in EventHolder.transform) {
                GameObject.Destroy(child.gameObject);
            }
            
        FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;


        Debug.Log("Prefilling events");
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get(prefillurl))
        {



            www.SetRequestHeader("Authorization", "Bearer " + j.token);
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();
            //this script tries to get the events again if it fails
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.responseCode);
                Debug.Log(www.url);
                Debug.Log(www.GetRequestHeader("Authorization"));
                Debug.Log(www.GetRequestHeader("Content-Type"));
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
                if (www.responseCode.ToString() == "503" & retry < 3)
                {
                    Debug.Log("Trying again : get prefill");
                    populateEvents();
                    retry++;
                }
            }
            else
            {
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
				Debug.Log(jsonString);
                prefill = JsonUtility.FromJson<prefill>(jsonString);
                if (jsonString != "{}")
                {
					StartCoroutine(Populator());
                }else{
                    Loading.SetActive(false);
                    calPop.StartCalendar();
                }
            }
        };
    }
    //this webcall populates the tile array with events from the event list
    IEnumerator Populator()
    {


            Debug.Log("Calling Populator");
            FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;

            //this script will spawn 10 or fewer events
            int amountOfEvents = 10;
            allEvents = new Event[prefill.events.Length];
            if (prefill.events.Length < amountOfEvents)
                amountOfEvents = prefill.events.Length;

            //this is where the web calls for all the events are called
            for (int i = 0; i < amountOfEvents; i++)
            {
                //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
                Debug.Log(prefill.events[i]);
            using (UnityWebRequest www = UnityWebRequest.Get(getEventurl + prefill.events[i]))
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

                        allEvents[i] = Event;
                        GameObject newEvent = Instantiate(prefabEvent, container.transform);
                        Instantiate(dotPrefab, dotContainer.transform);
                        newEvent.GetComponent<EventInitializer>().GetEvent(Event, prefill.distances[i]);
                        newEvent.GetComponent<EventInitializer>().button.onClick.AddListener(() => Details.GetComponent<Animator>().SetBool("Show", true));
                    }
                };
            }
        //this starts the calendar
            calPop.populateEvents();

            calPop.StartCalendar();

        //refresh here
        Loading.SetActive(false);
        scroll.Refresh();
    }
}
[System.Serializable]
public class prefill
{
    public int[] distances;
    public string[] events;
}




public class timePrefill
{
    public string[] events;
}
