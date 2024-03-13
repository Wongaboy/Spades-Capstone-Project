using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private VisualEffect effect;

    public void TriggerFX(Transform pos)
    {
        foreach (ParticleSystem p in particles)
        {
            ParticleSystem p_copy = Instantiate(p, pos.position, pos.rotation);
            p_copy.Play();
        }
        if (effect != null)
        {
            effect.Play();
        }
    }

}
