using ECTS.Pathfinder;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ECTS.Objects.GameObjects
{
    /// <summary>
    /// class for all game objects
    /// each game object needs the following fields set upon initialization
    /// mGameLoop, mTextureName.
    /// default Speed is 0, increase for movable objects.
    /// default Health is 100
    /// </summary>

    [DataContract]
    [KnownType("GetKnownTypes")]
    public abstract class GameObject
    {
        protected GameLoop mGameLoop;
        [DataMember] public int mLayer;
        [DataMember] protected Rectangle mPosition;     // this rectangle determines the object position (1x1 pixel in size)
        
        private Rectangle mActionArea;                  // = mPosition + mRange on each side, set when property ActionArea is called.
        protected int mRange;                       // action Range
        [DataMember] protected float mObjectLifetime = 100;          // Lifetime of object in seconds.
        [DataMember] protected float mObjectAge;                 // Age of object in seconds                FOR WHAT?

        internal Vector2 mMonsterAttackWallPosition;

        internal int SortingDistance { get; set; }                // can hold the distance to a certain point. Used for sorting Object lists based upon distance to a given point.

        // TEXTURE
        [DataMember] protected string mTextureName = "";
        [DataMember] protected string mOldTextureName = "";
        [DataMember] protected string mBackgroundTextureName = "";
        public Texture2D mSpriteTexture;
        protected Texture2D mBackgroundTexture;
        public Rectangle mFace;                                                     // source rectangle from texture
        protected Color mSpriteColor = new Color(100, 100, 100, 100);   // Color for foreground sprite

        // OBJECT TYPEs are used to determine Object-Object interactions
        public enum ObjectType {Player, Enemy, Spaceship, Tree, Stone, Food, Metal, Grenade, Bullet, Explosion, Wall, Gate, Background, Dna}
        public ObjectType mObjectType;
        
        // OBJECT STATE determines what an object is currently doing
        public enum ObjectState { Idle, Moving, Harvesting, Repairing, Destroying, Fighting}
        [DataMember] protected ObjectState mObjectState;
        [DataMember] public ObjectState mLastObjectState;

        // OBJECT PROPERTIES determine general object behaviour and interactions
        protected bool mIsMarkable;
        [DataMember] protected bool mIsMarked;               // true if an object has been selected. boolean default value is "false"
        [DataMember] protected bool mIsColliding;                         // true if an object cannot be crossed (node can not be passe don grid)
        [DataMember] protected bool mIsActing;                            // true if an object can perform any actions (Property cannot be changed).
        [DataMember] protected bool mIsInteracting;                       // true if the object can Interact with others, if false no interaction except collisions might occur
        
        [DataMember] protected int mSpeed;                                // 0 if object is not movable 
        [DataMember] protected float mHealth = 100;            // Health of game object, primarily applies to movable objects (but also to walls, spaceship etc.)
        [DataMember] protected int mLevel;                  // Level of player character

        // TARGET OBJECTS
        [DataMember] public List<GameObject> mTargetObjects = new List<GameObject>();
        [DataMember] public GameObject mCurrentTargetObject;
        [DataMember] public List<GameObject> mObjectTargetList;


        // PATHFINDER
        [DataMember] public Vector2 mDirectionSpeedVector;
        [DataMember] public Flock mFlock;

        // FIGHTING
        [DataMember] protected float mAttackStrength;
        protected float mAttackFrequency;
        [DataMember] protected float mDefenseStrength;
        [DataMember] protected bool mUnderAttack;
        [DataMember] protected float mTimeSinceAttack;
        protected float mMaxTimeSinceAttack = 1f;                   // Maximum time Monsters try to find an attacking unit before returning to other tasks

        [DataMember] protected GameObject mAttackingUnit;             // attacking Game Object.


        protected Vector2 mGunMuzzle;                   // Tip of gun for shooting objects
        protected int mStrikingDistance;                // Striking distance for direct object-object interaction (via axe, shovel etc.)

        protected GameObject(GameLoop gameLoop)
        {
            mLayer = 0;
            mGameLoop = gameLoop;
        }

        // PROPERTIES //
        // properties are used to access the classes protected variables (= attributes)
        internal Rectangle Position
        {
            get => mPosition;
            set => mPosition = value;
        }

        internal Vector2 ObjectCenter
        {
            get => Position.Center.ToVector2();
            set
            {
                mPosition.X = (int)(value.X - Position.Width / 2.0);
                mPosition.Y = (int)(value.Y - Position.Height / 2.0);
            }
        }

        internal Vector2 DirectionSpeedVector
        {
            get => mDirectionSpeedVector;
            set => mDirectionSpeedVector = value;
        }
        
        
        internal Rectangle ActionArea
        {
            get
            {
                mActionArea.X = Position.X - mRange/2;
                mActionArea.Y = Position.Y - mRange/2;
                mActionArea.Width = Position.Width + mRange;
                mActionArea.Height = Position.Height + mRange;
                return mActionArea;
            }
        }

        internal int Range
        {
            set => mRange = value;
        }

         internal bool IsMarked
        {
            get => mIsMarked;
            set
            {
                if (mIsMarkable)
                {
                    mIsMarked = value;
                }
            }
        }

        public bool IsColliding
        {
            get => mIsColliding;
            set 
            {
                // call function to adapt the grid here
                mIsColliding = value;
                mGameLoop.ObjectManager?.Pathfinder.AStar.mGrid.UpdateNodesInRectangle(Position);
            }
        }
        public bool IsInteracting
        {
            get => mIsInteracting;
            set => mIsInteracting = value;
        }
        public bool IsActing
        {
            get => mIsActing;
            set => mIsActing = value;
        }

        public ObjectType Type => mObjectType;

        internal ObjectState State
        {
            get => mObjectState;
            set => mObjectState = value;
        }

        internal int Speed
        {
            get => mSpeed;
            set => mSpeed = value;
        }

        internal float Health  
        {
            get => mHealth;
            set
            {
                mHealth = value;
                if (mHealth <= 0)
                {
                    mHealth = 0;
                    IsColliding = false;
                    ChangeFace("Dead");
                }

                if (value < 100 && value > 0 && Type == ObjectType.Wall)
                {
                    IsColliding = true;
                    if ((int)value == 1)
                    {
                        var tempActiveFlocks = new HashSet<Flock>(mGameLoop.ObjectManager.DataManager.ActiveFlocks);
                        foreach (var flock in tempActiveFlocks)
                        {
                            flock.ChangePath(mGameLoop);
                        }
                    }
                    ChangeFace("Broken");
                } 
                else if ((int)value == 100 && Type == ObjectType.Wall)
                {
                    IsColliding = true; 
                    ChangeFace("Complete");
                }

                if (Type == ObjectType.Spaceship)
                {
                    this.ChangeFace("");
                }
            }
        }

        public float ObjectLifetime => mObjectLifetime;

        public float ObjectAge
        {
            get => mObjectAge;
            set => mObjectAge = value;
        }

        public int Level
        {
            get => mLevel;
            set => mLevel = value;
        }

        
        public float AttackStrength
        {
            get => mAttackStrength;
            set => mAttackStrength = value;
        }

        public float TimeSinceAttack
        {
            get => mTimeSinceAttack;
            set => mTimeSinceAttack = value;
        }

        public bool UnderAttack
        {
            get => mUnderAttack;
            set => mUnderAttack = value;
        }

        public GameObject AttackingUnit
        {
            get => mAttackingUnit;
            set => mAttackingUnit = value;
        }
        protected float AttackFrequency => mAttackFrequency;

        public float DefenseStrength 
        {
            get => mDefenseStrength;
            set => mDefenseStrength = value;
        }

        public int StrikingDistance => mStrikingDistance;

        public Vector2 GunMuzzle => mGunMuzzle;

        // FUNCTIONS //
        public void LoadContent()
        {
            // load textures if names are provided
            if (mTextureName != "")
            {
                mSpriteTexture = mGameLoop.GetTexture(mTextureName);
            }
            if (mBackgroundTextureName != "")
            {
                mBackgroundTexture = mGameLoop.GetTexture(mBackgroundTextureName);
            }
        }

        /// <summary>
        /// Select sprite currently drawn for the object.
        /// </summary>
        /// <param name="face">Name of the current face.</param>
        public abstract void ChangeFace(string face);

        /// <summary>
        /// Draw sprite for object.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Update object.
        /// </summary>
        /// <param name="gameTime">Game Time.</param>
        public abstract void Update(GameTime gameTime);

        

        // OnDeSerialized: Call internal methods
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Health = Health;
            LoadContent();
        }

        // Serializer needs to know all Subclasses that are to be serialized. This methods solves this dynamically.
        // ReSharper disable once UnusedMember.Local  -> This Method is used by DataContractSerializer but the usage is only declared
        // with the [KnownType("GetKnownTypes")] attribute in this file.
        internal static Type[] GetKnownTypes() => Assembly.GetExecutingAssembly().GetTypes().Where(_ => _.IsSubclassOf(typeof(GameObject))).ToArray();
    }
}
