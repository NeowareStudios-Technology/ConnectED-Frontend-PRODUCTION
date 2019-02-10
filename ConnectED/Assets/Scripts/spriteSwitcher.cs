using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class spriteSwitcher : MonoBehaviour {

    //this is used heavily in the app to control which sprite is shown for an image, it is used on buttons primarily, but is also used in the calendar as well
    //pressed is used to see if buttons are pressed, and also creates a custom button system aswell
    private Sprite firstImage;
    public Sprite secondImage;
    private Sprite onImage;
    private Sprite offImage;
    public int currentImage = 1;
    public bool pressed = false;
	// Use this for initialization
	void Start () {
        firstImage = this.GetComponent<Image>().sprite;
        if (pressed)
        {
            onImage = firstImage;
            offImage = secondImage;
        }
        else
        {
            onImage = secondImage;
            offImage = firstImage;
        }
	}
	//this happens if it is just pressed
    public void isPressed()
    {
        pressed = !pressed;
        if(currentImage == 1)
        {
            this.GetComponent<Image>().sprite = secondImage;
            currentImage = 2;
        }
        else
        {
            this.GetComponent<Image>().sprite = firstImage;
            currentImage = 1;
        }

    }
    //you can also turn off or turn on buttons, this is nice for choices that can be exclusive to each other
    public void turnOff()
    {
        this.GetComponent<Image>().sprite = offImage;
        pressed = false;
    }
    public void turnOn()
    {
        this.GetComponent<Image>().sprite = onImage;
        pressed = true;
    }
}
