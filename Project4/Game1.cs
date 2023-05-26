using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Project4;
class Game1 : Game
{
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    Texture2D backgroundTexture;

    Player player;
    Animate playerAnimate;
    List<Texture2D> playerIdle = new List<Texture2D>();
    List<Texture2D> playerRun = new List<Texture2D>();
    List<Texture2D> playerDie = new List<Texture2D>();
    List<Texture2D> playerAttack = new List<Texture2D>();
    List<Texture2D> playerHit = new List<Texture2D>();

    List<(Animate,Enemy)> enemies = new List<(Animate,Enemy)>();
    List<Texture2D> enemyIdle = new List<Texture2D>();
    List<Texture2D> enemyRun = new List<Texture2D>();
    List<Texture2D> enemyDie = new List<Texture2D>();
    List<Texture2D> enemyAttack = new List<Texture2D>();
    List<Texture2D> enemyHit = new List<Texture2D>();

    Point EnemySize;
    double timerSpawnEnemy;
    double timeLastSpawnEnemy;

    List<Rectangle> collideBoxes = new List<Rectangle>();

    Texture2D HealthPackTexture;
    List<HealthPack> healthPacks = new List<HealthPack>();

    Texture2D HitUpgradeTexture;
    List<HitUpgrade> HitUpgrades = new List<HitUpgrade>();

    Random random = new Random();
    double timerSpawnPacks;
    double lastTimeSpawnPacks;

    Texture2D healthBarTexture;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        backgroundTexture = Content.Load<Texture2D>("backgroundTexture");

        // Загрузка спрайтов Player
        for (var i = 0; i < 4; i++) playerIdle.Add(Content.Load<Texture2D>($"PlayerIdle{i}"));
        for (var i = 0; i < 9; i++) playerRun.Add(Content.Load<Texture2D>($"PlayerRun{i}"));
        playerDie.Add(Content.Load<Texture2D>($"PlayerDie8"));
        for(var i = 0; i < 6; i++) playerAttack.Add(Content.Load<Texture2D>($"PlayerAttack{i}"));
        playerHit.Add(Content.Load<Texture2D>("PlayerAttack5"));

        player = new Player(new Vector2(400, 300));
        playerAnimate = new Animate((Status.Idle, playerIdle, 100), (Status.Run, playerRun, 100),
            (Status.Dead, playerDie, 100), (Status.Attack, playerAttack, 100), (Status.Hit, playerHit, 100));

        // Загрузка спрайтов Enemy
        for (var i = 0; i < 4; i++) enemyIdle.Add(Content.Load<Texture2D>($"EnemyIdle{i}"));
        for (var i = 0; i < 8; i++) enemyRun.Add(Content.Load<Texture2D>($"EnemyRun{i}"));
        for (var i = 0; i < 1; i++) enemyDie.Add(Content.Load<Texture2D>($"EnemyDie{i}"));
        for (var i = 0; i < 8; i++) enemyAttack.Add(Content.Load<Texture2D>($"EnemyAttack{i}"));
        enemyHit.Add(Content.Load<Texture2D>("EnemyAttack7"));

        HealthPackTexture = Content.Load<Texture2D>("HealthPack");
        HitUpgradeTexture = Content.Load<Texture2D>("AttackUpgrade");

        EnemySize = new Point(20, 40);

        // Загружаем ХП Бар
        spriteBatch = new SpriteBatch(GraphicsDevice);
        healthBarTexture = new Texture2D(GraphicsDevice, 1, 1);
        healthBarTexture.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (player.Status != Status.Dead)
            base.Update(gameTime);

        // Спавним Enemy каждые N секунд
        timerSpawnEnemy = gameTime.TotalGameTime.TotalMilliseconds;
        if (timerSpawnEnemy - timeLastSpawnEnemy >= 5000)
        {
            SpawnEnemy();
            timeLastSpawnEnemy = timerSpawnEnemy;
        }

        // Спавн Enemy.
        void SpawnEnemy()
        {
            int x = 0;
            int y = 0;
            switch (random.Next(0,4))
            {
                case 0:
                    x = 0; y = 0;
                    break;
                case 1:
                    x = 0; y = graphics.PreferredBackBufferHeight;
                    break;
                case 2:
                    x = graphics.PreferredBackBufferWidth; y = 0;
                    break;
                case 3:
                    x = graphics.PreferredBackBufferWidth; y = graphics.PreferredBackBufferHeight;
                    break;
            }
            Vector2 position = new Vector2(x, y);
            var collideBox = new Rectangle((int)position.X, (int)position.Y, EnemySize.X, EnemySize.Y);
            Enemy enemy = new Enemy(position, collideBox);
            enemies.Add((
                new Animate((Status.Idle, enemyIdle, 100), (Status.Run, enemyRun, 100), (Status.Dead, enemyDie, 100), (Status.Attack, enemyAttack, 100), (Status.Hit, enemyHit, 100)),
                enemy));
            collideBoxes.Add(collideBox);
        }

