#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Utils
{
    public class AddressSetTool : EditorWindow
    {
        private string prefix = "Game/Icons";
        private bool includeExtension = false;
        private AddressableAssetGroup selectedGroup;
        private string[] groupNames;
        private int selectedGroupIndex = 0;

        [MenuItem("Tools/Addressables/Set Address from Selection")]
        public static void ShowWindow()
        {
            var window = GetWindow<AddressSetTool>("Set Addressables");
            window.InitializeFromSelection();
        }

        private void OnGUI()
        {
            // Mô tả công cụ ở trên cùng cửa sổ
            EditorGUILayout.HelpBox(
                "Dùng công cụ này để đặt địa chỉ (Address) cho các asset Addressable đã chọn.\n" +
                "- Đặt prefix chung cho tất cả.\n" +
                "- Tự động chọn nhóm (Group) nếu các file cùng nhóm.\n" +
                "- Có thể chọn lại nhóm theo ý muốn.\n" +
                "- Tùy chọn giữ hoặc bỏ phần mở rộng file trong address.\n\n" +
                "Giúp quản lý asset Addressable theo file riêng biệt thay vì cả folder, tránh load thừa.",
                MessageType.Info);

            GUILayout.Space(10);

            GUILayout.Label("Set Addressables Address", EditorStyles.boldLabel);

            prefix = EditorGUILayout.TextField("Prefix (e.g. Game/Icons)", prefix);
            includeExtension = EditorGUILayout.Toggle("Include File Extension", includeExtension);

            if (groupNames != null && groupNames.Length > 0)
            {
                selectedGroupIndex = EditorGUILayout.Popup("Addressable Group", selectedGroupIndex, groupNames);
                selectedGroup = AddressableAssetSettingsDefaultObject.Settings.groups[selectedGroupIndex];
            }

            if (GUILayout.Button("Apply to Selected Files"))
            {
                ApplyAddressablesToSelection();
            }
        }

        private void InitializeFromSelection()
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null) return;

            var selectedObjects = Selection.objects;
            if (selectedObjects == null || selectedObjects.Length == 0) return;

            // Detect common prefix
            string[] prefixes = selectedObjects
                .Select(obj =>
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsValidFolder(path)) return null;
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    var entry = settings.FindAssetEntry(guid);
                    if (entry == null || string.IsNullOrEmpty(entry.address)) return null;

                    var address = entry.address.Replace("\\", "/");
                    var name = Path.GetFileNameWithoutExtension(path);
                    return address.EndsWith(name)
                        ? address.Substring(0, address.Length - name.Length).TrimEnd('/')
                        : null;
                })
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToArray();

            if (prefixes.Length == 1)
            {
                prefix = prefixes[0];
            }

            // Detect common group
            var groups = selectedObjects
                .Select(obj =>
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    if (AssetDatabase.IsValidFolder(path)) return null;
                    var guid = AssetDatabase.AssetPathToGUID(path);
                    var entry = settings.FindAssetEntry(guid);
                    return entry?.parentGroup;
                })
                .Where(g => g != null)
                .Distinct()
                .ToArray();

            // Get all group names
            groupNames = settings.groups.Select(g => g.Name).ToArray();

            if (groups.Length == 1)
            {
                selectedGroup = groups[0];
            }
            else
            {
                selectedGroup = settings.DefaultGroup;
            }

            selectedGroupIndex = settings.groups.IndexOf(selectedGroup);
        }

        private void ApplyAddressablesToSelection()
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (selectedGroup == null) selectedGroup = settings.DefaultGroup;

            var selectedObjects = Selection.objects;
            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select assets to set addressable paths.", "OK");
                return;
            }

            foreach (var obj in selectedObjects)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path)) continue;

                var guid = AssetDatabase.AssetPathToGUID(path);
                var entry = settings.CreateOrMoveEntry(guid, selectedGroup);

                var fileName = includeExtension
                    ? Path.GetFileName(path)
                    : Path.GetFileNameWithoutExtension(path);

                entry.address = $"{prefix}/{fileName}";
                Debug.Log($"Set Addressable: {entry.address} in Group: {selectedGroup.Name}");
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", "Addressables updated.", "OK");
        }
    }
}
#endif