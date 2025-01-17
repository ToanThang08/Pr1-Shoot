﻿using System.Collections;
using UnityEngine;

namespace ShootAR.Enemies
{
	[RequireComponent(typeof(CapsuleCollider))]
	public class Drone : Pyoopyoo
	{
		[SerializeField] protected const float ShootDelay = 5f;
		/// <summary>
		/// The point in time that the next shot is allowed.
		/// </summary>
		protected float nextShot;

		protected void FixedUpdate() {
			if (!gameState.GameOver) {
				if (lastBullet == null && Time.time > nextShot) {
					Shoot();
					nextShot = Time.time + ShootDelay;
				}

				//OrbitAround();
			}
		}
	}
}
