using System.Collections;
using UnityEngine;
using System.IO;

namespace TriLib
{
    namespace Samples
    {
        /// <summary>
        /// Represents a model downloader sample class.
        /// </summary>
        public class ModelDownloader : MonoBehaviour
        {
            /// <summary>
            /// Replace this constant value with your model URI.
            /// </summary>
            private const string ModelURI =
                "https://github.com/assimp/assimp/blob/master/test/models/FBX/spider.fbx?raw=true";

            /// <summary>
            /// Replace this cosntant value with the local path you want your model to be saved.
            /// </summary>
            private const string ModelLocalPath = "/spider.fbx";

            /// <summary>
            /// Change this constant value to change your model scale.
            /// </summary>
            /// 
            private const float ModelScale = 0.0025f;

            /// <summary>
            /// <see cref="UnityEngine.WWW"/> instance used to download the model.
            /// </summary>
            private WWW _www;

            /// <summary>
            /// <see cref="UnityEngine.GUIStyle"/> instance used to center the GUI text.
            /// </summary>
            private GUIStyle _centeredStyle;

            /// <summary>
            /// <see cref="UnityEngine.Rect"/> instance used to display the centered text.
            /// </summary>
            private Rect _centeredRect;

            /// <summary>
            /// Starts downloading the model and setups the centered rect.
            /// </summary>
            protected void Start()
            {
                StartCoroutine(DownloadModel());
                _centeredRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50);
            }

            /// <summary>
            /// Shows the download progress.
            /// </summary>
            protected void OnGUI()
            {
                if (_www.isDone)
                {
                    return;
                }
                if (_centeredStyle == null)
                {
                    _centeredStyle = GUI.skin.GetStyle("Label");
                    _centeredStyle.alignment = TextAnchor.UpperCenter;
                }
                GUI.Label(_centeredRect, string.Format("Downloaded {0:P2}", _www.progress), _centeredStyle);
            }

            /// <summary>
            /// Downloads the model.
            /// </summary>
            /// <returns>Coroutine <see cref="System.Collections.IEnumerator"/></returns>
            private IEnumerator DownloadModel()
            {
                _www = new WWW(ModelURI);
                while (!_www.isDone)
                {
                    yield return null;
                }
                var fullPath = Application.persistentDataPath + ModelLocalPath;
                File.WriteAllBytes(fullPath, _www.bytes);
                using (var assetLoader = new AssetLoader())
                {
                    var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
                    assetLoaderOptions.Scale = ModelScale;
                    assetLoader.LoadFromFile(fullPath, assetLoaderOptions, gameObject);
                }
            }

            //UNCOMENT THIS METHOD TO LOAD THE MODEL FROM MEMORY, WITHOUT SAVING IT
            ///// <summary>
            ///// Downloads the model.
            ///// </summary>
            ///// <returns>Coroutine <see cref="System.Collections.IEnumerator"/></returns>
            //private IEnumerator DownloadModel()
            //{
            //    _www = new WWW(ModelURI);
            //    while (!_www.isDone)
            //    {
            //        yield return null;
            //    }
            //    using (var assetLoader = new AssetLoader())
            //    {
            //        var assetLoaderOptions = AssetLoaderOptions.CreateInstance();
            //        assetLoaderOptions.Scale = ModelScale;
            //        assetLoader.LoadFromMemory(_www.bytes, ModelLocalPath, assetLoaderOptions, gameObject);
            //    }
            //}
        }
    }
}