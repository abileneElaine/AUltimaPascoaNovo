using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticle : MonoBehaviour
{
    ParticleSystem ps;
    void Awake() {
        ps = GetComponent<ParticleSystem>();
    }
    void OnEnable() {
        // destroi depois que o sistema parar de emitir e todas partículas sumirem
        Invoke(nameof(CheckAndDestroy), ps.main.duration + ps.main.startLifetime.constantMax + 0.1f);
    }
    void CheckAndDestroy() {
        if (!ps.IsAlive(true)) Destroy(gameObject);
        else Invoke(nameof(CheckAndDestroy), 0.2f);
    }
}