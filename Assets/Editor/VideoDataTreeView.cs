using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using UnityEngine.Assertions;

internal class VideoDataTreeView : UnityEditor.TreeViewExamples.TreeViewWithTreeModel<VideoDataTreeElement>
{
    const float kRowHeights = 20f;
    const float kToggleWidth = 18f;
    public bool showControls = true;

    // All columns
    enum MyColumns
    {
        Narration,
        Verb,
        Noun,
        Occurences,
        PlayVideo
    }

    public enum SortOption
    {
        Narration,
        Verb,
        Noun,
        Occurences,
        PlayVideo
    }

    // Sort options per column
    SortOption[] m_SortOptions =
    {
        SortOption.Narration,
        SortOption.Verb,
        SortOption.Noun,
        SortOption.Occurences,
        SortOption.PlayVideo
    };

    public static void TreeToList(TreeViewItem root, IList<TreeViewItem> result)
    {
        if (root == null)
            throw new NullReferenceException("root");
        if (result == null)
            throw new NullReferenceException("result");

        result.Clear();

        if (root.children == null)
            return;

        Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
        for (int i = root.children.Count - 1; i >= 0; i--)
            stack.Push(root.children[i]);

        while (stack.Count > 0)
        {
            TreeViewItem current = stack.Pop();
            result.Add(current);

            if (current.hasChildren && current.children[0] != null)
            {
                for (int i = current.children.Count - 1; i >= 0; i--)
                {
                    stack.Push(current.children[i]);
                }
            }
        }
    }

    public VideoDataTreeView(TreeViewState state, MultiColumnHeader multicolumnHeader, UnityEditor.TreeViewExamples.TreeModel<VideoDataTreeElement> model) : base(state, multicolumnHeader, model)
    {
        Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

        // Custom setup
        rowHeight = kRowHeights;
        columnIndexForTreeFoldouts = 0;
        showAlternatingRowBackgrounds = true;
        showBorder = true;
        customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
        extraSpaceBeforeIconAndLabel = kToggleWidth;
        multicolumnHeader.sortingChanged += OnSortingChanged;

        Reload();
    }


    // Note we We only build the visible rows, only the backend has the full tree information. 
    // The treeview only creates info for the row list.
    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {

        var rows = base.BuildRows(root);
        SortIfNeeded(root, rows);

        return rows;
    }

    void OnSortingChanged(MultiColumnHeader multiColumnHeader)
    {
        SortIfNeeded(rootItem, GetRows());
    }

    void SortIfNeeded(TreeViewItem root, IList<TreeViewItem> rows)
    {
        if (rows.Count <= 1)
            return;

        if (multiColumnHeader.sortedColumnIndex == -1)
        {
            return; // No column to sort for (just use the order the data are in)
        }

        // Sort the roots of the existing tree items
        SortByMultipleColumns();
        TreeToList(root, rows);
        Repaint();
    }

    void SortByMultipleColumns()
    {
        var sortedColumns = multiColumnHeader.state.sortedColumns;

        if (sortedColumns.Length == 0)
            return;

        var myTypes = rootItem.children.Cast<UnityEditor.TreeViewExamples.TreeViewItem<VideoDataTreeElement>>();
        var orderedQuery = InitialOrder(myTypes, sortedColumns);
        for (int i = 1; i < sortedColumns.Length; i++)
        {
            SortOption sortOption = m_SortOptions[sortedColumns[i]];
            bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

            switch (sortOption)
            {
                case SortOption.Narration:
                    orderedQuery = orderedQuery.ThenBy(l => l.data.name, ascending);
                    break;
                case SortOption.Occurences:
                    orderedQuery = orderedQuery.ThenBy(l => l.data.display_occurences_count, ascending);
                    break;
                             case SortOption.Verb:
                orderedQuery = orderedQuery.ThenBy(l => l.data.display_verb, ascending);
                break;
            case SortOption.Noun:
                orderedQuery = orderedQuery.ThenBy(l => l.data.display_noun, ascending);
                break;
        }
        }

        rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
    }

