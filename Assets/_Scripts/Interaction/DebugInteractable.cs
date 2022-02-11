using UnityEngine;

namespace _Scripts.Interaction
{
    public class DebugInteractable : MonoBehaviour, Interactable
    {
        [SerializeField] private SpawnParticlesOnDrop debugSpawner;
    
        public void Interact()
        {
            debugSpawner.SpawnDropParticles();
        }
    }
}