using UnityEngine;

public class PipeParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;

    public void SetColors(Color color)
    {
        foreach (var ps in particleSystems)
        {
            var main = ps.main;
            main.startColor = color;
        }
    }
}
