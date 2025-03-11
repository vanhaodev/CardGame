#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace World.Card
{
    [CreateAssetMenu(fileName = "CardTemplateListSO", menuName = "World/Card/CardTemplateListSO")]
    public class CardTemplateListSO : ScriptableObject
    {
        public SerializedDictionary<ushort, AssetReferenceT<CardTemplateSO>> CardTemplateRefs; // ID → Addressables Reference

#if UNITY_EDITOR
        [ContextMenu("Load All Card Templates")]
        public void LoadAllTemplates()
        {
            string templatesFolder = GetTemplatesFolder();
            if (string.IsNullOrEmpty(templatesFolder)) return;

            CardTemplateRefs.Clear();
            Dictionary<string, string> nameToPath = new();
            string[] guids = AssetDatabase.FindAssets("t:CardTemplateSO", new[] { templatesFolder });

            int duplicateCount = 0;
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                CardTemplateSO template = AssetDatabase.LoadAssetAtPath<CardTemplateSO>(assetPath);

                if (!IsValidTemplate(template, assetPath)) continue;

                ushort id = template.Info.Id;
                string name = template.Name;

                // ✅ Tạo AssetReference thay vì lưu key string
                AssetReferenceT<CardTemplateSO> assetRef = new AssetReferenceT<CardTemplateSO>(guid);
                
                // Check trùng ID
                if (CardTemplateRefs.ContainsKey(id))
                {
                    LogDuplicateError($"ID: {id}", assetPath, CardTemplateRefs[id].AssetGUID);
                    duplicateCount++;
                }
                else
                {
                    CardTemplateRefs[id] = assetRef;
                }

                // Check trùng Name
                if (nameToPath.ContainsKey(name))
                {
                    LogDuplicateError($"Infor.Name: {name}", assetPath, nameToPath[name]);
                    duplicateCount++;
                }
                else
                {
                    nameToPath[name] = assetPath;
                }
            }

            LogLoadResult(templatesFolder, duplicateCount);
        }

        private string GetTemplatesFolder()
        {
            string listSOPath = AssetDatabase.GetAssetPath(this);
            string parentFolder = Path.GetDirectoryName(listSOPath);
            string templatesFolder = Path.Combine(parentFolder, "Templates");

            if (!Directory.Exists(templatesFolder))
            {
                Debug.LogError($"❌ Folder '{templatesFolder}' does not exist. Please create it!");
                return null;
            }
            return templatesFolder;
        }

        private bool IsValidTemplate(CardTemplateSO template, string assetPath)
        {
            if (template == null || template.Info == null)
            {
                Debug.LogError($"❌ Invalid CardTemplateSO file: '{assetPath}'. Missing Info or data.");
                return false;
            }
            return true;
        }

        private void LogDuplicateError(string type, string newKey, string existingKey)
        {
            Debug.LogError($"❌ Duplicate {type}\n" +
                           $"🔹 New file: '{newKey}'\n" +
                           $"🔹 Existing file: '{existingKey}'");
        }

        private void LogLoadResult(string templatesFolder, int duplicateCount)
        {
            Debug.Log($"✅ Loaded {CardTemplateRefs.Count} card templates from '{templatesFolder}'.");
            if (duplicateCount > 0)
            {
                Debug.LogError($"⚠️ Found {duplicateCount} duplicate issues.");
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}