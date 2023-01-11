using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceImmigrants
{
    public class PlayerPowerup
    {
        public static Vector2 SpriteSize = new(20, 20);
        public Vector2 Velocity;
        public Vector2 Position;
        public Rectangle Rect;

        private Game1 _currentGame;

        public PlayerPowerup(Game1 currentGame)
        {
            Texture2D projectileSprite = currentGame.Content.Load<Texture2D>("PlasmaPellet");
            float RandomXPosition = new Random().Next(0, (int)Game1.ViewportSize.X);
            Vector2 startPosition = new Vector2(RandomXPosition, -SpriteSize.Y);
            Vector2 velocity = new Vector2(0, 300);

            this.Position = startPosition;
            this.Rect = new Rectangle(
                (int)this.Position.X,
                (int)this.Position.Y,
                (int)SpriteSize.X,
                (int)SpriteSize.Y
            );
            this.Velocity = velocity;
            this._currentGame = currentGame;

            currentGame.DrawQueue.Add(this, (double step) => {
                UpdatePosition(step);
                // Draw with the specified sprite size
                Game1.SpriteBatch.Draw(
                    projectileSprite,
                    this.Rect,
                    Color.Yellow
                );
            });
        }

        public void UpdatePosition(double step)
        {
            this.Position += this.Velocity * (float)step;
            this.Rect = new Rectangle(
                (int)this.Position.X,
                (int)this.Position.Y,
                (int)SpriteSize.X,
                (int)SpriteSize.Y
            );
            DetectCollision();
        }

        public void Destroy()
        {
            this._currentGame.DrawQueue.Remove(this);
        }

        public void DetectCollision()
        {
            Rectangle projectileRect = this.Rect;

            Rectangle playerRect = this._currentGame.LocalPlayer.PlayerRect;
            if (playerRect.Intersects(projectileRect))
            {
                _currentGame.LocalPlayer.ProjectileShooter.StartPowerup();
                Destroy();
            }
        }
    }
}