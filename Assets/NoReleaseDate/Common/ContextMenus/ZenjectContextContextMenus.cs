using UnityEditor;
using Zenject;

namespace NoReleaseDate.Common.ContextMenus
{
    public static class ZenjectContextContextMenus
    {
        [MenuItem("CONTEXT/Context/Find Mono Installers")]
        private static void FindMonoInstallers(MenuCommand command)
        {
            var body = (Context)command.context;

            var monoInstallers = body.GetComponentsInChildren<MonoInstaller>(true);
            
            body.Installers = monoInstallers;
        }
    }
}