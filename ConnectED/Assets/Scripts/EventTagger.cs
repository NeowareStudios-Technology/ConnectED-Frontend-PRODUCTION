using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTagger : MonoBehaviour {

    public InputField i;
    public FieldsController f;
    public GameObject button;
    //this script interacts with the fields controller to make an interactive choice for skills and interests
	public void Start()
	{
        i = transform.GetComponentInParent<InputField>();
	}
    //this is called when you pick a field
    public void SetI()
    {
        f.SetieT(i, this);
        if (i.text != "" && i.text != null)
            ResetButton();
    }
	public void ResetButton()
	{
        button.GetComponent<spriteSwitcher>().turnOff();
	}
}
