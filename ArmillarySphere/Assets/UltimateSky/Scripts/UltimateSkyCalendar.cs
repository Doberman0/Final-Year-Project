
using UnityEngine;
using System.Collections;


	
namespace UltimateSky {
	
	public class UltimateSkyCalendar : MonoBehaviour {


		public UltimateSkyConfiguration config;

			
		public bool editDate;
		public Vector3 yearMonthDay;

		public bool editTime;
		public bool isTimeLocal;
		public Vector3 hourMinSec;

		//public float MoonPhase;

		protected float SECONDSDAY = 86400.0f;
		public float HOURS_DAY;
		public float MINUTES_HOUR;
		public float SECONDS_MINUTE;
		public int MONTHS;
		//protected float SECONDSMOONPHASE;
		protected int[] MONTHDAYS;
		protected int[] YEARDAYS;
		public float totalseconds;
		protected float totalDaysYear;

		protected float obliquityEcliptic;
		
		public UltimateSkyCalendar() {
			/*
			MONTHDAYS = new int[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
			YEARDAYS = new int[12] { 
				31, 
				31 + 28, 
				31 + 28 + 31, 
				31 + 28 + 31 + 30, 
				31 + 28 + 31 + 30 + 31, 
				31 + 28 + 31 + 30 + 31 + 30, 
				31 + 28 + 31 + 30 + 31 + 30 + 31, 
				31 + 28 + 31 + 30 + 31 + 30 + 31 + 31, 
				31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30, 
				31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31, 
				31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31 + 30,
				31 + 28 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31 + 30 + 31};
			*/
			//SECONDSMOONPHASE = SECONDSDAY * 27.5f;
		}

		protected virtual void Start() {
			if (this.config != null) {
				obliquityEcliptic = config.obliquityOfEcliptic;
				_LatitudalSeasonPhaseKeys = new Keyframe[] {
					new Keyframe (-90.0f / 90.0f, 0.0f),
					new Keyframe (-obliquityEcliptic / 90.0f, 0.0f),
					new Keyframe (0.0f, 0.0f),
					new Keyframe (obliquityEcliptic / 90.0f, 0.0f),
					new Keyframe (90.0f / 90.0f, 0.0f)
				};
			}
		}

		protected bool CheckConfig() {
			if (this.config == null)
				return false;
			HOURS_DAY = (float) config.hoursPerDay;
			MINUTES_HOUR = (float) config.minutesPerHour;
			SECONDS_MINUTE = (float) config.secondsPerMinute;
			SECONDSDAY = config.DayLength;
			MONTHDAYS = config.monthDays;
			YEARDAYS = config.YearDays;
			MONTHS = MONTHDAYS.Length; 
			totalDaysYear = (float)YEARDAYS [YEARDAYS.Length - 1];
			obliquityEcliptic = config.obliquityOfEcliptic;
			return true;
		}

		public int DaysInMonth(int year, int month) {

			if (!CheckConfig())
				return 0;
			
			int days = MONTHDAYS [month-1];
			if (IsLeapMonth (year, month)) {
				days++;
			}
			return days;
		}

		protected float lon;
		protected float lat;

		public void SetLat(float new_val)
		{
			lat = new_val;
		}

		public void SetLong(float new_val)
		{
			lon = new_val;
		}

		public float UpdateTime(float deltaSec, float lon, float lat) {

			if (!CheckConfig())
				return 0.0f;

			HOURS_DAY = (float) config.hoursPerDay;
			MINUTES_HOUR = (float) config.minutesPerHour;
			SECONDS_MINUTE = (float) config.secondsPerMinute;

			SECONDSDAY = config.DayLength;

			MONTHDAYS = config.monthDays;
			YEARDAYS = config.YearDays;
			MONTHS = MONTHDAYS.Length; 

			this.lon = lon;
			this.lat = lat;
			hourMinSec.z += deltaSec;	
			while (hourMinSec.z >= SECONDS_MINUTE) {
				hourMinSec.y += 1.0f;
				hourMinSec.z -= SECONDS_MINUTE;

				//MoonPhase = (MoonPhase + (1.0f / SECONDSMOONPHASE)) % 1.0f;
			}
			while (hourMinSec.y >= MINUTES_HOUR) {
				hourMinSec.x += 1.0f;
				hourMinSec.y -= MINUTES_HOUR;
			}
			while (hourMinSec.x >= HOURS_DAY) {
				yearMonthDay.z += 1.0f;
				hourMinSec.x -= HOURS_DAY;
			}
			UpdateDate();

			CalculateMeanTime (lon, lat);

			CalculateSeasonalProgression();

			totalseconds = (hourMinSec.x * (SECONDS_MINUTE*MINUTES_HOUR)) + (hourMinSec.y * SECONDS_MINUTE) + hourMinSec.z;
			return totalseconds / SECONDSDAY;
		}
		
