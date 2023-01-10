using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Security.Policy;

namespace SpaceImmigrants
{
    public class Player
    {
        public Vector2 SpriteSize = new(40, 40);
        public Vector2 FireSize = new(18, 20);
        public static float MovementSpeed = 200;
        public Vector2 Velocity = new(0, 0);
        public Vector2 Position;

        private Texture2D _playerSprite;
        private Texture2D _movementSprite;
        private float _hitCooldown = 0;
        private StatusBar _statusBar;
        private static Dictionary<string, Vector2> _keyDirections = new()
        {
            { "A", new Vector2(-1, 0) },
            { "D", new Vector2(1, 0) },
            { "W", new Vector2(0, -1) },
            { "S", new Vector2(0, 1) },
        };
        private int _numMovementKeysHeld = 0;

        private Vector2 toUnitVector(Vector2 Vector)
        {
            return Vector / Vector.Length();
        }

        public Player(Game1 currentGame)
        {
            this._playerSprite = currentGame.Content.Load<Texture2D>("MongusShip");
            this._movementSprite = currentGame.Content.Load<Texture2D>("MongusFire");
            this.Position = new Vector2(
                Game1.ViewportSize.X / 2 - (this.SpriteSize.X / 2),
                Game1.ViewportSize.Y - 20
            );

            this._statusBar = new StatusBar(currentGame);
            new PlayerProjectileShooter(currentGame);

            // Listen to the InputBegan event and move the player when the user presses the arrow keys.
            Event.InputBegan.Invoked += (key) => {
                if (_keyDirections.ContainsKey(key.ToString()))
                {
                   this.Velocity += _keyDirections[key.ToString()] * MovementSpeed;
                }

            };
            Event.InputEnded.Invoked += (key) => {
                if (_keyDirections.ContainsKey(key.ToString()))
                {
                    _numMovementKeysHeld -= 1;
                    this.Velocity -= _keyDirections[key.ToString()] * MovementSpeed;
                }
            };

            Event.GamePostStepped.Invoked += (step) => {
                this.DetectCollisions(currentGame, step);
            };

            // Add a callback to the DrawQueue to draw the player, using gameTime argument to animate the sprite.
			currentGame.DrawQueue.Add(this, (double step) => {
                this.UpdatePosition(step);
                Vector2 playerPosition = this.Position;

                Rectangle destination = new Rectangle(
                    (int)playerPosition.X,
                    (int)playerPosition.Y,
                    (int)this.SpriteSize.X,
                    (int)this.SpriteSize.Y
                );
                //Debug.WriteLine(playerPosition);

				Game1.SpriteBatch.Draw(
					texture: this._playerSprite,
					destinationRectangle: destination,
                    color: Color.White
				);

                if (this.Velocity.Length() > 0.1)
                {
                    Rectangle fireDestination = new Rectangle(
                        (int)playerPosition.X + ((int)this.SpriteSize.X / 2 - (int)this.FireSize.X / 2),
                        (int)playerPosition.Y + 24,
                        (int)this.FireSize.X,
                        (int)this.FireSize.Y
                    );

                    Game1.SpriteBatch.Draw(
                        texture: this._movementSprite,
                        destinationRectangle: fireDestination,
                        color: Color.White
                    );
                }
			});
        }

        public void PlayerHit(Game1 currentGame)
        {
            if (_hitCooldown > 0)
            {
                return;
            }

            _hitCooldown = 1;
            this._statusBar.Health -= 1;

            if (this._statusBar.Health <= 0)
            {
                currentGame.GameOver();
            }
        }

        public virtual void DetectCollisions(Game1 currentGame, double step)
        {
            _hitCooldown = Math.Max(_hitCooldown - (float)step, 0);
            if (_hitCooldown > 0)
            {
                return;
            }

            foreach (HurtBox hurtBox in currentGame.HurtBoxes)
            {
                // error: 'object' does not contain a definition for 'Position' and no accessible extension method 'Position' accepting a first argument of type 'object' could be found
                // fix: cast to HurtBox
                // like so: ((HurtBox)hurtBox).Position

                if (hurtBox.Position.X < this.Position.X + this.SpriteSize.X &&
                    hurtBox.Position.X + hurtBox.SpriteSize.X > this.Position.X &&
                    hurtBox.Position.Y < this.Position.Y + this.SpriteSize.Y &&
                    hurtBox.Position.Y + hurtBox.SpriteSize.Y > this.Position.Y)
                {
                    // Collision detected
                    PlayerHit(currentGame);
                    return;
                }
            }
        }

        public virtual void UpdatePosition(double Step)
        {
            Vector2 normalizedVelocity = this.Velocity;
            // To prevent 0 / 0 -> NaN
            if (this.Velocity.Length() > 0.1)
            {
                // Normalize velocity to prevent diagonal movement
                // from being faster than horizontal/vertical movement
                normalizedVelocity /= this.Velocity.Length();
            }

            this.Position += normalizedVelocity * MovementSpeed * (float)Step;
            // Clamp the player's position to the screen
            this.Position.X = Math.Clamp(this.Position.X, 0, Game1.ViewportSize.X - this.SpriteSize.X);
            this.Position.Y = Math.Clamp(this.Position.Y, 0, Game1.ViewportSize.Y - this.SpriteSize.Y);
        }
    }
}
