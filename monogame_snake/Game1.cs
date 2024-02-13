using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace monogame_snake
{
    public class Game1 : Game
    {
        public static bool GameOver = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _gridTexture;
        private Snake _snake;
        private BallSpawner _ball;
        private SpriteFont _font;
        private RenderTarget2D _gameOverTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            (int, int) gridSize = (
                _graphics.PreferredBackBufferWidth / 20,
                _graphics.PreferredBackBufferHeight / 20
            );
            _snake = new Snake(gridSize);
            _ball = new BallSpawner(gridSize);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gridTexture = Content.Load<Texture2D>("grid");

            //Generate Game Over screen as a texture
            _font = Content.Load<SpriteFont>("arial");
            Vector2 gameOverSize = _font.MeasureString("Game Over!");
            _gameOverTexture = new RenderTarget2D(
                _graphics.GraphicsDevice,
                (int)Math.Ceiling(gameOverSize.X),
                (int)Math.Ceiling(gameOverSize.Y)
            );

            GraphicsDevice.SetRenderTarget(_gameOverTexture);
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, "Game Over!", Vector2.Zero, Color.Red);
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            //-----

            _snake.LoadContent(Content);
            _ball.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }
            else
            {
                if (
                    GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                    || Keyboard.GetState().IsKeyDown(Keys.Escape)
                )
                    Exit();

                _snake.Update(gameTime, _ball);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_gridTexture, Vector2.Zero, new Color(0f, 0.1f, 0f));
            _snake.Draw(_spriteBatch);
            _ball.Draw(_spriteBatch);

            if (GameOver)
            {
                Vector2 halfScreenVector = new Vector2(
                    _graphics.PreferredBackBufferWidth / 2,
                    _graphics.PreferredBackBufferHeight / 2
                );
                Vector2 halfGameOverVector = new Vector2(
                    _gameOverTexture.Width / 2,
                    _gameOverTexture.Height / 2
                );
                _spriteBatch.Draw(
                    _gameOverTexture,
                    halfScreenVector,
                    null,
                    Color.White,
                    0f,
                    halfGameOverVector,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
