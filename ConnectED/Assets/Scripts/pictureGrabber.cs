using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pictureGrabber : MonoBehaviour {
    //this script controls the native gallery plug in
    //this is where you want the image to end up
    public RawImage image;
    //when you click on an image
    public void pick()
    {
        //using 0 makes it use the images values
        PickImage(0);
    }

    private void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

					image.color = Color.white;
                    RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                    Graphics.Blit(texture, tmp);
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = tmp;
                    Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
                    myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
                    myTexture2D.Apply();
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(tmp);
                    //https://support.unity3d.com/hc/en-us/articles/206486626-How-can-I-get-pixels-from-unreadable-textures-
                    //read in with texture2d.loadimage(bytedata);

                Color[] c = myTexture2D.GetPixels(myTexture2D.width / 2 - 400, myTexture2D.height / 2 - 400, 800, 800);
                Texture2D m2Texture = new Texture2D(800, 800);
                m2Texture.SetPixels(c);
                m2Texture.Apply();
                texture = m2Texture;
                image.texture = texture;
                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
            }
        }, "Select a PNG image", "image/png", maxSize);

        Debug.Log("Permission result: " + permission);
    }

}
