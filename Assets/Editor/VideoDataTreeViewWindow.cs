using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;


    class VideoDataTreeViewWindow : EditorWindow
    {
        [NonSerialized] bool m_Initialized;
        [SerializeField] TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        SearchField m_SearchField;
        VideoDataTreeView m_TreeView;
        VideoDataAsset m_MyTreeAsset;

        [MenuItem("Kitchen/Datas Window")]
        public static VideoDataTreeViewWindow GetWindow()
        {
            var window = GetWindow<VideoDataTreeViewWindow>();
            window.titleContent = new GUIContent("Datas Browser");
            window.Focus();
            window.Repaint();
            return window;
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var myTreeAsset = EditorUtility.InstanceIDToObject(instanceID) as VideoDataAsset;
            if (myTreeAsset != null)
            {
                var window = GetWindow();
                window.SetTreeAsset(myTreeAsset);
                return true;
            }
            return false; // we did not handle the open
        }

        void SetTreeAsset(VideoDataAsset myTreeAsset)
        {
            m_MyTreeAsset = myTreeAsset;
            m_Initialized = false;
        }

        Rect multiColumnTreeViewRect
        {
            get { return new Rect(20, 30, position.width*0.60f-20f, position.height - 60); }
        }

        Rect toolbarRect
        {
            get { return new Rect(20f, 10f, position.width*0.60f-20f, 20f); }
        }

        Rect bottomToolbarRect
        {
            get { return new Rect(20f, position.height - 18f, position.width*0.60f-20f, 16f); }
        }

        Rect videoDataInfoRect
        {
            get { return new Rect(position.width*0.60f+20f,10f,position.width-position.width*0.6f-40f,300f);}
        }
        public VideoDataTreeView treeView
        {
            get { return m_TreeView; }
        }

        void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState = VideoDataTreeView.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MyMultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<VideoDataTreeElement>(GetData());

                m_TreeView = new VideoDataTreeView(m_TreeViewState, multiColumnHeader, treeModel);

                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

                m_Initialized = true;
            }
        }

        IList<VideoDataTreeElement> GetData()
        {
            if (m_MyTreeAsset != null && m_MyTreeAsset.treeElements != null && m_MyTreeAsset.treeElements.Count > 0)
                return m_MyTreeAsset.treeElements;

            VideoDataAsset loadedAsset = Resources.Load<VideoDataAsset>("VideoDataAsset");
            if(loadedAsset == null)
            {
                Debug.LogError("Can't find VideoDataAsset in Resources folder");
            }
            // generate some test data
            return loadedAsset.treeElements;
        }

        void OnSelectionChange()
        {
            if (!m_Initialized)
                return;

            var myTreeAsset = Selection.activeObject as VideoDataAsset;
            if (myTreeAsset != null && myTreeAsset != m_MyTreeAsset)
            {
                m_MyTreeAsset = myTreeAsset;
                m_TreeView.treeModel.SetData(GetData());
                m_TreeView.Reload();
            }
        }
        void OnGUI()
        {
            InitIfNeeded();
            SearchBar(toolbarRect);
            DoTreeView(multiColumnTreeViewRect);
            BottomToolBar(bottomToolbarRect);
            VideoDataInfo(videoDataInfoRect);
        }

        void VideoDataInfo(Rect rect)
        {
            m_TreeView.VideoDataInfo(rect);
        }

        void SearchBar(Rect rect)
        {
            treeView.searchString = m_SearchField.OnGUI(rect, treeView.searchString);
        }

        void DoTreeView(Rect rect)
        {
            m_TreeView.OnGUI(rect);
        }

        void BottomToolBar(Rect rect)
        {
            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {

                var style = "miniButton";
                if (GUILayout.Button("Expand All", style))
                {
                    treeView.ExpandAll();
                }

                if (GUILayout.Button("Collapse All", style))
                {
                    treeView.CollapseAll();
                }

                if (GUILayout.Button("Show Full Names", style))
                {
                    treeView.showControls = !treeView.showControls;
                }
            }

            GUILayout.EndArea();
        }
    }


