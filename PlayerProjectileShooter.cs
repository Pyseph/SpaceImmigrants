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
        public PlayerProjectileShooter(Game1 currentGame)
        {
            Event.InputBegan.Invoked += (key) => {
                if (key == Keys.J)
                {
                    BombProjectile bomb = new(
                        currentGame,
                        currentGame.LocalPlayer.Position + new Vector2(0, 20),
                        new Vector2(0, -500),
                        "Enemy"
                    );
                }
            };
        }
    }
}