using UnityEngine;
using System.Collections;


namespace UltimateSky {

	[CreateAssetMenu(fileName="SkyConfig", menuName="UltimateSky/Configuration", order=30)]
	public class UltimateSkyConfiguration : ScriptableObject {

		public int TotalDaysYear {
			get { 
				int days = 0;
				foreach (int d in monthDays) {
					days += d;
				}
				return days;
			}
		}


		[Range(1,24)]public int hoursPerDay = 24;
		[Range(1,60)]public int minutesPerHour = 60;
		[Range(1,60)]public int secondsPerMinute = 60;

		protected float dayLength;
		public float DayLength {
			get { 
				dayLength = ((float)secondsPerMinute * (float)minutesPerHour * (float)hoursPerDay);
				return dayLength;
			}
		}

		public int[] monthDays = new int[1];

		public bool hasLeapYear;
		public int leapYearInterval;
		public int leapYearIntervalException;
		public int leapYearIntervalNotException;
		public int monthOfLeapDay;

		protected int[] yearDays = new int[1];
		public int[] YearDays {
			get { 
				if (yearDays == null || yearDays.Length != monthDays.Length) {
					yearDays = new int[monthDays.Length];
				}
				int days = 0;
				for (int i = 0; i < monthDays.Length; i++) {
					days += monthDays [i];
					yearDays [i] = days;
				}
				return yearDays;
			}
		}

		[Range(1,89)]public float obliquityOfEcliptic;
		public Vector3 atmosphericScatteringWavelength = new Vector3(0.65f,0.57f,0.475f);
		public float planetaryRadius = 6371.0f;
		public float atmosphereDepth = 100.0f;
	}
}