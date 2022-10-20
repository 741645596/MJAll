using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

namespace UIParticles
{
    public class UIParticlePool
    {
        static UIParticlePool instance;
        public static UIParticlePool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIParticlePool();
                }
                return instance;
            }
        }

        List<ParticleSystem.Particle[]> pools;

        public UIParticlePool()
        {
            pools = new List<ParticleSystem.Particle[]>();
        }

        public ParticleSystem.Particle[] Get(int size)
        {
            int count = pools.Count;
            ParticleSystem.Particle[] particles;
            if (count == 0)
            {
                particles = new ParticleSystem.Particle[size];
                return particles;
            }
            for (int i = count - 1; i >= 0; i--)
            {
                particles = pools[i];
                if (particles.Length == size)
                {
                    pools.RemoveAt(i);
                    //Debug.LogWarning($"Use Pool  {size}");
                    return particles;
                }
            }
            particles = new ParticleSystem.Particle[size];
            return particles;
        }

        public void Release(ParticleSystem.Particle[] unit)
        {
            pools.Add(unit);
            //Debug.LogWarning($"Release Particle  {pools.Count}");
        }
    }
}