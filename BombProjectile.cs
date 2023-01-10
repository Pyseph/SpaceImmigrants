using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceImmigrants
{
    public class BombProjectile
    {
        public static Vector2 SpriteSize = new(20, 20);
        public Vector2 Velocity;
        public Vector2 Position;
        // Mode: Player, Enemy
        public string Mode;

        private Game1 _currentGame;

        public BombProjectile(Game1 currentGame, Vector2 StartPosition, Vector2 Velocity, string Mode)
        {
            Texture2D bombProjectileSprite = currentGame.Content.Load<Texture2D>("BombProjectile");

            this.Position = StartPosition;
            this.Velocity = Velocity;
            this.Mode = Mode;
            this._currentGame = currentGame;

            currentGame.DrawQueue.Add(this, (double step) => {
                UpdatePosition(step);
                Game1.SpriteBatch.Draw(bombProjectileSprite, this.Position, Color.White);
            });
        }

        public void UpdatePosition(double step)
        {
            this.Position += this.Velocity * (float)step;
            DetectCollision();
        }

        public void Destroy()
        {
            this._currentGame.DrawQueue.Remove(this);
        }

        public void DetectCollision()
        {
            if (this.Mode == "Player")
            {
                Vector2 playerPosition = _currentGame.LocalPlayer.Position;
                Vector2 playerSize = _currentGame.LocalPlayer.SpriteSize;

                if (this.Position.X > playerPosition.X && this.Position.X < playerPosition.X + playerSize.X)
                {
                    if (this.Position.Y > playerPosition.Y && this.Position.Y < playerPosition.Y + playerSize.Y)
                    {
                        _currentGame.LocalPlayer.PlayerHit(this._currentGame);
                        Destroy();
                    }
                }
            } else {
                foreach (Enemy enemy in _currentGame.Enemies)
                {
                    Vector2 enemyPosition = enemy.Position;
                    Vector2 enemySize = enemy.SpriteSize;

                    if (this.Position.X > enemyPosition.X && this.Position.X < enemyPosition.X + enemySize.X)
                    {
                        if (this.Position.Y > enemyPosition.Y && this.Position.Y < enemyPosition.Y + enemySize.Y)
                        {
                            enemy.EnemyHit(this._currentGame);
                            Destroy();
                        }
                    }
                }
            }
        }
    }
}