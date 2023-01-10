using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SpaceImmigrants
{
    public class TankProjectileShooter
    {
		public static double ShootingSpeed = 2;
		public static float ProjectileSpeed = 300;
        public TankProjectileShooter(Game1 currentGame, Enemy tank)
        {
			double timePassed = 0;

            Event.GamePreStepped.Invoked += (double step) => {
				timePassed += step;
				if (timePassed < ShootingSpeed) return;
				timePassed = 0;

				Vector2 localPlayerPosition = currentGame.LocalPlayer.Position;
				Vector2 direction = localPlayerPosition - tank.Position;
				direction.Normalize();

				Projectile pellet = new(
					currentGame,
					tank.Position + new Vector2(0, 20),
					direction * ProjectileSpeed,
					Mode: "Player"
				);
			};
        }
    }
}