		protected void UpdateDate() {



			yearMonthDay.y = Mathf.Clamp (yearMonthDay.y, 1, MONTHS);
			int days = MONTHDAYS [(int)yearMonthDay.y-1];
			if (IsLeapMonth ((int)yearMonthDay.x, (int)yearMonthDay.y)) {
				days++;
			}
			if (yearMonthDay.z > (float) days) {
				yearMonthDay.y += 1.0f;
				yearMonthDay.z = 1.0f;
			}
			if (yearMonthDay.y > (float) MONTHS) {
				yearMonthDay.x += 1.0f;
				yearMonthDay.y = 1.0f;
			}
		}
		
		protected bool IsLeapYear(int year) {
			if (config.hasLeapYear) {
				if (year % config.leapYearInterval == 0 && (year % config.leapYearIntervalNotException == 0 || year % (config.leapYearIntervalNotException*config.leapYearIntervalException) != 0)) {
					return true;
				}
			}
			return false;
		}
		
		protected bool IsLeapMonth(int year, int month) {
			if (config.hasLeapYear) {
				if (IsLeapYear (year) && month == config.monthOfLeapDay) {
					return true;
				}
			}
			return false;
		}

		public float EclipticLongitude;
		public float ObliquityEcliptic;
		public float EclipticLongitudeRadians;
		public float ObliquityEclipticRadians;
		public float JulianDate;
		public float LocalMeanSiderealTime;
		public float Hour;
		public float EpochTime;



		public void CalculateMeanTime(float lon, float lat) {

			int year = (int)yearMonthDay.x;
			int month = (int)yearMonthDay.y;
			int day = (int)yearMonthDay.z;
			float hour = hourMinSec.x;
			float min = hourMinSec.y;
			float sec = hourMinSec.z;

			// timezone shift
			if (isTimeLocal) {
				hour -= (HOURS_DAY/2) * (lon / 180.0f);
			}
			
			// Get day of the year, e.g. Feb 1 = 32, Mar 1 = 61 on leap years
			if (month > 1) {
				day = day + YEARDAYS [month - 1];
			}
			
			if (IsLeapYear(year) && (month > 1)) {
				day++;
			}
			
			// Get Julian date - 2400000
			hour = hour + (min / MINUTES_HOUR) + (sec / (MINUTES_HOUR*SECONDS_MINUTE));
			float delta = (float)year - 2000.0f;//1949.0f;
			float leap = 0.0f;
			if (config.hasLeapYear) {
				leap = Mathf.Floor ((delta / (float)config.leapYearInterval));
			}

			float dd = (delta * 365.0f * (1.0f - totalDaysYear / 365.0f)) + leap + ((float)day) + (hour / HOURS_DAY);

			const float EPOCH = 2451545.0f; // 32916.5f
			float time = (dd*(365.0f/totalDaysYear))-1.0f; // -1 because day 1 is epoch 0
			float jd = EPOCH + time;

			JulianDate = jd;
			Hour = hour;

			// The input to the Atronomer's almanach is the difference between
			// the Julian date and JD 2451545.0 (noon, 1 January 2000)
			//float time = jd - 51545.0f;
			EpochTime = time;
			// Ecliptic coordinates
			
			// Mean longitude
			float mnlong = 280.460f + 0.9856474f * time;
			mnlong = mnlong % 360.0f;
			if (mnlong < 0.0f) {
				mnlong += 360.0f;
			}
			
			// Mean anomaly
			float mnanom = 357.528f + 0.9856003f * time;
			mnanom = mnanom % 360.0f;
			if (mnanom < 0.0f) {
				mnanom += 360.0f;
			}
			mnanom = mnanom * Mathf.Deg2Rad;
			
			// Ecliptic longitude and obliquity of ecliptic
			float eclong = mnlong + 1.915f * Mathf.Sin (mnanom) + 0.020f * Mathf.Sin (2.0f * mnanom);
			eclong = eclong % 360.0f;
			if (eclong < 0.0f) {
				eclong += 360.0f;
			}
			
			float oblqec = obliquityEcliptic - 0.0000004f * time; // 23.439f
			float eclong_deg = eclong;
			float oblqec_deg = oblqec;
			eclong = eclong * Mathf.Deg2Rad;
			oblqec = oblqec * Mathf.Deg2Rad;
			EclipticLongitudeRadians = eclong;
			ObliquityEclipticRadians = oblqec;

			//float SZ = 360.0f / HOURS_DAY; // 15.0
			hour = hour * (24.0f / HOURS_DAY);

			// Local coordinates
			// Greenwich mean sidereal time
			float gmst = 6.697375f + 0.0657098242f * time + hour;
			gmst = gmst % 24.0f;
			if (gmst < 0.0f) {
				gmst += 24.0f;
			}
			
			// Local mean sidereal time
			float lmst = gmst + lon / 15.0f;
			lmst = lmst % 24.0f;
			if (lmst < 0.0f) {
				lmst += 24.0f;
			}
			lmst = lmst * 15.0f * Mathf.Deg2Rad; //sz=15.0f;

			
			EclipticLongitude = eclong_deg;
			ObliquityEcliptic = oblqec_deg;
			LocalMeanSiderealTime = lmst;
		}

