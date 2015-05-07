using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoxerRun
{
    class Player
    {
        public Animation PlayerAnimation;
        public Vector2 Position;
        public bool Active;
        public int Health;

        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public int blokX
        {
            get { return PlayerAnimation.blokX; }
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            this.PlayerAnimation = animation;
            this.Position = position;

            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Update(gameTime);
            PlayerAnimation.Position = Position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }

    }
}
