using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;
public class otherFunc : MonoBehaviour {
    public GameObject captured;
    public GameObject instruc;
    public void takescreenshot()
    {
        string Screen_Shot_File_Name = "Screenshot" + System.DateTime.Now.ToString("HH:MM:ss__yyyy-MM-dd") + ".png";
        ScreenshotManager.SaveScreenshot(Screen_Shot_File_Name, "Area +");
        captured.SetActive(true);
        Invoke("removeCapturedIcon", 2);
    }
    void removeCapturedIcon()
    {
        captured.SetActive(false);
    }
    public void shareshot()
    {
        StartCoroutine(TakeSSAndShare());
    }
    private IEnumerator TakeSSAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);
#if UNITY_ANDROID
        new NativeShare().AddFile(filePath).SetSubject("").SetText("*Area + (http://areaplus.area-ar.com)*").Share();
#elif UNITY_IPHONE
        new NativeShare().AddFile(filePath).SetSubject("").SetText("*Area + (http://areaplus.area-ar.com)*").Share();
#endif
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }
    public void closeApp()
    {
        //if (Camera.main.transform.childCount>1)
        //{
        //    for (int i = 1; i < Camera.main.transform.childCount; i++)
        //    {
        //        Destroy(Camera.main.transform.GetChild(i).gameObject);
        //    }
        //}
        SceneManager.LoadScene("MarkerReco");
    }
    public void RateUS()
    {
#if UNITY_ANDROID
        string  url = "http://play.google.com/store/apps/details?id=" + Application.identifier;
#elif UNITY_IPHONE
        string url = "http://areaplus.area-ar.com";
#endif
        Application.OpenURL(url);
    }

    public void Instruction()
    {
        instruc.SetActive(true);
    }
    public void InstructionBack()
    {
        instruc.SetActive(false);
    }
    public void OpenUrl()
    {
        Application.OpenURL("http://areaplus.area-ar.com");
    }
}
