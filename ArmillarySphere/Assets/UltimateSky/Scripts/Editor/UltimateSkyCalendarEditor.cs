using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace UltimateSky {
	[CustomEditor(typeof(UltimateSkyCalendar))]
	public class UltimateSkyCalendartEditor : Editor  {
		public override void OnInspectorGUI() {
			UltimateSkyCalendar calendar = (UltimateSkyCalendar) target;

			
			if (EditorApplication.isPlaying) {

			}



			/////////////////////////////////////////////////////////
			// Year, Month, Day
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Current date.", MessageType.None);

			calendar.editDate = EditorGUILayout.Toggle ("Edit", calendar.editDate);

			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!calendar.editDate)) {
			#else
			EditorGUI.BeginDisabledGroup(!calendar.editDate);
			#endif
				calendar.yearMonthDay.x = (float)EditorGUILayout.IntSlider ("Year", (int)calendar.yearMonthDay.x, -2000, 4000);
				int month = Mathf.Clamp ((int)calendar.yearMonthDay.y, 1, calendar.MONTHS);
				calendar.yearMonthDay.y = (float)EditorGUILayout.IntSlider ("Month", month, 1, calendar.MONTHS);
				int days = calendar.DaysInMonth ((int)calendar.yearMonthDay.x, (int)calendar.yearMonthDay.y);
				int day = Mathf.Clamp ((int)calendar.yearMonthDay.z, 1, days);
				calendar.yearMonthDay.z = (float)EditorGUILayout.IntSlider ("Day", day, 1, days);
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif

			if (calendar.editDate) {
				EditorGUILayout.HelpBox ("Creates realistic seasonal changes in the day night cycle.", MessageType.Info);
			}

			GUILayout.EndVertical ();

			/////////////////////////////////////////////////////////
			// Hour, Minute, Second
			/////////////////////////////////////////////////////////
			GUILayout.BeginVertical ("Box");
			EditorGUILayout.HelpBox ("Current local time.", MessageType.None);

			calendar.editTime = EditorGUILayout.Toggle ("Edit", calendar.editTime);
			#if UNITY_5_4_OR_NEWER
			using (new EditorGUI.DisabledScope (!calendar.editTime)) {
			#else
			EditorGUI.BeginDisabledGroup(!calendar.editTime);
			#endif
				calendar.hourMinSec.x = (float)EditorGUILayout.IntSlider ("Hour", (int)calendar.hourMinSec.x, 0, (int)calendar.HOURS_DAY-1);
				calendar.hourMinSec.y = (float)EditorGUILayout.IntSlider ("Minute", (int)calendar.hourMinSec.y, 0, (int)calendar.MINUTES_HOUR-1);
				calendar.hourMinSec.z = (float)EditorGUILayout.IntSlider ("Second", (int)calendar.hourMinSec.z, 0, (int)calendar.SECONDS_MINUTE-1);
				calendar.isTimeLocal = EditorGUILayout.Toggle ("Use Local Time", calendar.isTimeLocal);
			#if UNITY_5_4_OR_NEWER
			}
			#else
			EditorGUI.EndDisabledGroup();
			#endif

			if (calendar.editTime) {
				EditorGUILayout.HelpBox ("With local time enabled changing the longitude will not affect the day/night cycle as the local timezone is added to the calculations.", MessageType.Info);
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
				EditorGUILayout.FloatField ("Ecliptic Longitude", calendar.EclipticLongitude);
				EditorGUILayout.FloatField ("Obliquity of Ecliptic", calendar.ObliquityEcliptic);
				EditorGUILayout.FloatField ("Julian Date", calendar.JulianDate);
				EditorGUILayout.FloatField ("Epoch Time", calendar.EpochTime);
				EditorGUILayout.FloatField ("Local Mean Sidereal Time", calendar.LocalMeanSiderealTime);
				EditorGUILayout.CurveField ("Latitudal Season Phases", calendar.LatitudalSeasonPhases);
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
