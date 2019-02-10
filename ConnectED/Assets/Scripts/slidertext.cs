using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slidertext : MonoBehaviour {
    private Text s;
    public Slider slide;
	// Use this for initialization
	void Start () {
        s = this.GetComponent<Text>();
        s.text = slide.value.ToString() + " Miles";
	}
	//this is called when you move the slider that controls the miles away filter
    public void ChangedValue(){
        s.text = slide.value.ToString() + " Miles";
    }
	
}
