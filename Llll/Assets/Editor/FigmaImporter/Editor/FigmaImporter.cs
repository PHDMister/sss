using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FigmaImporter.Editor.EditorTree;
using FigmaImporter.Editor.EditorTree.TreeData;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Task = System.Threading.Tasks.Task;

namespace FigmaImporter.Editor
{
    public class FigmaImporter : EditorWindow
    {
        [MenuItem("Tools/FigmaImporter")]
        static void Init()
        {
            FigmaImporter window = (FigmaImporter)EditorWindow.GetWindow(typeof(FigmaImporter));
            window.Show();

            if (defineFont == null)
            {
                string fontPath = EditorPrefs.GetString("FigmaImporterFontPath", "");
                _fontPath = fontPath;
                if (!string.IsNullOrEmpty(fontPath)) defineFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            }
        }

        private static FigmaImporterSettings _settings = null;
        public static GameObject _rootObject;
        private static List<Node> _nodes = null;
        public static Font defineFont;
        private MultiColumnLayout _treeView;
        private string _lastClickedNode = String.Empty;

        private static string _fileName;
        private static string _nodeId;
        private float _scale = 1f;
        private static string _fontPath;
        private static int ReqTimeOut = 360;
        private static bool _IgPreLoading = true;
        Dictionary<string, Texture2D> _texturesCache = new Dictionary<string, Texture2D>();
        List<string> dowingCache = new List<string>();
        public float Scale => _scale;



        void OnGUI()
        {
            if (_settings == null)
                _settings = FigmaImporterSettings.GetInstance();

            int currentPosY = 0;
            if (GUILayout.Button("OpenOauthUrl"))
            {
                OpenOauthUrl();
            }

            _settings.ClientCode = EditorGUILayout.TextField("ClientCode", _settings.ClientCode);
            _settings.State = EditorGUILayout.TextField("State", _settings.State);

            EditorUtility.SetDirty(_settings);

            if (GUILayout.Button("GetToken"))
            {
                _settings.Token = GetOAuthToken();
            }

            GUILayout.TextArea("Token:" + _settings.Token);
            _settings.Url = EditorGUILayout.TextField("Url", _settings.Url);
            _settings.RendersPath = EditorGUILayout.TextField("RendersPath", _settings.RendersPath);

            _rootObject =
                (GameObject)EditorGUILayout.ObjectField("Root Object", _rootObject, typeof(GameObject), true);
            //字库文件
            defineFont = (Font)EditorGUILayout.ObjectField("Font", defineFont, typeof(Font), false);
            if (defineFont != null)
            {
                string path = AssetDatabase.GetAssetPath(defineFont);
                if (_fontPath != path)
                {
                    _fontPath = path;
                    EditorPrefs.SetString("FigmaImporterFontPath", path);
                }
            }

            _scale = EditorGUILayout.Slider("Scale", _scale, 0.01f, 4f);
            _IgPreLoading = EditorGUILayout.Toggle("忽略预下载", _IgPreLoading);

            var redStyle = new GUIStyle(EditorStyles.label);

            redStyle.normal.textColor = UnityEngine.Color.yellow;
            EditorGUILayout.LabelField(
                "Preview on the right side loaded via Figma API. It doesn't represent the final result!!!!", redStyle);

            if (GUILayout.Button("Get Node Data"))
            {
                string apiUrl = ConvertToApiUrl(_settings.Url);
                GetNodes(apiUrl);
            }

            if (_nodes != null)
            {
                DrawAdditionalButtons();
                DrawNodeTree();
                DrawPreview();
                ShowExecuteButton();
            }
        }

