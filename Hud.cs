using Godot;
using System;

public partial class Hud : CanvasLayer
{
	//private Level _level;
	private Player _player;
	private EntityManager _playerEntityManager;
	private Label _coinLabel;
	private Label _healthLabel;

	public override void _Ready()
	{
		base._Ready();

		_player = GetNode<Player>("/root/Level/Player");
		_playerEntityManager = GetNode<EntityManager>("/root/Level/Player/EntityManager");
		_coinLabel = GetNode<Label>("CoinLabel");
		_healthLabel = GetNode<Label>("HealthLabel");

		_coinLabel.Text = _player.Coins.ToString();

		_player.CoinsChanged += (int coins) => {
			_coinLabel.Text = coins.ToString();
		};

		_healthLabel.Text = $"{_playerEntityManager.CurrentHealth} / {_playerEntityManager.MaxHealth}";

		_playerEntityManager.HealthChanged += (double currentHealth, double maxHealth) => {
			_healthLabel.Text = $"{currentHealth} / {maxHealth}";
		};
	}
}
