using System.Diagnostics;
using System.Reflection;

namespace CustomScreamer.Utils
{
    public class RuntimeInfo
    {
        /// <summary>
        /// The absolute path to the startup directory of this game.
        /// </summary>
        public static string StartupDirectory { get; } = AppContext.BaseDirectory;

        /// <summary>
        /// Returns the absolute path of osu.Framework.dll.
        /// </summary>
        public static string GetFrameworkAssemblyPath()
        {
            var assembly = Assembly.GetAssembly(typeof(RuntimeInfo));
            Debug.Assert(assembly != null);

            return assembly.Location;
        }

        public static Platform OS { get; }

        public static bool IsUnix => OS != Platform.Windows;
        public static bool IsDesktop => OS == Platform.Linux || OS == Platform.macOS || OS == Platform.Windows;
        public static bool IsApple => OS == Platform.macOS;

        static RuntimeInfo()
        {
            if (OperatingSystem.IsWindows())
                OS = Platform.Windows;
            if (OperatingSystem.IsMacOS())
                OS = OS == 0 ? Platform.macOS : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.macOS)}, but is already {Enum.GetName(OS)}");
            if (OperatingSystem.IsLinux())
                OS = OS == 0 ? Platform.Linux : throw new InvalidOperationException($"Tried to set OS Platform to {nameof(Platform.Linux)}, but is already {Enum.GetName(OS)}");

            if (OS == 0)
                throw new PlatformNotSupportedException("Operating system could not be detected correctly.");
        }

        // todo: revisit when we have a way to exclude enum members from naming rules
        public enum Platform
        {
            Windows = 1,
            Linux = 2,
            macOS = 3,
        }
    }
}
