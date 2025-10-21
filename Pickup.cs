using Godot;

/// <summary>
/// A basic collectible item.  When the player enters its Area2D, it spawns an
/// optional particle effect and then removes itself from the scene.  Attach this
/// script to an Area2D node with a CollisionShape2D and, optionally, assign a
/// Particles2D scene to the ParticlesScene property.
/// </summary>
public partial class Pickup : Area2D
{
	/// <summary>
	/// A PackedScene pointing to a Particles2D node that will be instantiated when the pickup is collected.
	/// </summary>
	[Export]
	public PackedScene ParticlesScene;

	[Export]
	public int Value = 1;

	private AnimatedSprite2D _anim;

	public override void _Ready()
	{
		_anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_anim.Play("default");

		// Connect the body_entered signal to detect when the player touches the pickup
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	private void OnBodyEntered(Node2D body)
	{
		// Only react to the Player (customise this check as needed)
		if (body is Player player)
		{
			// Spawn particle effect at pickup position
			if (ParticlesScene != null)
			{
				var particles = ParticlesScene.Instantiate() as Node2D;
				if (particles != null)
				{
					particles.GlobalPosition = GlobalPosition;
					GetParent().AddChild(particles);
				}
			}

			player.AddCoins(Value);

			// Remove the pickup from the scene
			QueueFree();
		}
	}
}
