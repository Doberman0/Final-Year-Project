using UnityEngine;
using System.Collections;

namespace UltimateSky {

	[RequireComponent (typeof(LensFlare))]
	public class FlareControl : MonoBehaviour {

		public Color color = Color.white;
		public float brightness = 1.0f;
		public float stability = 0.98f;

		protected LensFlare flare;

		protected static System.Random rnd;

		protected virtual void Awake () {
			
		}

		protected virtual void Start () {
			flare = this.GetComponent<LensFlare> ();
			flare.color = color;
		}
		
		protected virtual void Update () {
			if (rnd == null) {
				rnd = new System.Random();
			}
			float m = ((float)rnd.NextDouble ());
			Apply (m);
		}

		public void Apply(float phase) {
			
			flare = this.GetComponent<LensFlare> ();
			float strength = 0.1f;

			float d = phase * (1.0f - stability);

			flare.brightness = strength * (brightness * (stability + d));
			flare.color = color;
		}
	}
}
