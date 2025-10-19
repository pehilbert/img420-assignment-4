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

	/// <summary>
	/// Exposed NodePath to assign the target (e.g. Player) in the editor.
	/// </summary>
	[Export]
	public NodePath TargetPath;

	private NavigationAgent2D _navAgent;
	private Node2D _target;
	private AnimatedSprite2D _anim;
	
	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		_anim.Play("idle");

		if (TargetPath != null)
		{
			_target = GetNode<Node2D>(TargetPath);
		}

		// Configure navigation agent properties if needed (e.g., max speed)
		// _navAgent.MaxSpeed = Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_target == null || _navAgent == null)
			return;

		// Update the navigation target each frame to follow the player's current position
		_navAgent.TargetPosition = _target.GlobalPosition;

		// Retrieve the next point along the computed path
		Vector2 nextPoint = _navAgent.GetNextPathPosition();

		// Compute direction towards the next point
		Vector2 direction = (nextPoint - GlobalPosition).Normalized();

		// Move towards the target
		Velocity = direction * Speed;

		if (Math.Abs(Velocity.Length()) > 0)
		{
			_anim.FlipH = Velocity.X < 0;
			_anim.Play("walk");
		}
		else
		{
			_anim.Play("idle");
		}

		MoveAndSlide();
	}
}
