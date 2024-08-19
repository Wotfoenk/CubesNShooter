using System.Collections;
using System.Collections.Generic;
using CubesNShoot.Manager;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CubesNShoot.Cubes
{
	public class CubeBehavior : MonoBehaviour
	{
		[SerializeField] private float _speed = 2.0f;
		[SerializeField] private float _chaserSpeed = 2.0f;
		private  Vector3 _initialScale;
		private int _cubeID;
		private Vector3 _direction;
		private Rigidbody _cubeRigitBody;

		[HideInInspector] public CubeState cubeState = CubeState.Default;
		[HideInInspector] public Camera cameraPoint;
		[HideInInspector] public CubeBehavior targetVictim;
		
		public TextMeshProUGUI floatingText;
		
		public int CubeID
		{
			get { return _cubeID; }
			set 
			{
				if (value > 0) 
				{
					_cubeID = value; 
				}
			}
		}
		
		private void OnEnable() 
		{
			_initialScale = transform.localScale;
			_cubeRigitBody = GetComponent<Rigidbody>();
			SetRandomDirection();
		}
		
		private void DestroyCube()
		{
			CubesManagerScript.Instance.OnCubeDestroyed(this);
			if(floatingText)
			{
				Destroy(floatingText.gameObject);
			}
			Destroy(gameObject);
		}
		
		void LateUpdate()
		{
			if(cameraPoint)
			floatingText.rectTransform.anchoredPosition = cameraPoint.WorldToScreenPoint(transform.position);
		}
		
		private void FixedUpdate() 
		{
				switch(cubeState)
				{
					case CubeState.Still:
					StillState();
					break;
					case CubeState.Moving:
					MovingState();
					break;
					case CubeState.Chaser:
					ChaseState();
					break;
					default:break;
				}
		}
		
		void StillState()
		{
			_cubeRigitBody.velocity = Vector3.zero;
	 		_cubeRigitBody.constraints = RigidbodyConstraints.FreezeAll;
		}
		
		void MovingState()
		{
			_cubeRigitBody.constraints = RigidbodyConstraints.FreezeRotation;
			_cubeRigitBody.velocity = new Vector3(_direction.x * _speed, _cubeRigitBody.velocity.y, _direction.z * _speed);
		}
		
		void ChaseState()
		{
			_cubeRigitBody.constraints = RigidbodyConstraints.FreezeRotation;
			transform.localScale = Vector3.Lerp(transform.localScale, _initialScale * 2f, Time.deltaTime);
			if(targetVictim)
			{
				MoveTo(targetVictim.transform.position);
			}
			
			else
			{
				targetVictim = CubesManagerScript.Instance.getVictim();
				if(!targetVictim)
				cubeState = CubeState.Still;
			}
		}

		public void ShotByBullet()
		{
			cubeState = CubeState.Still;
			CubesManagerScript.Instance.RemoveFromAlive(this);
		}
		
		 private void SetRandomDirection()
		 {
		 	_direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
		 }
		
		private void OnCollisionEnter(Collision other)
		{
			if (cubeState==CubeState.Moving && (other.gameObject.tag == "Wall" || other.gameObject.tag == "Cube"))
			{
				BounceOffWall(other.contacts[0].normal);
			}
			
			if(cubeState==CubeState.Chaser)
			{
				CubeBehavior cube  = other.gameObject.GetComponent<CubeBehavior>();
				if(cube)
				{
					cube.DestroyCube();
				}
			}
		}
		
		private void BounceOffWall(Vector3 wallNormal)
		{
				_direction = Vector3.Reflect(_direction, wallNormal);
		}
		
		private void MoveTo(Vector3 target)
		{
			Vector2 aimDirecton = (new Vector2(target.x, target.z) - new Vector2(transform.position.x, transform.position.z)).normalized;
			_cubeRigitBody.velocity = new Vector3(aimDirecton.x * _chaserSpeed, _cubeRigitBody.velocity.y, aimDirecton.y * _chaserSpeed);
		}
	}
}
