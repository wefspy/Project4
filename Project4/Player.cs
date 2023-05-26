using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Project4;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;

public class Player
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
    private int health = 50;
    public int MaxHealth { get { return maxHealth; } }
    private int maxHealth = 50;
    public int ImpactForce { get { return impactForce; } }
    private int impactForce = 20;
    public int AttackRange { get { return attackRange; } }
    private int attackRange;

    public Vector2 HealthBarPosition;

    private int speed = 3;
    private int attackInterval;
    private double timeLastAttack;

    public Player(Vector2 position, int attackRange = 40, int attackInterval = 500)
    {
        this.position = position;
        this.attackRange = attackRange;
        this.attackInterval = attackInterval;
    }

    public void Hit(GameTime gameTime)
    {
        if(gameTime.TotalGameTime.TotalMilliseconds - timeLastAttack >= attackInterval)
            status = Status.Hit;
    }

    public void Wave(GameTime gameTime)
    {
        status = Status.Attack;
        timeLastAttack = gameTime.TotalGameTime.TotalMilliseconds;
    }

    public void Move(KeyboardState keyboardState, List<Rectangle> collideBoxes)
    {
        status = Status.Run;
        if (keyboardState.IsKeyDown(Keys.W))
        {
            position.Y -= speed;
        }
        else if (keyboardState.IsKeyDown(Keys.A))
        {
            position.X -= speed;
            direction = Direction.Left;
        }
        else if (keyboardState.IsKeyDown(Keys.S))
        {
            position.Y += speed;
        }
        else if (keyboardState.IsKeyDown(Keys.D))
        {
            position.X += speed;
            direction = Direction.Right;
        }
        collideBox = new Rectangle((int)position.X, (int)position.Y, 40, 40);
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

    public void UpdateHealth(int newHealth)
    {
        health = newHealth;
    }
    public void UpdateRandomAttackSkill()
    {
        Random random = new Random();
        switch (random.Next(0, 1))
        {
            case 0:
                impactForce++;
                break;
            case 1:
                attackRange++;
                break;
        }
    }
}
