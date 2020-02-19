using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UltimateSky {

	[ExecuteInEditMode]
	public class StarfieldCreator : MonoBehaviour {

		public enum TextureSize { x64, x128, x256, x512, x1024, x2048, x4096 }
		protected int[] sizes = new int[] {64, 128, 256, 512, 1024, 2048, 4096 };

		public TextureSize texSize;
		public FlareControl starPrototype;
		public TextAsset starlist;
		protected Cubemap cubemap;

		public float starDistance = 1000;
		public AnimationCurve magnitudeBrightness;
		public AnimationCurve brightnessScale;

		[Range(0, 100000)] public int randomStarCount = 10000;
		[Range(0, 10)] public int randomStarMagnitudeMin = 5;
		[Range(0, 10)] public int randomStarMagnitudeMax = 10;
		public AnimationCurve randomStarMagnitudeCurve;


		protected List<Dictionary<string, object>> stars;
		protected MaterialPropertyBlock properties;

		public void CreateStarfield () {

			int childcount = this.transform.childCount;
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.DisplayProgressBar("Clearing starfield", childcount + " / " + childcount, 0.0f);
			#endif

			while (this.transform.childCount > 0) {

				#if UNITY_EDITOR
				UnityEditor.EditorUtility.DisplayProgressBar("Clearing starfield", this.transform.childCount + " / " + childcount, ((float)this.transform.childCount/(float)childcount));
				#endif

				DestroyImmediate (this.transform.GetChild (0).gameObject);
			}

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.ClearProgressBar();
			#endif

			if (starlist != null) {
				
				stars = CSVReader.Parse (starlist.text);

				for (int i = 0; i < stars.Count; i++) {
					Dictionary<string, object> stardata = stars [i];

					string starname = (string)stardata ["name"];
					if (starname.Equals ("Sol")) {
						continue;
					}

					if (starname.StartsWith ("#")) {
						// commented entry
					} else {

						/* X,Y,Z: The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth. 
						 * +X is in the direction of the vernal equinox (at epoch 2000), 
					 	* +Z towards the north celestial pole, and 
					 	* +Y in the direction of R.A. 6 hours, declination 0 degrees.
					 	*/

						Vector3 pos = new Vector3 (-(float)stardata ["x"], (float)stardata ["z"], -(float)stardata ["y"]);
						pos = pos.normalized;

						pos = EquatorialToEcliptic (pos.normalized);

						GameObject star = (GameObject)GameObject.Instantiate (starPrototype.gameObject, pos, Quaternion.identity);
						star.transform.parent = this.transform;

						star.name = starname;

						FlareControl control = star.GetComponent<FlareControl> ();

						float magnitude = (float)stardata ["magnitude"];

						float brightness = brightnessScale.Evaluate (magnitudeBrightness.Evaluate (magnitude));
						//float brightness = magnitudeBrightness.Evaluate (magnitude);

						control.brightness = brightness;

						pos = pos.normalized * starDistance;
						star.transform.localPosition = pos;

						string spectra = (string)stardata ["spectra"];
						if (spectra.StartsWith ("O")) {
							control.color = new Color (0.7f, 0.7f, 1.0f, 1.0f);
						} else if (spectra.StartsWith ("B")) {
							control.color = new Color (0.8f, 0.8f, 1.0f, 1.0f);
						} else if (spectra.StartsWith ("A")) {
							control.color = new Color (0.9f, 0.9f, 1.0f, 1.0f);
						} else if (spectra.StartsWith ("F")) {
							control.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
						} else if (spectra.StartsWith ("G")) {
							control.color = new Color (1.0f, 1.0f, 0.9f, 1.0f);
						} else if (spectra.StartsWith ("K")) {
							control.color = new Color (1.0f, 0.9f, 0.8f, 1.0f);
						} else if (spectra.StartsWith ("M")) {
							control.color = new Color (1.0f, 0.7f, 0.7f, 1.0f);
						}

						//if (starname.Equals ("Polaris")) {
						//	control.brightness = 1.0f;
						//	control.color = new Color (0.0f, 1.0f, 0.0f, 1.0f);
						//}

						control.Apply (1.0f);
					}

				}

				#if UNITY_EDITOR
				UnityEditor.EditorUtility.DisplayProgressBar("Creating Random Stars", "0 / " + randomStarCount, 0.0f);
				#endif

				float max = (float)randomStarCount;
				for (int i = 0; i < randomStarCount; i++) {

					float _f = (float)i;
					#if UNITY_EDITOR
					UnityEditor.EditorUtility.DisplayProgressBar("Creating Random Stars", i + " / " + randomStarCount, (_f/max));
					#endif
					
					string starname = "Random[" + i + "]";
	
					/* X,Y,Z: The Cartesian coordinates of the star, in a system based on the equatorial coordinates as seen from Earth. 
						 * +X is in the direction of the vernal equinox (at epoch 2000), 
					 	* +Z towards the north celestial pole, and 
					 	* +Y in the direction of R.A. 6 hours, declination 0 degrees.
					 	*/

					Vector3 pos = Random.onUnitSphere;
					pos = pos.normalized;
					pos = EquatorialToEcliptic (pos.normalized);

					GameObject star = (GameObject)GameObject.Instantiate (starPrototype.gameObject, pos, Quaternion.identity);
					star.transform.parent = this.transform;

					star.name = starname;

					FlareControl control = star.GetComponent<FlareControl> ();

					//float magnitude = ((float)Random.Range(randomStarMagnitudeMin * 10, randomStarMagnitudeMax * 10)) / 10.0f;

					float magnitude = Mathf.Lerp (randomStarMagnitudeMin, randomStarMagnitudeMax, randomStarMagnitudeCurve.Evaluate (Random.value));

					float brightness = brightnessScale.Evaluate (magnitudeBrightness.Evaluate (magnitude));

					control.brightness = brightness;

					pos = pos.normalized * starDistance;
					star.transform.localPosition = pos;

					int spectra = Random.Range(0, 7);
					if (spectra == 0) {
						control.color = new Color (0.7f, 0.7f, 1.0f, 1.0f);
					} else if (spectra == 1) {
						control.color = new Color (0.8f, 0.8f, 1.0f, 1.0f);
					} else if (spectra == 2) {
						control.color = new Color (0.9f, 0.9f, 1.0f, 1.0f);
					} else if (spectra == 3) {
						control.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					} else if (spectra == 4) {
						control.color = new Color (1.0f, 1.0f, 0.9f, 1.0f);
					} else if (spectra == 5) {
						control.color = new Color (1.0f, 0.9f, 0.8f, 1.0f);
					} else if (spectra == 6) {
						control.color = new Color (1.0f, 0.7f, 0.7f, 1.0f);
					}

					control.Apply (1.0f);

				}

				#if UNITY_EDITOR
				UnityEditor.EditorUtility.ClearProgressBar();
				#endif
			}
		}


		public void RenderStarfield () {
			int size = sizes [(int)texSize];
			cubemap = new Cubemap (size, TextureFormat.RGB24, false);
			Camera.main.RenderToCubemap (cubemap);
			cubemap.Apply ();
			#if UNITY_EDITOR
			string path = UnityEditor.EditorUtility.SaveFilePanel("Save Cubemap", "", "starfield", "cubemap");
			if (path != null && path.Length > 0 && path.Contains("/Assets/")) {
				path = path.Substring(path.IndexOf("Assets/"));
				UnityEditor.AssetDatabase.CreateAsset(cubemap, path);
			}
			#endif
		}

		public static Vector3 EquatorialToEcliptic(Vector3 equatorial) {
			Vector3 eq = equatorial;
			eq.x = -equatorial.x;
			eq.y = -equatorial.z;
			eq.z = equatorial.y;
			Matrix4x4 mat = Matrix4x4.identity;
			float OBL = Mathf.Deg2Rad * 23.44f;
			mat.SetRow(0, new Vector4(1.0f,0.0f,0.0f,0.0f));
			mat.SetRow(1, new Vector4(0.0f,Mathf.Cos(OBL),Mathf.Sin(OBL),0.0f));
			mat.SetRow(2, new Vector4(0.0f,-Mathf.Sin(OBL),Mathf.Cos(OBL),0.0f));
			mat.SetRow(3, new Vector4(0.0f,0.0f,0.0f,1.0f));
			Vector3 ecl = mat * eq;
			ecl.Normalize ();
			Vector3 ecliptic = ecl;
			ecliptic.x = -ecl.x;
			ecliptic.y = ecl.z;
			ecliptic.z = -ecl.y;
			return ecliptic;
		}
	}
}
