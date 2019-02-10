using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCreationController : MonoBehaviour {

    public GameObject Fields;
    public GameObject Skills;
    public GameObject EventSetup;
    public GameObject LeadersAndTags;
    public GameObject Address;
    public GameObject EventSettings;
    public GameObject EventSubmission;
    public EventCreator evcr;
    //this handles the event creation next button and sets things active or false respectively
    public void Next()
    {
        

        if (EventSetup.activeSelf)
        {
            if(evcr.title.text != null && evcr.title.text != "" && evcr.description.text != "" && evcr.NumberofVolunteers.text != ""){
                
				EventSetup.SetActive(false);
				Address.SetActive(true);
            }
            return;
        }
        if (Address.activeSelf)
        {
            if (evcr.Street.text != "" && evcr.city.text != "" && evcr.State.text != "" && evcr.zipcode.text != "")
            {
                Address.SetActive(false);
                LeadersAndTags.SetActive(true);
            }
            return;
        }
        if (LeadersAndTags.activeSelf)
        {
            LeadersAndTags.SetActive(false);
            EventSettings.SetActive(true);
            return;
        }
        if (EventSettings.activeSelf)
        {
            EventSettings.SetActive(false);
            EventSubmission.SetActive(true);
            return;
        }
        if (EventSubmission.activeSelf)
        {
            evcr.initEvent();
            return;
        }
    }

    public void GetFields(){
        Fields.SetActive(true);
    }

    public void GetSkills()
    {
        Skills.SetActive(true);
    }


    //this handles the back button
    public void Back()
    {
        if (EventSetup.activeSelf)
        {
            EventSetup.SetActive(false);
            Fields.SetActive(true);
            return;
        }
        if (Address.activeSelf)
        {
            Address.SetActive(false);
            EventSetup.SetActive(true);
            return;
        }
        if (LeadersAndTags.activeSelf)
        {
            LeadersAndTags.SetActive(false);
            Address.SetActive(true);
            return;
        }
        if (EventSettings.activeSelf)
        {
            EventSettings.SetActive(false);
            LeadersAndTags.SetActive(true);
            return;
        }
        if (EventSubmission.activeSelf)
        {
			EventSubmission.SetActive(false);
            EventSettings.SetActive(true);
            return;
        }
    }
    //this is called to reset the page
    public void setNew()
    {
        Fields.SetActive(false);
        EventSetup.SetActive(true);
        LeadersAndTags.SetActive(false);
        EventSettings.SetActive(false);
        EventSubmission.SetActive(false);
    }
}
