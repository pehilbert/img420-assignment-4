using Godot;
using System;

/// <summary>
/// A basic player controller for a 2D game.  This script demonstrates simple
/// physics (movement, gravity and jumping), collision handling via CharacterBody2D
/// and playing idle/walk animations on an AnimatedSprite2D child.  To use this
/// script, attach it to a CharacterBody2D node in your scene and add an
/// AnimatedSprite2D and CollisionShape2D as children.  Define the animations
/// "idle" and "walk" in the AnimatedSprite2D's SpriteFrames resource.
/// </summary>
public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 80f;
	[Export]
	public PackedScene FireballScene;
	[Export]
	public PackedScene ExplosionScene;
	[Export]
	public float FireballSpeed = 200f;
	[Export]
	public int Damage = 10;
	[Export]
	public float FireRate = 1.0f;

	public int Coins = 0;
	public int NumUpgrades = 0;

	[Signal]
	public delegate void CoinsChangedEventHandler(int coins);

	private AnimatedSprite2D _anim;
	private bool _canFire = true;
	private Timer _fireTimer;

	public override void _Ready()
	{
		// Cache a reference to the AnimatedSprite2D for switching animations and flipping
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		_fireTimer = new Timer();
		_fireTimer.WaitTime = 1.0f / FireRate;
		_fireTimer.Timeout += () => 
		{
			_canFire = true;
			_fireTimer.WaitTime = 1.0f / FireRate;
		};
		AddChild(_fireTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		// Obtain the current velocity so we can modify it
		Vector2 v = Vector2.Zero;

		if (Input.IsActionPressed("move_left"))
		{
			v.X -= Speed;
		}
		if (Input.IsActionPressed("move_right"))
		{
			v.X += Speed;
		}
		if (Input.IsActionPressed("move_up"))
		{
			v.Y -= Speed;
		}
		if (Input.IsActionPressed("move_down"))
		{
			v.Y += Speed;
		}

		// Assign the modified velocity back to the CharacterBody2D and move
		Velocity = v.Normalized() * Speed;
		MoveAndSlide();

		// Flip and play animations based on movement
		if (_anim != null)
		{
			if (Math.Abs(v.X) > 0)
			{
				_anim.FlipH = v.X < 0;
				_anim.Play("walk_side");
			}
			else if (v.Y > 0)
			{
				_anim.Play("walk_down");
			}
			else if (v.Y < 0)
			{
				_anim.Play("walk_up");
			}
			else
			{
				_anim.Play("idle");
			}
		}

		// Fire a projectile if the fire action is pressed
		if (Input.IsActionPressed("fire"))
		{
			fire();
		}
	}

	public void AddCoins(int amount)
	{
		Coins += amount;
		EmitSignal(SignalName.CoinsChanged, Coins);
	}

	public void SetCoins(int amount)
	{
		Coins = amount;
		EmitSignal(SignalName.CoinsChanged, Coins);
	}

	private void fire()
	{
		if (_canFire)
		{
			_canFire = false;
			_fireTimer.Start();

			var bullet = FireballScene.Instantiate<RigidBody2D>();
			bullet.Position = GlobalPosition;
			var mousePos = GetGlobalMousePosition();

			bullet.LookAt(mousePos);
			bullet.LinearVelocity = (mousePos - GlobalPosition).Normalized() * FireballSpeed;

			// Enable contact monitoring for collision detection
			bullet.ContactMonitor = true;

			bullet.BodyEntered += (Node body) =>
			{
				if (!(body is Player))
				{
					var entityManager = body.GetNodeOrNull<EntityManager>("EntityManager");
					if (entityManager != null)
					{
						entityManager.TakeDamage(Damage);
					}

					var explosion = ExplosionScene.Instantiate<CpuParticles2D>();
					explosion.Position = bullet.Position;
					explosion.Emitting = true;
					GetParent().AddChild(explosion);

					bullet.QueueFree();
				}
			};

			GetParent().AddChild(bullet);
		}
	}
}
