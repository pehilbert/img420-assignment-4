using Godot;
using System;

/// <summary>
/// A simple enemy that uses a NavigationAgent2D to follow the player.  To use this
/// script, create a CharacterBody2D with a NavigationAgent2D child.  In the
/// inspector, set the "TargetPath" export to point to the Player node.  Ensure
/// your TileSet has a NavigationLayer painted on walkable tiles and that your
/// TileMap has a NavigationRegion2D so the agent can compute paths.  The enemy
/// will continuously update its target and move along the computed path.
/// </summary>
public partial class Enemy : CharacterBody2D
{
	[Export]
	public float Speed = 50f;

	[Export]
	public float DetectionRadius = 200f;

	[Export]
	public float AttackRadius = 16f;

	[Export]
	public float AttackRange = 16f;

	[Export]
	public int AttackDamage = 10;

	[Export]
	public float AttackCooldown = 3.0f;

	[Export]
	public PackedScene CoinScene;

	[Export]
	public int MinCoinsDropped = 1;

	[Export]
	public int MaxCoinsDropped = 3;

	/// <summary>
	/// Exposed NodePath to assign the target (e.g. Player) in the editor.
	/// </summary>
	[Export]
	public NodePath TargetPath;

	private NavigationAgent2D _navAgent;
	private Node2D _target;
	private AnimatedSprite2D _anim;
	private bool _isChasing = false;
	private bool _isAttacking = false;
	private bool _canAttack = true;
	private Timer _attackTimer;
	private EntityManager _entityManager;

	// Reuse a single RayCast2D instead of creating one every frame.
	private RayCast2D _raycast;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_entityManager = GetNode<EntityManager>("EntityManager");

		_anim.Play("idle");

		if (TargetPath != null)
		{
			_target = GetNode<Node2D>(TargetPath);
		}

		// Create and configure a RayCast2D once.
		_raycast = new RayCast2D
		{
			// position the ray origin at the enemy's local origin (0,0)
			Position = Vector2.Zero,
			CollideWithAreas = true,
			CollideWithBodies = true,
			Enabled = true
		};
		AddChild(_raycast);

		_anim.AnimationFinished += AttackHit;
		
		_attackTimer = new Timer();
		_attackTimer.WaitTime = AttackCooldown;
		_attackTimer.Timeout += _attackTimer_Timeout;
		AddChild(_attackTimer);

		_entityManager.Died += OnDied;
	}

	private void _attackTimer_Timeout()
	{
		_canAttack = true;
	}

	private void OnDied(Node entityDied)
	{
		// Drop coins upon death
		var rand = new Random();
		int coinsToDrop = rand.Next(MinCoinsDropped, MaxCoinsDropped + 1);
		for (int i = 0; i < coinsToDrop; i++)
		{
			if (CoinScene != null)
			{
				int xOffset = rand.Next(-8, 9);
				int yOffset = rand.Next(-8, 9);

				var coin = CoinScene.Instantiate<Node2D>();
				coin.GlobalPosition = this.GlobalPosition + new Vector2(xOffset, yOffset);

				// Defer adding to the scene tree to avoid changing physics state while flushing queries.
				var parent = GetParent();
				if (parent != null)
					parent.CallDeferred("add_child", coin);
			}
		}
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_target == null || _navAgent == null)
			return;

		// Update the navigation target each frame to follow the player's current position
		_navAgent.TargetPosition = _target.GlobalPosition;

		Vector2 distance = _target.GlobalPosition - GlobalPosition;

		// Begin chasing only if within detection radius AND we have line-of-sight.
		if (distance.Length() <= DetectionRadius && IsTargetInLineOfSight())
		{
			_isChasing = true;
		}

		if (_isChasing && !_isAttacking && distance.Length() > AttackRadius)
		{
			// Retrieve the next point along the computed path
			Vector2 nextPoint = _navAgent.GetNextPathPosition();

			// Compute direction towards the next point
			Vector2 direction = (nextPoint - GlobalPosition).Normalized();

			// Move towards the target
			Velocity = direction * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}

		// Update animation based on movement if not attacking
		if (!_isAttacking)
		{
			if (_canAttack && distance.Length() <= AttackRadius && IsTargetInLineOfSight())
			{
				Attack();
			}
			else if (Math.Abs(Velocity.Length()) > 0)
			{
				_anim.FlipH = Velocity.X < 0;
				_anim.Play("walk");
			}
			else
			{
				_anim.Play("idle");
			}
		}

		MoveAndSlide();
	}

	private void Attack()
	{
		if (_isAttacking || !_canAttack)
			return;
		
		_isAttacking = true;
		_canAttack = false;
		_anim.Play("attack");
		_attackTimer.Start();
	}

	private void AttackHit()
	{
		if (_anim.Animation == "attack")
		{
			if (_target != null && IsTargetInLineOfSight())
			{
				Vector2 distance = _target.GlobalPosition - GlobalPosition;

				if (distance.Length() <= AttackRange)
				{
					var entityManager = _target.GetNodeOrNull<EntityManager>("EntityManager");

					if (entityManager != null)
					{
						entityManager.TakeDamage(AttackDamage);
					}
				}
			}

			_isAttacking = false;
		}
	}

	private bool IsTargetInLineOfSight()
	{
		// Update the raycast to point at the target and check line of sight. (assisted by GitHub Copilot)
		_raycast.GlobalPosition = GlobalPosition;
		_raycast.TargetPosition = _target.GlobalPosition - GlobalPosition;
		_raycast.ForceRaycastUpdate();

		bool hasLineOfSight = false;

		if (_raycast.IsColliding())
		{
			// If the first collider hit is the target, we have line of sight.
			var collider = _raycast.GetCollider() as Node;
			if (collider == _target)
			{
				hasLineOfSight = true;
			}
			else
			{
				// As a fallback, check whether the collision point is very near the target (in case of collider wrappers).
				Vector2 collisionPoint = _raycast.GetCollisionPoint();
				if (collisionPoint.DistanceTo(_target.GlobalPosition) < 8.0f)
					hasLineOfSight = true;
			}
		}
		else
		{
			// No collider was hit at all. If your player is a physics body this case probably won't happen;
			// treat it as "no line of sight" unless you expect non-colliding targets.
			hasLineOfSight = false;
		}

		return hasLineOfSight;
	}
}
