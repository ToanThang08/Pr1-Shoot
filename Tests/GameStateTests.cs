﻿using NUnit.Framework;
using ShootAR;
using ShootAR.TestTools;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class GameStateTests
{
	[UnityTest]
	public IEnumerator UseLastShotToHitCapsuleAndTakeBullets() {
		GameState gameState = GameState.Create(0);
		Player player = Player.Create(
			health: Player.MAXIMUM_HEALTH,
			camera: new GameObject().AddComponent<Camera>(),
			bullet: Bullet.Create(10),
			ammo: 1,
			gameState: gameState);
		Capsule capsule = Capsule.Create(
			type: Capsule.CapsuleType.Bullet,
			speed: 0,
			player: player,
			gameState: gameState);

		// Create an enemy to stop game manager from switching state to "round won".
		var enemy = TestTarget.Create();
		enemy.transform.Translate(Vector3.right * 500f);

		GameManager.Create(player, gameState);

		yield return null;  // without this, player.Shoot() will return null.

		capsule.transform.Translate(new Vector3(10f, 10f, 10f));
		player.transform.LookAt(capsule.transform);
		player.Shoot()
			.gameObject.SetActive(true);


		yield return new WaitWhile(() => player.Ammo > 0);

		Assert.False(gameState.GameOver,
				"The game must not end, if restocked on bullets.");
	}

	[UnityTest]
	public IEnumerator UseLastShotToKillLastEnemy() {
		GameState gameState = GameState.Create(0);
		Camera camera = new GameObject("Camera").AddComponent<Camera>();
		Player player = Player.Create(
			health: Player.MAXIMUM_HEALTH,
			camera: camera,
			bullet: Bullet.Create(100f),
			ammo: 1,
			gameState: gameState);
		TestEnemy enemy = TestEnemy.Create(0, 0, 0, 10, 10, 10, gameState);
		GameManager.Create(player, gameState);

		yield return null;  //without this, firedBullet will be null

		camera.transform.LookAt(enemy.transform);
		var firedBullet = player.Shoot();
		firedBullet.gameObject.SetActive(true);

		yield return new WaitUntil(() => gameState.RoundWon);

		Assert.False(gameState.GameOver,
			"The game must not end if the last enemy dies by the last bullet.");
		Assert.True(gameState.RoundWon,
			"The round should be won when the last enemy dies by the last bullet.");
	}

	[TearDown]
	public void CleanUp() {
		var objects = Object.FindObjectsOfType<GameObject>();
		foreach (var o in objects)
			Object.Destroy(o.gameObject);
	}
}
