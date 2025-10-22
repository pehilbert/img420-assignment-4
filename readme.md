# Dungeon Game
In this game, wander around a dungeon and defeat skeletons, earning coins to upgrade yourself. However, the more you upgrade, the more difficult the game gets! Good luck!

## How to Run
To run, you must first open this project in Godot (.NET version). Then, simply build and run from Godot's UI.

## Controls
WASD - Move
Left click - Shoot fireball
C - Toggle Shop

## Assets
- The tileset texture and enemies were download from craftpix.net
- The character is from here: https://sscary.itch.io/the-adventurer-female?download
- The enemies are from here: https://monopixelart.itch.io/skeletons-pack
- Miscellaneous textures like coins, fireball, and heart icon from Google Images

## Project Requirements Explanations

### Particles
There are two particle systems: one for the trail on the fireball the player can shoot, and one for the explosion effect when the fireball hits something.

### Enemies
Enemies will chase players once they are within a certain range and within line of sight, using navigation to avoid obstacles.