        private void DrawAdditionalButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("To Generate"))
                SwitchNodesToGenerate();
            if (GUILayout.Button("To Transform"))
                SwitchNodesToTransform();
#if VECTOR_GRAHICS_IMPORTED
            if (GUILayout.Button("To SVG"))
                SwitchSVGToTransform();
#endif
            EditorGUILayout.EndHorizontal();
        }

        private void SwitchSVGToTransform()
        {
            var nodesTreeElements = _treeView.TreeView.treeModel.Data;
            NodesAnalyzer.AnalyzeSVGMode(_nodes, nodesTreeElements);
        }

        private void SwitchNodesToTransform()
        {
            var nodesTreeElements = _treeView.TreeView.treeModel.Data;
            NodesAnalyzer.AnalyzeTransformMode(_nodes, nodesTreeElements);
        }

        private void SwitchNodesToGenerate()
        {
            var nodesTreeElements = _treeView.TreeView.treeModel.Data;
            NodesAnalyzer.AnalyzeRenderMode(_nodes, nodesTreeElements);
        }

        private void DrawPreview()
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            var widthMax = position.width / 2f;
            var heightMax = this.position.height - lastRect.yMax - 50;
            var height = heightMax;
            var width = widthMax;
            _texturesCache.TryGetValue(_lastClickedNode, out var lastLoadedPreview);
            if (lastLoadedPreview != null)
            {
                CalculatePreviewSize(lastLoadedPreview, widthMax, heightMax, out width, out height);
            }

            var previewRect = new Rect(position.width / 2f, lastRect.yMax + 20, width, height);
            if (lastLoadedPreview != null)
                GUI.DrawTexture(previewRect, lastLoadedPreview);
        }

        private void CalculatePreviewSize(Texture2D lastLoadedPreview, float widthMax, float heightMax, out float width,
            out float height)
        {
            if (lastLoadedPreview.width < widthMax && lastLoadedPreview.height < heightMax)
            {
                width = lastLoadedPreview.width;
                height = lastLoadedPreview.height;
            }
            else
            {
                width = widthMax;
                height = widthMax * lastLoadedPreview.height / lastLoadedPreview.width;
                if (height > heightMax)
                {
                    height = heightMax;
                    width = heightMax * lastLoadedPreview.width / lastLoadedPreview.height;
                }
            }
        }

        private void OnDestroy()
        {
            if (_treeView != null && _treeView.TreeView != null)
                _treeView.TreeView.OnItemClick -= ItemClicked;
            _treeView = null;
            _nodes = null;
            foreach (var texture in _texturesCache)
            {
                DestroyImmediate(texture.Value);
            }

            _texturesCache.Clear();
        }

        private void DrawNodeTree()
        {
            bool justCreated = false;
            if (_treeView == null)
            {
                _treeView = new MultiColumnLayout();
                justCreated = true;
            }

            var lastRect = GUILayoutUtility.GetLastRect();
            var width = position.width / 2f;
            var treeRect = new Rect(0, lastRect.yMax + 20, width, this.position.height - lastRect.yMax - 50);
            _treeView.OnGUI(treeRect, _nodes);
            var nodesTreeElements = _treeView.TreeView.treeModel.Data;
            if (justCreated)
            {
                _treeView.TreeView.OnItemClick += ItemClicked;
                NodesAnalyzer.AnalyzeRenderMode(_nodes, nodesTreeElements);
                LoadAllRenders(nodesTreeElements);
            }

            NodesAnalyzer.CheckActions(_nodes, nodesTreeElements);
        }

        private void LoadAllRenders(IList<NodeTreeElement> nodesTreeElements)
        {
            if (nodesTreeElements == null || nodesTreeElements.Count == 0)
                return;

            if (_IgPreLoading)
            {
                _lastClickedNode = nodesTreeElements[0].figmaId;
                return;
            }

            int preDownloadCount = nodesTreeElements.Count;
            if (nodesTreeElements.Count >= 100)
            {
                preDownloadCount = 50;
                Debug.Log($"FigmaImporter  start download asset , total count:{nodesTreeElements.Count}, too many requests so only pre download {preDownloadCount}");
            }
            else
            {
                Debug.Log($"FigmaImporter  start download asset , total count:{nodesTreeElements.Count}");
            }

            var tasks = new List<Func<Task>>();
            for (int i = 1; i <= preDownloadCount; i++)
            {
                tasks.Add(() => GetImage(nodesTreeElements[i].figmaId));
            }
            tasks.WhenAll(5);
            _lastClickedNode = nodesTreeElements[0].figmaId;
        }

        private async void ItemClicked(string obj)
        {
            Debug.Log($"[FigmaImporter] {obj} clicked");
            _lastClickedNode = obj;
            if (!_texturesCache.TryGetValue(obj, out var tex))
            {
                if (!dowingCache.Contains(obj))
                {
                    await GetImage(obj, false);
                }
                else
                {
                    Debug.Log($"[FigmaImporter] {obj}  is downing......");
                }
            }
            Repaint();
        }

        private void ShowExecuteButton()
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            var buttonRect = new Rect(lastRect.xMin, this.position.height - 30, lastRect.width, 30f);
            if (GUI.Button(buttonRect, "Generate nodes"))
            {
                string apiUrl = ConvertToApiUrl(_settings.Url);
                GetFile(apiUrl);
            }
        }

        public async Task GetNodes(string url)
        {
            OnDestroy();
            _nodes = await GetNodeInfo(url);
            FigmaNodesProgressInfo.HideProgress();
        }

        private string ConvertToApiUrl(string s)
        {
            var substrings = s.Split('/');
            var length = substrings.Length;
            bool isNodeUrl = substrings[length - 1].Contains("node-id");
            _fileName = substrings[length - 2];
            if (!isNodeUrl)
            {
                return $"https://api.figma.com/v1/files/{_fileName}";
            }

            _nodeId = substrings[length - 1].Substring(substrings[length - 1].IndexOf("node-id=") + "node-id=".Length);
            return $"https://api.figma.com/v1/files/{_fileName}/nodes?ids={_nodeId.Replace("-", ":")}";
        }

        private const string ApplicationKey = "msRpeIqxmc8a7a6U0Z4Jg6";
        private const string RedirectURI = "https://manakhovn.github.io/figmaImporter";

        private const string OAuthUrl =
            "https://www.figma.com/oauth?client_id={0}&redirect_uri={1}&scope=file_read&state={2}&response_type=code";

        public void OpenOauthUrl()
        {
            var state = Random.Range(0, Int32.MaxValue);
            string formattedOauthUrl = String.Format(OAuthUrl, ApplicationKey, RedirectURI, state.ToString());
            Application.OpenURL(formattedOauthUrl);
        }

        private const string ClientSecret = "VlyvMwuA4aVOm4dxcJgOvxbdWsmOJE";

        private const string AuthUrl =
            "https://www.figma.com/api/oauth/token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}&grant_type=authorization_code";

        private string GetOAuthToken()
        {
            WWWForm form = new WWWForm();
            string request = String.Format(AuthUrl, ApplicationKey, ClientSecret, RedirectURI, _settings.ClientCode);
            using (UnityWebRequest www = UnityWebRequest.Post(request, form))
            {
                www.timeout = ReqTimeOut;
                www.SendWebRequest();

                while (!www.isDone)
                {
                }

                if (www.result == UnityWebRequest.Result.ConnectionError
                    || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error + "  GetOAuthToken()");
                }
                else
                {
                    var result = www.downloadHandler.text;
                    Debug.Log(result);
                    return JsonUtility.FromJson<AuthResult>(result).access_token;
                }
            }

            return "";
        }

        private async void GetFile(string fileUrl)
        {
            if (_rootObject == null)
            {
                Debug.LogError($"[FigmaImporter] Root object is null. Please add reference to a Canvas or previous version of the object");
                return;
            }

            if (_nodes == null)
            {
                FigmaNodesProgressInfo.CurrentNode = FigmaNodesProgressInfo.NodesCount = 0;
                FigmaNodesProgressInfo.CurrentTitle = "Loading nodes info";
                await GetNodes(fileUrl);
            }

            FigmaNodeGenerator generator = new FigmaNodeGenerator(this);
            foreach (var node in _nodes)
            {
                var nodeTreeElements = _treeView.TreeView.treeModel.Data;
                await generator.GenerateNode(node, _rootObject, nodeTreeElements);
            }

            //==========================================================
            ClearCompOnRootNode();
            GenCompOnNode(_rootObject.transform.Find(_nodes[0].name));


            //==========================================================
            FigmaNodesProgressInfo.HideProgress();
        }

        private async Task<List<Node>> GetNodeInfo(string nodeUrl)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(nodeUrl))
            {
                www.timeout = ReqTimeOut;
                www.SetRequestHeader("Authorization", $"Bearer {_settings.Token}");
                www.SendWebRequest();
                while (!www.isDone && www.result != UnityWebRequest.Result.ConnectionError)
                {
                    FigmaNodesProgressInfo.CurrentInfo = "Loading nodes info";
                    FigmaNodesProgressInfo.ShowProgress(www.downloadProgress);
                    await Task.Delay(100);
                }

                FigmaNodesProgressInfo.HideProgress();

                if (www.result == UnityWebRequest.Result.ConnectionError
                    || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    var result = www.downloadHandler.text;
                    FigmaParser parser = new FigmaParser();
                    return parser.ParseResult(result);
                }

                FigmaNodesProgressInfo.HideProgress();
            }

            return null;
        }

        private const string ImagesUrl = "https://api.figma.com/v1/images/{0}?ids={1}&svg_include_id=true&format=png&scale={2}";

        public async Task<Texture2D> GetImage(string nodeId, bool showProgress = true)
        {
            if (_texturesCache.TryGetValue(nodeId, out var tex))
            {
                dowingCache.Remove(nodeId);
                return _texturesCache[nodeId];
            }
            if (dowingCache.Contains(nodeId))
            {
                return null;
            }
            dowingCache.Add(nodeId);
            string request = string.Format(ImagesUrl, _fileName, nodeId, _scale);
            var requestResult = await MakeRequest<string>(request, showProgress);
            var substrs = string.IsNullOrEmpty(requestResult) ? new string[0] : requestResult.Split('"');
            FigmaNodesProgressInfo.CurrentInfo = "Loading node texture";
            List<string> sList = new List<string>(substrs);
            string url = sList.Find(s => s.Contains("http"));
            if (string.IsNullOrEmpty(url))
            {
                dowingCache.Remove(nodeId);
                return null;
            }
            var texture = await LoadTextureByUrl(url, showProgress);
            _texturesCache[nodeId] = texture;
            dowingCache.Remove(nodeId);
            return texture;
        }

