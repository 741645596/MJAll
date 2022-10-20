using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Unity.Collections;
using static UnityEngine.ParticleSystem;

namespace UIParticles
{
    /// <summary>
    /// Ui Parcticles, requiere ParticleSystem component
    /// </summary>
    [RequireComponent (typeof(ParticleSystem))]
	public class UIParticles : MaskableGraphic
	{
        public static bool USE_POOL = false;
		#region InspectorFields

		/// <summary>
		/// ParticleSystem used for generate particles
		/// </summary>
		[SerializeField]
	    [FormerlySerializedAs ("m_ParticleSystem")]
		private ParticleSystem m_ParticleSystem;

	    /// <summary>
	    /// If true, particles renders in streched mode
	    /// </summary>
	    [FormerlySerializedAs ("m_RenderMode")]
	    [SerializeField]
	    [Tooltip("Render mode of particles")]
	    private UiParticleRenderMode m_RenderMode  = UiParticleRenderMode.Billboard;

	    /// <summary>
	    /// Scale particle size, depends on particle velocity
	    /// </summary>
	    [FormerlySerializedAs ("m_StretchedSpeedScale")]
	    [SerializeField]
	    [Tooltip("Speed Scale for streched billboards")]
	    private float m_StretchedSpeedScale = 1f;

	    /// <summary>
	    /// Sclae particle length in streched mode
	    /// </summary>
	    [FormerlySerializedAs ("m_StretchedLenghScale")]
	    [SerializeField]
	    [Tooltip("Speed Scale for streched billboards")]
	    private float m_StretchedLenghScale = 1f;


		[FormerlySerializedAs ("m_IgnoreTimescale")]
		[SerializeField]
		[Tooltip("If true, particles ignore timescale")]
		private bool m_IgnoreTimescale = false;

	    #endregion





		#region Public properties
		/// <summary>
		/// ParticleSystem used for generate particles
		/// </summary>
		/// <value>The particle system.</value>
		public ParticleSystem ParticleSystem {
			get { return m_ParticleSystem; }
			set {
				if (UISetPropertyUtility.SetClass (ref m_ParticleSystem, value))
					SetAllDirty ();
			}
		}

		/// <summary>
		/// Texture used by the particles
		/// </summary>
        /// 
        //
		//public override Texture mainTexture {
		//	get {
		//		if (material != null && material.mainTexture != null) {
		//			return material.mainTexture;
		//		}
		//		return s_WhiteTexture;
		//	}
		//}

        /// <summary>
        /// Particle system render mode (billboard, strechedBillobard)
        /// </summary>
	    public UiParticleRenderMode RenderMode
	    {
	        get { return m_RenderMode; }
	        set
	        {
	            if(UISetPropertyUtility.SetStruct(ref m_RenderMode, value))
	                SetAllDirty();
	        }
	    }

		#endregion


		private ParticleSystemRenderer m_ParticleSystemRenderer;
        private bool m_InitParticles = false;
		private ParticleSystem.Particle[] m_Particles;

        private Vector2[] m_uvs = new Vector2[4];
		private MinMaxCurve m_frameOverTime;
		private TextureSheetAnimationModule m_textureAnimator;

		protected override void Awake ()
		{
			var _particleSystem = GetComponent<ParticleSystem> ();
			var _particleSystemRenderer = GetComponent<ParticleSystemRenderer> ();
			if (m_Material == null) {
				m_Material = _particleSystemRenderer.sharedMaterial;
			}
		    if(_particleSystemRenderer.renderMode == ParticleSystemRenderMode.Stretch)
		        RenderMode = UiParticleRenderMode.StreachedBillboard;

			base.Awake ();
			ParticleSystem = _particleSystem;
            var m = _particleSystem.main;
            m.maxParticles = m.maxParticles > 120 ? 120 : m.maxParticles;
            raycastTarget = false;
			m_ParticleSystemRenderer = _particleSystemRenderer;
			m_textureAnimator = _particleSystem.textureSheetAnimation;
			m_frameOverTime = m_textureAnimator.frameOverTime;
        }

		protected override void OnDestroy()
        {
            if (m_InitParticles)
            {
                if (USE_POOL)
                {
					UIParticlePool.Instance.Release(m_Particles);
                }
            }
        }


		public override void SetMaterialDirty ()
		{
			base.SetMaterialDirty ();
			if (m_ParticleSystemRenderer != null)
				m_ParticleSystemRenderer.sharedMaterial = m_Material;
		}

