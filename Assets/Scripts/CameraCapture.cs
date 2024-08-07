using Interioverse;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : SingletonComponent<CameraCapture>
{
    public Camera targetCamera; // Reference to the camera you want to capture
    //public Image targetImage; // Reference to the image you want to display the captured image on
    public Material BGMaterial;

    public void CaptureCameraView()
    {
        // Capture the camera view
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        targetCamera.targetTexture = renderTexture;
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        targetCamera.Render();
        RenderTexture.active = renderTexture;
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Apply the captured image to the image sprite
        Sprite capturedSprite = Sprite.Create(screenTexture, new Rect(0, 0, screenTexture.width, screenTexture.height), new Vector2(0.5f, 0.5f));
        //targetImage.sprite = capturedSprite;
        BGMaterial.mainTexture = capturedSprite.texture;
    }
}
