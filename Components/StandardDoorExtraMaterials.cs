﻿using UnityEngine;

namespace BBPlusAnimations.Components
{
	internal class StandardDoorExtraMaterials : MonoBehaviour
	{
		[SerializeField]
		public Material[] doorLockedMat;

		[SerializeField]
		public Texture2D[] defaultTex;

		public Material[] ogTexs = new Material[2];
	}
}
