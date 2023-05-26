using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4
{
    class Enemy
    {
        public Rectangle CollideBox { get { return collideBox; } }
        private Rectangle collideBox;
        public Status Status { get { return status; } }
        private Status status = Status.Idle;
        public Vector2 Position { get { return position; } }
        private Vector2 position;
        public Direction Direction { get { return direction; } }
        private Direction direction;
        public int Health { get { return health; } }
        private int health;
        public int MaxHealth { get { return maxHealth; } }
        private int maxHealth;
        public int ImpactForce { get { return impactForce; } }
        private int impactForce;
        public double AttackRange { get { return attackRange; } }
        private double attackRange;
        public int AngryRadius { get { return angryRadius; } }
        private int angryRadius;

        public Vector2 HealthBarPosition;

        private Vector2 directionRun;
        private int speed;
        private int attackInterval;
        private double timeLastAttack;

        public Enemy(Vector2 position, Rectangle collideBox, int health = 50, int speed = 2, int impactForce = 10, int angryRadius = 300, 
            double attackRange = 40, int attackInterval = 800)
        {
            this.position = position;
            this.health = health;
            this.maxHealth = health;
            this.speed = speed;
            this.impactForce = impactForce;
            this.angryRadius = angryRadius;
            this.attackRange = attackRange;
            this.attackInterval = attackInterval;
            this.collideBox = collideBox;
        }

        public void Hit(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - timeLastAttack >= attackInterval)
                status = Status.Hit;
        }

        public void Wave(GameTime gameTime)
        {
            status = Status.Attack;
            timeLastAttack = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public void Move(Vector2 playerPosition, List<Rectangle> collideBoxes) 
        {
            status = Status.Run;
            directionRun = playerPosition - position;
            directionRun.Normalize();
            var potencialPosition = position + directionRun * speed;
            var potencialCollideBox = new Rectangle((int)potencialPosition.X, (int)potencialPosition.Y, 20, 40);
            bool canMove = true;
            foreach(var collideBox in collideBoxes)
                if(!collideBox.Equals(this.collideBox) && potencialCollideBox.Intersects(collideBox))
                    canMove = false;
            if (canMove)
            {
                direction = directionRun.X > 0 ? Direction.Right : Direction.Left;
                position += directionRun * speed;
                for (int i = 0; i < collideBoxes.Count; i++)
                    if (collideBoxes[i].Equals(this.collideBox))
                        collideBoxes[i] = potencialCollideBox;
                collideBox = potencialCollideBox;
            }
            else
            {
                position += new Vector2(1f, 0) * speed;
                direction = new Vector2(1f, 0).X > 0 ? Direction.Right : Direction.Left;
                for (int i = 0; i < collideBoxes.Count; i++)
                    if (collideBoxes[i].Equals(this.collideBox))
                        collideBoxes[i] = new Rectangle((int)position.X, (int)position.Y, 20, 40);
                collideBox = new Rectangle((int)position.X, (int)position.Y, 20, 40);
            }
        }


        public void TakeDamage(int damage)
        {
            health -= damage;
            if (IsDead())
                status = Status.Dead;
        }

        public bool IsDead()
        {
            return health <= 0;
        }

        public void ChangeStatus(Status status)
        {
            this.status = status;
        }
    }
}