        // Обновление ХП Бара Player
        player.HealthBarPosition = new Vector2(player.Position.X - healthBarTexture.Width / 2, player.Position.Y - healthBarTexture.Height - 10);
        // Player Update
        if (player.IsDead()) player.ChangeStatus(Status.Dead);
        else if (player.Status == Status.Attack) player.Hit(gameTime);
        else if (keyboardState.IsKeyDown(Keys.Space)) player.Wave(gameTime);
        else if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.A)
            || keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.D))
            player.Move(keyboardState, collideBoxes);
        else player.ChangeStatus(Status.Idle);

        foreach (var tuple in enemies)
        {
            var enemy = tuple.Item2;

            // Enemy Update
            double distanceToPlayer = Vector2.Distance(player.Position, enemy.Position);
            if (enemy.IsDead()) enemy.ChangeStatus(Status.Dead);
            else if (enemy.Status == Status.Attack) enemy.Hit(gameTime);
            else if (distanceToPlayer <= enemy.AttackRange) enemy.Wave(gameTime);
            else enemy.Move(player.Position, collideBoxes);

            // Подразумеваем, что Enemy всегда смотрит на Player.
            if (enemy.Status == Status.Hit && (player.Position - enemy.Position).Length() <= enemy.AttackRange) 
                player.TakeDamage(enemy.ImpactForce);
            if (player.Status == Status.Hit && (player.Position - enemy.Position).Length() <= player.AttackRange && player.Direction != enemy.Direction) 
                enemy.TakeDamage(player.ImpactForce);

            // Обновление ХП Бара Enemy
            enemy.HealthBarPosition = new Vector2(enemy.Position.X - healthBarTexture.Width / 2, enemy.Position.Y - healthBarTexture.Height - 10);
        }
        
        // Удаляем убитых врагов
        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].Item2.Status == Status.Dead)
            {
                enemies.RemoveAt(i);
                collideBoxes.RemoveAt(i);
            }
        }

        // Добавляем HealthPack или HitUpgrade
        timerSpawnPacks = gameTime.TotalGameTime.TotalSeconds;
        if (timerSpawnPacks - lastTimeSpawnPacks >= 6)
        {
            var randomInt = random.Next(0, 2);
            switch (randomInt)
            {
                case 0:
                    SpawnHealthPack();
                    break;
                case 1:
                    SpawnHitUpgrade();
                    break;
            }
            lastTimeSpawnPacks = timerSpawnPacks;
        }

        // Обновляем HealthPack'и
        foreach (var healthPack in healthPacks)
            healthPack.Collect(player);
        healthPacks.RemoveAll(hp => hp.IsCollected);

        // Обновляем HitUpgrade'ы
        foreach (var attackUpgrade in HitUpgrades)
            attackUpgrade.Collect(player);
        HitUpgrades.RemoveAll(hp => hp.IsCollected);

        void SpawnHealthPack()
        {
            int x = random.Next(0, graphics.PreferredBackBufferWidth - HealthPackTexture.Width);
            int y = random.Next(0, graphics.PreferredBackBufferHeight - HealthPackTexture.Height);
            HealthPack healthPack = new HealthPack(HealthPackTexture, new Vector2(x, y));
            healthPacks.Add(healthPack);
        }

        void SpawnHitUpgrade()
        {
            int x = random.Next(0, graphics.PreferredBackBufferWidth - HitUpgradeTexture.Width);
            int y = random.Next(0, graphics.PreferredBackBufferHeight - HitUpgradeTexture.Height);
            HitUpgrade hitUpgrade = new HitUpgrade(HitUpgradeTexture, new Vector2(x, y));
            HitUpgrades.Add(hitUpgrade);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

        // Отрисовка заднего фона
        spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

        // Отрисовка HealthPack'ов
        foreach (var healthPack in healthPacks)
            spriteBatch.Draw(HealthPackTexture, healthPack.Position, Color.White);

        // Отрисовка HitUpgrade'ов
        foreach (var hitUpgrade in HitUpgrades)
            spriteBatch.Draw(HitUpgradeTexture, hitUpgrade.Position, Color.White);

        // Отрисовка ХП Бара player
        Rectangle healthBarRect = new Rectangle((int)player.HealthBarPosition.X, (int)player.HealthBarPosition.Y, player.Health, 5);
        Rectangle backgroundHealthBarRect = new Rectangle((int)player.HealthBarPosition.X, (int)player.HealthBarPosition.Y, player.MaxHealth, 5);
        spriteBatch.Draw(healthBarTexture, backgroundHealthBarRect, Color.Gray);
        spriteBatch.Draw(healthBarTexture, healthBarRect, Color.Red);

        // Отрисовка Player
        var playerSpriteEffects = player.Direction == Direction.Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        spriteBatch.Draw(playerAnimate.GetSprite(gameTime, player.Status), 
            player.Position,
            null,
            Color.White,
            0,
            Vector2.Zero,
            1f,
            playerSpriteEffects,
            0);

        // Отрисовка enemy
        foreach (var tuple in enemies)
        {
            var enemyAnimate = tuple.Item1;
            var enemy = tuple.Item2;

            // Отрисовка enemy
            var enemySpriteEffects = enemy.Direction == Direction.Left ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(enemyAnimate.GetSprite(gameTime, enemy.Status),
            enemy.Position,
            null,
            Color.White,
            0,
            Vector2.Zero,
            1f,
            enemySpriteEffects,
            0);

            // Отрисовка ХП Бара Enemy
            Rectangle enmHealthBarRect = new Rectangle((int)enemy.HealthBarPosition.X, (int)enemy.HealthBarPosition.Y, enemy.Health, 5);
            Rectangle enmbackgroundHealthBarRect = new Rectangle((int)enemy.HealthBarPosition.X, (int)enemy.HealthBarPosition.Y, enemy.MaxHealth, 5);
            spriteBatch.Draw(healthBarTexture, enmbackgroundHealthBarRect, Color.Gray);
            spriteBatch.Draw(healthBarTexture, enmHealthBarRect, Color.Red);
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }
}