		protected override void OnPopulateMesh (VertexHelper toFill)
		{
			if (m_ParticleSystem == null) {
				base.OnPopulateMesh (toFill);
				return;
			}
			GenerateParticlesBillboards (toFill);

		}

		protected virtual void Update ()
		{
			if (!m_IgnoreTimescale)
			{
				if (m_ParticleSystem != null && m_ParticleSystem.isPlaying)
				{
					SetVerticesDirty();
				}
			}
			else
			{
				if (m_ParticleSystem != null)
				{
					m_ParticleSystem.Simulate(Time.unscaledDeltaTime, true, false);
					SetVerticesDirty();
				}
			}

			// disable default particle renderer, we using our custom
			if (m_ParticleSystemRenderer != null && m_ParticleSystemRenderer.enabled)
				m_ParticleSystemRenderer.enabled = false;
		}


		private void GenerateParticlesBillboards (VertexHelper vh)
		{
            if (USE_POOL)
            {
                if (!m_InitParticles)
                {
                    m_Particles = UIParticlePool.Instance.Get(m_ParticleSystem.main.maxParticles);
                    m_InitParticles = true;
                    if (m_ParticleSystem.main.maxParticles > 200)
                    {
                        if (gameObject.transform.parent != null)
                        {
							WLDebug.LogWarning($"粒子个数太多 {gameObject.transform.parent.name} : {ParticleSystem.main.maxParticles}");
                        }
                        else
                        {
							WLDebug.LogWarning($"粒子个数太多 {gameObject.name} : {ParticleSystem.main.maxParticles}");
                        }
                    }
                }
            }
            else
            {
                if (m_Particles == null || m_Particles.Length < m_ParticleSystem.main.maxParticles)
                {
                    m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
                }
            }
			int numParticlesAlive = m_ParticleSystem.GetParticles(m_Particles);
            int length = numParticlesAlive > m_Particles.Length ? m_Particles.Length : numParticlesAlive;
			vh.Clear ();

			for (int i = 0; i < length; i++) {
				DrawParticleBillboard (m_Particles [i], vh);
			}
		}

