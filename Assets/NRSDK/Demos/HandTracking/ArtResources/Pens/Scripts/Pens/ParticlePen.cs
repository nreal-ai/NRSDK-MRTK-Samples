using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePen : BasePen
{
    public ParticleSystem[] particles;

    void Update()
    {
        if (IsDrawing)
        {
            PlayAllParticles();
        }
        else
        {
            StopAllParticles();
        }
    }

    private void PlayAllParticles()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i] == null)
                continue;
            if (!particles[i].isPlaying)
            {
                particles[i].Play();
            }
        }
    }

    private void StopAllParticles()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if (particles[i] == null)
                continue;
            if (particles[i].isPlaying)
            {
                particles[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
