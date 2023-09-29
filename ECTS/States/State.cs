using Microsoft.Xna.Framework;

namespace ECTS.States
{
    /// <summary>
    /// States are for managing the various scenarios in which the program can be located. (Menu, In game)
    /// </summary>
    public abstract class State
    {
        protected readonly GameLoop mGameLoop;

        protected State(GameLoop game)
        {
            mGameLoop = game;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw();
        public abstract void DrawGui(GameTime gameTime);

    }
}