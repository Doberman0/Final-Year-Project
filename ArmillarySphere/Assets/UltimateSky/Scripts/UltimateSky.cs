using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UltimateSky {

	[RequireComponent (typeof(UltimateSkyCalendar))]
	[ExecuteInEditMode]
	public class UltimateSky : MonoBehaviour {

		public UltimateSkyConfiguration config;

		public Transform sun;

		public bool editLonLatValues;
		public Vector2 lonLat;

		public bool editCubeValues;
		//public Vector3 starCubeRotation = new Vector3 (-17.4f, -130.9f, 29.4f);
		public Vector3 starCubeRotation = new Vector3 (-23.0f, 0.0f, 90.0f);
		public Vector3 starCubeScale = new Vector3(1.0f, 1.0f, 1.0f);
		public Color groundColor = new Color(0.13f, 0.09f, 0.05f);

		public UltimateSkyCalendar calendar;

		public bool editClouds;
		public bool enableClouds;
		public float cloudFadeIn = 1.0f;
		public float cloudGloominess = 0.0f;
		public Texture2D cloudTexture1;
		public Texture2D cloudTexture2;
		public float cloudScale1 = 0.1f;
		public float cloudScale2 = 0.1f;
		public bool useWind = true;
		public float cloudWindMultiplier1 = 0.0001f;
		public float cloudWindMultiplier2 = 0.001f;

		[Range(0.0f,1.0f)] public float phase;
		[Range(0.0f,1.0f)] public float lightphase;
		protected float phaseDelta;

		public bool editTime;
		public bool accelerateTimeScale = false;
		public float timeAcceleration = 1.0f;
		public float timestep;

		public Vector3 celestialNorth;

		[System.Serializable]
		public struct Ambience
		{
			public Color day;
			public Color night;
			public AnimationCurve curve;
			public AnimationCurve intensity;
		}
		public bool editAmbience;
		public Ambience ambience;
		public float AmbiencePhase { get { return ambience.curve.Evaluate(lightphase); } }
		public float LightIntensity { get { return ambience.intensity.Evaluate(lightphase); } }

		protected Skybox _skybox;
		protected Material _material;

		protected WindZone[] windzones;
		protected Vector3 cloudShift1;
		protected Vector3 cloudShift2;

		public UltimateSky() {
			if (ambience.curve == null) {
				ambience.day = Color.white;
				ambience.night = Color.black;
				ambience.curve = new AnimationCurve ();
			}
			if (ambience.intensity == null) {
				ambience.intensity = new AnimationCurve ();
				ambience.intensity.AddKey (new Keyframe (0, 0));
				ambience.intensity.AddKey (new Keyframe (0.49f, 0));
				ambience.intensity.AddKey (new Keyframe (0.51f, 1));
				ambience.intensity.AddKey (new Keyframe (1, 1));
			}
		}

		protected virtual void Awake () {
			calendar = this.GetComponent<UltimateSkyCalendar> ();

			_skybox = this.GetComponent<Skybox> ();
			if (_skybox != null) {
				_material = _skybox.material;
			} else {
				_material = RenderSettings.skybox;
			}
			windzones = GameObject.FindObjectsOfType<WindZone> ();

			if (this.config != null) {
				calendar.config = this.config;
				TimeStep ();
				CalculateLightPhase ();
				ApplyAmbience ();
			}
		}

		protected virtual void Start () {
			if (enableClouds && _material != null) {
				_material.SetTexture ("_CloudTex1", cloudTexture1);
				_material.SetTexture ("_CloudTex2", cloudTexture2);
			}
		}
			
		protected virtual void Update () {

			// clamp latitude
			lonLat.y = Mathf.Clamp(lonLat.y, -89.9f, 89.9f);

			if (this.config != null) {
				calendar.config = this.config;
				TimeStep ();
				CalculateLightPhase ();
				ApplyAmbience ();

				celestialNorth = (new Vector3 (90.0f-lonLat.y, 0.0f, 0.0f));
			}

			UpdateShader ();
		}
			
		protected void TimeStep() {
			if (accelerateTimeScale) {
				if (timeAcceleration <= 100.0f) {
					Time.timeScale = timeAcceleration;
					timestep = Time.deltaTime;
				} else {
					Time.timeScale = 100.0f;
					timestep = (timeAcceleration / 100.0f) * Time.deltaTime;
				}
			} else {
				timestep = timeAcceleration * Time.deltaTime;
			}
			float oldphase = phase;
			phase = calendar.UpdateTime (timestep, lonLat.x, lonLat.y);
			phaseDelta = phase - oldphase;
			if (phaseDelta < 0.0f)
				phaseDelta += 1.0f;
		}

		protected void CalculateLightPhase() {
			Vector2 el_az = calendar.SunPos();
			Vector3 angle = ElevationAzimuthAngle (el_az.x, el_az.y);
			if (sun == null) {
				lightphase = 0.0f;
				return;
			}
			sun.rotation = Quaternion.identity;
			sun.LookAt (-angle);
			float sunelevation = Mathf.Clamp(el_az.x / (Mathf.PI * 0.5f), -1.0f, 1.0f);
			lightphase = (1.0f+sunelevation) * 0.5f;
			if (lightphase < 0.5f) {
				lightphase = lightphase * (lightphase+0.5f);
			}
			Light light = sun.gameObject.GetComponent<Light> ();
			if (light != null) {
				light.intensity = LightIntensity;
			}
		}

		protected void ApplyAmbience() {
			if (enableClouds && cloudGloominess > 0.0f) {
				float darken = (1.0f - (cloudGloominess/2.0f));
				RenderSettings.ambientLight = Color.Lerp (ambience.night, ambience.day, ambience.curve.Evaluate (lightphase * darken));
				RenderSettings.ambientIntensity = ambience.curve.Evaluate (lightphase * darken);
			} else {
				RenderSettings.ambientLight = Color.Lerp (ambience.night, ambience.day, ambience.curve.Evaluate (lightphase));
				RenderSettings.ambientIntensity = ambience.curve.Evaluate (lightphase);
			}
		}

		protected void UpdateShader() {
			if (_material != null) {

				if (this.config != null) {
					float innerRadius = config.planetaryRadius;
					_material.SetFloat("fInnerRadius", innerRadius);
					_material.SetFloat("fInnerRadius2", innerRadius*innerRadius);

					float outerRadius = config.planetaryRadius + config.atmosphereDepth;
					_material.SetFloat("fOuterRadius", outerRadius);
					_material.SetFloat("fOuterRadius2", outerRadius*outerRadius);

					float scale = 1.0f / (outerRadius - innerRadius);
					_material.SetFloat("fScale", scale);

					_material.SetFloat("fScaleDepth", 0.25f);

					_material.SetFloat("fScaleOverScaleDepth", scale / 0.25f);

					Vector4 wavelength = new Vector4(config.atmosphericScatteringWavelength.x, config.atmosphericScatteringWavelength.y, config.atmosphericScatteringWavelength.z, 1.0f);
					_material.SetVector("v3Wavelength", wavelength);

					Vector4 inverseWavelength = wavelength;
					inverseWavelength.x = 1.0f / Mathf.Pow (inverseWavelength.x, 4);
					inverseWavelength.y = 1.0f / Mathf.Pow (inverseWavelength.y, 4);
					inverseWavelength.z = 1.0f / Mathf.Pow (inverseWavelength.z, 4);
					_material.SetVector("v3InvWavelength", inverseWavelength);

				}

				_material.SetVector("_LonLat", lonLat);
				_material.SetMatrix("_CubeRotation", Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(starCubeRotation), Vector3.one));
				_material.SetVector("_CubeScale", starCubeScale);
				_material.SetColor("_GroundColor", groundColor);
				_material.SetFloat("_EclipticLongitude", calendar.EclipticLongitude);
				_material.SetFloat("_ObliquityEcliptic", calendar.ObliquityEcliptic);
				_material.SetFloat("_LocalMeanSiderealTime", calendar.LocalMeanSiderealTime);
				_material.SetFloat("_Phase", phase);
				_material.SetFloat("_LightPhase", lightphase);
				_material.SetFloat ("_EnableClouds", enableClouds ? 1 : 0);
				if (enableClouds) {
					
					_material.SetFloat("_CloudFadeIn", cloudFadeIn);
					_material.SetFloat("_CloudGloominess", cloudGloominess);
					_material.SetFloat("_CloudScale1", cloudScale1);
					_material.SetFloat("_CloudScale2", cloudScale2);
					_material.SetTexture("_CloudTex1", cloudTexture1);
					_material.SetTexture("_CloudTex2", cloudTexture2);

					Vector3 wind = Vector3.zero;
					foreach (WindZone zone in windzones) {
						if (zone.mode == WindZoneMode.Directional) {
							wind += zone.transform.forward * zone.windMain;
						}
					}
					wind *= timestep;
					if (useWind) {
						cloudShift1 += wind * cloudWindMultiplier1;
					}
					cloudShift1.x = cloudShift1.x % 1.0f;
					cloudShift1.y = cloudShift1.y % 1.0f;
					cloudShift1.z = cloudShift1.z % 1.0f;
					if (useWind) {
						cloudShift2 += wind * cloudWindMultiplier2;
					}
					cloudShift2.x = cloudShift2.x % 1.0f;
					cloudShift2.y = cloudShift2.y % 1.0f;
					cloudShift2.z = cloudShift2.z % 1.0f;
					_material.SetVector ("_CloudShift1", cloudShift1);
					_material.SetVector ("_CloudShift2", cloudShift2);
				}
			}
		}

		protected Vector3 ElevationAzimuthAngle(float el, float az) {
			float z = Mathf.Sin (el);
			float hyp = Mathf.Cos (el);
			float y = hyp * Mathf.Cos (az);
			float x = hyp * Mathf.Sin (az);
			return new Vector3 (x, z, y);
		}

	}
}
