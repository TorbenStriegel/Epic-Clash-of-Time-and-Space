using System;

namespace ECTS
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            using var game = new GameLoop();
            game.Run();
        }
    }
#endif
}
