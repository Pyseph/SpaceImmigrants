﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SpaceImmigrants
{
    public class Game1 : Game
	{
		public Dictionary<object, Action<double>> DrawQueue = new();
		public List<HurtBox> HurtBoxes = new();
		public List<Enemy> Enemies = new();
		public bool GameEnded = false;
		public Dictionary<string, Texture2D> EnemySprites = new();
		public int Points = 0;
		public double GameTime = 0;

        public static SpriteBatch SpriteBatch;
		public Player LocalPlayer;
		public static Vector2 ViewportSize = new(800, 480);
        private Dictionary<Keys, bool> _keysDown = new();
        private Dictionary<string, bool> _mouseDown = new();
        private GraphicsDeviceManager _graphics;
		private float _currentTime = 0;
		private float _lastSpawnedEnemy = 0;
		private float _lastSpawnedPowerup = 0;
        public Game1()
        {
        	_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
			LocalPlayer = new Player(this);

            base.Initialize();
			new GameEndedMenu(this);
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

			EnemySprites.Add("Normal", Content.Load<Texture2D>("EnemyMongus"));
			EnemySprites.Add("Fly", Content.Load<Texture2D>("MongusFly"));
			EnemySprites.Add("Tank", Content.Load<Texture2D>("MongusTank"));
        }

		private void updateInputs()
		{
			MouseState mouseState = Mouse.GetState();
			KeyboardState keyboardState = Keyboard.GetState();

            // Fire MouseButton1Down event if the left mouse button is down this frame and wasn't last frame.
            if (mouseState.LeftButton == ButtonState.Pressed && !_mouseDown.ContainsKey("Left"))
            {
                Event.MouseButton1Down.Invoke(mouseState.Position);
            }

			// Fire InputBegan for each key that is down this frame and wasn't last frame.
			foreach (Keys key in keyboardState.GetPressedKeys())
			{
				if (!_keysDown.ContainsKey(key))
				{
					Event.InputBegan.Invoke(key);
				}
			}

			// Fire InputEnded for all keys which were pressed last frame but aren't now.
			foreach (Keys key in _keysDown.Keys)
			{
				if (!keyboardState.IsKeyDown(key))
				{
					Event.InputEnded.Invoke(key);
				}
			}

			// Clear the keysDown dictionary and add all keys which are currently down.
			_keysDown.Clear();
			foreach (Keys key in keyboardState.GetPressedKeys())
			{
				_keysDown.Add(key, true);
			}

            // Clear the mouseDown dictionary and add all keys which are currently down.
            _mouseDown.Clear();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _mouseDown.Add("Left", true);
            }
		}

		private string getEnemyType(float currentTime)
		{
			float random = (float)new Random().NextDouble();
			Dictionary<string, float> enemyTypeWeights = new()
			{
				{ "Normal", 0.5f },
				{ "Fly", 0.25f },
				{ "Tank", 0.1f }
			};

			// Increase the weights of the harder enemies over time
			enemyTypeWeights["Fly"] += (float)Math.Min(currentTime / 20, 0.25);
			enemyTypeWeights["Tank"] += (float)Math.Min(currentTime / 20, 0.1);

			// Normalize the weights
			float totalWeight = 0;
			foreach (float weight in enemyTypeWeights.Values)
			{
				totalWeight += weight;
			}
			foreach (string enemyType in enemyTypeWeights.Keys)
			{
				enemyTypeWeights[enemyType] /= totalWeight;
			}

			// Use weighted random to determine which enemy type to spawn
			float currentWeight = 0;
			foreach (string enemyType in enemyTypeWeights.Keys)
			{
				currentWeight += enemyTypeWeights[enemyType];
				if (random < currentWeight)
				{
					return enemyType;
				}
			}

			return "Normal";
		}
		private void spawnEnemies()
		{
			// Start spawning enemies every 3 seconds, and slowly increase the spawn rate
			float spawnRate = 2 - (_currentTime / 20);
			spawnRate = Math.Max(spawnRate, 0.5f);

			if (_currentTime - _lastSpawnedEnemy > spawnRate)
			{
				int randomXPixel = new Random().Next(0, (int)ViewportSize.X);

				Random randomVelocityGen = new Random();

				// Shift [0, 1] domain to [2, 1] to [0.5, 0.25]
				float randomXVelocity = ((float)randomVelocityGen.NextDouble() + 1) / 4;
				// Shift [0, 1] domain to [-2, -1]
				float randomYVelocity = (float)randomVelocityGen.NextDouble() + 1;

				if (randomXPixel > ViewportSize.X / 2)
				{
					randomXVelocity *= -1;
				}

				Vector2 size = new(50, 50);

				_lastSpawnedEnemy = _currentTime;
				Enemy enemy = new Enemy(this, new Enemy.EnemyData(
					type: getEnemyType(_currentTime),
					position: new Vector2(randomXPixel, -size.Y),
					velocity: new Vector2(randomXVelocity, randomYVelocity),
					size: size
				));
			}
		}
		private void spawnPowerup()
		{
			if (_currentTime - _lastSpawnedPowerup < 10)
			{
				return;
			}

			_lastSpawnedPowerup = _currentTime;
			new PlayerPowerup(this);
		}

		public void GameOver()
		{
			if (GameEnded) return;

            void gameEndedConnection(double step)
			{
				GameEnded = true;
                Event.GameEnded.Invoke(true);
                Event.GamePreStepped.Invoked -= gameEndedConnection;
            }

            Event.InvokedEvent<double>.InvokedDelegate preSteppedConnection = gameEndedConnection;
            Event.GamePreStepped.Invoked += preSteppedConnection;
		}

        protected override void Update(GameTime gameTime)
        {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			this.GameTime = gameTime.TotalGameTime.TotalSeconds;

			TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
			double delta = (float)elapsedGameTime.TotalSeconds;

			if (GameEnded) return;
			Event.GamePreStepped.Invoke(delta);

			_currentTime += (float)delta;

            updateInputs();
			spawnEnemies();
			spawnPowerup();

			base.Update(gameTime);
        }

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(30, 32, 39));

			SpriteBatch.Begin(SpriteSortMode.BackToFront, null);

			TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
			double delta = (float)elapsedGameTime.TotalSeconds;

			if (GameEnded)
			{
				delta = 0;
			}
			// Iterate through the draw queue dictionary and draw each item
			foreach (KeyValuePair<object, Action<double>> item in DrawQueue)
			{
				item.Value.Invoke(delta);
			}
			SpriteBatch.End();

			base.Draw(gameTime);

			elapsedGameTime = gameTime.ElapsedGameTime;
			delta = (double)elapsedGameTime.TotalSeconds;
			Event.GamePostStepped.Invoke(delta);
		}
    }
}