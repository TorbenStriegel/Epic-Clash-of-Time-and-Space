using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ECTS.Components;
using ECTS.Objects.GameObjects.uncontrollable_units.dna;
using Microsoft.Xna.Framework;


namespace ECTS.Objects.GameObjects.controllable_units
{
    [DataContract]
    internal sealed class MonsterPig: GameObject
    {
        private AnimationManager AnimationManager { get; set; }
        private bool mDnaDropped;
        /// <summary>
        /// Monster of type pig.
        /// </summary>
        public MonsterPig(GameLoop gameLoop) : base(gameLoop)
        {
            mGameLoop = gameLoop;
            Initialize();                               // initialization (except for game loop) has to be done inside of the Initialize function, which is used by the serializer too.
        }

        private void Initialize()
        {
            AnimationManager = new AnimationManager();
            mTextureName = "Player/Monster";
            mFace.X = 287; mFace.Y = 20; mFace.Width = 48; mFace.Height = 52;

            mObjectType = ObjectType.Enemy;
            mObjectTargetList = new List<GameObject>();

            mIsColliding = false;
            mIsActing = true;
            IsInteracting = true;
            mIsMarked = false;
            mIsMarkable = true;

            mSpeed = 2;

            mRange = 300;

            mAttackStrength = 30;
            mAttackFrequency = 5;
            mStrikingDistance = 70;
            mDefenseStrength = 30;
        }

        // DeSerialization: Call internal initializing methods:
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            //**** Set all attributes that are initialized in this class here:
            mGameLoop = Global.mGameLoop;

            //**** Call all functions that initialize attributes here:
            Initialize();
        }

        public override void ChangeFace(string face)
        {
            if (face.Equals("Front"))
            {
                mFace.Y = 20;
            }

            if (face.Equals("Left"))
            {
                mFace.Y = 93;
            }

            if (face.Equals("Right"))
            {
                mFace.Y = 165;
            }

            if (face.Equals("Back"))
            {
                mFace.Y = 236;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Health <= 0)
            {
                if (IsMarked)
                {
                    mGameLoop.ObjectManager.DataManager.MarkedEnemiesObjects.Remove(this);
                }

                // DROP DNA
                if (!mDnaDropped)
                {
                    var r = new Random();
                    if (r.Next(0, 9) < 2)
                    {
                        mGameLoop.RenderManager.SoundManager.PlaySound("dna_appears");
                        mGameLoop.ObjectManager.mTempTree.Add(new Dna(mGameLoop)
                            { Position = new Rectangle(Position.X, Position.Y, 33, 33) });
                    }
                    mDnaDropped = true;
                }
                ObjectAge = ObjectLifetime;
                mGameLoop.ObjectManager.DataManager.mKilledEnemies += 1;
                mGameLoop.Settings.mKilledEnemies += 1;
            }

            // STOP Fighting / Destroying if CurrentTargetObject dead.
            if (mCurrentTargetObject != null && State != ObjectState.Moving && mCurrentTargetObject.Health <= 0)
            {
                State = ObjectState.Idle;
            }

            if (UnderAttack && TimeSinceAttack < mMaxTimeSinceAttack)
            {
                TimeSinceAttack += (float)gameTime.ElapsedGameTime.TotalSeconds; 
            }
            else if (TimeSinceAttack >= mMaxTimeSinceAttack)
            { UnderAttack = false;
                TimeSinceAttack = 0;
            }
            
            if (mObjectState == ObjectState.Idle)
            {
                mTextureName = "Player/Monster";
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);

            } else if (mObjectState == ObjectState.Moving)
            {
                mTextureName = "Player/Monster";
                AnimationManager.Animate(gameTime, this, 48, 3, 0f, 287);

            } else if (mObjectState == ObjectState.Destroying)
            {
                mTextureName = "Player/monsterpig_fight_destroy_animation";
                if (AnimationManager.Animate(gameTime, this, 48, 6, 1/AttackFrequency, 287))
                {
                    mGameLoop.ObjectManager.ActionManager.Destroy.AxeStroke(this, mCurrentTargetObject);
                }
            } else if (mObjectState == ObjectState.Fighting)
            {
                mTextureName = "Player/monsterpig_fight_destroy_animation";
                if (AnimationManager.Animate(gameTime, this, 48, 6, 1/AttackFrequency, 287))
                {
                    mGameLoop.ObjectManager.ActionManager.Fight.AxeStroke(this, mCurrentTargetObject);
                }
            }
            if (mOldTextureName != mTextureName)
            {
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);
                mOldTextureName = mTextureName;
                mFace.X = 287;
            }

        }
        public override void Draw()
        {
            mSpriteColor = Global.mColor;
            mSpriteColor.A = (byte)(127 + 0.5 * (mHealth / 100) * 255);
            LoadContent();
            mGameLoop.SpriteBatch.Draw(mSpriteTexture, mPosition, mFace,  mSpriteColor);
        }
    }
}
