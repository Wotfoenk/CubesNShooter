using System.Collections;
using System.Collections.Generic;
using CubesNShoot.Cubes;
using TMPro;
using UnityEngine;

namespace CubesNShoot.Manager
{
	public enum CubeState
	{
		Default,
		Still,
		Moving,
		Chaser
	}
	
	public class CubesManagerScript : MonoBehaviour
	{

		[SerializeField] private Camera _mainCam;
		[SerializeField] private Transform _cubesIDCanvas;
		[SerializeField] private GameObject _cubeWalkerPrefab;
		[SerializeField] private GameObject _cubeSpawnZone;
		[SerializeField] [Min(1)] private int _cubesAmount;
		
		private List<CubeBehavior> _cubesInstancesList;
		private List<CubeBehavior> _cubesAliveInstancesList;
		
		public static CubesManagerScript Instance;
		
		private void Awake() {
			if (!Instance)
			{
				Instance = this;
			}
		}
		

		public void Spawn()
		{
			_cubesInstancesList = new List<CubeBehavior>();
			Bounds _cubeSpawnZoneBounds = _cubeSpawnZone.GetComponent<BoxCollider>().bounds;
			int cubesToSpawn = Random.Range(1, _cubesAmount + 1);
			for (int i = 0; i < cubesToSpawn; i++)
			{
				int createdCubeID = i + 1;
				
				float spawnPosX = Random.Range(_cubeSpawnZoneBounds.min.x, _cubeSpawnZoneBounds.max.x);
				float spawnPosY = Random.Range(_cubeSpawnZoneBounds.min.y, _cubeSpawnZoneBounds.max.y);
				float spawnPosZ = Random.Range(_cubeSpawnZoneBounds.min.z, _cubeSpawnZoneBounds.max.z);
				Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
				
				GameObject cubeWalkerInstance = Instantiate(_cubeWalkerPrefab, spawnPos, Quaternion.identity);
				CubeBehavior cube = cubeWalkerInstance.GetComponentInChildren<CubeBehavior>();
				_cubesInstancesList.Add(cube);
				cube.CubeID = createdCubeID;
				cube.cameraPoint = _mainCam;

				 TextMeshProUGUI floatingText = cubeWalkerInstance.GetComponentInChildren<TextMeshProUGUI>();
				 floatingText.rectTransform.SetParent(_cubesIDCanvas);
				 floatingText.transform.rotation = Quaternion.identity;
				 floatingText.transform.localScale = Vector3.one;
			
				 floatingText.text = createdCubeID.ToString();
				
				SetRandomColor(cubeWalkerInstance);
			}
		}
		
		public void Move()
		{
			foreach(CubeBehavior cube in _cubesInstancesList)
			{
				cube.cubeState = CubeState.Moving;
			}
		}
		
		public void Chase()
		{
			CubeBehavior Chaser =_cubesInstancesList[Random.Range(0,_cubesInstancesList.Count)];
			_cubesAliveInstancesList = new List<CubeBehavior>();
			_cubesAliveInstancesList.AddRange(_cubesInstancesList);
			Chaser.targetVictim = getVictim(Chaser);
			foreach (CubeBehavior cube in _cubesInstancesList)
			{
				cube.cubeState = cube == Chaser ? CubeState.Chaser:CubeState.Moving;
			}
		}
		
		public CubeBehavior getVictim(CubeBehavior excludeCube = null)
		{
			if (_cubesAliveInstancesList == null)
			{
				_cubesAliveInstancesList = new List<CubeBehavior>();
				_cubesAliveInstancesList.AddRange(_cubesInstancesList);
			}
			if(excludeCube && _cubesAliveInstancesList.Contains(excludeCube)) 
			{
				_cubesAliveInstancesList.Remove(excludeCube);
			}
			
			if(_cubesAliveInstancesList.Count>0)
			return _cubesAliveInstancesList[Random.Range(0,_cubesAliveInstancesList.Count)];
			else
			return null;
		}
		public void RemoveFromAlive(CubeBehavior shotedCube)
		
		{
			if(_cubesAliveInstancesList.Contains(shotedCube))
			{
				_cubesAliveInstancesList.Remove(shotedCube);
			}
			
		}
		
		private void SetRandomColor(GameObject cubeWalkerInstance)
		{
			Renderer renderer = cubeWalkerInstance.GetComponent<Renderer>();
			if (renderer != null)
			{
				Color randomColor = new Color(Random.value, Random.value, Random.value);
				renderer.material.color = randomColor;
			}
		}
		
		public void OnCubeDestroyed(CubeBehavior cubeToDestroy)
		{
			if(cubeToDestroy)
			{
			if(_cubesAliveInstancesList.Contains(cubeToDestroy))
			{
				_cubesAliveInstancesList.Remove(cubeToDestroy);
			}
			
			if(_cubesInstancesList.Contains(cubeToDestroy))
			{
				_cubesInstancesList.Remove(cubeToDestroy);
			}
			}
		}
	}
}
