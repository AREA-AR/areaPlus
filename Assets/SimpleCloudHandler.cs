using Vuforia;
using System;
using System.Collections;
using UnityEngine;
using System.IO;
using TriLib;
using UnityEngine.UI;
using RenderHeads.Media.AVProVideo;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results.
/// </summary>
public class SimpleCloudHandler : MonoBehaviour, ICloudRecoEventHandler
{
    public static bool active = false;
    public static bool opened = false;
    public string URL;
    public string Name;
    public string MURL;
    public string MName;    
    public Text downloadingText;
    public GameObject radialProgress;
    public UnityEngine.UI.Image LoadingBar;
    public GameObject DisplayImage;
    public ImageTargetBehaviour imageTargetBehaviour;
    private GameObject newImageTarget;
    private GameObject plane;
    public GameObject noConnection;
    public GameObject LoadingEffect;
    public static float height = 0;
    public static float width = 0;
    private GameObject model;
    private bool cameraMode = false;
    private bool doUpdate = false;
    #region PRIVATE_MEMBER_VARIABLES

    // CloudRecoBehaviour reference to avoid lookups
    private CloudRecoBehaviour mCloudRecoBehaviour;
    // ImageTracker reference to avoid lookups
    private ObjectTracker mObjectTracker;

    private ObjectTracker mImageTracker;
    private GameObject mParentOfImageTargetTemplate;
    private bool mIsScanning = false;

    private string mTargetMetadata = "";

    #endregion // PRIVATE_MEMBER_VARIABLES
    #region EXPOSED_PUBLIC_VARIABLES

    /// <summary>
    /// can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
    /// </summary>
    public ImageTargetBehaviour ImageTargetTemplate;

    #endregion

    #region UNTIY_MONOBEHAVIOUR_METHODS

