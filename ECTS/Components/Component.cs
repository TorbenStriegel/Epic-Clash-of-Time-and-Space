using Microsoft.Xna.Framework.Graphics;

namespace ECTS.Components
{
    public abstract class Component // Basis for components in the program (e.g. buttons and sliders)
    {
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Update();
    }
}