    TreeViewItem<VideoDataTreeElement> currentItem;
    protected override void SelectionChanged(IList<int> selectedIds)
    {
        List<TreeViewItem> selectedItems = new List<TreeViewItem>(FindRows(selectedIds));
        if(selectedItems == null || selectedItems.Count != 1)
        {
            currentItem = null;
        }
        else
        {
            currentItem = (TreeViewItem<VideoDataTreeElement>)selectedItems[0];
        }
    }

    void SelectItem(int id)
    {
        List<int> list = new List<int>();
        list.Add(id);
        SetSelection(list, TreeViewSelectionOptions.FireSelectionChanged);
    }
    void NamedLabel(string name, string content)
    {
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        GUILayout.Label(name+":", EditorStyles.centeredGreyMiniLabel,GUILayout.ExpandWidth(false));
        GUILayout.Label(content,EditorStyles.label, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
    }
    void CategoryGUI(TreeViewItem<VideoDataTreeElement> item)
    {
        GUILayout.Label("Category item", EditorStyles.boldLabel);
    }

    void NarrationGUI(TreeViewItem<VideoDataTreeElement> item)
    {
        GUILayout.Label("Narration item", EditorStyles.boldLabel);
        GUILayout.BeginVertical();
        NamedLabel("Narration", item.data.name);
        NamedLabel("Occurences", item.data.display_occurences_count.ToString());
        NamedLabel("Noun", item.data.display_noun);
        NamedLabel("Verb", item.data.display_verb);
        if(GUILayout.Button("Play all clips"))
        {
            VideoPlayerWindow.GetWindow<VideoPlayerWindow>().PlayClips(item.data.occurences);
        }
        GUILayout.EndVertical();
    }

    void ClipGUI(TreeViewItem<VideoDataTreeElement> item)
    {
        TrainActionLabel d = item.data.occurences[0]; 
        GUILayout.Label("Clip item", EditorStyles.boldLabel);
        GUILayout.BeginVertical();
        NamedLabel("Video id", d.video_id);
        NamedLabel("Narration", d.narration);
        NamedLabel("Start frame", d.start_frame.ToString());
        NamedLabel("Stop frame", d.stop_frame.ToString());
        NamedLabel("Duration", string.Format("{0:0.##} seconds", ((float)(d.stop_frame-d.start_frame)/30f)));
        if(GUILayout.Button("Play Clip"))
        {
            VideoPlayerWindow.GetWindow<VideoPlayerWindow>().PlayClip(d);

        }
        GUILayout.EndVertical();
    }

    public void VideoDataInfo(Rect rect)
    {
        GUILayout.BeginArea(rect, EditorStyles.helpBox);
        if(currentItem != null)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            if(currentItem.depth == 2)
            {
                if(GUILayout.Button(currentItem.parent.parent.displayName, EditorStyles.label, GUILayout.ExpandWidth(false)))
                {
                    SelectItem(currentItem.parent.parent.id);
                }
            GUILayout.Label("/", GUILayout.ExpandWidth(false));
                
            }
            if(currentItem.depth != 0)
            {
                if(GUILayout.Button((currentItem.parent).displayName, EditorStyles.label, GUILayout.ExpandWidth(false)))
                {                    
                    SelectItem(currentItem.parent.id);
                }
            GUILayout.Label("/", GUILayout.ExpandWidth(false));
            }
            GUILayout.Label(currentItem.displayName, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            switch(currentItem.depth)
            {
                case 0:
                CategoryGUI(currentItem);
                break;
                case 1:
                NarrationGUI(currentItem);
                break;
                case 2:
                ClipGUI(currentItem);
                break;
            }
        }
        GUILayout.EndArea();
    }

    IOrderedEnumerable<UnityEditor.TreeViewExamples.TreeViewItem<VideoDataTreeElement>> InitialOrder(IEnumerable<UnityEditor.TreeViewExamples.TreeViewItem<VideoDataTreeElement>> myTypes, int[] history)
    {
        SortOption sortOption = m_SortOptions[history[0]];
        bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
        switch (sortOption)
        {
            case SortOption.Narration:
                return myTypes.Order(l => l.data.name, ascending);
            case SortOption.Occurences:
                return myTypes.Order(l => l.data.display_occurences_count, ascending);
            case SortOption.Verb:
                return myTypes.Order(l => l.data.display_verb, ascending);
            case SortOption.Noun:
                return myTypes.Order(l => l.data.display_noun, ascending);
        //case SortOption.Value2:
        //    return myTypes.Order(l => l.data.floatValue2, ascending);
        default:
                Assert.IsTrue(false, "Unhandled enum");
                break;
        }

        // default
        return myTypes.Order(l => l.data.name, ascending);
    }



    protected override void RowGUI(RowGUIArgs args)
    {
        var item = (UnityEditor.TreeViewExamples.TreeViewItem<VideoDataTreeElement>)args.item;
        for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                Rect cellRect = args.GetCellRect(i);
            
                CellGUI(cellRect, item, (MyColumns)args.GetColumn(i), ref args);
            }
    }

