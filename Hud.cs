using Godot;
using System;

public partial class Hud : CanvasLayer
{
	//private Level _level;
	private Player _player;
	private EntityManager _playerEntityManager;
	private Label _coinLabel;
	private Label _healthLabel;
	private Node2D _deathScreen;
	private Button _playAgainButton;
	private Shop _shop;

	public override void _Ready()
	{
		base._Ready();

		_player = GetNode<Player>("/root/Level/Player");
		_playerEntityManager = GetNode<EntityManager>("/root/Level/Player/EntityManager");
		_coinLabel = GetNode<Label>("CoinLabel");
		_healthLabel = GetNode<Label>("HealthLabel");
		_deathScreen = GetNode<Node2D>("DeathScreen");
		_playAgainButton = _deathScreen?.GetNode<Button>("PlayAgainButton");
		_shop = GetNode<Shop>("Shop");

		_coinLabel.Text = _player.Coins.ToString();

		_player.CoinsChanged += (int coins) => {
			_coinLabel.Text = coins.ToString();
		};

		_healthLabel.Text = $"{_playerEntityManager.CurrentHealth} / {_playerEntityManager.MaxHealth}";

		_playerEntityManager.HealthChanged += (double currentHealth, double maxHealth) => {
			_healthLabel.Text = $"{currentHealth} / {maxHealth}";
		};

		if (_playAgainButton != null)
		{
			_playAgainButton.Pressed += _playAgainButton_Pressed;
		}

		_playerEntityManager.Died += (Node player) =>
		{
			_deathScreen.Visible = true;
		};
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("shop"))
		{
			if (_shop != null)
			{
				_shop.Visible = !_shop.Visible;
			}
		}
	}

	private void _playAgainButton_Pressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
