using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubesNShoot.Manager
{
	public class HuntModeScript : MonoBehaviour
	{
		public bool isStartHunt = false;
		private List<GameObject> _cubeWalkerInstances;
		// Start is called before the first frame update
		void Start()
		{
			_cubeWalkerInstances = new List<GameObject>(GameObject.FindGameObjectsWithTag("Cube"));
		}

		// Update is called once per frame
		void Update()
		{
			if(isStartHunt)
			{
					
			}
		}
		
		private void StartHunt()
		{
			isStartHunt = true;
		}
	}
}
