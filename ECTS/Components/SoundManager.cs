using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
// for sound effects

// for background music


namespace ECTS.Components
{
    /// <summary>
    /// manages playing different sounds
    /// call SoundManager to play a sound
    /// </summary>
    public sealed class SoundManager
    {
        private readonly Dictionary<string, Song> mMusicDict = new Dictionary<string, Song>();
        private readonly Dictionary<string, SoundEffect> mSoundEffectDict = new Dictionary<string, SoundEffect>();
        
        private string mSongPlaying = "";

        // used to control maximum number of bullet (laser) shots playing at once
        private int mBulletEffectsPlaying ;
        private int mMaxBulletEffectsPlaying = 5;
        private float mBulletEffectInterval = 0.3f;
        private float mElapsedGameTimeBullet;

        // used to control maximum number of explosions playing at once
        private int mExplosionEffectsPlaying;
        private int mMaxExplosionEffectsPlaying = 2;
        private float mExplosionEffectInterval = 0.3f;
        private float mElapsedGameTimeExplosion;

        private GameLoop GameLoop { get; }


        // constructor
        public SoundManager(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
            SetVolume();
        }

        /// <summary>
        /// Load sound manager music and effects.
        /// </summary>
        public void LoadContent()
        {
            
            // SOUND EFFECT Dictionary
            mSoundEffectDict.Add("error", GameLoop.Content.Load<SoundEffect>("Sound/Effects/error"));
            mSoundEffectDict.Add("save-game", GameLoop.Content.Load<SoundEffect>("Sound/Effects/save-game"));
            mSoundEffectDict.Add("load-game", GameLoop.Content.Load<SoundEffect>("Sound/Effects/load-game"));
            mSoundEffectDict.Add("button", GameLoop.Content.Load<SoundEffect>("Sound/Effects/Button1(Eig)"));

            mSoundEffectDict.Add("clone", GameLoop.Content.Load<SoundEffect>("Sound/Effects/Klonen(Eig)"));
            mSoundEffectDict.Add("dna", GameLoop.Content.Load<SoundEffect>("Sound/Effects/Klonen1(Eig)"));

            mSoundEffectDict.Add("win", GameLoop.Content.Load<SoundEffect>("Sound/Effects/Sieg"));
            mSoundEffectDict.Add("lose", GameLoop.Content.Load<SoundEffect>("Sound/Effects/Niederlage"));

            mSoundEffectDict.Add("lasershot", GameLoop.Content.Load<SoundEffect>("Sound/Effects/lasershot"));
            mSoundEffectDict.Add("explosion", GameLoop.Content.Load<SoundEffect>("Sound/Effects/explosion"));

            mSoundEffectDict.Add("dna_appears", GameLoop.Content.Load<SoundEffect>("Sound/Effects/dna_appears"));
            mSoundEffectDict.Add("dna_collect", GameLoop.Content.Load<SoundEffect>("Sound/Effects/dna_collect"));
            mSoundEffectDict.Add("dna_wasted", GameLoop.Content.Load<SoundEffect>("Sound/Effects/dna_wasted"));

            mSoundEffectDict.Add("hammering", GameLoop.Content.Load<SoundEffect>("Sound/Effects/hammering"));

            mSoundEffectDict.Add("achievement", GameLoop.Content.Load<SoundEffect>("Sound/Effects/achievement"));


            // MUSIC Dictionary
            mMusicDict.Add("Day", GameLoop.Content.Load<Song>("Sound/Music/Day"));
            mMusicDict.Add("Night", GameLoop.Content.Load<Song>("Sound/Music/Night"));
            mMusicDict.Add("Menu", GameLoop.Content.Load<Song>("Sound/Music/Menu"));
            mMusicDict.Add("Finale", GameLoop.Content.Load<Song>("Sound/Music/Finale"));
            
        }

        /// <summary>
        /// Apply music and effect volume from settings to the MediaPlayer and SoundEffect player.
        /// </summary>
        public void SetVolume()
        // applies Music and Effect volume from the Settings class
        {
            MediaPlayer.Volume = GameLoop.Settings.VolumeMusic;
            SoundEffect.MasterVolume = GameLoop.Settings.VolumeEffect;
        }

        
        /// <summary>
        /// Stops the currently running song and plays the given song.
        /// Songs: "Day", "Night", "Menu", "Finale".
        /// </summary>
        ///<param name="assetName">Name of song to be played.</param>
        public void PlayMusic(String assetName)
        {
            
            if (mMusicDict.ContainsKey(assetName) && mSongPlaying != assetName)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(mMusicDict[assetName]);
            }

            MediaPlayer.IsRepeating = true;
            
        }
        /// <summary>
        /// Plays the given sound effect. Already running sound effects are not stopped, sound effects may overlap.
        /// </summary>
        /// <param name="assetName">Name of sound effect to be played.</param>
        public void PlaySound(String assetName)
        {
            
            if (mSoundEffectDict.ContainsKey(assetName))
            {
                var effect = mSoundEffectDict[assetName];
                // LIMIT for number of lasershot effects playing
                if (assetName == "lasershot")
                {
                    if (mBulletEffectsPlaying <= mMaxBulletEffectsPlaying)
                    {
                        mBulletEffectsPlaying += 1;
                        effect.CreateInstance().Play();
                    }
                }
                // LIMIT for number of explosion effects playing
                else if (assetName == "explosion")
                {
                    if (mExplosionEffectsPlaying <= mMaxExplosionEffectsPlaying)
                    {
                        mExplosionEffectsPlaying += 1;
                        effect.CreateInstance().Play();
                    }
                }
                // NO LIMITS for other sounds
                else
                {
                    effect.CreateInstance().Play();
                }
            }
        }

        public void Update(GameTime gameTime)
        {

            // Counters needed for control of sound effect numbers
            mElapsedGameTimeBullet += (float)gameTime.ElapsedGameTime.TotalSeconds; //add time since last update
            mElapsedGameTimeExplosion += (float)gameTime.ElapsedGameTime.TotalSeconds; //add time since last update

            if (mElapsedGameTimeBullet >= mBulletEffectInterval)
            {
                mBulletEffectsPlaying -= 1;
                mElapsedGameTimeBullet = 0;
            }
            if (mElapsedGameTimeExplosion >= mExplosionEffectInterval)
            {
                mExplosionEffectsPlaying -= 1;
                mElapsedGameTimeExplosion = 0;
            }
        }

    }
}
