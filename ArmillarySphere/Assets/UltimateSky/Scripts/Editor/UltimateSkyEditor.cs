using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UltimateSky {
	[CustomEditor(typeof(UltimateSky))]
	public class UltimateSkyEditor : Editor  {
		public override void OnInspectorGUI() {
			UltimateSky ultimateSky = (UltimateSky) target;

			/////////////////////////////////////////////////////////
			// Config
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Configuration asset and material.", MessageType.None);

			ultimateSky.config = (UltimateSkyConfiguration) EditorGUILayout.ObjectField (ultimateSky.config, typeof(UltimateSkyConfiguration), false);
			if (ultimateSky.config != null && ultimateSky.calendar != null) {
				ultimateSky.calendar.config = ultimateSky.config;
				int[] yearDays = ultimateSky.config.YearDays;
				if (yearDays != null && yearDays.Length > 0) {
					//int totalDays = yearDays [yearDays.Length - 1];
					//EditorGUILayout.HelpBox ("Config length of year: " + totalDays, MessageType.Info);
				}
			}
		
			GUILayout.EndVertical ();

			/////////////////////////////////////////////////////////
			// Sun
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Transform of the main directional light in the scene.", MessageType.None);
			ultimateSky.sun = (Transform) EditorGUILayout.ObjectField("Sun", ultimateSky.sun, typeof(Transform), true);
			GUILayout.EndVertical ();

			/////////////////////////////////////////////////////////
			// Ambience
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Ambient color and intensity adjustment (evaluated from runtime Light Phase value).", MessageType.None);
			ultimateSky.editAmbience = EditorGUILayout.Toggle ("Edit", ultimateSky.editAmbience);

			if (ultimateSky.editAmbience) {
				EditorGUILayout.HelpBox ("The ambient light color settings for day and night are pretty self explanatory. The Curve on the other hand evaluates the lightphase value to produce ambient intensity, and the ambient intensity is used to lerp between the colors.\nThe lightphase value is computed from sun elevation and is always (0.5) at daybreak and nightfall, where the lower and upper end depends on how high and low the sun goes, thus latitude and date affects it a lot.", MessageType.Info);
			}

			EditorGUILayout.HelpBox ("Lighting colors and intensities", MessageType.None);
			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!ultimateSky.editAmbience)) {
			#else
			EditorGUI.BeginDisabledGroup(!ultimateSky.editAmbience);
			#endif
			ultimateSky.ambience.day = EditorGUILayout.ColorField ("Ambient Day", ultimateSky.ambience.day);
			ultimateSky.ambience.night = EditorGUILayout.ColorField ("Ambient Night", ultimateSky.ambience.night);
			ultimateSky.ambience.curve = EditorGUILayout.CurveField ("Ambient Intensity", ultimateSky.ambience.curve); 
			ultimateSky.ambience.intensity = EditorGUILayout.CurveField ("Directional Intensity", ultimateSky.ambience.intensity); 
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif
			GUILayout.EndVertical ();

			/////////////////////////////////////////////////////////
			// Ground
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Ground color of skybox.", MessageType.None);
			ultimateSky.groundColor = EditorGUILayout.ColorField("Ground Color", ultimateSky.groundColor);
			GUILayout.EndVertical ();
			/////////////////////////////////////////////////////////
			// Starcube
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Rotation and scaling of night cubemap.", MessageType.None);
			ultimateSky.editCubeValues = EditorGUILayout.Toggle ("Edit", ultimateSky.editCubeValues);

			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!ultimateSky.editCubeValues)) {
			#else
			EditorGUI.BeginDisabledGroup(!ultimateSky.editCubeValues);
			#endif
				ultimateSky.starCubeRotation = EditorGUILayout.Vector3Field ("Cubemap Rotation", ultimateSky.starCubeRotation);
				ultimateSky.starCubeScale = EditorGUILayout.Vector3Field ("Cubemap Scale", ultimateSky.starCubeScale);
				if (GUILayout.Button ("Set to Obliquity of Ecliptic")) {
					if (ultimateSky.config != null) {
						ultimateSky.starCubeRotation = new Vector3 (-ultimateSky.config.obliquityOfEcliptic, 0.0f, 90.0f);
					} else {
						ultimateSky.starCubeRotation = new Vector3 (0.0f, 0.0f, 90.0f);
					}
					ultimateSky.starCubeScale = Vector3.one;
				}
				if (ultimateSky.editCubeValues) {
					EditorGUILayout.HelpBox ("To accurately position stars the cubemap has to be rotated and possibly scaled. This is because the source images are probably not aligned in any particular way. To get it right you need to experiment. The alternative would be to prepare a cubemap in a specific orientation and that is probably much more work.", MessageType.Info);
				}
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif
			GUILayout.EndVertical ();
			/////////////////////////////////////////////////////////
			// Longitude & Latitude
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Location of the scene in earth coordinates.", MessageType.None);
			ultimateSky.editLonLatValues = EditorGUILayout.Toggle ("Edit", ultimateSky.editLonLatValues);
			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!ultimateSky.editLonLatValues)) {
			#else
			EditorGUI.BeginDisabledGroup(!ultimateSky.editLonLatValues);
			#endif
				ultimateSky.lonLat.x = EditorGUILayout.Slider ("Longitude", ultimateSky.lonLat.x, -180.0f, 180.0f);
				ultimateSky.lonLat.y = EditorGUILayout.Slider ("Latitude", ultimateSky.lonLat.y, -90.0f, 90.0f);
				if (ultimateSky.editLonLatValues) {
					EditorGUILayout.HelpBox ("The location of the scene in geographical coordinates. This will together with date affect the length of day, position of the sun and stars.\nIf not using local time (in calendar) then longitude will also affect day/night cycle.", MessageType.Info);
				}
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif
			GUILayout.EndVertical ();
			/////////////////////////////////////////////////////////
			// Clouds
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("High altitude cloud settings.", MessageType.None);
			ultimateSky.editClouds = EditorGUILayout.Toggle ("Edit", ultimateSky.editClouds);

			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!ultimateSky.editClouds)) {
			#else
			EditorGUI.BeginDisabledGroup(!ultimateSky.editClouds);
			#endif
				ultimateSky.enableClouds = EditorGUILayout.Toggle ("Enable Clouds", ultimateSky.enableClouds);
				ultimateSky.cloudFadeIn = EditorGUILayout.Slider ("Overcast", ultimateSky.cloudFadeIn, 0.0f, 1.0f);
			ultimateSky.cloudGloominess = EditorGUILayout.Slider ("Gloominess", ultimateSky.cloudGloominess, 0.0f, 1.0f);
				#if UNITY_5_4_OR_NEWER
				using (new EditorGUI.DisabledScope (!ultimateSky.enableClouds)) {
				#else
				EditorGUI.BeginDisabledGroup(!ultimateSky.enableClouds);
				#endif
					ultimateSky.cloudTexture1 = (Texture2D)EditorGUILayout.ObjectField ("Cloud Texture 1", ultimateSky.cloudTexture1, typeof(Texture2D), false);
					ultimateSky.cloudTexture2 = (Texture2D)EditorGUILayout.ObjectField ("Cloud Texture 2", ultimateSky.cloudTexture2, typeof(Texture2D), false);
					ultimateSky.cloudScale1 = EditorGUILayout.FloatField ("Cloud Scale 1", ultimateSky.cloudScale1);
					ultimateSky.cloudScale2 = EditorGUILayout.FloatField ("Cloud Scale 2", ultimateSky.cloudScale2);
					ultimateSky.cloudWindMultiplier1 = EditorGUILayout.Slider ("Wind Multiplier 1", ultimateSky.cloudWindMultiplier1, 0.0f, 0.001f);
					ultimateSky.cloudWindMultiplier2 = EditorGUILayout.Slider ("Wind Multiplier 2", ultimateSky.cloudWindMultiplier2, 0.0f, 0.01f);
				#if UNITY_5_4_OR_NEWER
				}
				#else
				EditorGUI.EndDisabledGroup();
				#endif
				if (ultimateSky.editClouds) {
					EditorGUILayout.HelpBox ("The shader supports two noisemaps (they have to be seamlessly tiling) to create a simple parallax cloudscape. You can use the cloud texture generator or produce your own. The movement of the clouds depend on directional WindZones, so you have to have at least one in the scene for the clouds to move. The first texture is 'higher' and moves slower.\nWith the fade in you can create transitions from overcast to clear.", MessageType.Info);
				}
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif
			GUILayout.EndVertical ();

			/////////////////////////////////////////////////////////
			// Time Acceleration
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Time settings.", MessageType.None);
			ultimateSky.editTime = EditorGUILayout.Toggle ("Edit", ultimateSky.editTime);

			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!ultimateSky.editTime)) {
			#else
			EditorGUI.BeginDisabledGroup(!ultimateSky.editTime);
			#endif
				ultimateSky.timeAcceleration = EditorGUILayout.Slider ("Time Acceleration", ultimateSky.timeAcceleration, 0.0f, 3600.0f);
				ultimateSky.accelerateTimeScale = EditorGUILayout.Toggle ("Modify Time.timeScale", ultimateSky.accelerateTimeScale);
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif
			if (ultimateSky.editTime) {
				EditorGUILayout.HelpBox ("Time acceleration affects the simulation step for daytime transitions.\nIt will not affect anything outside UltimateSky unless you select 'Modify Time.timeScale', but beware that increasing that is only possible to 100 and physics calculations will slow down the framerate.\nIn ether case, the time acceleration is mainly for previewing and debugging phase transitions.", MessageType.Info);
			}
			GUILayout.EndVertical ();
			/////////////////////////////////////////////////////////
			// Debug
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Runtime values.", MessageType.None);
			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (true)) {
			#else
			EditorGUI.BeginDisabledGroup(true);
			#endif
				EditorGUILayout.Slider ("Phase", ultimateSky.phase, 0.0f, 1.0f);
				EditorGUILayout.Slider ("Light Phase", ultimateSky.lightphase, 0.0f, 1.0f);
				EditorGUILayout.Vector3Field("Celestial North", ultimateSky.celestialNorth);
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif
			GUILayout.EndVertical ();
			
			//base.OnInspectorGUI ();
			if (!EditorApplication.isPlaying && GUI.changed){
				EditorUtility.SetDirty(target);
			}
		}
	}
}