    /// <summary>
    /// register for events at the CloudRecoBehaviour
    /// </summary>
    void Start()
    {
        // register this event handler at the cloud reco behaviour
        LoadingEffect.SetActive(false);
        CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        if (cloudRecoBehaviour)
        {
            cloudRecoBehaviour.RegisterEventHandler(this);
        }

        // remember cloudRecoBehaviour for later
        mCloudRecoBehaviour = cloudRecoBehaviour;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            noConnection.SetActive(true);
        }
        else
        {
            noConnection.SetActive(false);
        }
        if (height != 0 && doUpdate == true)
        {
            plane = imageTargetBehaviour.gameObject.transform.Find("Plane").gameObject;
            float aspect = height / width;
            plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f * aspect);
        }
    }
    #endregion // UNTIY_MONOBEHAVIOUR_METHODS


    #region ICloudRecoEventHandler_IMPLEMENTATION

    /// <summary>
    /// called when TargetFinder has been initialized successfully
    /// </summary>
    public void OnInitialized(TargetFinder targetFinder)
    {
        // get a reference to the Image Tracker, remember it
        mImageTracker = (ObjectTracker)TrackerManager.Instance.GetTracker<ObjectTracker>();
    }

    /// <summary>
    /// visualize initialization errors
    /// </summary>
    public void OnInitError(TargetFinder.InitState initError)
    {
    }

    /// <summary>
    /// visualize update errors
    /// </summary>
    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
    }

    /// <summary>
    /// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
    /// </summary>


    public void OnStateChanged(bool scanning)
    {
        mIsScanning = scanning;
        if (scanning)
        {
            // clear all known trackables
            ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            tracker.TargetFinder.ClearTrackables(false);            
        }
    }

    /// <summary>
    /// Handles new search results
    /// </summary>
    /// <param name="targetSearchResult"></param>

    // Here we handle a cloud target recognition event
    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult =
     (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        // duplicate the referenced image target
        newImageTarget = Instantiate(ImageTargetTemplate.gameObject) as GameObject;

        GameObject augmentation = null;
        if (augmentation != null)
            augmentation.transform.parent = newImageTarget.transform;

        // enable the new result with the same ImageTargetBehaviour:
        imageTargetBehaviour = (ImageTargetBehaviour)mImageTracker.TargetFinder.EnableTracking(targetSearchResult, newImageTarget);

        // do something with the target metadata
        mTargetMetadata = cloudRecoSearchResult.MetaData;
        string[] splitString = mTargetMetadata.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        var type = splitString[0];
        var iurl = splitString[2];    
        if (type == "video")
        {
            doUpdate = true;
            Destroy(imageTargetBehaviour.gameObject.transform.Find("dImage").gameObject);
            Destroy(imageTargetBehaviour.gameObject.transform.Find("Wrapper_RootNode").gameObject);
            playVideo();
        }
        else if (type == "image")
        {
            doUpdate = false;
            Destroy(imageTargetBehaviour.gameObject.transform.Find("Plane").gameObject);
            Destroy(imageTargetBehaviour.gameObject.transform.Find("Wrapper_RootNode").gameObject);
            StartCoroutine(DownloadImage(iurl));
        }
        else
        {
            doUpdate = false;
            Destroy(imageTargetBehaviour.gameObject.transform.Find("dImage").gameObject);
            Destroy(imageTargetBehaviour.gameObject.transform.Find("Plane").gameObject);
            Show3DModel();
        }        
        
        if (!mIsScanning)
        {
            // stop the target finder
            mCloudRecoBehaviour.CloudRecoEnabled = true;
        }
    }
    private IEnumerator DownloadImage(string URL)
    {
        var _www = new WWW(URL);
        //_www = WWW.LoadFromCacheOrDownload(ModelURI, 5);
        while (!_www.isDone)
        {
            yield return null;
        }
        DisplayImage = imageTargetBehaviour.transform.Find("dImage").gameObject;
        float heighti = _www.texture.height;
        float widthi = _www.texture.width;
        float aspect = heighti / widthi;
        DisplayImage.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f * aspect);
        DisplayImage.GetComponent<Renderer>().material.mainTexture = _www.texture;
        DisplayImage.transform.parent = Camera.main.transform;
    } 
    void playVideo()
    {
        plane = imageTargetBehaviour.gameObject.transform.Find("Plane").gameObject;
        // Vide clip from Url
        string[] splitString = mTargetMetadata.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        URL = splitString[2];
        MediaPlayer mediaPlayer = plane.GetComponent<MediaPlayer>();
        mediaPlayer.enabled = true;
        mediaPlayer.m_VideoPath = URL;
       // plane.transform.parent = Camera.main.transform;
    }    
    void Show3DModel()
    {
        string[] ModelString = mTargetMetadata.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        MName = ModelString[1];
        MURL = ModelString[2];
        if (MName != "")
        {
            if (File.Exists(Application.persistentDataPath + "/" + MName))
            {
                radialProgress.SetActive(false);
                LoadModel(MName);
            }
            else
            {
                StartCoroutine(DownloadModel(MURL, MName));
            }
        }
    }

    void LoadModel(string ModelName)
    {
        model = imageTargetBehaviour.gameObject.transform.Find("Wrapper_RootNode").gameObject;
        using (var assetLoader = new AssetLoader())
        {
            var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
            assetLoaderOptions.AutoPlayAnimations = true;
            assetLoaderOptions.Scale = 1f;
            GameObject _rootGameObject = assetLoader.LoadFromFile(Application.persistentDataPath + "/" + ModelName, assetLoaderOptions, model);
           
            _rootGameObject.transform.localScale = new Vector3(1,1,1);
            _rootGameObject.transform.localPosition = new Vector3(0,0,0);
            _rootGameObject.transform.localEulerAngles = new Vector3(0,180,0);
            _rootGameObject.transform.parent = Camera.main.transform;
            _rootGameObject.transform.localScale = new Vector3(5, 5, 5);
            _rootGameObject.transform.localPosition = new Vector3(0, -3, 100);
        }
    }

    private IEnumerator DownloadModel(string ModelURI, string ModelName)
    {
        var _www = new WWW(ModelURI);
        //_www = WWW.LoadFromCacheOrDownload(ModelURI, 5);
        while (!_www.isDone)
        {
            yield return null;
            float number = (float)Math.Round(_www.progress * 100, 1);
            radialProgress.SetActive(true);
            LoadingBar.fillAmount = _www.progress;
            downloadingText.text = number.ToString() + "%";
        }
        radialProgress.SetActive(false);

        var ModelLocalPath = "/" + ModelName;

        var fullPath = Application.persistentDataPath + ModelLocalPath;
        File.WriteAllBytes(fullPath, _www.bytes);
        StartCoroutine(LoadAfterDownload(ModelName));
    }

    IEnumerator LoadAfterDownload(string ModelName)
    {
        yield return new WaitForSeconds(3f);
        LoadModel(ModelName);
    }
   
    
    // Restart TargetFinder

    #endregion // ICloudRecoEventHandler_IMPLEMENTATION
}