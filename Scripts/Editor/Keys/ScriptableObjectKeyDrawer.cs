using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KodachiGames.Economy.Editor
{
    [CustomPropertyDrawer(typeof(WalletKey))]
    public class WalletKeyDrawer : ScriptableObjectKeyDrawer<WalletKey> { }

    [CustomPropertyDrawer(typeof(InventoryKey))]
    public class InventoryKeyDrawer : ScriptableObjectKeyDrawer<InventoryKey> { }

    public abstract class ScriptableObjectKeyDrawer<T> : PropertyDrawer where T : ScriptableObject
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyCopy = property.Copy();

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems    = Align.Center;

            var fieldLabel = new Label(property.displayName);
            fieldLabel.style.minWidth       = 120;
            fieldLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            container.Add(fieldLabel);

            var keyNameLabel = new Label();
            keyNameLabel.style.flexGrow                = 1;
            keyNameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            keyNameLabel.style.unityTextAlign          = TextAnchor.MiddleLeft;
            keyNameLabel.style.marginLeft              = 4;
            container.Add(keyNameLabel);

            var deleteButton = new Button { text = "✕" };
            deleteButton.style.width        = 22;
            deleteButton.style.height       = 20;
            deleteButton.style.paddingLeft  = 0;
            deleteButton.style.paddingRight = 0;
            deleteButton.style.marginRight  = 2;
            deleteButton.style.color        = new Color(0.9f, 0.4f, 0.4f);
            deleteButton.clicked += () =>
            {
                var asset = propertyCopy.objectReferenceValue as T;
                if (asset == null) return;

                if (!EditorUtility.DisplayDialog(
                    $"Delete {typeof(T).Name}",
                    $"Delete \"{asset.name}\"?\n\nThis will remove the asset from the project. Any references to it will become missing.",
                    "Delete", "Cancel"))
                    return;

                propertyCopy.objectReferenceValue = null;
                propertyCopy.serializedObject.ApplyModifiedProperties();
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
                RefreshLabel(propertyCopy, keyNameLabel);
                RefreshDeleteButton(propertyCopy, deleteButton);
            };
            container.Add(deleteButton);

            var dropdownButton = new Button { text = "▾" };
            dropdownButton.style.width        = 22;
            dropdownButton.style.height       = 20;
            dropdownButton.style.paddingLeft  = 0;
            dropdownButton.style.paddingRight = 0;
            dropdownButton.clicked += () => KeySelectorDropdown.Show(
                dropdownButton.worldBound,
                typeof(T),
                propertyCopy,
                () =>
                {
                    propertyCopy.serializedObject.Update();
                    RefreshLabel(propertyCopy, keyNameLabel);
                    RefreshDeleteButton(propertyCopy, deleteButton);
                });
            container.Add(dropdownButton);

            RefreshLabel(propertyCopy, keyNameLabel);
            RefreshDeleteButton(propertyCopy, deleteButton);
            return container;
        }

        private static void RefreshLabel(SerializedProperty property, Label label)
        {
            var asset         = property.objectReferenceValue as T;
            label.text        = asset != null ? asset.name : "(none)";
            label.style.color = asset != null
                ? new Color(0.7f, 0.9f, 0.7f)
                : new Color(0.9f, 0.4f, 0.4f);
        }

        private static void RefreshDeleteButton(SerializedProperty property, Button button)
        {
            button.SetEnabled(property.objectReferenceValue != null);
        }
    }
}
