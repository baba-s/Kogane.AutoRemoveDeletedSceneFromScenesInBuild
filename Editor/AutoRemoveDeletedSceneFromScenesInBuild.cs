using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Kogane.Internal
{
    /// <summary>
    /// Deleted なシーンを Build Settings の Scenes In Build から自動で削除するエディタ拡張
    /// </summary>
    internal sealed class AutoRemoveDeletedSceneFromScenesInBuild
        : AssetPostprocessor,
          IPreprocessBuildWithReport
    {
        //==============================================================================
        // プロパティ
        //==============================================================================
        public int callbackOrder => 0;

        //==============================================================================
        // 関数
        //==============================================================================
        /// <summary>
        /// ビルドを開始した時に呼び出されます
        /// </summary>
        void IPreprocessBuildWithReport.OnPreprocessBuild( BuildReport report )
        {
            Remove();
        }

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

            var isChangedScenes =
                    ArrayUtility.Contains( importedAssets, path ) ||
                    deletedAssets.Any( x => x.EndsWith( ".unity" ) )
                ;

            if ( !isChangedScenes ) return;

            Remove();
        }

        /// <summary>
        /// Deleted なシーンを削除します
        /// </summary>
        private static void Remove()
        {
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