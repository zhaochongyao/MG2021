using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StoryLine
{
    public class OpenDoor : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private Collider _collider;
        
        public static event Action DoorOpen = delegate { };

        [SerializeField] private AudioClip _openDoorSound;
        private AudioSource _audioSource;
        
        private void Start()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
        }

        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Input.GetMouseButtonDown(0))
            {
                _animator.enabled = true;
                _collider.enabled = false;
                _audioSource.PlayOneShot(_openDoorSound);
                DoorOpen.Invoke();
            }
        }
    }
}