    void CellGUI(Rect cellRect, UnityEditor.TreeViewExamples.TreeViewItem<VideoDataTreeElement> item, MyColumns column, ref RowGUIArgs args)
    {
        // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
        CenterRectUsingSingleLineHeight(ref cellRect);

        switch (column)
        {
                case MyColumns.Narration:
                {
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                }
                break;

                case MyColumns.Verb:
                {
                    string value = item.data.display_verb;
                    if(item.depth == 0 && showControls)
                    {
                        value = "";
                    }
                        DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    
                    break;
                }
                case MyColumns.Noun:
                {
                    string value = item.data.display_noun;
                    if(item.depth == 0 && showControls)
                    {
                        value = "";
                    }
                    DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                    break;
                }
        case MyColumns.Occurences:
            {
                int occurences = item.data.display_occurences_count;
                string value;
                if(occurences == -1)
                {
                    TrainActionLabel v = item.data.occurences[0];
                    value = string.Format("{0:0.##} seconds", ((float)(v.stop_frame-v.start_frame)/30f));
                }
                else
                {
                    value = occurences.ToString();
                }

                DefaultGUI.LabelRightAligned(cellRect, value, args.selected, args.focused);
                
            }
            break;

        case MyColumns.PlayVideo:
            {
                if(item.depth == 2)
                {
                    if (GUI.Button(cellRect, "Play"))
                    {
                        VideoPlayerWindow.GetWindow<VideoPlayerWindow>().PlayClip(item.data.occurences[0]);
                    }
                }
            }
            break;
        }
    }

    // Rename
    //--------

    protected override bool CanRename(TreeViewItem item)
    {
        return false;
        // Only allow rename if we can show the rename overlay with a certain width (label might be clipped by other columns)
        Rect renameRect = GetRenameRect(treeViewRect, 0, item);
        return renameRect.width > 30;
    }

    protected override void RenameEnded(RenameEndedArgs args)
    {
        // Set the backend name and reload the tree to reflect the new model
        if (args.acceptedRename)
        {
            var element = treeModel.Find(args.itemID);
            element.name = args.newName;
            Reload();
        }
    }

    protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
    {
        Rect cellRect = GetCellRectForTreeFoldouts(rowRect);
        CenterRectUsingSingleLineHeight(ref cellRect);
        return base.GetRenameRect(cellRect, row, item);
    }

// Misc
//--------

protected override bool CanStartDrag(CanStartDragArgs args)
{
    return false;
}

protected override bool CanMultiSelect(TreeViewItem item)
    {
        return false;
    }

    public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
    {
        var columns = new[]
        {
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Narration"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 250,
                minWidth = 250,
                autoResize = false,
                allowToggleVisibility = false
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Verb", "In sed porta ante. Nunc et nulla mi."),
                headerTextAlignment = TextAlignment.Right,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 100,
                minWidth = 100,
                autoResize = true
            },
                  new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Noun", "In sed porta ante. Nunc et nulla mi."),
                headerTextAlignment = TextAlignment.Right,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 100,
                minWidth = 100,
                autoResize = true
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Occurences", "In sed porta ante. Nunc et nulla mi."),
                headerTextAlignment = TextAlignment.Right,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 75,
                minWidth = 75,
                autoResize = true
            },
            new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Play", "Maecenas congue non tortor eget vulputate."),
                headerTextAlignment = TextAlignment.Right,
                canSort = false,
                width = 50,
                minWidth = 50,
                autoResize = true,
                allowToggleVisibility = true
            }
        };

        Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

        var state = new MultiColumnHeaderState(columns);
        return state;
    }
}

