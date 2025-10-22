using Godot;
using System;

public partial class Shop : Control
{
	[Export]
	public int DamageBoostCost = 10;

	[Export]
	public int FireRateBoostCost = 10;

	[Export]
	public int SpeedBoostCost = 5;

	private Player _player;
	private Button _damageBoostButton;
	private Button _fireRateBoostButton;
	private Button _speedBostButton;

	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Level/Player");
		_damageBoostButton = GetNode<Button>("DamageBoost");
		_fireRateBoostButton = GetNode<Button>("FireRateBoost");
		_speedBostButton = GetNode<Button>("SpeedBoost");

		_damageBoostButton.Text = $"+10% damage ({DamageBoostCost} Coins)";
		_fireRateBoostButton.Text = $"+0.5 fire rate ({FireRateBoostCost} Coins)";
		_speedBostButton.Text = $"+5% movement speed ({SpeedBoostCost} Coins)";

		_damageBoostButton.Pressed += OnDamageBoostButtonPressed;
		_fireRateBoostButton.Pressed += OnFireRateBoostButtonPressed;
		_speedBostButton.Pressed += OnSpeedBoostButtonPressed;
	}

	private void OnDamageBoostButtonPressed()
	{
		if (_player.Coins >= DamageBoostCost)
		{
			_player.Coins -= DamageBoostCost;
			_player.Damage += 10;
			_player.EmitSignal(nameof(Player.CoinsChanged), _player.Coins);
			_player.NumUpgrades++;
		}
	}

	private void OnFireRateBoostButtonPressed()
	{
		if (_player.Coins >= FireRateBoostCost)
		{
			_player.Coins -= FireRateBoostCost;
			_player.FireRate += 0.5f;
			_player.EmitSignal(nameof(Player.CoinsChanged), _player.Coins);
			_player.NumUpgrades++;
		}
	}

	private void OnSpeedBoostButtonPressed()
	{
		if (_player.Coins >= SpeedBoostCost)
		{
			_player.Coins -= SpeedBoostCost;
			_player.Speed += 10f;
			_player.EmitSignal(nameof(Player.CoinsChanged), _player.Coins);
			_player.NumUpgrades++;
		}
	}
}
