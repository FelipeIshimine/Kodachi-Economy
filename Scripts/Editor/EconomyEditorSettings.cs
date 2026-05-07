using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KodachiGames.Economy.Editor
{
    public class EconomyEditorSettings : ScriptableSingleton<EconomyEditorSettings>
    {
        [SerializeField] private string keyRootFolder = "Assets/ScriptableObjects";
        [SerializeField] private List<TypeFolderEntry> typeOverrides = new();

        [Serializable]
        public class TypeFolderEntry
        {
            public string typeName;
            public string folder;
        }

        public string GetFolderForType(Type type)
        {
            foreach (var entry in typeOverrides)
                if (entry.typeName == type.Name) return entry.folder;
            return $"{keyRootFolder}/{type.Name}";
        }

        [MenuItem("Tools/Kodachi/Economy Settings")]
        public static void ShowSettings() => Selection.activeObject = instance;
    }
}
