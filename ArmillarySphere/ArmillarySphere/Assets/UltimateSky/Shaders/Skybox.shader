// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "UltimateSky/Skybox" {
	Properties {
		_LightPhase("Light Phase", Range(0,1)) = 0
		[NoScaleOffset] _StarsTex ("Stars Cubemap (HDR)", Cube) = "grey" {}
		_StarsBlend("Stars Blend", Range(0.0, 1.0)) = 1.0
		_StarsMax("Stars Max", Range(0.0, 1.0)) = 1.0
		_GroundColor ("Ground Color", Color) = (.13, .09, .05)
		[MaterialToggle] _EnableClouds("Enable Clouds", Float) = 0
		[MaterialToggle] _EnableGround("Enable Ground", Float) = 1
		[MaterialToggle] _EnableAtmosphere("Enable Atmosphere", Float) = 1
		_CloudTex1 ("Cloud Texture 1", 2D) = "white" { }
		_CloudTex2 ("Cloud Texture 2", 2D) = "white" { }
		_CloudHorizonTreshold ("CloudHorizonTreshold", Range(-0.1, 0.1)) = 0
	}

	SubShader {

		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }

		Cull Off 
		ZWrite Off


		//////////////////////////////////////////////////////////////////////////////////////
		// COMBINED DAY NIGHT PASS
		//////////////////////////////////////////////////////////////////////////////////////	
		Pass {

			
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#define PI 3.1415926535

			float _Phase;
			float _LightPhase;
			samplerCUBE _StarsTex;
			half4 _StarsTex_HDR;
			float4x4 _CubeRotation;
			float3 _CubeScale;
			float4 _LonLat;
			float _ObliquityEcliptic;
			float _EclipticLongitude;
			float _LocalMeanSiderealTime;
			float3 _GroundColor;
			float _EnableClouds;
			float _EnableGround;
			float _EnableAtmosphere;
			float _StarsBlend;
			float _StarsMax;
			float _CloudFadeIn;
			float _CloudGloominess;
			sampler2D _CloudTex1;
			sampler2D _CloudTex2;
			float _CloudScale1;
			float _CloudScale2;
			float3 _CloudShift1;
			float3 _CloudShift2;
			float _CloudHorizonTreshold;

			struct v2f 
			{
    			float4 pos : SV_POSITION;
    			float3 vertex : TEXCOORD0;
    			float3 uv : TEXCOORD1;
    			float3 c0 : COLOR0;
    			float3 c1 : COLOR1;
    			float3 n0 : COLOR2;
    			float3 n1 : COLOR3;
			};

			//////////////////////////////////////////////////////////////////////////////////////
			// LIGHT SCATTERING CONSTANTS AND FUNCTIONS
			//////////////////////////////////////////////////////////////////////////////////////	
			static const float fCameraHeight 		= 0.0001;
			static const float fESun 				= 20.0; 			// Sun brightness constant
			static const float fKr 					= 0.0025; 			// Rayleigh scattering constant
			static const float fKm 					= 0.0010; 			// Mie scattering constant
			static const float fG 					= -0.990;				// The Mie phase asymmetry factor, must be between 0.999 to -0.999
			static const float fG2					= fG*fG;				// The Mie phase asymmetry factor squared
			static const float fHdrExposure			= 0.8;		// HDR exposure
			static const float fKrESun				= fKr * fESun;			// Kr * ESun
			static const float fKmESun				= fKm * fESun;			// Km * ESun
			static const float fKr4PI				= fKr * 4 * PI;			// Kr * 4 * PI
			static const float fKm4PI				= fKm * 4 * PI;			// Km * 4 * PI

			uniform float fInnerRadius;
			uniform float fInnerRadius2;
			uniform float fOuterRadius;
			uniform float fOuterRadius2;
			uniform float fScale;
			uniform float fScaleDepth;
			uniform float fScaleOverScaleDepth;
			uniform float3 v3Wavelength;
			uniform float3 v3InvWavelength;
			

			float scale(float fCos)
			{
				float x = 1.0 - fCos;
				return 0.25 * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
			}

			// Calculates the Mie phase function
			float getMiePhase(float fCos, float fCos2, float g, float g2)
			{
				float _div = 1.0 + g2 - 2.0 * g * fCos;
				_div = pow(_div, pow(0.04,0.65) * 10);
				_div = max(_div,0.00001);
				return 1.5 * ((1.0 - g2) / (2.0 + g2)) * (1.0 + fCos2) / _div;
			}

			// Calculates the Rayleigh phase function
			half getRayleighPhase(half eyeCos2)
			{
				return 0.75 + 0.75*eyeCos2;
			}

			half getRayleighPhase(half3 light, half3 ray)
			{
				half eyeCos	= dot(light, ray);
				return getRayleighPhase(eyeCos * eyeCos);
			}

			//////////////////////////////////////////////////////////////////////////////////////
			// STAR SAMPLING CONSTANTS AND FUNCTIONS
			//////////////////////////////////////////////////////////////////////////////////////	

			float3 RotateX(float3 angle, float deg) {
				float r = deg * UNITY_PI / 180.0;
				float sina, cosa;
				sincos(r, sina, cosa);
				float3x3 m1 = float3x3(1.0, 0.0, 0.0, 0.0, cosa, -sina, 0.0, sina, cosa);
				return mul(m1, angle);
			}

			float3 RotateY(float3 angle, float deg) {
				float r = deg * UNITY_PI / 180.0;
				float sina, cosa;
				sincos(r, sina, cosa);
				float3x3 m1 = float3x3(0.0, 1.0, 0.0, cosa, 0.0, -sina, sina, 0.0, cosa);
				return mul(m1, angle);
			}

			float3 RotateZ(float3 angle, float deg) {
				float r = deg * UNITY_PI / 180.0;
				float sina, cosa;
				sincos(r, sina, cosa);
				float3x3 m1 = float3x3(0.0, 0.0, 1.0, cosa, -sina, 0.0, sina, cosa, 0.0);
				return mul(m1, angle);
			}

			float3 SampleStarCoords (float3 pos) {

				float lon = _LonLat.x;
				float lat = _LonLat.y;

				// rotate for latitude
				pos = RotateX(pos, lat);

				// rotate for ecliptic longitude
				// rotate for earth longitude
				pos = RotateZ(pos, -(lon+_Phase*360)-_EclipticLongitude);

				// adjust cubemap
				pos = mul(_CubeRotation, float4(pos,1)).xyz;

	    		return pos * _CubeScale;
			}

			//////////////////////////////////////////////////////////////////////////////////////
			// CLOUD SAMPLING CONSTANTS AND FUNCTIONS
			//////////////////////////////////////////////////////////////////////////////////////	

			const float2 UV_ZERO_OFFSET = float2(0,0);

			float2 GetCloudUV(half3 ray, float3 shift, float scale) {
				half y = ray.y;
				
				half2 uv = half2(0,0); 
				float x_d = ray.x * scale;
				float y_d = ray.z * scale;
				
				x_d = lerp(x_d, 0, sqrt(y*0.9));
				y_d = lerp(y_d, 0, sqrt(y*0.9));
				
				x_d = (x_d + 1.0) / 2;
				y_d = (y_d + 1.0) / 2;

				uv.x = x_d - shift.x;
				uv.y = y_d - shift.z;

				return uv;
			}

			float3 GetCloudColor(half3 ray, float3 shift, sampler2D tex, float scale) {
				float2 uv = GetCloudUV(ray, shift, scale);
				return tex2D (tex, uv);
			}


			float3 GetCloudNormal(half3 ray, float3 shift, sampler2D tex, float scale) {

				float2 _uvsize = float2(2.0, 0.0);
				float3 _uvoff = float3(0.01, 0.0, -0.01);

				float3 col = tex2Dlod(tex, float4(GetCloudUV(ray, shift, scale) + UV_ZERO_OFFSET, 0, 0));
				float3 _c01 = tex2Dlod(tex, float4(GetCloudUV(ray, shift, scale) + float2(0.01,0), 0, 0));
				float3 _c21 = tex2Dlod(tex, float4(GetCloudUV(ray, shift, scale) + float2(-0.01,0), 0, 0));
				float3 _c10 = tex2Dlod(tex, float4(GetCloudUV(ray, shift, scale) + float2(0,0.01), 0, 0));
				float3 _c12 = tex2Dlod(tex, float4(GetCloudUV(ray, shift, scale) + float2(0,-0.01), 0, 0));

				float s11 = col.x;

				float s01 = _c01.x;
				float s21 = _c21.x;
				float s10 = _c10.x;
				float s12 = _c12.x;

				//float3 va = normalize(float3(_uvsize.xy,s21-s01));
   				//float3 vb = normalize(float3(_uvsize.yx,s12-s10));
    			

    			float3 va = normalize(float3(_uvsize.x, s21-s01, _uvsize.y)); 
    			float3 vb = normalize(float3(_uvsize.y, s12-s10, _uvsize.x));

    			//float4 bump = float4(cross(va,vb), s11);

    			return cross(va,vb);
			}

			float4 BlendCloudLayer(float4 col, half3 ray, float3 _mie, float3 _col, float3 _norm) {
				float _DT = 0;
				float _density = sqrt(_col.r * _col.g * _col.b);
				if (_density > _DT) {

					float _alpha = _density;
					float _proximity = sqrt(sqrt(max(0,ray.y)));// - 0.025)));

					// color multiplier to tint clouds black at night
					float _multiplier = pow(min(1,_LightPhase*2), 4);

					// sun tint with sunlight
					// angle to sun from cloud
					float fCos = dot(_WorldSpaceLightPos0.xyz, -ray);
					float3 _cloudLightNormal = ray - _WorldSpaceLightPos0.xyz;
					float _clny = abs(length(_cloudLightNormal));
					float _fcl = (2-(fCos+1))/3;
					float3 _suntint = 3 * _mie * (_fcl/fKm);
					float _exp = (_fcl) * (_clny);

					half _nl = max(0, dot(-_norm, _WorldSpaceLightPos0.xyz));
                	float3 _diff = _nl * _LightColor0.rgb;
                	float3 _ambient = ShadeSH9(half4(-_norm,1));
                	fixed3 _lighting = _diff + _ambient;

                	_col = _col + (_col * _lighting + _col * ((1.0-_lighting)*_mie));
                	//_col = _col + (_col * _lighting);


					col = lerp(col, float4(_col*_multiplier, 1), min(1,_CloudFadeIn*_alpha*_proximity));
					//if (_CloudFadeIn == 1.0) 
					{
						col *= (1.0-((_CloudGloominess/2)*_CloudFadeIn));
					}

				}
				return col;
			}

			//////////////////////////////////////////////////////////////////////////////////////
			// VERTEX FUNCTION
			//////////////////////////////////////////////////////////////////////////////////////	

			v2f vert(appdata_base v)
			{

				v.texcoord = v.vertex;

				float3 v3CameraPos = float3(0,fInnerRadius + fCameraHeight,0); 	// The camera's current position

				float fCameraHeight = length(v3CameraPos);					// The camera's current height
			
				// Get the ray from the camera to the vertex and its length (which is the far point of the ray passing through the atmosphere)
				//float3 v3Ray = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
				float3 v3Ray = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
				float fFar = sqrt(fOuterRadius2 + fInnerRadius2 * v3Ray.y * v3Ray.y - fInnerRadius2) - fInnerRadius * v3Ray.y;//length(v3Ray);

				// Calculate the ray's starting position, then calculate its scattering offset
				float3 v3Start = v3CameraPos;
				float fHeight = length(v3Start);
				float fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fCameraHeight));
				float fStartAngle = dot(v3Ray, v3Start) / fHeight;
				float fStartOffset = fDepth*scale(fStartAngle);
				
				const float fSamples = 2.0;
			
				// Initialize the scattering loop variables
				float fSampleLength = fFar / fSamples;
				float fScaledLength = fSampleLength * fScale;
				float3 v3SampleRay = v3Ray * fSampleLength;
				float3 v3SamplePoint = v3Start + v3SampleRay * 0.5;
				
				// Now loop through the sample rays
				float3 v3FrontColor = float3(0.0, 0.0, 0.0);
				for(int i=0; i<int(fSamples); i++)
				{
					float fHeight = length(v3SamplePoint);
					float fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fHeight));

					float fLightAngle = dot(_WorldSpaceLightPos0.xyz, v3SamplePoint) / fHeight;

					float fCameraAngle = dot(v3Ray, v3SamplePoint) / fHeight;
					fCameraAngle = abs(fCameraAngle);

					float fScatter = (fStartOffset + fDepth*(scale(fLightAngle) - scale(fCameraAngle)));
					fScatter = abs(fScatter);

					float3 v3Attenuate = exp(-fScatter * (v3InvWavelength * fKr4PI + fKm4PI));

					v3FrontColor += v3Attenuate * (fDepth * fScaledLength);
					v3SamplePoint += v3SampleRay;


				}
			
    			v2f OUT;
    			OUT.pos = UnityObjectToClipPos(v.vertex);
    			OUT.vertex = v.vertex;

    			// Finally, scale the Mie and Rayleigh colors and set up the varying variables for the pixel shader
 
				OUT.c0 = v3FrontColor * (v3InvWavelength * fKrESun);
				OUT.c1 = v3FrontColor * fKmESun * ((1.0-clamp(-v3Ray.y,0,1)));

				// calculate star texture coordinate
				OUT.uv = SampleStarCoords(v.texcoord); 

				// calculate cloud normal
				float3 _cn1 = GetCloudNormal(v3Ray, _CloudShift1, _CloudTex1, _CloudScale1);
				float3 _cn2 = GetCloudNormal(v3Ray, _CloudShift2, _CloudTex2, _CloudScale2);
				OUT.n0 = _cn1;
				OUT.n1 = _cn2;

    			return OUT;
			}
			
			//////////////////////////////////////////////////////////////////////////////////////
			// FRAGMENT FUNCTION
			//////////////////////////////////////////////////////////////////////////////////////	
			
			half4 frag(v2f IN) : SV_Target
			{

				float4 col = float4(0,0,0,1);

				//half3 v3ray = normalize(mul((float3x3)unity_ObjectToWorld, -IN.vertex));
				half3 v3ray = normalize(mul((float3x3)unity_ObjectToWorld, -IN.vertex));

				//////////////////////////////////////////////////////////////////////////////////////
				// COLOR FROM LIGHT SCATTER
				//////////////////////////////////////////////////////////////////////////////////////
				float3 _rayleigh = float3(0,0,0);
				float3 _mie = float3(0,0,0);
				if (_EnableAtmosphere == 1) {
					if (v3ray.y < 0.2) {
						// calculate light scattering
						float fCos = dot(_WorldSpaceLightPos0.xyz, v3ray);
						float fCos2 = fCos*fCos;
						_rayleigh = getRayleighPhase(fCos2) * IN.c0;
						_mie = getMiePhase(fCos, fCos2, fG, fG2) * IN.c1 * _LightColor0.xyz;

						float3 _col = _rayleigh + _mie;

						// damp artifacts that appear below horizon
						if (v3ray.y > 0) {
							_col = lerp(_col, float4(0,0,0,1), v3ray.y*5);
						}

						// adjust color from HDR
						_col = 1.0 - exp(_col * -(_LightPhase*fHdrExposure*2));
						col.xyz = _col;
					}
				}
				// save values for further processing
				// check current athmospheric value
				float3 _skycolor = col.xyz;
				half _athmo = length(_skycolor)*_LightPhase*4;

				//////////////////////////////////////////////////////////////////////////////////////
				// GROUND COLOR
				//////////////////////////////////////////////////////////////////////////////////////
				if (_EnableGround == 1) {
					if (v3ray.y > 0) {
						// add simple ground coloring with slight fade towards horizon
						if (v3ray.y < 0.15) {
							col.xyz = lerp(_GroundColor, col.xyz, (0.15-v3ray.y)*6.666);
						} else {
							col.xyz = _GroundColor;
						}
					}
				}

				//////////////////////////////////////////////////////////////////////////////////////
				// COLOR FROM CUBE (STARMAP)
				//////////////////////////////////////////////////////////////////////////////////////
				if ((_EnableGround == 0 || v3ray.y < 0.1) && (_EnableAtmosphere == 0 || _LightPhase < 0.5)) {
					// sample from star texture cube
					half4 tex = texCUBE (_StarsTex, IN.uv);
					half3 _col = DecodeHDR (tex, _StarsTex_HDR);
					_col *= _StarsBlend;
					if (_StarsMax < 1.0 && length(_col) > _StarsMax) {
						_col *= (_StarsMax / length(_col));
					}

					// if star texture pixel brightness exceeds previous value
					float _C = length(_col);
					if (_C > length(col.xyz)) {
						// reduce athmospheric value from star brightness and apply with remains as alpha
						//float _S = max(_col.r, _col.b);
						//half _brightness = (_S*_C)*(1.0-_LightPhase) - _athmo;
						half _brightness = (_C)*(1.0-_LightPhase) - _athmo;

						if (_brightness > 0) {
							//col.xyz = lerp(col.xyz, _col, sqrt(_brightness));
							col.xyz += _col * _brightness;
						}
					}
				}

				//////////////////////////////////////////////////////////////////////////////////////
				// COLOR FROM CLOUDS
				//////////////////////////////////////////////////////////////////////////////////////
				if (_EnableClouds == 1 && _CloudFadeIn > 0 && v3ray.y < _CloudHorizonTreshold) { // -0.025) {
					//half3 ray = normalize(mul((float3x3)unity_ObjectToWorld, IN.vertex));
					half3 ray = normalize(mul((float3x3)unity_ObjectToWorld, IN.vertex));

					float3 _col1 = GetCloudColor(ray, _CloudShift1, _CloudTex1, _CloudScale1);
					col = BlendCloudLayer(col, ray, _mie, _col1, IN.n0);

					float3 _col2 = GetCloudColor(ray, _CloudShift2, _CloudTex2, _CloudScale2);
					col = BlendCloudLayer(col, ray, _mie, _col2, IN.n1);
				}
				col.a = length(col.rgb);
				return col;
			}
			
			ENDCG
			
		}

		//GrabPass {"_SkyboxPass"}
	}

	Fallback Off

}
