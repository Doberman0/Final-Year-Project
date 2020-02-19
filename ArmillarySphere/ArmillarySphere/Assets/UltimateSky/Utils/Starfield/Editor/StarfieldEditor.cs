using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UltimateSky {
	[CustomEditor(typeof(StarfieldCreator))]
	public class StarfieldEditor : Editor  {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI ();
			StarfieldCreator starfield = (StarfieldCreator)target;

			if (GUILayout.Button ("Generate Stars")) {
				starfield.CreateStarfield();
			}

			if (GUILayout.Button ("Render To Cubemap")) {
				starfield.RenderStarfield();
			}
		}
	
	}
}
