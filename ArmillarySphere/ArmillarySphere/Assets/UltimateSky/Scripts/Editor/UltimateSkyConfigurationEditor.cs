using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UltimateSky {
	[CustomEditor(typeof(UltimateSkyConfiguration))]
	public class UltimateSkyConfigurationEditor : Editor  {

		protected Texture iconObliquity;

		public override void OnInspectorGUI() {
			UltimateSkyConfiguration configuration = (UltimateSkyConfiguration)target;

			if (iconObliquity == null) {
				
				iconObliquity = (Texture) Resources.Load ("obliquity");
			}

			/////////////////////////////////////////////////////////
			// Calendar
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Day cycle", MessageType.None);
			configuration.hoursPerDay = EditorGUILayout.IntSlider ("Hours in Day", configuration.hoursPerDay, 1, 1000);
			configuration.minutesPerHour = EditorGUILayout.IntSlider ("Minutes in Hour", configuration.minutesPerHour, 1, 1000);
			configuration.secondsPerMinute = EditorGUILayout.IntSlider ("Seconds in Minute", configuration.secondsPerMinute, 1, 1000);
			int totalDayLength = ((configuration.hoursPerDay) * configuration.minutesPerHour) * configuration.secondsPerMinute;
			EditorGUILayout.HelpBox ("Total length of day: " + totalDayLength + " seconds.", MessageType.None);
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Year cycle", MessageType.None);
			int months = configuration.monthDays.Length;
			months = EditorGUILayout.IntSlider ("Months", months, 1, 100);
			if (months < configuration.monthDays.Length) {
				int[] monthArray = new int[months];
				System.Array.Copy (configuration.monthDays, monthArray, months);
				configuration.monthDays = monthArray;
			} else if (months > configuration.monthDays.Length) {
				int[] monthArray = new int[months];
				System.Array.Copy (configuration.monthDays, monthArray, configuration.monthDays.Length);
				for (int i = configuration.monthDays.Length; i < months; i++) {
					monthArray [i] = monthArray [configuration.monthDays.Length - 1];
				}
				configuration.monthDays = monthArray;
			}
			int yearLength = 0;
			for (int i = 0; i < months; i++) {
				configuration.monthDays[i] = EditorGUILayout.IntSlider ("Days in Month " + (i+1), configuration.monthDays[i], 1, 100);
				yearLength += configuration.monthDays [i];
			}
			EditorGUILayout.HelpBox ("Total length of year: " + yearLength + " days.", MessageType.None);
			GUILayout.EndVertical ();


			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Leap years", MessageType.None);
			configuration.hasLeapYear = EditorGUILayout.Toggle ("Has Leap Years", configuration.hasLeapYear);

			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!configuration.hasLeapYear)) {
			#else
			EditorGUI.BeginDisabledGroup(!configuration.hasLeapYear);
			#endif
				configuration.monthOfLeapDay = EditorGUILayout.IntSlider ("Month of Leap Day", configuration.monthOfLeapDay, 1, months);
				configuration.leapYearInterval = EditorGUILayout.IntSlider ("Leap Year Interval", configuration.leapYearInterval, 2, 100);
				configuration.leapYearIntervalException = EditorGUILayout.IntSlider ("Exception Interval", configuration.leapYearIntervalException, 3, 1000);
				configuration.leapYearIntervalNotException = EditorGUILayout.IntSlider ("Exception Ignore", configuration.leapYearIntervalNotException, 2, 1000);
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif

			if (configuration.hasLeapYear) {
				EditorGUILayout.HelpBox ("Every " + NumberToOrder (configuration.leapYearInterval) + " year is a leap year, " +
					"except for every " + NumberToOrder (configuration.leapYearIntervalException) + " year unless its the " +
					NumberToOrder (configuration.leapYearIntervalNotException) + " year.", MessageType.Info);
			}
			GUILayout.EndVertical ();


			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Obliquity of Ecliptic", MessageType.None);
			GUILayout.BeginHorizontal ();
			configuration.obliquityOfEcliptic = EditorGUILayout.Slider ("Axis Tilt", configuration.obliquityOfEcliptic, 1, 89);

			Rect r = EditorGUILayout.GetControlRect (GUILayout.MinWidth (32), GUILayout.MaxWidth (32), GUILayout.MinHeight (32), GUILayout.MaxHeight (32));
			r.x = Screen.width - 40;
			Matrix4x4 matrixBackup = GUI.matrix;
			GUIUtility.RotateAroundPivot(-configuration.obliquityOfEcliptic, r.center);
			GUI.DrawTexture(r, iconObliquity);
			GUI.matrix = matrixBackup;
			GUILayout.EndHorizontal ();

			EditorGUILayout.HelpBox ("Atmospheric light scattering wavelength", MessageType.None);
			configuration.atmosphericScatteringWavelength = EditorGUILayout.Vector3Field ("Wavelength", configuration.atmosphericScatteringWavelength);
			EditorGUILayout.HelpBox ("Planetary Radius and Atmosphere Depth in kilometers", MessageType.None);
			configuration.planetaryRadius = EditorGUILayout.Slider ("Radius", configuration.planetaryRadius, 100, 10000);
			configuration.atmosphereDepth = EditorGUILayout.Slider ("Depth", configuration.atmosphereDepth, 10, 1000);

			GUILayout.EndVertical ();
		}

		protected string NumberToOrder(int num) {
			if (num == 0) return "0th";
			if (num == 1) return "1st";
			if (num == 2) return "2nd";
			if (num == 3) return "3rd";
			return num+"th";
		}

	}
}
