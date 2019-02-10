using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpController : MonoBehaviour {
    //this controls where you are in the signup process and if you can proceed or not based on certain criteria...
    public GameObject Email;
    public InputField Ema;
    public GameObject Password;
    public InputField Pass;
    public InputField rePass;
    public GameObject rePassword;
    public GameObject Name;
    public InputField first;
    public InputField last;
    public GameObject ProfilePic;
    public GameObject Schedule;
    public GameObject Fields;
    public GameObject Education;
    public GameObject Skills;
    public Animator a;
    public Jsonparser signup;

    public int current = 1;
    //if you press the next button certain things happen based off of the value of current which refers to the current page you are on
    public void Next()
    {
        if(current == 1)
        {
            if (Ema.text == null || Ema.text.Length < 10 )
                return;
            Email.SetActive(false);
            Password.SetActive(true);
            current++;
            return;
        }
        if(current == 2)
        {
            if (Pass.text.Length < 6)
                return;
            Password.SetActive(false);
            rePassword.SetActive(true);
            current++;
            return;
        }
        if(current == 3)
        {
            if (rePass.text != Pass.text)
                return;
            rePassword.SetActive(false);
            Name.SetActive(true);
            current++;
            return;
        }
        if(current == 4)
        {
            if (first.text == "" || last.text == "" || first.text == null || last.text == null)
                return;
            Name.SetActive(false);
            ProfilePic.SetActive(true);
            current++;
            return;
        }
        if(current == 5)
        {
            ProfilePic.SetActive(false);
            Schedule.SetActive(true);
            current++;
            return;
        }
        if(current == 6)
        {
            Schedule.SetActive(false);
            Fields.SetActive(true);
            current++;
            return;
        }
        if(current == 7)
        {
            Fields.SetActive(false);
            Skills.SetActive(true);
            current++;
            return;
        }
        if (current == 8)
        {
            Skills.SetActive(false);
            Education.SetActive(true);
            current++;
            return;
        }
        if (current == 9)
        {
            signup.CreateProfile();
            a.SetTrigger("SignupOver");
        }
    }
    //this does the same as next but in the opposite direction
    public void Back()
    {

        if (current == 2)
        {
            Password.SetActive(false);
            Email.SetActive(true);
            current--;
            return;
        }
        if (current == 3)
        {
            rePassword.SetActive(false);
            Password.SetActive(true);
            current--;
            return;
        }
        if (current == 4)
        {
            Name.SetActive(false);
            rePassword.SetActive(true);
            current--;
            return;
        }
        if (current == 5)
        {
            ProfilePic.SetActive(false);
            Name.SetActive(true);
            current--;
            return;
        }
        if (current == 6)
        {
            Schedule.SetActive(false);
            ProfilePic.SetActive(true);
            current--;
            return;
        }
        if (current == 7)
        {
            Fields.SetActive(false);
            Schedule.SetActive(true);
            current--;
            return;
        }
        if (current == 8)
        {
            Skills.SetActive(false);
            Fields.SetActive(true);
            current--;
            return;
        }
        if (current == 9)
        {
            Education.SetActive(false);
            Skills.SetActive(true);
            current--;
            return;
        }
        if (current == 1)
        {
            a.SetTrigger("SignupOver");
        }
    }
}
