using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4
{
    public class HitUpgrade
    {
        public Vector2 Position { get { return position; } }
        private Vector2 position;
        public Rectangle CollideBox { get { return collideBox; } }
        private Rectangle collideBox;
        public bool IsCollected { get { return isCollected; } }
        bool isCollected;

        public HitUpgrade(Texture2D texture, Vector2 position)
        {
            this.position = position;
            collideBox = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            isCollected = false;
        }

        public void Collect(Player player)
        {
            if (!isCollected && collideBox.Intersects(player.CollideBox))
            {
                player.UpdateRandomAttackSkill();
                isCollected = true;
            }
        }
    }
}
