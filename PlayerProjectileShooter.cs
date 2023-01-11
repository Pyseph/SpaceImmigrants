using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SpaceImmigrants
{
    public class PlayerProjectileShooter
    {
        private float _powerupRemaining = 0;
        private bool _holdingKey;
        private float _lastSpawned;

        public PlayerProjectileShooter(Game1 currentGame)
        {
            Event.InputBegan.Invoked += (key) => {
                if (key == Keys.J)
                {
                    _holdingKey = true;
                    spawnProjectile(currentGame);
                }
            };
            Event.InputEnded.Invoked += (key) => {
                if (key == Keys.J)
                {
                    _holdingKey = false;
                }
            };

            Event.GamePostStepped.Invoked += (double step) => {
                if (_powerupRemaining > 0)
                {
                    _powerupRemaining -= (float)step;
                    if (_powerupRemaining <= 0)
                    {
                        _powerupRemaining = 0;
                    }
                }

                if (_holdingKey)
                {
                    spawnProjectile(currentGame);
                }
            };
        }

        private void spawnProjectile(Game1 currentGame)
        {
            if (currentGame.GameTime - _lastSpawned < 0.2)
            {
                return;
            }
            _lastSpawned = (float)currentGame.GameTime;

            Projectile pellet = new(
                currentGame,
                currentGame.LocalPlayer.Position + new Vector2(0, 20),
                new Vector2(0, -500),
                "Enemy"
            );

            if (_powerupRemaining > 0)
            {
                for (int i = -1; i < 2; i = i + 2)
                {
                    Vector2 direction = new Vector2(100 * i, -500);
                    direction.Normalize();
                    direction *= 500; 

                    Projectile pellet2 = new(
                        currentGame,
                        currentGame.LocalPlayer.Position + new Vector2(0, 20),
                        direction,
                        "Enemy"
                    );
                }
            }
        }
    

        public void StartPowerup()
        {
            _powerupRemaining = 6;
        }
    }
}