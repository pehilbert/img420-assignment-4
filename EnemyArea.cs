using Godot;
using System;

public partial class EnemyArea : Area2D
{
	[Export]
	public int MinEnemies = 1;

	[Export]
	public int MaxEnemies = 3;

	[Export]
	public float Cooldown = 10.0f;

	[Export]
	public PackedScene EnemyScene;

	private bool _canSpawnEnemies = true;
	private Timer _cooldownTimer;
	private int _currentEnemyCount = 0;

	public override void _Ready()
	{
		_cooldownTimer = new Timer();
		_cooldownTimer.WaitTime = Cooldown;
		_cooldownTimer.Timeout += () => _canSpawnEnemies = true;
		AddChild(_cooldownTimer);

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		// spawn enemies when the player enters the area
		if (body is Player player && _canSpawnEnemies && _currentEnemyCount == 0)
		{
			Random random = new Random();
			int enemyCount = random.Next(MinEnemies + (player.NumUpgrades / 2), MaxEnemies + (player.NumUpgrades / 2) + 1);
			for (int i = 0; i < enemyCount; i++)
			{
				if (EnemyScene != null)
				{
					var enemy = EnemyScene.Instantiate() as Enemy;
					enemy.TargetPath = body.GetPath();
					enemy.AttackDamage += player.NumUpgrades * 5;

					var enemyManager = enemy?.GetNode<EntityManager>("EntityManager");

					if (enemyManager != null)
					{
						enemyManager.Died += (Node entityDied) => _currentEnemyCount--;
						enemyManager.MaxHealth += player.NumUpgrades * 12;
					}

					if (enemy != null)
					{
						Vector2 randomOffset = new Vector2(
							(float)(random.NextDouble() - 0.5) * GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size.X,
							(float)(random.NextDouble() - 0.5) * GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size.Y
						);
						enemy.GlobalPosition = GlobalPosition + randomOffset;

						// Defer adding the enemy so we don't modify physics state while queries are flushing
						var parent = GetParent();
						if (parent != null)
							parent.CallDeferred("add_child", enemy);
					}
				}
			}

			_currentEnemyCount += enemyCount;
			_canSpawnEnemies = false;
			_cooldownTimer.Start();
		}
	}
}
