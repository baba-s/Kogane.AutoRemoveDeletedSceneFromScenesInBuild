using System.IO;
using System.Linq;
using UnityEditor;

namespace Kogane.Internal
{
    /// <summary>
    /// Deleted なシーンを Build Settings の Scenes In Build から自動で削除するエディタ拡張
    /// </summary>
    internal sealed class AutoRemoveDeletedSceneFromScenesInBuild : AssetPostprocessor
    {
        //==============================================================================
        // 関数(static)
        //==============================================================================
        /// <summary>
        /// アセットがインポートされた時などに呼び出されます
        /// </summary>
        private static void OnPostprocessAllAssets
        (
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            const string path = "ProjectSettings/EditorBuildSettings.asset";

            var isChangedScenes = ArrayUtility.Contains( importedAssets, path );

            if ( !isChangedScenes ) return;

            // Build Settings では Deleted なシーンかどうかは File.Exists で確認している
            // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/BuildPlayerSceneTreeView.cs
            var newScenes = EditorBuildSettings.scenes
                    .Where( x => File.Exists( x.path ) )
                    .ToArray()
                ;

            if ( newScenes.Length == EditorBuildSettings.scenes.Length ) return;

            EditorBuildSettings.scenes = newScenes;
            AssetDatabase.SaveAssets();
        }
    }
}