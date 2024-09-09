using System;
using NoReleaseDate.Common.Runtime.VAwait;
using UnityEditor;

namespace NoReleaseDate.Common.Editor.VAwait
{
    public static class InitVAwaitEditor
    {
        // Ensure class initializer is called whenever scripts recompile
        [InitializeOnLoad]
        public static class PlayModeStateChangedExample
        {
            // Register an event handler when the class is initialized
            static PlayModeStateChangedExample()
            {
                EditorApplication.playModeStateChanged += LogPlayModeState;

                PlayerLoopUpdate.playerLoopUtil ??= new PlayerLoopUpdate();

                PlayerLoopUpdate.playerLoopUtil.getCurrentFrame = GetFrame;
                EditorApplication.update += PlayerLoopUpdate.playerLoopUtil.EditModeRunner;
            }

            private static void LogPlayModeState(PlayModeStateChange state)
            {
                switch (state)
                {
                    case PlayModeStateChange.EnteredPlayMode:
                        EditorApplication.update -= PlayerLoopUpdate.playerLoopUtil.EditModeRunner;
                        EditorApplication.update += PlayerLoopUpdate.playerLoopUtil.EditModeRunner;
                        break;
                    case PlayModeStateChange.ExitingPlayMode:
                        Wait.DestroyAwaits();
                        Wait.playMode = UPlayStateMode.None;
                        PlayerLoopUpdate.playerLoopUtil.getCurrentFrame = GetFrame;
                        EditorApplication.update -= PlayerLoopUpdate.playerLoopUtil.EditModeRunner;
                        break;
                    case PlayModeStateChange.EnteredEditMode:
                        break;
                    case PlayModeStateChange.ExitingEditMode:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }

        private static int GetFrame() => PlayerLoopUpdate.playerLoopUtil.dummyFrame;
    }
}