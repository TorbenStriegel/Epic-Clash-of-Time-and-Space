
using Microsoft.Xna.Framework;

namespace ECTS.Components
{
    /// <summary>
    /// Provides Static GameLoop reference for the Serialization and other cases.
    /// </summary>
    internal static class Global
    {
        internal static GameLoop mGameLoop;
        internal static Color mColor = Color.White;
        internal static bool mPause = false;
    }
}