#if VECTOR_GRAHICS_IMPORTED
        private const string SvgImagesUrl = "https://api.figma.com/v1/images/{0}?ids={1}&format=svg";
        public async Task<byte[]> GetSvgImage(string nodeId, bool showProgress = true)
        {
            //WWWForm form = new WWWForm();
            string request = string.Format(SvgImagesUrl, _fileName, nodeId);
            var svgInfoRequest = await MakeRequest<string>(request, showProgress);
            var substrs = svgInfoRequest.Split('"');
            foreach (var str in substrs)
            {
                if (str.Contains("https"))
                {
                    var svgData = await MakeRequest<byte[]>(str, showProgress, false);
                    return svgData;
                }
            }
            return null;
        }
#endif

        private async Task<T> MakeRequest<T>(string request, bool showProgress, bool appendBearerToken = true) where T : class
        {
            using (UnityWebRequest www = UnityWebRequest.Get(request))
            {
                if (appendBearerToken)
                {
                    www.SetRequestHeader("Authorization", $"Bearer {_settings.Token}");
                }
                www.timeout = ReqTimeOut;
                www.SendWebRequest();
                while (!www.isDone && www.result != UnityWebRequest.Result.ConnectionError)
                {
                    FigmaNodesProgressInfo.CurrentInfo = "Getting node image info";
                    if (showProgress) FigmaNodesProgressInfo.ShowProgress(www.downloadProgress);
                    await Task.Delay(100);
                }

                FigmaNodesProgressInfo.HideProgress();

                if (www.result == UnityWebRequest.Result.ConnectionError
                    || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log(www.error + " \n " + request);
                    return null;
                }
                else
                {
                    if (typeof(T) == typeof(string)) return www.downloadHandler.text as T;
                    return www.downloadHandler.data as T;
                }
            }
        }

        public string GetRendersFolderPath()
        {
            return _settings.RendersPath;
        }

        private async Task<Texture2D> LoadTextureByUrl(string url, bool showProgress = true)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                request.timeout = ReqTimeOut;
                request.SendWebRequest();
                while (request.downloadProgress < 1f)
                {
                    if (showProgress)
                        FigmaNodesProgressInfo.ShowProgress(request.downloadProgress);
                    await Task.Delay(100);
                }

                if (request.result == UnityWebRequest.Result.ConnectionError
                    || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("request error url :" + url + "  \nerror:" + request.error);
                    return null;
                }

                var data = request.downloadHandler.data;
                Texture2D t = new Texture2D(0, 0);
                t.LoadImage(data);
                FigmaNodesProgressInfo.HideProgress();
                return t;
            }
        }


        [Serializable]
        public class AuthResult
        {
            [SerializeField] public string access_token;
            [SerializeField] public string expires_in;
            [SerializeField] public string refresh_token;
        }


        //===============================================================================
        public void RecursiveUI(Transform parent, UnityAction<Transform> callback)
        {
            if (callback != null)
                callback(parent);

            if (parent.childCount >= 0)
            {
                for (int i = 0; i < parent.childCount; i++)
                {
                    Transform child = parent.GetChild(i);

                    RecursiveUI(child, callback);
                }
            }
        }

        //去除根节点上不必要的组件
        private void ClearCompOnRootNode()
        {
            //生成prefab节点之后
            Transform uiroot = _rootObject.transform.Find(_nodes[0].name);
            TransformUtils.SetDefineConstrasints(uiroot as RectTransform);
            Image image = uiroot.GetComponent<Image>();
            if (image) DestroyImmediate(image);
        }
        //生成节点之后根据节点的命名添加组件
        private void GenCompOnNode(Transform uiroot)
        {
            //自动添加button组件  btn_
            List<Transform> btnTransforms = new List<Transform>();
            RecursiveUI(uiroot, t => { if (t.name.Contains("button-")) btnTransforms.Add(t); });
            btnTransforms.ForEach(btn =>
            {
                Image img = btn.GetComponent<Image>();
                if (!img) img = btn.GetComponentInChildren<Image>();
                if (!img) return;
                Button button = btn.gameObject.AddComponent<Button>();
                button.targetGraphic = img;
            });

            ////toggle toggle_
            //btnTransforms.Clear();
            //RecursiveUI(uiroot, t => { if (t.name.Contains("toggle_")) btnTransforms.Add(t); });
            //btnTransforms.ForEach(btn =>
            //{
            //    Image img = btn.GetComponent<Image>();
            //    if (!img) img = btn.GetComponentInChildren<Image>();
            //    if (!img) return;
            //    Toggle toggle = btn.gameObject.AddComponent<Toggle>();
            //    toggle.targetGraphic = img;
            //});

            ////slider slider_
            //btnTransforms.Clear();
            //RecursiveUI(uiroot, t => { if (t.name.Contains("slider_")) btnTransforms.Add(t); });
            //btnTransforms.ForEach(btn =>
            //{
            //    Image[] img = btn.GetComponentsInChildren<Image>();
            //    Slider toggle = btn.gameObject.AddComponent<Slider>();
            //    if (img != null && img.Length > 0) toggle.targetGraphic = img[0];
            //    if (img != null && img.Length > 1) toggle.fillRect = img[1].rectTransform;
            //});
        }


    }
}