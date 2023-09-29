using System;
using System.Collections.Generic;
using ECTS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECTS.States
{
    internal sealed class SettingsState : State
    {
        private readonly List<Component> mComponents;
        private readonly Texture2D mBackground;
        private readonly Slider mSliderMusicVolume;
        private readonly Slider mSliderEffectsVolume;
        private readonly State mPreviousState;

        public SettingsState(GameLoop game, State prevState) : base(game)
        {
            mPreviousState = prevState;
            var content = mGameLoop.Content;
            // Loads all required graphics
            mBackground = content.Load<Texture2D>("Background");
            var buttonTextureMusicVolumeTextfield = content.Load<Texture2D>("Components/SettingsMenu/Music_Volume");
            var buttonTextureEffectsVolumeTextfield = content.Load<Texture2D>("Components/SettingsMenu/Effect_Volume");
            var buttonTextureWindowModeTextfield = content.Load<Texture2D>("Components/SettingsMenu/Display_Mode");

            var buttonTextureFullscreen = content.Load<Texture2D>("Components/SettingsMenu/FullScreen");
            var buttonTextureWindowed = content.Load<Texture2D>("Components/SettingsMenu/Windowed");
            var buttonTextureBorderless = content.Load<Texture2D>("Components/SettingsMenu/Borderless");

            var buttonTextureBack = content.Load<Texture2D>("Components/BackButton");
            // Creates the slider to be displayed in the menu.
            mSliderMusicVolume = new Slider((int)(mGameLoop.DisplayHeight * 0.208), mGameLoop.Settings.VolumeMusic);
            mSliderEffectsVolume = new Slider((int)(mGameLoop.DisplayHeight * 0.347), mGameLoop.Settings.VolumeEffect);
            // Creates the buttons to be displayed in the menu.
            var musicVolumeTextfield = new Button(new Vector2(mSliderMusicVolume.mMinValue, (int)(mSliderMusicVolume.mHeight-mGameLoop.DisplayHeight * 0.06)), buttonTextureMusicVolumeTextfield, 60, true);
            var effectsVolumeTextfield = new Button(new Vector2(mSliderEffectsVolume.mMinValue, (int)(mSliderEffectsVolume.mHeight - mGameLoop.DisplayHeight * 0.06)), buttonTextureEffectsVolumeTextfield, 60, true);
            var fullscreenButton = new Button(new Vector2(mSliderMusicVolume.mMinValue, (int)(mGameLoop.DisplayHeight * 0.52)), buttonTextureFullscreen, 38);
            fullscreenButton.Click += fullscreenButton_Click; // Add the corresponding EventHandler.
            var windowedButton = new Button(new Vector2(fullscreenButton.Position.X+fullscreenButton.mButtonWidth+60, (int)(mGameLoop.DisplayHeight * 0.52)), buttonTextureWindowed, 38);
            windowedButton.Click += windowedButton_Click; // Add the corresponding EventHandler.
            var borderlessButton = new Button(new Vector2(windowedButton.Position.X + windowedButton.mButtonWidth+60, (int)(mGameLoop.DisplayHeight * 0.52)), buttonTextureBorderless, 38);
            borderlessButton.Click += borderlessButton_Click; // Add the corresponding EventHandler.

            var windowModeTextfield = new Button(new Vector2(fullscreenButton.Position.X, (int)(fullscreenButton.Position.Y-mGameLoop.DisplayHeight * 0.083)), buttonTextureWindowModeTextfield, 60, true);

            var backButton = new Button(buttonTextureBack, 40, (int)(mGameLoop.DisplayHeight*0.92), false); // Back_Button
            backButton.Click += BackButton_Click; // Add the corresponding EventHandler.
            
            mComponents = new List<Component> // Saves all components in a list
            {
                backButton,
                mSliderMusicVolume,
                mSliderEffectsVolume,
                fullscreenButton,
                windowedButton,
                borderlessButton,
                musicVolumeTextfield,
                effectsVolumeTextfield,
                windowModeTextfield
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in mComponents)
            {
                component.Update();
            }
            mGameLoop.Settings.VolumeMusic = mSliderMusicVolume.GetValueOfTheSlider();
            mGameLoop.Settings.VolumeEffect = mSliderEffectsVolume.GetValueOfTheSlider();
        }

        public override void Draw()
        {
            
        }

        public override void DrawGui(GameTime gameTime)
        {
            mGameLoop.SpriteBatch.Draw(mBackground, new Rectangle(0, 0, mGameLoop.DisplayWidth, mGameLoop.DisplayHeight), Color.White); // Background
            foreach (var component in mComponents) // Draw all created components.
            {
                component.Draw(mGameLoop.SpriteBatch);
            }
        }

        private void BackButton_Click(object sender, EventArgs e) // EventHandler for the BackButton
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.ChangeState(mPreviousState);
        }
        private void fullscreenButton_Click(object sender, EventArgs e) // EventHandler
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.mGraphics.IsFullScreen = true;
            mGameLoop.mGraphics.ApplyChanges();
            mGameLoop.Window.IsBorderless = false;
            mGameLoop.Settings.mWindowState = 1;
        }
        private void windowedButton_Click(object sender, EventArgs e) // EventHandler
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.mGraphics.PreferredBackBufferWidth = mGameLoop.DisplayWidth;
            mGameLoop.mGraphics.PreferredBackBufferHeight = mGameLoop.DisplayHeight;
            mGameLoop.mGraphics.IsFullScreen = false;
            mGameLoop.mGraphics.HardwareModeSwitch = true;
            mGameLoop.mGraphics.ApplyChanges();
            mGameLoop.Window.Position = new Point(-20, -30);
            mGameLoop.Window.IsBorderless = false;
            mGameLoop.Settings.mWindowState = 2;
        }
        private void borderlessButton_Click(object sender, EventArgs e) // EventHandler
        {
            mGameLoop.RenderManager.SoundManager.PlaySound("button");
            mGameLoop.Window.Position = new Point(0, 0);
            mGameLoop.Window.IsBorderless = true;
            mGameLoop.Settings.mWindowState = 3;
        }
    }
}
