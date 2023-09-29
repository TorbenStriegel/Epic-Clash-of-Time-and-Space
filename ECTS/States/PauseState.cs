using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.States
{
    internal sealed class PauseState : State
    {
        public PauseState(GameLoop gameLoop) : base(gameLoop)
        {
            Global.mColor.R = 50;
            Global.mColor.G = 50;
            Global.mColor.B = 50;
        }

        public override void Update(GameTime gameTime)
        {

        }
        public override void Draw()
        {
            mGameLoop.RenderManager.DrawVisibleElements();
        }

        public override void DrawGui(GameTime gameTime)
        {
            mGameLoop.RenderManager.DrawGui();
        }
    }
}
