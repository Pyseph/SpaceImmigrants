using Microsoft.Xna.Framework;
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

        public static SpriteBatch SpriteBatch;
		public Player LocalPlayer;
		public static Vector2 ViewportSize = new(800, 480);
        private Dictionary<Keys, bool> _keysDown = new();
        private Dictionary<string, bool> _mouseDown = new();
        private GraphicsDeviceManager _graphics;
		private float _currentTime = 0;
		private float _lastSpawnedEnemy = 0;
        public Game1()
        {
        	_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
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

            // TODO: use this.Content to load your game content here
        }

		public void UpdateInputs()
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
			// There exist 3 enemy types: the first has a 100% chance of spawning since the start
			// while the other two have a 50% chance of spawning after 20 seconds and 40 seconds respectively.
			// The enemy types are: Normal, Fly, Tank
			if (currentTime < 20) return "Normal";
			if (currentTime < 40) return new Random().Next(0, 2) == 0 ? "Normal" : "Fly";
			return new Random().Next(0, 2) == 0 ? "Fly" : "Tank";
		}
		public void UpdateEnemies(double delta)
		{
			_currentTime += (float)delta;
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

		public void GameOver()
		{
			GameEnded = true;
			Event.GameEnded.Invoke(true);
		}

        protected override void Update(GameTime gameTime)
        {
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
			double delta = (float)elapsedGameTime.TotalSeconds;

			if (GameEnded)
			{
				return;
			}
			Event.GamePreStepped.Invoke(delta);

            UpdateInputs();
			UpdateEnemies(delta);

			base.Update(gameTime);

			elapsedGameTime = gameTime.ElapsedGameTime;
			delta = (double)elapsedGameTime.TotalSeconds;
			Event.GamePostStepped.Invoke(delta);
        }

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(new Color(30, 32, 39));

			SpriteBatch.Begin();

			TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
			double delta = (float)elapsedGameTime.TotalSeconds;

			// Iterate through the draw queue dictionary and draw each item
			if (GameEnded)
			{
				delta = 0;
			}
			foreach (KeyValuePair<object, Action<double>> item in DrawQueue)
			{
				item.Value.Invoke(delta);
			}
			SpriteBatch.End();

			base.Draw(gameTime);
		}
    }
}