using System;
using System.Collections;
using UnityEngine;
using Utilities.DataStructures;
using Utilities.DesignPatterns;

namespace Singletons
{
    public class DoubleBGMController : LSingleton<DoubleBGMController>
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _first;
        [SerializeField] private AudioClip _second;
        
        private void Start()
        {
            _audioSource.loop = true;
            _audioSource.playOnAwake = true;

            StartCoroutine(PlayCo());
        }

        private IEnumerator PlayCo()
        {
            while (true)
            {
                _audioSource.clip = _first;
                _audioSource.Play();
                yield return Wait.Seconds(_first.length);
                _audioSource.Stop();
                _audioSource.clip = _second;
                _audioSource.Play();
                yield return Wait.Seconds(_second.length);
                _audioSource.Stop();
            }
        }

        public void ChangeBGM(AudioClip bgm)
        {
            StopAllCoroutines();
            
            _audioSource.Stop();
            _audioSource.clip = bgm;
            _audioSource.Play();
        }
    }
}