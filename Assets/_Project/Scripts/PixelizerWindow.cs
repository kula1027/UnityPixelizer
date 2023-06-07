using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pixelizer {
    public class PixelizerWindow : EditorWindow {
        private static GUIStyle headStyle;
        private static PixelizerWindowData data;

        public static Texture2D GeneratedColor { get; private set; }
        public static Texture2D GeneratedNormal { get; private set; }

        public static float TextureAspectRatio { get; private set; }

        private PixelizerMethod[] colorMethods = {
            new Pixelize_RgbAvg(),
            new Palettize_Rgb(),
            new Palettize_RgbPercep(),
            new Palettize_Hsv(),
            new Palettize_Hsv2(),
            new Palettize_HsvWeight(),
        };

        private PixelizerMethod[] normalMethods = {
            new Pixelize_RgbAvg(),
            new Palettize_Sectionize()
        };

        private List<PixelizerMethod> selectedColorMethods = new List<PixelizerMethod>();
        private List<PixelizerMethod> selectedNormalMethods = new List<PixelizerMethod>();

        #region Init

        [InitializeOnLoadMethod]
        private static void OnLoad() {
            if (!data) {
                data = AssetDatabase.LoadAssetAtPath<PixelizerWindowData>("Assets/_Project/PixelizerWindowData.asset");

                if (data) return;

                data = CreateInstance<PixelizerWindowData>();

                AssetDatabase.CreateAsset(data, "Assets/_Project/PixelizerWindowData.asset");
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Pixelizer/Pixelizer")]
        static void Init() {
            PixelizerWindow window = (PixelizerWindow)GetWindow(typeof(PixelizerWindow));

            window.Show();
        }

        #endregion

        Vector2 scrollPosition;
        void OnGUI() {
            headStyle = new GUIStyle(EditorStyles.boldLabel);
            headStyle.fontSize = 15;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GenerateButtons();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            OnGUI_ColorMethods();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            OnGUI_NormalMethods();

            GUILayout.Space(30);



            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            OnGUI_GenerateAllInDirectory();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            OnGUI_Settings();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


            OnGUI_Setup();

            EditorGUILayout.EndScrollView();

            EditorUtility.SetDirty(data);
        }

        private void GenerateButtons() {
            if (GUILayout.Button("Generate", GUILayout.Height(40))) {
                Debug.Log("[Pixelizer] Generate");

                if (GeneratedColor) {
                    DestroyImmediate(GeneratedColor);
                }
                if (GeneratedNormal) {
                    DestroyImmediate(GeneratedNormal);
                }

                /////////// color /////////////
                ///
                Texture2D srcColor = data.inputRtColor.CreateTexture2D();
                PixelizerIn inputColor = new PixelizerIn {
                    Texture = srcColor,
                    BlockSize = data.blockSize,
                    AlphaCut = data.alphaCut,
                };
                for (int loop = 0; loop < selectedColorMethods.Count; loop++) {
                    inputColor.Texture = selectedColorMethods[loop].Process(inputColor);
                }
                GeneratedColor = inputColor.Texture;

                /////////// color /////////////

                /////////// normal /////////////

                Texture2D srcNormal = data.inputRtNormal.CreateTexture2D();
                PixelizerIn inputNormal = new PixelizerIn {
                    Texture = srcNormal,
                    BlockSize = data.blockSize,
                    AlphaCut = data.alphaCut,
                };
                for (int loop = 0; loop < selectedNormalMethods.Count; loop++) {
                    inputNormal.Texture = selectedNormalMethods[loop].Process(inputNormal);
                }
                GeneratedNormal = inputNormal.Texture;

                /////////// normal /////////////

            }

            if (GUILayout.Button("Save Generated", GUILayout.Height(40))) {
                if (GeneratedColor != null) {
                    Pixelizer.WriteToFile(GeneratedColor, data.savePath, $"{data.fileNameColor}{data.fileNum}");
                } else {
                    Debug.LogWarning($"[Pixelizer] texture is null");
                }

                if (GeneratedNormal != null) {
                    Pixelizer.WriteToFile(GeneratedNormal, data.savePath, $"{data.fileNameNormal}{data.fileNum}");
                } else {
                    Debug.LogWarning($"[Pixelizer] texture is null");
                }
                data.fileNum++;
            }


        }

        private void OnGUI_ColorMethods() {
            GUILayout.Label("Color Methods", headStyle);
            GUILayout.Space(10);

            ///////////////////

            int idx = 0;
            while (idx < selectedColorMethods.Count) {
                PixelizerMethod pm = selectedColorMethods[idx];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"+ {pm.Name}", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    selectedColorMethods.RemoveAt(idx);

                }
                GUILayout.EndHorizontal();

                pm.OnGUI();

                ++idx;

                GUILayout.Space(10);
            }

            GUILayout.Space(20);

            OnGUI_AddMethods(colorMethods, selectedColorMethods);
        }

        private void OnGUI_NormalMethods() {
            GUILayout.Label("Normal Methods", headStyle);
            GUILayout.Space(10);

            ///////////////////

            int idx = 0;
            while (idx < selectedNormalMethods.Count) {
                PixelizerMethod pm = selectedNormalMethods[idx];
                GUILayout.BeginHorizontal();
                GUILayout.Label($"+ {pm.Name}", EditorStyles.boldLabel);
                if (GUILayout.Button("Remove", GUILayout.Width(80))) {
                    selectedNormalMethods.RemoveAt(idx);
                }
                GUILayout.EndHorizontal();

                pm.OnGUI();

                ++idx;

                GUILayout.Space(10);
            }

            GUILayout.Space(20);

            OnGUI_AddMethods(normalMethods, selectedNormalMethods);
        }


        private void OnGUI_AddMethods(PixelizerMethod[] methodPool, List<PixelizerMethod> selectedMethods) {
            GUILayout.Label("Add Method", EditorStyles.boldLabel);
            GUILayout.BeginVertical("Box");

            int selGridInt = -1;
            selGridInt = GUILayout.SelectionGrid(selGridInt, methodPool.Select(mtd => mtd.Name).ToArray(), 2);
            if (selGridInt != -1) {
                selectedMethods.Add(methodPool[selGridInt]);
            }

            GUILayout.EndVertical();
        }


        private void OnGUI_Settings() {
            GUILayout.Label("Settings", headStyle);

            //////// Input Size ////////
            data.inputSize = EditorGUILayout.Vector2IntField("Input Size", data.inputSize);
            data.inputSize.x = Mathf.Max(4, data.inputSize.x);
            data.inputSize.y = Mathf.Max(4, data.inputSize.y);
            TextureAspectRatio = (float)data.inputSize.x / data.inputSize.y;

            if (data.inputRtColor != null) {
                if (data.inputRtColor.width != data.inputSize.x || data.inputRtColor.height != data.inputSize.y) {
                    data.inputRtColor.Release();
                    data.inputRtColor.width = data.inputSize.x;
                    data.inputRtColor.height = data.inputSize.y;
                    data.inputRtColor.Create();
                }
            }
            if (data.inputRtNormal != null) {
                if (data.inputRtNormal.width != data.inputSize.x || data.inputRtNormal.height != data.inputSize.y) {
                    data.inputRtNormal.Release();
                    data.inputRtNormal.width = data.inputSize.x;
                    data.inputRtNormal.height = data.inputSize.y;
                    data.inputRtNormal.Create();
                }
            }
            //////// Input Size ////////

            //////// Pixels Per Block ////////
            GUILayout.BeginHorizontal();
            data.blockSize = EditorGUILayout.IntField("Pixels Per Block", data.blockSize);
            data.blockSize = Mathf.Max(1, data.blockSize);
            GUI.enabled = false;
            GUILayout.TextArea($"({data.inputSize.x / data.blockSize}, {data.inputSize.y / data.blockSize})");
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            //////// Pixels Per Block ////////           

            data.alphaCut = EditorGUILayout.IntField("alphaCut", data.alphaCut, GUILayout.Width(200));


            GUILayout.Space(20);



            GUILayout.BeginHorizontal();

            data.savePath = EditorGUILayout.TextField("Save Path", data.savePath);
            if (GUILayout.Button("Find", GUILayout.Width(40))) {
                data.savePath = EditorUtility.OpenFolderPanel("Load png Textures", "", "");
            }
            if (GUILayout.Button("Open", GUILayout.Width(40))) {
                EditorUtility.RevealInFinder(data.savePath);
            }

            GUILayout.EndHorizontal();

            data.fileNameColor = EditorGUILayout.TextField("FileName Color", data.fileNameColor, GUILayout.Width(250));
            data.fileNameNormal = EditorGUILayout.TextField("FileName Normal", data.fileNameNormal, GUILayout.Width(250));
            data.fileNum = EditorGUILayout.IntField("FileNumber", data.fileNum, GUILayout.Width(200));


        }

        private void OnGUI_GenerateAllInDirectory() {
            GUILayout.Label("Generate All In Directory", headStyle);

            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            data.genAll_targetPath = EditorGUILayout.TextField("Target Path", data.genAll_targetPath);
            GUI.enabled = true;
            if (GUILayout.Button("Find", GUILayout.Width(40))) {
                data.genAll_targetPath = EditorUtility.OpenFolderPanel("Target Path", "", "");
            }
            if (GUILayout.Button("Open", GUILayout.Width(40))) {
                EditorUtility.RevealInFinder(data.genAll_targetPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            data.genAll_savePath = EditorGUILayout.TextField("Save Path", data.genAll_savePath);
            GUI.enabled = true;
            if (GUILayout.Button("Find", GUILayout.Width(40))) {
                data.genAll_savePath = EditorUtility.OpenFolderPanel("Save Path", "", "");
            }
            if (GUILayout.Button("Open", GUILayout.Width(40))) {
                EditorUtility.RevealInFinder(data.genAll_savePath);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Generate All In Directory", GUILayout.Height(40))) {
                string[] files = Directory.GetFiles(data.genAll_targetPath, "*.png", SearchOption.TopDirectoryOnly);
                foreach (string file in files) {
                    Debug.Log($"[Pixelizer] Generate {Path.GetFileNameWithoutExtension(file)}");

                    byte[] fileData = File.ReadAllBytes(file);
                    Texture2D srcColor = new Texture2D(2, 2);
                    srcColor.LoadImage(fileData);

                    PixelizerIn inputColor = new PixelizerIn {
                        Texture = srcColor,
                        BlockSize = data.blockSize,
                        AlphaCut = data.alphaCut,
                    };
                    for (int loop = 0; loop < selectedColorMethods.Count; loop++) {
                        inputColor.Texture = selectedColorMethods[loop].Process(inputColor);
                    }
                    Pixelizer.WriteToFile(inputColor.Texture, data.genAll_savePath, $"{Path.GetFileNameWithoutExtension(file)}");

                    DestroyImmediate(srcColor);
                }
            }


        }

        private void OnGUI_Setup() {
            GUILayout.Label("Setup", headStyle);

            data.inputRtColor = EditorGUILayout.ObjectField(
                "Input_RT_Color",
                data.inputRtColor,
                typeof(RenderTexture), false)
                as RenderTexture;

            data.inputRtNormal = EditorGUILayout.ObjectField(
                "Input_RT_Normal",
                data.inputRtNormal,
                typeof(RenderTexture), false)
                as RenderTexture;


            GUILayout.Space(10);
        }
    }
}