		private void DrawParticleBillboard (ParticleSystem.Particle particle, VertexHelper vh)
		{
			var center =  particle.position;
			var rotation = Quaternion.Euler (particle.rotation3D);


			if (m_ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
			{
				center = rectTransform.InverseTransformPoint (center);
			}

			float timeAlive = particle.startLifetime - particle.remainingLifetime;
			float globalTimeAlive = timeAlive / particle.startLifetime;

			Vector3 size3D = particle.GetCurrentSize3D (m_ParticleSystem);

			if(m_RenderMode == UiParticleRenderMode.StreachedBillboard)
			{
				GetStrechedBillboardsSizeAndRotation(particle,globalTimeAlive,ref size3D, out rotation);
			}

            var halfX = size3D.x * 0.5f;
            var halfy = size3D.y * 0.5f;
            var leftTop = new Vector3(-halfX, halfy);
            var rightTop = new Vector3(halfX, halfy);
            var rightBottom = new Vector3(halfX, -halfy);
            var leftBottom = new Vector3(-halfX, -halfy);


			leftTop = rotation * leftTop + center;
			rightTop = rotation * rightTop + center;
			rightBottom = rotation * rightBottom + center;
			leftBottom = rotation * leftBottom + center;

			Color32 color32 = particle.GetCurrentColor (m_ParticleSystem);
			var i = vh.currentVertCount;

			if (!m_ParticleSystem.textureSheetAnimation.enabled)
			{
				EvaluateQuadUVs(m_uvs);
			}
			else
			{
				EvaluateTexturesheetUVs(particle, timeAlive, m_uvs);
			}

			vh.AddVert (leftBottom, color32, m_uvs[0]);
			vh.AddVert (leftTop, color32, m_uvs[1]);
			vh.AddVert (rightTop, color32, m_uvs[2]);
			vh.AddVert (rightBottom, color32, m_uvs[3]);

			vh.AddTriangle (i, i + 1, i + 2);
			vh.AddTriangle (i + 2, i + 3, i);
		}


		/// <summary>
		/// Evaluate uvs for simple billboard without animations
		/// </summary>
		/// <param name="uvs"></param>
		private void EvaluateQuadUVs(Vector2[] uvs)
		{
			//uvs[0] = new Vector2(0f, 0f);
			//uvs[1] = new Vector2(0f, 1f);
			//uvs[2] = new Vector2(1f, 1f);
			//uvs[3] = new Vector2(1f, 0f);

			uvs[0].x = 0f;
			uvs[0].y = 0f;
			uvs[1].x = 0f;
			uvs[1].y = 1f;
			uvs[2].x = 1f;
			uvs[2].y = 1f;
			uvs[3].x = 1f;
			uvs[3].y = 0f;
		}

		/// <summary>
		/// Evaluate uvs for billboard with texturesheet animation
		/// </summary>
		/// <param name="particle">target particle</param>
		/// <param name="timeAlive"></param>
		/// <param name="uvs"></param>
		private void EvaluateTexturesheetUVs(ParticleSystem.Particle particle, float timeAlive, Vector2[] uvs)
		{
			float lifeTimePerCycle = particle.startLifetime / m_textureAnimator.cycleCount;
			float timePerCycle = timeAlive % lifeTimePerCycle;
			float timeAliveAnim01 = timePerCycle / lifeTimePerCycle; // in percents


			var totalFramesCount = m_textureAnimator.numTilesY * m_textureAnimator.numTilesX;
			var frame01 = m_frameOverTime.Evaluate(timeAliveAnim01);

			var frame = 0f;
			switch (m_textureAnimator.animation)
			{
				case ParticleSystemAnimationType.WholeSheet:
				{
					frame = Mathf.Clamp(Mathf.Floor(frame01 * totalFramesCount), 0, totalFramesCount - 1);
					break;
				}
				case ParticleSystemAnimationType.SingleRow:
				{
					frame = Mathf.Clamp(Mathf.Floor(frame01 * m_textureAnimator.numTilesX), 0, m_textureAnimator.numTilesX - 1);
					int row = m_textureAnimator.rowIndex;
                    //if (m_textureAnimator.useRandomRow)  源码修改，去掉警告
                    if (m_textureAnimator.rowMode == ParticleSystemAnimationRowMode.Random)
					{
						Random.InitState((int) particle.randomSeed);
						row = Random.Range(0, m_textureAnimator.numTilesY);
					}
					frame += row * m_textureAnimator.numTilesX;
					break;
				}
			}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        

			int x = (int) frame % m_textureAnimator.numTilesX;
			int y = (int) frame / m_textureAnimator.numTilesX;


			var xDelta = 1f / m_textureAnimator.numTilesX;
			var yDelta = 1f / m_textureAnimator.numTilesY;
			y = m_textureAnimator.numTilesY - 1 - y;
			var sX = x * xDelta;
			var sY = y * yDelta;
			var eX = sX + xDelta;
			var eY = sY + yDelta;

			//uvs[0] = new Vector2(sX, sY);
			//uvs[1] = new Vector2(sX, eY);
			//uvs[2] = new Vector2(eX, eY);
			//uvs[3] = new Vector2(eX, sY);
			uvs[0].x = sX;
			uvs[0].y = sY;
			uvs[1].x = sX;
			uvs[1].y = eY;
			uvs[2].x = eX;
			uvs[2].y = eY;
			uvs[3].x = eX;
			uvs[3].y = sY;

		}


		/// <summary>
		/// Evaluate size and roatation of particle in streched billboard mode
		/// </summary>
		/// <param name="particle">particle</param>
		/// <param name="timeAlive01">current life time percent [0,1] range</param>
		/// <param name="size3D">particle size</param>
		/// <param name="rotation">particle rotation</param>
		private void GetStrechedBillboardsSizeAndRotation(ParticleSystem.Particle particle, float timeAlive01,
			ref Vector3 size3D, out Quaternion rotation)
		{
			var velocityOverLifeTime = Vector3.zero;

			if (m_ParticleSystem.velocityOverLifetime.enabled)
			{
				velocityOverLifeTime.x = m_ParticleSystem.velocityOverLifetime.x.Evaluate(timeAlive01);
				velocityOverLifeTime.y = m_ParticleSystem.velocityOverLifetime.y.Evaluate(timeAlive01);
				velocityOverLifeTime.z = m_ParticleSystem.velocityOverLifetime.z.Evaluate(timeAlive01);
			}

			var finalVelocity = particle.velocity + velocityOverLifeTime;
			var ang = Vector3.Angle(finalVelocity,  Vector3.up);
			var horizontalDirection = finalVelocity.x < 0 ? 1 : -1;
			rotation = Quaternion.Euler(new Vector3(0,0, ang*horizontalDirection));
			size3D.y *=m_StretchedLenghScale;
			size3D+= new Vector3(0, m_StretchedSpeedScale*finalVelocity.magnitude);
		}
	}


	/// <summary>
	/// Particles Render Modes
	/// </summary>
    public enum UiParticleRenderMode
    {
        Billboard,
        StreachedBillboard
    }
}
