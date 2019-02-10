using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldsController : MonoBehaviour {

    public InputField i;
    public EventTagger eT;

    public void returnField(GameObject o)
    {
        i.text = o.transform.GetChild(0).GetComponent<Text>().text;
        eT.button = o;
        this.gameObject.SetActive(false);
    }
    public void SetieT(InputField inputField, EventTagger eventTagger)
    {
        i = inputField;
        eT = eventTagger;
    }
}
