using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class QRDecodeTest : MonoBehaviour
{
	public QRCodeDecodeController e_qrController;

    private string qrResult;
    public Jsonparser jsonparser;
    public GameObject canvas;
	#if (UNITY_ANDROID||UNITY_IOS) && !UNITY_EDITOR
	bool isTorchOn = false;
	#endif
	public Sprite torchOnSprite;
	public Sprite torchOffSprite;
	public Image torchImage;
    public GameObject camera;
    public GameObject QRpage;
    public RawImage QRimage;
    public float transition;
	/// <summary>
	/// when you set the var is true,if the result of the decode is web url,it will open with browser.
	/// </summary>
	public bool isOpenBrowserIfUrl;

	private void Update()
	{
        if(transition > 0f){
            transition -= .01f;
        }
        else if(responsePanel.activeSelf && transition <= 0.01f){
            responsePanel.SetActive(false);
            cameraCanvas.SetActive(false);
        }
            
	}

	private void Start()
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.onQRScanFinished += new QRCodeDecodeController.QRScanFinished(this.qrScanFinished);
		}
	}

    public void setQRCode(Texture2D tex)
    {
        QRimage.texture = tex;
    }
	private void qrScanFinished(string dataText)
	{
		string token = jsonparser.token;
        QRScan(dataText,token);
    }

   

    private string dbqrScanPut = "https://connected-dev-214119.appspot.com/_ah/api/connected/v1/events/";
    private IEnumerator coroutine;
    public void QRScan(string s,string t)
    {
        string s1 = s.Split('/')[0];
        string s2 = s.Split('/')[1];
        UnityWebRequest www2 = UnityWebRequest.Get(dbqrScanPut+s1.ToLower()+"/"+s2+"/"+"qr");
        www2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www2.SetRequestHeader("Authorization", "Bearer " + t);
        coroutine = Put(www2);
        StartCoroutine(coroutine);
    }
    private string jsonString;
    private Response response;
    public GameObject responsePanel;
    public GameObject cameraCanvas;
    public Text responseText;
    private IEnumerator Put(UnityWebRequest www)
    {
        yield return www.SendWebRequest();

        Debug.Log("Status Code: " + www.responseCode);
        Debug.Log(www.error);
        Debug.Log(www.downloadHandler.text);
        if (www.error == null)
        {
            jsonString = Encoding.UTF8.GetString(www.downloadHandler.data);
            response = JsonUtility.FromJson<Response>(jsonString);
        }
        Debug.Log(www.url);
        Debug.Log(www.GetRequestHeader("Authorization"));
        if (www.error == null)
        {
            responsePanel.SetActive(true);
            responseText.text = response.response;
            Stop();
            Reset();
        } else
        {
            Reset();
        }
    }

   
	public void Reset()
	{
		if (this.e_qrController != null)
		{
			this.e_qrController.Reset();
		}
		if (this.qrResult != null)
		{
            this.qrResult = string.Empty;
		}

	}

	public void Play()
	{
		Reset ();
        camera.SetActive(true);
        cameraCanvas.SetActive(true);
        canvas.SetActive(false);
		if (this.e_qrController != null)
		{
			this.e_qrController.StartWork();
		}
	}

	public void Stop()
	{
        canvas.SetActive(true);
        responsePanel.SetActive(true);
        transition = 1f;
        camera.SetActive(false);
		if (this.e_qrController != null)
		{
			this.e_qrController.StopWork();
		}
	}

    public void setText()
    {
        responseText.text = "Exiting \nQR Scan Mode";
    }

	/// <summary>
	/// Toggles the torch by click the ui button
	/// note: support the feature by using the EasyWebCam Component 
	/// </summary>
	//public void toggleTorch()
	//{
	//	#if (UNITY_ANDROID||UNITY_IOS) && !UNITY_EDITOR
	//	if (EasyWebCam.isActive) {
	//		if (isTorchOn) {
	//			torchImage.sprite = torchOffSprite;
	//			EasyWebCam.setTorchMode (TBEasyWebCam.Setting.TorchMode.Off);
	//		} else {
	//			torchImage.sprite = torchOnSprite;
	//			EasyWebCam.setTorchMode (TBEasyWebCam.Setting.TorchMode.On);
	//		}
	//		isTorchOn = !isTorchOn;
	//	}
	//	#endif
	//}


}
[System.Serializable]
public class Response 
{
    public string response;
}