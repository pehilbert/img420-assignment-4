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

	private AnimatedSprite2D _anim;

	public override void _Ready()
	{
		// Cache a reference to the AnimatedSprite2D for switching animations and flipping
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
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
	}
}
