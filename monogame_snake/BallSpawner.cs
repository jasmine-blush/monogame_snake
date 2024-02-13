using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogame_snake
{
    internal class BallSpawner
    {
        public Vector2 BallPosition;
        private (int, int) _gridSize;
        private Random _rng;
        private Texture2D _ballTexture;

        public BallSpawner((int, int) gridSize)
        {
            _rng = new Random();
            _gridSize = gridSize;
            SpawnNew();
        }

        public void LoadContent(ContentManager content)
        {
            _ballTexture = content.Load<Texture2D>("ball");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _ballTexture,
                BallPosition * new Vector2(20, 20) + new Vector2(2f, 2f),
                Color.White
            );
        }

        #region Utility
        public void SpawnNew()
        {
            BallPosition = new Vector2(
                _rng.NextInt64(_gridSize.Item1),
                _rng.NextInt64(_gridSize.Item2)
            );
        }
        #endregion Utility
    }
}
