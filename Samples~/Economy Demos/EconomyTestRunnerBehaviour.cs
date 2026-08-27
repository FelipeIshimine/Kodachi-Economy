using UnityEngine;
using UnityEngine.UIElements;

namespace KodachiGames.Economy.Samples
{
    [RequireComponent(typeof(UIDocument))]
    public class EconomyTestRunnerBehaviour : MonoBehaviour
    {
        [SerializeReference, TypeSelector] private EconomyTestCase testA;
        [SerializeReference, TypeSelector] private EconomyTestCase testB;
        [SerializeReference, TypeSelector] private EconomyTestCase testC;

        private void OnEnable()  => Rebuild();
        private void OnDisable() => GetComponent<UIDocument>()?.rootVisualElement.Clear();

        [ContextMenu("Rebuild")]
        private void Rebuild()
        {
            var doc = GetComponent<UIDocument>();
            if (doc == null) return;
            var root = doc.rootVisualElement;
            root.Clear();
            testA?.Build(root);
            testB?.Build(root);
            testC?.Build(root);
        }
    }
}
