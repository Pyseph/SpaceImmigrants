using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceImmigrants
{
    public class Projectile
    {
        public static Vector2 SpriteSize = new(20, 20);
        public Vector2 Velocity;
        public Vector2 Position;
        public Rectangle Rect;
        // Mode: Player, Enemy
        public string Mode;

        private Game1 _currentGame;

        public Projectile(Game1 currentGame, Vector2 StartPosition, Vector2 Velocity, string Mode)
        {
            Texture2D projectileSprite = currentGame.Content.Load<Texture2D>("PlasmaPellet");

            this.Position = StartPosition;
            this.Rect = new Rectangle(
                (int)this.Position.X,
                (int)this.Position.Y,
                (int)SpriteSize.X,
                (int)SpriteSize.Y
            );
            this.Velocity = Velocity;
            this.Mode = Mode;
            this._currentGame = currentGame;

            currentGame.DrawQueue.Add(this, (double step) => {
                UpdatePosition(step);
                // Draw with the specified sprite size
                Game1.SpriteBatch.Draw(
                    texture: projectileSprite,
                    destinationRectangle: this.Rect,
                    color: this.Mode == "Player" ? Color.Red : Color.White,
                    effects: SpriteEffects.None,
                    layerDepth: 0.1f,
                    rotation: 0,
                    origin: new Vector2(0, 0),
                    sourceRectangle: null
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
            // wait for GamePreStepped to run once before removing the enemy
            // This is to prevent race conditions with removing objects from the list while also iterating over it
            void diedConnection(double step)
			{
                this._currentGame.DrawQueue.Remove(this);
                Event.GamePreStepped.Invoked -= diedConnection;
            }

            Event.InvokedEvent<double>.InvokedDelegate preSteppedConnection = diedConnection;
            Event.GamePreStepped.Invoked += preSteppedConnection;
        }

        public void DetectCollision()
        {
            Rectangle projectileRect = this.Rect;

            if (this.Mode == "Player")
            {
                Rectangle playerRect = this._currentGame.LocalPlayer.PlayerRect;
                if (playerRect.Intersects(projectileRect))
                {
                    _currentGame.LocalPlayer.PlayerHit(this._currentGame);
                    Destroy();
                }
            } else {
                foreach (Enemy enemy in _currentGame.Enemies)
                {
                    Rectangle enemyHurtBoxRect = enemy.HurtBox.Rect;

                    if (enemyHurtBoxRect.Intersects(projectileRect))
                    {
                        enemy.EnemyHit(this._currentGame);
                        Destroy();
                    }
                }
            }
        }
    }
}