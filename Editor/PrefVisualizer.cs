using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
public class PrefsVisualizer : EditorWindow
{
    private Vector2 m_PlayerScroll;
    private Vector2 m_EditorScroll;
    private string m_SearchFilter = "";
    private string m_LastSearchFilter = "";

    // Visibility toggles
    private bool m_ShowPlayerPrefs = true;
    private bool m_ShowEditorPrefs = true;

    private readonly List<PrefItem> m_RawPlayerPrefs = new List<PrefItem>();
    private readonly List<PrefItem> m_RawEditorPrefs = new List<PrefItem>();
    
    private readonly List<PrefItem> m_FilteredPlayerPrefs = new List<PrefItem>();
    private readonly List<PrefItem> m_FilteredEditorPrefs = new List<PrefItem>();

    private struct PrefItem
    {
        public string key;
        public string value;
    }

    [MenuItem("GrygTools/Prefs Visualizer")]
    public static void ShowWindow()
    {
        GetWindow<PrefsVisualizer>("Prefs Visualizer");
    }

    private void OnEnable()
    {
        RefreshPrefsFromFileSystem();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        
        // Search filter row
        EditorGUI.BeginChangeCheck();
        m_SearchFilter = EditorGUILayout.TextField("Filter Keys:", m_SearchFilter);
        if (EditorGUI.EndChangeCheck() || m_SearchFilter != m_LastSearchFilter)
        {
            ApplySearchFilter();
            m_LastSearchFilter = m_SearchFilter;
        }
        
        GUILayout.Space(5);

        // Visibility Toggles Row
        EditorGUILayout.BeginHorizontal();
        m_ShowPlayerPrefs = EditorGUILayout.ToggleLeft("Show PlayerPrefs", m_ShowPlayerPrefs, GUILayout.Width(150));
        m_ShowEditorPrefs = EditorGUILayout.ToggleLeft("Show EditorPrefs", m_ShowEditorPrefs, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("Force Refresh From Disk", GUILayout.Height(30)))
        {
            RefreshPrefsFromFileSystem();
        }

        GUILayout.Space(10);

        // Calculate dynamic layouts based on active columns
        int activeColumns = 0;
        if (m_ShowPlayerPrefs) activeColumns++;
        if (m_ShowEditorPrefs) activeColumns++;

        if (activeColumns == 0)
        {
            EditorGUILayout.HelpBox("Please select at least one Preference type above to display data.", MessageType.Info);
            return;
        }

        // Subtract structural spacing padding
        float columnWidth = (position.width / activeColumns) - (6 * activeColumns);

        EditorGUILayout.BeginHorizontal();

        // Left Column: PlayerPrefs
        if (m_ShowPlayerPrefs)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(columnWidth));
            EditorGUILayout.LabelField($"PlayerPrefs ({m_FilteredPlayerPrefs.Count})", EditorStyles.boldLabel);
            m_PlayerScroll = EditorGUILayout.BeginScrollView(m_PlayerScroll);
            DrawPrefsList(m_FilteredPlayerPrefs, columnWidth, true);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        // Right Column: EditorPrefs
        if (m_ShowEditorPrefs)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(columnWidth));
            EditorGUILayout.LabelField($"EditorPrefs ({m_FilteredEditorPrefs.Count})", EditorStyles.boldLabel);
            m_EditorScroll = EditorGUILayout.BeginScrollView(m_EditorScroll);
            DrawPrefsList(m_FilteredEditorPrefs, columnWidth, false);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawPrefsList(List<PrefItem> items, float allocatedColumnWidth, bool isPlayerPrefs)
    {
        if (items.Count == 0)
        {
            EditorGUILayout.LabelField("No preferences found.", EditorStyles.miniLabel);
            return;
        }

        // Reserve 25px for the scrollbar, and 24px for the delete button if applicable
        float deleteBtnWidth = isPlayerPrefs ? 24f : 0f;
        float usableRowWidth = allocatedColumnWidth - 25f - deleteBtnWidth; 
        
        float keyWeightedWidth = usableRowWidth * 0.30f;
        float valueWeightedWidth = usableRowWidth * 0.70f;
        
        GUIStyle valueStyle = new GUIStyle(EditorStyles.selectionRect)
        {
            wordWrap = true
        };

        // Red cross button styling
        GUIStyle deleteButtonStyle = new GUIStyle(GUI.skin.button);
        deleteButtonStyle.normal.textColor = Color.red;
        deleteButtonStyle.fontStyle = FontStyle.Bold;

        for (int i = 0; i < items.Count; i++)
        {
            float dynamicHeight = Mathf.Max(22, valueStyle.CalcHeight(new GUIContent(items[i].value), valueWeightedWidth));

            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height(dynamicHeight));
            
            // Key & Value layout distribution
            EditorGUILayout.LabelField(items[i].key, EditorStyles.wordWrappedLabel, GUILayout.Width(keyWeightedWidth));
            EditorGUILayout.SelectableLabel(items[i].value, valueStyle, GUILayout.Width(valueWeightedWidth), GUILayout.Height(dynamicHeight - 4));
            
            // Delete button rendering (Only populated inside PlayerPrefs)
            if (isPlayerPrefs)
            {
                if (GUILayout.Button("X", deleteButtonStyle, GUILayout.Width(20), GUILayout.Height(18)))
                {
                    // Confirmation dialog popup prevents accidental workflow loss
                    if (EditorUtility.DisplayDialog("Delete PlayerPref?", $"Are you sure you want to permanently erase key: '{items[i].key}'?", "Delete", "Cancel"))
                    {
                        PlayerPrefs.DeleteKey(items[i].key);
                        PlayerPrefs.Save(); // Push changes immediately to disk
                        
                        DelayedRefreshPrefsFromFileSystem();
                        // Instantly reload arrays to present clean UI state
                        GUIUtility.ExitGUI(); // Prevent layout desync errors on immediate destruction passes
                    }
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
    
    private async void DelayedRefreshPrefsFromFileSystem()
    {
        await Task.Delay(10);
        RefreshPrefsFromFileSystem();
        Repaint();
    }

    private void RefreshPrefsFromFileSystem()
    {
        m_RawPlayerPrefs.Clear();
        m_RawEditorPrefs.Clear();

#if UNITY_EDITOR_WIN
        FetchWindowsPrefs();
#elif UNITY_EDITOR_OSX
        FetchMacPrefs();
#endif

        ApplySearchFilter();
    }

    private void ApplySearchFilter()
    {
        m_FilteredPlayerPrefs.Clear();
        m_FilteredEditorPrefs.Clear();

        bool hasFilter = !string.IsNullOrEmpty(m_SearchFilter);

        for (int i = 0; i < m_RawPlayerPrefs.Count; i++)
        {
            if (!hasFilter || m_RawPlayerPrefs[i].key.Contains(m_SearchFilter, StringComparison.OrdinalIgnoreCase))
                m_FilteredPlayerPrefs.Add(m_RawPlayerPrefs[i]);
        }

        for (int i = 0; i < m_RawEditorPrefs.Count; i++)
        {
            if (!hasFilter || m_RawEditorPrefs[i].key.Contains(m_SearchFilter, StringComparison.OrdinalIgnoreCase))
                m_FilteredEditorPrefs.Add(m_RawEditorPrefs[i]);
        }
    }

#if UNITY_EDITOR_WIN
    private void FetchWindowsPrefs()
    {
        string editorPlayerPrefsPath = $@"Software\Unity\UnityEditor\{Application.companyName}\{Application.productName}";
        ReadRegistryKey(editorPlayerPrefsPath, m_RawPlayerPrefs);

        if (m_RawPlayerPrefs.Count == 0)
        {
            string standalonePath = $@"Software\{Application.companyName}\{Application.productName}";
            ReadRegistryKey(standalonePath, m_RawPlayerPrefs);
        }

        string editorPath = $@"Software\Unity Technologies\Unity Editor 5.x";
        ReadRegistryKey(editorPath, m_RawEditorPrefs);
    }

    private void ReadRegistryKey(string path, List<PrefItem> targetList)
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
        {
            if (key != null)
            {
                foreach (string valueName in key.GetValueNames())
                {
                    string cleanKey = valueName;
                    int lastUnderscoreIndex = valueName.LastIndexOf('_');
                    if (lastUnderscoreIndex > 0)
                    {
                        cleanKey = valueName[..lastUnderscoreIndex];
                    }

                    object val = key.GetValue(valueName);
                    string displayValue = val != null ? val.ToString() : "null";

                    if (val is byte[] bytes)
                    {
                        displayValue = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
                    }

                    targetList.Add(new PrefItem { key = cleanKey, value = displayValue });
                }
            }
        }
    }
#endif

#if UNITY_EDITOR_OSX
    private void FetchMacPrefs()
    {
        string playerPlist = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"Library/Preferences/unity.{Application.companyName}.{Application.productName}.plist");
        ParseMacPlist(playerPlist, rawPlayerPrefs);

        string editorPlist = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Preferences/com.unity3d.UnityEditor5.x.plist");
        ParseMacPlist(editorPlist, rawEditorPrefs);
    }

    private void ParseMacPlist(string path, List<PrefItem> targetList)
    {
        if (!File.Exists(path)) return;

        try
        {
            System.Diagnostics.Process.Start("plutil", $"-convert xml1 \"{path}\"").WaitForExit();
            string[] lines = File.ReadAllLines(path);
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("<key>"))
                {
                    string key = lines[i].Replace("<key>", "").Replace("</key>", "").Trim();
                    string value = "Binary/Complex Data";

                    if (i + 1 < lines.Length)
                    {
                        string nextLine = lines[i + 1];
                        if (nextLine.Contains("<string>")) value = nextLine.Replace("<string>", "").Replace("</string>", "").Trim();
                        else if (nextLine.Contains("<integer>")) value = nextLine.Replace("<integer>", "").Replace("</integer>", "").Trim();
                        else if (nextLine.Contains("<true/>")) value = "True";
                        else if (nextLine.Contains("<false/>")) value = "False";
                    }
                    targetList.Add(new PrefItem { key = key, value = value });
                }
            }
            System.Diagnostics.Process.Start("plutil", $"-convert binary1 \"{path}\"");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse plist: {e.Message}");
        }
    }
#endif
}