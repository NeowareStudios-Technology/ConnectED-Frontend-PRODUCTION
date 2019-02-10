using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.Networking;
using System.Text;

public class profileNotificationSetter : MonoBehaviour {
    //this fills up the notifications 
    public GameObject notificationPrefab;
    public GameObject notificationDotPrefab;
    public GameObject notificationContainer;
    public GameObject notificationDotContainer;
    private Jsonparser j;
    private Notifications notifications;
    public string notificationURL = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/profiles/updates";
    public void getNotifications(){
        j = GameObject.FindWithTag("Player").GetComponent<Jsonparser>();
        StartCoroutine(NotificationLister());
    }
    private int retry = 0;
    private string jsonString;
    //this gets the notifications
    IEnumerator NotificationLister()
    {
        

        Debug.Log("Prefilling notifications");
        //using (UnityWebRequest www = UnityWebRequest.Get("https://webhook.site/8e284497-5145-481d-8a18-0883dfd599e5"))
        using (UnityWebRequest www = UnityWebRequest.Get(notificationURL))
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
                if (www.responseCode.ToString() == "503" & retry < 3)
                {
                    Debug.Log("Trying again : get notifications");
                    getNotifications();
                }
            }
            else
            {
                Debug.Log(www.url+"<--------");
                Debug.Log(www.responseCode);
                byte[] results = www.downloadHandler.data;
                jsonString = "";
                jsonString = Encoding.UTF8.GetString(results);
                Debug.Log(jsonString);
                notifications = JsonUtility.FromJson<Notifications>(jsonString);
                if (jsonString != "{}")
                    notificationInitializer();
                else
                    gameObject.SetActive(false);

            }
        }
    }
    public GameObject NotificationContainerPrefab;
    //this initializes the notification center
    public void notificationInitializer(){
        Instantiate(notificationDotPrefab, notificationDotContainer.transform);

        for (int i = 0; i < notifications.updates.Length;i++){
            //if we have room in our current container
            if(notificationContainer.transform.childCount < 8){
                GameObject newNotification = Instantiate(notificationPrefab, notificationContainer.transform);
                newNotification.transform.GetChild(0).GetComponent<Text>().text =notifications.updates[i]+ " "+notifications.events[i]+" " +notifications.u_datetime[i];
            }
            //if we have more notifications and our container if full
            if(notificationContainer.transform.childCount == 8 && i != notifications.updates.Length -1){
                notificationContainer = Instantiate(NotificationContainerPrefab, notificationContainer.transform.parent);
                Instantiate(notificationDotPrefab, notificationDotContainer.transform);
            }
        }

        notificationContainer.transform.parent.parent.gameObject.GetComponent<ScrollSnapRect>().enabled = true;
        notificationContainer.transform.parent.parent.gameObject.GetComponent<ScrollSnapRect>().Refresh();
        gameObject.SetActive(false);
    }

}


public class Notifications{
    public string[] events;
    public string[] u_datetime;
    public string[] updates;
}