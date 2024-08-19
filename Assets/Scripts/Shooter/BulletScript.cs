using System.Collections;
using System.Collections.Generic;
using CubesNShoot.Cubes;
using CubesNShoot.Manager;
using UnityEngine;

namespace CubesNShoot.Shooter
{
	public class BulletScript : MonoBehaviour
	{
		[SerializeField] private float timeToLive = 5f;
		public CubeBehavior nameOnTheBullet;
		void OnCollisionEnter(Collision collision)
		{
			CubeBehavior _cube = collision.gameObject.GetComponent<CubeBehavior>();
			if (_cube && _cube == nameOnTheBullet)
			{
				_cube.ShotByBullet();
			}
		}
		
		void Update()
		{
			timeToLive -= Time.deltaTime;
			if(timeToLive<=0)Destroy(this.gameObject);
			
		}

	}
}
