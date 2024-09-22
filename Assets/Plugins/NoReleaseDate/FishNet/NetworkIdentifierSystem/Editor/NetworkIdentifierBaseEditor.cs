using NoReleaseDate.FishNet.NetworkIdentifierSystem.Runtime;

namespace NoReleaseDate.FishNet.NetworkIdentifierSystem.Editor
{
    [UnityEditor.CustomEditor(typeof(NetworkIdentifierBase), true)]
    public class NetworkIdentifierBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var networkIdentifier = (NetworkIdentifierBase)target;

            // Draw the ID field.
            UnityEditor.EditorGUILayout.LabelField("ID", networkIdentifier.id.ToString());

            base.OnInspectorGUI();
        }
    }
}