using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.vegetation
{
    [DataContract]
    internal sealed class Grass1: GameObject
    {
        /// <summary>
        /// Decorative grass, cannot be harvested.
        /// </summary>
        public Grass1(GameLoop gameLoop) : base(gameLoop)
        // initialization (except for game loop) has to be done inside of the Initialize function,
        // which can be used by the serializer too.
        {
            mGameLoop = gameLoop;
            Initialize();



        }
        private void Initialize()
        {

            mIsColliding = false;
            mIsActing = false;
            IsInteracting = false;
            mHealth = 0;                   // less health, less resources
            mObjectType = ObjectType.Background;
            mTextureName = "Objects/trees";
            mSpeed = 0;
            mFace = new Rectangle(257, 49, 15, 15);
            mIsMarked = false;
            mIsMarkable = false;
        }
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            mGameLoop = Global.mGameLoop;
            Initialize();
        }
        public override void ChangeFace(string face)
        {

        }
        public override void Update(GameTime gameTime)
        {
            //
        }
        public override void Draw()
        {
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, Global.mColor);
        }

    }
}
