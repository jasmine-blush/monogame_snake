using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System;

namespace monogame_snake
{
    internal struct BodyPart
    {
        public BodyPart(float x, float y, int direction)
        {
            Position = new Vector2(x, y);
            Direction = direction;
        }

        public Vector2 Position { get; set; }
        public int Direction { get; set; }
    }

    internal class Snake
    {
        private const float MOVEMENT_COUNTER = 300f;
        private List<BodyPart> _body;
        private int _direction;
        private Texture2D _headTexture;
        private Texture2D _bodyTexture;
        private Texture2D _tailTexture;
        private (int, int) _gridSize;

        private float _snakeSpeed;
        private float _movementCounter;
        private Keys _lastKey = Keys.None;

        public Snake((int, int) gridSize)
        {
            _gridSize = gridSize;
            _direction = 1;
            _body = new List<BodyPart>
            {
                new BodyPart(2, 0, _direction),
                new BodyPart(1, 0, _direction),
                new BodyPart(0, 0, _direction)
            };
            _snakeSpeed = 1.0f;
            _movementCounter = MOVEMENT_COUNTER;
        }

        public void LoadContent(ContentManager content)
        {
            _headTexture = content.Load<Texture2D>("snake_head");
            _bodyTexture = content.Load<Texture2D>("snake_body");
            _tailTexture = content.Load<Texture2D>("snake_tail");
        }

        public void Update(GameTime gameTime, BallSpawner ball)
        {
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
            {
                _lastKey = Keys.W;
            }

            if (kstate.IsKeyDown(Keys.S))
            {
                _lastKey = Keys.S;
            }

            if (kstate.IsKeyDown(Keys.A))
            {
                _lastKey = Keys.A;
            }

            if (kstate.IsKeyDown(Keys.D))
            {
                _lastKey = Keys.D;
            }

            _movementCounter -= gameTime.ElapsedGameTime.Milliseconds;
            if (_movementCounter <= 0)
            {
                _movementCounter = MOVEMENT_COUNTER * _snakeSpeed;

                BodyPart head = _body[0];
                if (_lastKey == Keys.W && !(head.Direction == 2))
                {
                    head.Direction = 0;
                }

                if (_lastKey == Keys.S && !(head.Direction == 0))
                {
                    head.Direction = 2;
                }

                if (_lastKey == Keys.A && !(head.Direction == 1))
                {
                    head.Direction = 3;
                }

                if (_lastKey == Keys.D && !(head.Direction == 3))
                {
                    head.Direction = 1;
                }
                _body[0] = head;

                int currDirection = head.Direction;
                for (int i = 0; i < _body.Count; i++)
                {
                    BodyPart part = _body[i];
                    part.Position += GetMovement(part.Direction);
                    if (i == 0)
                    {
                        if (
                            part.Position.X < 0
                            || part.Position.Y < 0
                            || part.Position.X >= _gridSize.Item1
                            || part.Position.Y >= _gridSize.Item2
                        )
                        {
                            Game1.GameOver = true;
                            break;
                        }
                    }
                    int nextDirection = part.Direction;
                    part.Direction = currDirection;
                    currDirection = nextDirection;
                    _body[i] = part;
                }
                for (int i = 1; i < _body.Count; i++)
                {
                    if (_body[i].Position == _body[0].Position)
                    {
                        Game1.GameOver = true;
                        break;
                    }
                }

                if (ball.BallPosition == _body[0].Position)
                {
                    ball.SpawnNew();
                    _snakeSpeed -= 0.1f;
                    if (_snakeSpeed < 0.2f)
                        _snakeSpeed = 0.2f;
                    BodyPart tail = _body[_body.Count - 1];
                    Vector2 offset = GetMovement(tail.Direction);
                    offset *= new Vector2(-1, -1);
                    Vector2 newPartPosition = tail.Position + offset;
                    _body.Add(new BodyPart(newPartPosition.X, newPartPosition.Y, tail.Direction));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BodyPart part in _body)
            {
                if (_body.IndexOf(part) == 0)
                {
                    spriteBatch.Draw(
                        _headTexture,
                        part.Position * new Vector2(20, 20) + new Vector2(10, 10),
                        null,
                        Color.White,
                        MathHelper.ToRadians(90 * part.Direction),
                        new Vector2(_headTexture.Width / 2, _headTexture.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else if (_body.IndexOf(part) == _body.Count - 1)
                {
                    spriteBatch.Draw(
                        _tailTexture,
                        part.Position * new Vector2(20, 20) + new Vector2(10, 10),
                        null,
                        Color.White,
                        MathHelper.ToRadians(90 * part.Direction),
                        new Vector2(_tailTexture.Width / 2, _tailTexture.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
                else
                {
                    spriteBatch.Draw(
                        _bodyTexture,
                        part.Position * new Vector2(20, 20) + new Vector2(10, 10),
                        null,
                        Color.White,
                        MathHelper.ToRadians(90 * part.Direction),
                        new Vector2(_bodyTexture.Width / 2, _bodyTexture.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }

        #region Utility
        private static Vector2 GetMovement(int direction)
        {
            return direction switch
            {
                0 => new Vector2(0, -1),
                1 => new Vector2(1, 0),
                2 => new Vector2(0, 1),
                3 => new Vector2(-1, 0),
                _ => new Vector2(0, 0),
            };
        }

        #endregion
    }
}
