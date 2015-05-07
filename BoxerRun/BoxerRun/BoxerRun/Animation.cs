using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BoxerRun
{
    class Animation
    {
        //parametry Draw
        Texture2D spriteStrip;
        Rectangle destinationRect = new Rectangle();
        Rectangle sourceRect = new Rectangle();

        Color color;
        //klatki, animacja
        public int elapsedTime;
        public int frameTime;
        public int frameCount;

        public int FrameWidth;
        public int FrameHeight;
        public int blokX;
        public int blokY;
        //dodatkowe
        float scale;
        public bool Active;
        public bool Looping;
        public bool Attack;
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight,
            int frameCount, int frametime, Color color, float scale, bool looping,int blokx, int bloky, bool attack)
        {
            Position = position;
            spriteStrip = texture;
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frametime;
            this.scale = scale;
            Looping = looping;

            elapsedTime = 0;

            blokX = blokx;
            blokY = bloky;

            Active = true;

            Attack = attack;

        }

        public void Update(GameTime gameTime)
        {
            if (Active == false)
            {
                return;
            }

            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                blokX++;
                if (blokX == frameCount)
                {
                    blokX = 0;
                    if (Looping == false)
                        Active = false;
                }
                elapsedTime = 0;
            }
            destinationRect = new Rectangle((int)Position.X, (int)Position.Y, (int)(FrameWidth * scale), (int)(FrameHeight * scale));
            sourceRect = new Rectangle(blokX * FrameWidth, blokY*FrameHeight, FrameWidth, FrameHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