		public Vector2 SunPos() {
			const float TWOPI = 2 * Mathf.PI;
			float oblqec = ObliquityEclipticRadians;
			float eclong = EclipticLongitudeRadians;

			// Celestial coordinates
			// Right ascension and declination
			float num = Mathf.Cos (oblqec) * Mathf.Sin (eclong);
			float den = Mathf.Cos (eclong);
			float ra = Mathf.Atan (num / den);
			if (den < 0.0f) {
				ra += Mathf.PI;
			}
			if (den >= 0.0f && num < 0.0f) {
				ra += TWOPI;
			}
			float dec = Mathf.Asin(Mathf.Sin (oblqec) * Mathf.Sin (eclong));

			// Hour angle
			float ha = LocalMeanSiderealTime - ra;
			if (ha < -Mathf.PI) {
				ha += TWOPI;
			}
			if (ha > Mathf.PI) {
				ha -= TWOPI;
			}

			// Latitude to radians
			float lat_r = lat * Mathf.Deg2Rad;
			
			// Azimuth and elevation
			float el = Mathf.Asin (Mathf.Sin (dec) * Mathf.Sin (lat_r) + Mathf.Cos (dec) * Mathf.Cos (lat_r) * Mathf.Cos (ha));
			float az = Mathf.Asin(-Mathf.Cos(dec) * Mathf.Sin(ha) / Mathf.Cos(el));
			
			// For logic and names, see Spencer, J.W. 1989. Solar Energy. 42(4):353
			bool cosAzPos = (0.0f <= Mathf.Sin (dec) - Mathf.Sin (el) * Mathf.Sin (lat_r));
			bool sinAzNeg = (Mathf.Sin (az) < 0.0f);
			if (cosAzPos && sinAzNeg) {
				az += TWOPI;
			}
			if (!cosAzPos) {
				az = Mathf.PI - az;
			}
			return new Vector2 (el, az);
		}

		public float YearPhase;
		public float SeasonPhase;
		public AnimationCurve LatitudalSeasonPhases = new AnimationCurve();
		protected Keyframe[] _LatitudalSeasonPhaseKeys = new Keyframe[] {new Keyframe(-90.0f/90.0f, 0.0f), new Keyframe(-23.4f/90.0f, 0.0f), new Keyframe(0.0f, 0.0f), new Keyframe(23.4f/90.0f, 0.0f), new Keyframe(90.0f/90.0f, 0.0f)};


		protected void CalculateSeasonalProgression() {
			int monthIndex = (int)yearMonthDay.y-1;
			int day =  (int)yearMonthDay.z-1;
			int maxdays = YEARDAYS [MONTHS-1];
			int yeardays = YEARDAYS [monthIndex] - MONTHDAYS [monthIndex] + day;
			if (IsLeapYear((int)yearMonthDay.x)) {
				maxdays++;
				if (monthIndex > 2) {
					yeardays++;
				}
			}
			YearPhase = (float)yeardays / (float)maxdays;

			SeasonPhase = YearPhase-0.1065574f; 
			if (SeasonPhase < 0.0f)
				SeasonPhase += 1.0f;

			Keyframe[] keys = _LatitudalSeasonPhaseKeys;

			if (SeasonPhase < 0.25f) {
				// spring to summer
				float _ADD = SeasonPhase / 0.25f;
				keys [0].value = 1.0f - _ADD;
				keys [1].value = 2.0f - _ADD;
				keys [2].value = 3.0f - _ADD;
				keys [3].value = 2.0f + _ADD;
				keys [4].value = 1.0f + _ADD;
			} else if (SeasonPhase < 0.5f) {
				// summer to autumn
				float _ADD = (SeasonPhase-0.25f) / 0.25f;
				keys [0].value = 0.0f + _ADD;
				keys [1].value = 1.0f + _ADD;
				keys [2].value = 2.0f + _ADD;
				keys [3].value = 3.0f - _ADD;
				keys [4].value = 2.0f - _ADD;
			} else if (SeasonPhase < 0.75f) {
				// autumn to winter
				float _ADD = (SeasonPhase-0.5f) / 0.25f;
				keys [0].value = 1.0f + _ADD;
				keys [1].value = 2.0f + _ADD;
				keys [2].value = 3.0f - _ADD;
				keys [3].value = 2.0f - _ADD;
				keys [4].value = 1.0f - _ADD;
			} else if (SeasonPhase <= 1.0f) {
				// winter to spring
				float _ADD = (SeasonPhase-0.75f) / 0.25f;
				keys [0].value = 2.0f - _ADD;
				keys [1].value = 3.0f - _ADD;
				keys [2].value = 2.0f + _ADD;
				keys [3].value = 1.0f + _ADD;
				keys [4].value = 0.0f + _ADD;
			}

			LatitudalSeasonPhases.keys = keys;

			LatitudalSeasonPhases.SmoothTangents(1, 0);
			LatitudalSeasonPhases.SmoothTangents(2, 0);
			LatitudalSeasonPhases.SmoothTangents(3, 0);

		}

		public float GetSeasonAtLatitude(float lat) {
			float d = lat/90.0f;
			d = d * Mathf.Abs(d);
			return LatitudalSeasonPhases.Evaluate(d);
		}
	}
}