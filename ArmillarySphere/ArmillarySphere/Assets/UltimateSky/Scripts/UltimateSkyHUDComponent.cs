using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace UltimateSky {
	public class UltimateSkyHUDComponent : MonoBehaviour {

		public UltimateSky ultimateSky;

		public Canvas canvas;

		public List<UltimateSkyConfiguration> configurations;
		public WindZone windzone;

		public Dropdown dropdownConfig;
		public Text textConfigInfo;
		public InputField inputLatitude;
		public Slider inputLatSlider;
		public InputField inputLongitude;
		public InputField inputDay;
		public InputField inputMonth;
		public InputField inputYear;
		public InputField inputHour;
		public InputField inputMinute;
		public Toggle inputLocaltime;
		public Slider inputAcceleration;
		public Slider inputCloudFadein;
		public Slider inputCloudGloominess;
		public InputField inputCloudScale1;
		public InputField inputCloudScale2;
		public InputField inputCloudSpeed1;
		public InputField inputCloudSpeed2;

		public Toggle inputWindEnabled;
		public InputField inputWindDirection;

		public Text textCompass;
		public Text textDirection;

		protected UltimateSkyCalendar calendar;

		protected virtual void Start () {
			if (ultimateSky == null) {
				dropdownConfig.interactable = false;
				inputLongitude.interactable = false;
				inputLatitude.interactable = false;
				inputLatSlider.interactable = false;
				inputDay.interactable = false;
				inputMonth.interactable = false;
				inputYear.interactable = false;
				inputHour.interactable = false;
				inputMinute.interactable = false;
				inputLocaltime.interactable = false;
				inputAcceleration.interactable = false;
				inputCloudFadein.interactable = false;
				inputCloudGloominess.interactable = false;
				inputWindEnabled.interactable = false;
				inputWindDirection.interactable = false;
				inputCloudScale1.interactable = false;
				inputCloudScale2.interactable = false;
				inputCloudSpeed1.interactable = false;
				inputCloudSpeed2.interactable = false;
				return;
			}


			calendar = ultimateSky.calendar;

			for (int i = 0; i < configurations.Count; i++) {
				UltimateSkyConfiguration config = configurations [i];
				if (config == ultimateSky.config) {
					dropdownConfig.value = i;
					break;
				}
			}
			UpdateConfigInfo ();
		}


		protected virtual void Update () {
			if (Input.GetKeyDown (KeyCode.H) && canvas != null) {
				canvas.enabled = !canvas.enabled;

				Cursor.visible = canvas.enabled;
				Cursor.lockState = canvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;
			} else if (Input.GetKeyDown (KeyCode.Escape)) {
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			} else if ((canvas == null || !canvas.enabled) && Input.GetMouseButton(0)) {
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}

			if (ultimateSky == null) {
				return;
			}
			if (!inputLongitude.isFocused) inputLongitude.text = "" + ultimateSky.lonLat.x;
			if (!inputLatitude.isFocused) inputLatitude.text = "" + ultimateSky.lonLat.y;
			if (!inputDay.isFocused) inputDay.text = "" + calendar.yearMonthDay.z;
			if (!inputMonth.isFocused) inputMonth.text = "" + calendar.yearMonthDay.y;
			if (!inputYear.isFocused) inputYear.text = "" + calendar.yearMonthDay.x;
			if (!inputHour.isFocused) inputHour.text = "" + calendar.hourMinSec.x;
			if (!inputMinute.isFocused) inputMinute.text = "" + calendar.hourMinSec.y;
			inputLocaltime.isOn = calendar.isTimeLocal;
			inputAcceleration.value = ultimateSky.timeAcceleration;
			inputCloudFadein.value = ultimateSky.cloudFadeIn;
			inputCloudGloominess.value = ultimateSky.cloudGloominess;
			inputLatSlider.value = ultimateSky.lonLat.y;


			//float dir = Mathf.Round(Camera.main.transform.localEulerAngles.y);
			float dir = Mathf.Round(Camera.main.transform.eulerAngles.y);
			if (dir < 0) {
				dir += 360.0f;
			}
			textCompass.text = "" + dir;
			if (dir > 315.0) {
				textDirection.text = "North";
			} else if (dir > 225.0) {
				textDirection.text = "West";
			} else if (dir > 135.0) {
				textDirection.text = "South";
			} else if (dir > 45.0) {
				textDirection.text = "East";
			} else {
				textDirection.text = "North";
			}

			inputWindEnabled.isOn = ultimateSky.useWind;
			if (!inputWindDirection.isFocused && windzone != null) {
				dir = Mathf.Round(windzone.transform.eulerAngles.y);
				inputWindDirection.text = "" + dir;
			}

			if (!inputCloudScale1.isFocused) inputCloudScale1.text = "" + ultimateSky.cloudScale1;
			if (!inputCloudScale2.isFocused) inputCloudScale2.text = "" + ultimateSky.cloudScale2;
			if (!inputCloudSpeed1.isFocused) inputCloudSpeed1.text = "" + System.Math.Round(ultimateSky.cloudWindMultiplier1 / 0.001f, 3);
			if (!inputCloudSpeed2.isFocused) inputCloudSpeed2.text = "" + System.Math.Round(ultimateSky.cloudWindMultiplier2 / 0.001f, 3);
		}

		public void SetLongitude(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < 0.0f) {
					return;
				}
				val = val % 360.0f;
				ultimateSky.lonLat.x = val;
			}
		}

		public void SetLatitude(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < -90.0f || val > 90.0f) {
					return;
				}
				ultimateSky.lonLat.y = val;
			}
		}

		public void SetLatitude(float val) {
			ultimateSky.lonLat.y = val;
		}

		public void SetDay(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < 0.0f || val > 31.0f) {
					return;
				}
				calendar.yearMonthDay.z = val;
			}
		}

		public void SetMonth(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < 0.0f || val > 31.0f) {
					return;
				}
				calendar.yearMonthDay.y = val;
			}
		}

		public void SetYear(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < 0.0f || val > 31.0f) {
					return;
				}
				calendar.yearMonthDay.x = val;
			}
		}

		public void SetHour(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < 0.0f || val > 24.0f) {
					return;
				}
				calendar.hourMinSec.x = val;
			}
		}

		public void SetMinute(string text) {
			float val;
			if (float.TryParse (text, out val)) {
				if (val < 0.0f || val > 60.0f) {
					return;
				}
				calendar.hourMinSec.y = val;
			}
		}

		public void SetLocaltime(bool val) {
			calendar.isTimeLocal = val;
		}

		public void SetTimeAcceleration(float val) {
			ultimateSky.timeAcceleration = val;
		}

		public void SetCloudFadein(float val) {
			ultimateSky.cloudFadeIn = val;
		}

		public void SetCloudGloominess(float val) {
			ultimateSky.cloudGloominess = val;
		}

		public void SetConfig(int i) {
			if (configurations != null && configurations.Count > i) {
				UltimateSkyConfiguration config = configurations [i];
				ultimateSky.config = config;
				UpdateConfigInfo ();
			}
		}

		public void SetWindEnabled(bool val) {
			ultimateSky.useWind = val;
		}

		public void SetWindDirection(string val) {
			float dir = 0.0f;
			if (float.TryParse (val, out dir)) {
				dir = dir % 360.0f;
				if (windzone != null) {
					windzone.transform.rotation = Quaternion.Euler (Vector3.up * dir);
				}
			}
		}

		public void SetCloudScale1(string val) {
			float scale = 0.0f;
			if (float.TryParse (val, out scale)) {
				
				ultimateSky.cloudScale1 = scale;
			}
		}

		public void SetCloudScale2(string val) {
			float scale = 0.0f;
			if (float.TryParse (val, out scale)) {

				ultimateSky.cloudScale2 = scale;
			}
		}

		public void SetCloudSpeed1(string val) {
			float scale = 0.0f;
			if (float.TryParse (val, out scale)) {

				ultimateSky.cloudWindMultiplier1 = scale * 0.001f;
			}
		}

		public void SetCloudSpeed2(string val) {
			float scale = 0.0f;
			if (float.TryParse (val, out scale)) {

				ultimateSky.cloudWindMultiplier2 = scale * 0.001f;
			}
		}

		protected void UpdateConfigInfo() {
			UltimateSkyConfiguration config = ultimateSky.config;
			string text = string.Format("{0} days in {1} months and {2} hours with {3} minutes.", config.TotalDaysYear, config.monthDays.Length, config.hoursPerDay, config.minutesPerHour);
			textConfigInfo.text = text;
		}

		public void RandomizeValues() {
			UltimateSkyConfiguration config = ultimateSky.config;
			UltimateSkyCalendar calendar = ultimateSky.calendar;

			ultimateSky.lonLat.x = Random.Range (0, 360);
			ultimateSky.lonLat.y = Random.Range (-90, 90);
			calendar.yearMonthDay.x = Random.Range (0, 4000);
			calendar.yearMonthDay.y = Random.Range (0, config.monthDays.Length);
			calendar.yearMonthDay.z = Random.Range (0, config.monthDays[(int)calendar.yearMonthDay.y]);

			calendar.hourMinSec.x = Random.Range (0, config.hoursPerDay);
			calendar.hourMinSec.y = Random.Range (0, config.minutesPerHour);
			calendar.hourMinSec.z = Random.Range (0, config.secondsPerMinute);

			ultimateSky.cloudFadeIn = Random.value;
			ultimateSky.cloudGloominess = Random.value;

			if (windzone != null) {
				windzone.transform.rotation = Quaternion.Euler (Vector3.up * Random.value * 360.0f);
			}
		}
	}
}
