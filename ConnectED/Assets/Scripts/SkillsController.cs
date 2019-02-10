using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsController : MonoBehaviour {

    public InputField i;
    public EventTagger eT;
    //this helps set the skills of the profile
    public void returnField(GameObject o)
    {
        i.text = o.transform.GetChild(0).GetComponent<Text>().text;
        eT.button = o;
        this.gameObject.SetActive(false);
    }
    //this sets which skillfield will be filled
    public void SetieT(InputField inputField, EventTagger eventTagger)
    {
        i = inputField;
        eT = eventTagger;
    }
}
