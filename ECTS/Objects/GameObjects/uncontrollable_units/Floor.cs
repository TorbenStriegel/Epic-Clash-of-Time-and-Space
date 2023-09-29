using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units
{
    [DataContract]
    internal sealed class Floor : GameObject
    {
        public Floor(GameLoop gameLoop, int x, int y) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize(x, y);
        }

        private void Initialize(int x, int y)
        {
            mTextureName = "background_tileset";
            mPosition = new Rectangle(x, y, 48, 48);
            mFace = new Rectangle(0, 15, 48, 48);
            mObjectType = ObjectType.Background;
            mObjectState = ObjectState.Idle;
            mLayer = -1;
        }
        public override void ChangeFace(string face)
        {
        }

        public override void Draw()
        {
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, Global.mColor);
        }

        public override void Update(GameTime gameTime)
        {
        }

        // DeSerialization: Call internal initializing methods:
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            //**** Set all attributes that are initialized in this class here:
            // For GameLoop use "mGameLoop = Global.mGameLoop;"
            mGameLoop = Global.mGameLoop;

            //**** Call all functions that initialize attributes here:
            Initialize(0,0);  // Position x,y will be overwrite by serializer anyway

        }
    }
}
