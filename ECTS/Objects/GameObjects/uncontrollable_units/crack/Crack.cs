using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.Objects.GameObjects.uncontrollable_units.crack
{
    [DataContract]
    internal sealed class Crack : GameObject
    {
        /// <summary>
        /// Decorative grass, cannot be harvested.
        /// </summary>
        public Crack(GameLoop gameLoop) : base(gameLoop)
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
            IsInteracting = true;
            mHealth = 100;                   // less health, less resources
            mObjectType = ObjectType.Tree;
            mTextureName = "Objects/KI_Spawn";
            mSpeed = 0;
            mFace = new Rectangle(0, 0, 794, 512);
            mIsMarked = false;
            mIsMarkable = false;
            mObjectLifetime = 255;          // disappears after 255s (new day)
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
            if (Health <= 0)
            {
                ObjectAge = ObjectLifetime;
            }
        }
        public override void Draw()
        {
            if (mSpriteTexture == null)
            {
                mGameLoop.ObjectManager.LoadContent();
            }
            
            mSpriteColor = Global.mColor;
            mSpriteColor.A = (byte)(mHealth / 100 * 255);
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace, mSpriteColor);
        }
    }
}
