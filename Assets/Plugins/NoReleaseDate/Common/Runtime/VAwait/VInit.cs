using UnityEngine;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    public static class VInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            PlayerLoopUpdate.playerLoopUtil.getCurrentFrame = GetFrame;
            Wait.playMode = UPlayStateMode.PlayMode;
            Wait.StartAwait();
        }

        private static int GetFrame() => Time.frameCount;
    }
}