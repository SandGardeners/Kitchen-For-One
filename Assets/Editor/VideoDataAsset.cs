using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "VideoDataAsset", menuName = "Video Data Asset", order = 1)]
    public class VideoDataAsset : ScriptableObject
    {
        [SerializeField] List<VideoDataTreeElement> m_TreeElements = new List<VideoDataTreeElement>();

        internal List<VideoDataTreeElement> treeElements
        {
            get { return m_TreeElements; }
            set { m_TreeElements = value; }
        }

        void Awake()
        {
            if (m_TreeElements.Count == 0)
                m_TreeElements = VideoDataTreeGenerator.GenreateVideoDataTree();
        }
    }

