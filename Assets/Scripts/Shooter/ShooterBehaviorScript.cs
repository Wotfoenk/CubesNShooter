using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using CubesNShoot.Cubes;
using CubesNShoot.Manager;
using TMPro;
using UnityEngine;

namespace CubesNShoot.Shooter
{
	public class ShooterBehavior : MonoBehaviour
	{
		[SerializeField] private GameObject _bulletPrefab;
		[SerializeField] [Min(0)] private float _shootForce =  10f;
		[SerializeField] [Min(0)] private float _fireRate = 1f;
		[SerializeField] private Transform _attackPoint;  // The point where the bullet will be shot from
		[SerializeField] private Camera _cameraPoint;
		
		private CubeBehavior _victim;
		private float _shootTimer;
		private bool _isNewTarget = false;
		
		public TextMeshProUGUI floatingText;
				
		public void SetShootingState()
		{
			StartCoroutine(ShootingRoutine());
		}
		
		public IEnumerator ShootingRoutine()
		
		{
			while(true)
			{
				if(_victim)
				{
					if(_isNewTarget)
					{
					floatingText.text = "Shooting at:" + _victim.CubeID.ToString();
					yield return new WaitForSeconds(0.5f);
					_isNewTarget=false;
					}
					Shoot();
					yield return new WaitForSeconds(1f/_fireRate);
					
				}
				else
				{
					AquireNewTarget();
				}
			}
		}
		
		private void Shoot()
		{
			Rigidbody currenTargetRB = _victim.GetComponent<Rigidbody>();
			if(_victim.cubeState != CubeState.Still)
			{
				GameObject bulletInstance = Instantiate(_bulletPrefab, _attackPoint.position, _attackPoint.rotation);
				bulletInstance.GetComponent<BulletScript>().nameOnTheBullet = _victim;
				Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
				Vector3 AimCorrection = (currenTargetRB.velocity*Time.deltaTime);
					
				bulletRb.AddForce(Vector3.Normalize(_victim.transform.position - (_attackPoint.position + AimCorrection)) * _shootForce, ForceMode.Impulse);
			}
			else
			{
				AquireNewTarget();
			}
		}
		
		private void AquireNewTarget()
		{
			_victim = CubesManagerScript.Instance.getVictim();
			if(!_victim)
			{
				StopAllCoroutines();
			}
			else
			{
				_isNewTarget=true;
			}
			
		}
		
		void LateUpdate()
		{
			if(_cameraPoint)
			floatingText.rectTransform.anchoredPosition = _cameraPoint.WorldToScreenPoint(transform.position);
		}
	}
}
