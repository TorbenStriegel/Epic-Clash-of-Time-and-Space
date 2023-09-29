using System.Runtime.Serialization;
using ECTS.Components;
using Microsoft.Xna.Framework;

namespace ECTS.GUI
{
    /// <summary>
    /// manages camera movement with transformation matrix 
    /// </summary>
    
    [DataContract]
    public sealed class CameraManager
    {
        [DataMember] public Vector2 mPosition;
        [DataMember] public Matrix Transform { get; private set; }
        private GameLoop GameLoop { get; set; }
        private Matrix mCameraPosition;
        private Matrix mScreenOffset;
        private Matrix mZoomOffset;
        private Matrix mZoomMatrix;
        private float mZoom;

        public CameraManager(GameLoop gameLoop)
        {
            mZoom = 1;
            GameLoop = gameLoop;
            mPosition.X = 0;
            mPosition.Y = 0;
            SetCameraPosition(-1150,-1800);
        }


        public void SetCameraPosition(int x,int y) // Shift the camera in X and Y direction.
        {
            mCameraPosition = Matrix.CreateTranslation(
                mPosition.X + (x- GameLoop.DisplayWidth / 2),
                mPosition.Y + (y - GameLoop.DisplayHeight / 2),
                0);
            mPosition.X += x;
            mPosition.Y += y;

            mScreenOffset = Matrix.CreateTranslation(
                GameLoop.DisplayWidth / 2f,
                GameLoop.DisplayHeight / 2f,
                0);
            Transform = mCameraPosition * mScreenOffset * mZoomMatrix * mZoomOffset;
        }

        public void SetZoom(float zoom)
        {
            var tempZoomValue = zoom;
            if (GameLoop.mTechDemo)
            {
                tempZoomValue *= (float)1.5;
            }
            else
            {
                tempZoomValue *= ((float)(GameLoop.DisplayHeight * GameLoop.DisplayWidth)/3686400)*((float)1440/GameLoop.DisplayHeight);
            }
            
            mZoomOffset = Matrix.CreateTranslation(
                -(GameLoop.DisplayWidth / 2f)*(tempZoomValue - 1),
                -(GameLoop.DisplayHeight / 2f)*(tempZoomValue - 1),
                0);
            mZoom = tempZoomValue;
            mZoomMatrix = Matrix.CreateScale(mZoom, mZoom, 1.0f);
            Transform = mCameraPosition * mScreenOffset * mZoomMatrix * mZoomOffset;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            GameLoop = Global.mGameLoop;
            SetZoom(GameLoop.ObjectManager.DataManager.mZoomValue);
        }
    }
}
