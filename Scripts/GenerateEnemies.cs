using System;
using System.Collections.Generic;
using System.Timers;
using Godot;

public class GenerateEnemies {

	private List<KinematicBody2D> EnemiesList = new List<KinematicBody2D> ();

	private static GenerateEnemies _instance = null;

	public static GenerateEnemies instance {
		get {
			if (_instance == null)
				_instance = new GenerateEnemies ();

			return _instance;
		}
	}

	public List<KinematicBody2D> GenerateNewEnemies (PackedScene enemyScene, Vector2 viewSize, int maxEnemies) {
		List<KinematicBody2D> list = new List<KinematicBody2D> ();
		Random rnd = new Random ();

		int xCounter = 0;
		int totalSteps = (int) (viewSize.x / maxEnemies);

		for (int i = 0; i < maxEnemies; i++) {
			var enemy = (KinematicBody2D) enemyScene.Instance ();
			var enemyWidth = (int) enemy.Get ("enemyWidth");
			var enemyHeight = (int) enemy.Get ("enemyHeight");

			xCounter += (enemyWidth + totalSteps);
			if (xCounter >= (viewSize.x + enemyWidth))
				xCounter = 0;

			var randomXPos = xCounter; //rnd.Next ((0 + enemyWidth), ((int) (viewSize.x - enemyWidth)));
			var randomYPos = rnd.Next ((0 + enemyHeight), ((int) ((viewSize.y / 2) - enemyHeight)));

			enemy.Position = new Vector2 (randomXPos, randomYPos);

			list.Add (enemy);
		}

		EnemiesList.AddRange (list);
		return list;
	}

	public void FireBullet () {
		KinematicBody2D enemy = EnemiesList[new Random ().Next (0, EnemiesList.Count)];
		enemy.Call ("FireBullet");
	}

	public int RemoveEnemy (KinematicBody2D enemy) {
		EnemiesList.Remove (enemy);
		return EnemiesList.Count;

	}

	public void RemoveAllEnemyInstance () {

		for (int i = 0; i < EnemiesList.Count; i++) {
			try {
				EnemiesList[i].Free ();
			} catch (Exception e) {

			}
		}
		EnemiesList.Clear ();
	}

	public int Count () {
		return EnemiesList.Count;
	}

}
