/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>
/// 
/// 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System;

public class QREncodeTest : MonoBehaviour {
	public QRCodeEncodeController e_qrController;
	public RawImage qrCodeImage;
	private string nameOfEvent;
	public Text infoText;

    public Texture2D codeTex;
	// Use this for initialization
	void Start () {
		if (e_qrController != null) {
			e_qrController.onQREncodeFinished += qrEncodeFinished;//Add Finished Event
		}
	}
	
	void qrEncodeFinished(Texture2D tex)
	{
		if (tex != null && tex != null) {
			int width = tex.width;
			int height = tex.height;
			float aspect = width * 1.0f / height;
            codeTex = tex;
        } else {
		}
	}

    public void setCodeType(int typeId)
    {
        e_qrController.eCodeFormat = (QRCodeEncodeController.CodeMode)(typeId);
        Debug.Log("clicked typeid is " + e_qrController.eCodeFormat);
    }


    public string Encode(string s)
	{
		if (e_qrController != null) {
            string valueStr = s;
			int errorlog = e_qrController.Encode(valueStr);
			if (errorlog == -13) {
				infoText.text = "Must contain 12 digits,the 13th digit is automatically added !";

			} else if (errorlog == -8) {
				infoText.text = "Must contain 7 digits,the 8th digit is automatically added !";
			}
            else if (errorlog == -39)
            {
                infoText.text = "Only support digits";
            }
            else if (errorlog == -128) {
				infoText.text = "Contents length should be between 1 and 80 characters !";

			} else if (errorlog == -1) {
				infoText.text = "Please select one code type !";
			}
			else if (errorlog == 0) {
                Debug.Log("Saved successfully");
                return SaveCode();
			}
        }
		return "";
    }

	public void ClearCode()
	{
		qrCodeImage.texture = null;
		nameOfEvent = "";
		infoText.text = "";
	}
    public EventCreator eventCreator;
    
    public string SaveCode()
    {
        if (codeTex != null)
        {
            string qr = Convert.ToBase64String(codeTex.EncodeToJPG());
            return qr;
        }
        return "";

    }
}
