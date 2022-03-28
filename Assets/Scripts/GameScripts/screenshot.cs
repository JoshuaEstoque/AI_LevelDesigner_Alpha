using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenshot : MonoBehaviour
{
    private int screenshotCount = 0;
    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            //ScreenCapture.CaptureScreenshot(Application.dataPath /*"/screenshots/" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")*/ + "screenshot" + ".png");
            //UnityEditor.AssetDatabase.Refresh();
            TakeScreenshot();
            Debug.Log("Saved to "+ Application.dataPath);
        }
    }

    public IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string path = Application.persistentDataPath + "Screenshots/"
                + "_" + screenshotCount + "_" + Screen.width + "X" + Screen.height + "" + ".png";

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();
        //Convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();

        //Save image to file
        System.IO.File.WriteAllBytes(path, imageBytes);

    }







}
