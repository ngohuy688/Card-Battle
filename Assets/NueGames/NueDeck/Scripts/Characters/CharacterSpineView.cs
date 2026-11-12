using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NueGames.NueDeck.Scripts.Characters
{
    public class CharacterSpineView : MonoBehaviour
    {
        [Header("Spine References")]
        [SerializeField] private SkeletonAnimation skeletonAnimation;

        [Header("Animation Names")]
        [SerializeField] private string idleAnimation;
        [SerializeField] private string attackAnimation;
        [SerializeField] private string hitAnimation;
        [SerializeField] private string healAnimation;
        [SerializeField] private string deathAnimation;

        private readonly HashSet<string> _reportedMissingAnimations = new HashSet<string>();

        private void Awake()
        {
            if (!skeletonAnimation)
                skeletonAnimation = GetComponent<SkeletonAnimation>();

            if (!skeletonAnimation)
                skeletonAnimation = GetComponentInChildren<SkeletonAnimation>(true);
        }

        public void PlayIdle()
        {
            TryPlay(idleAnimation, true);
        }

        public void PlayAttack()
        {
            PlayTransient(attackAnimation);
        }

        public void PlayHit()
        {
            PlayTransient(hitAnimation);
        }

        public void PlayHeal()
        {
            PlayTransient(healAnimation);
        }

        public void PlayDeath(Action onComplete)
        {
            if (!TryPlay(deathAnimation, false, out var trackEntry))
            {
                onComplete?.Invoke();
                return;
            }

            trackEntry.Complete += _ => onComplete?.Invoke();
        }

        private void PlayTransient(string animationName)
        {
            if (!TryPlay(animationName, false))
                return;

            if (HasAnimation(idleAnimation))
                skeletonAnimation.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }

        private bool TryPlay(string animationName, bool loop)
        {
            return TryPlay(animationName, loop, out _);
        }

        private bool TryPlay(string animationName, bool loop, out TrackEntry trackEntry)
        {
            trackEntry = null;
            if (string.IsNullOrWhiteSpace(animationName) || !skeletonAnimation ||
                skeletonAnimation.Skeleton == null || skeletonAnimation.AnimationState == null)
                return false;

            if (!HasAnimation(animationName))
                return false;

            trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, animationName, loop);
            return trackEntry != null;
        }

        private bool HasAnimation(string animationName)
        {
            if (string.IsNullOrWhiteSpace(animationName) || !skeletonAnimation ||
                skeletonAnimation.Skeleton == null || skeletonAnimation.Skeleton.Data == null)
                return false;

            if (skeletonAnimation.Skeleton.Data.FindAnimation(animationName) != null)
                return true;

            if (_reportedMissingAnimations.Add(animationName))
                Debug.LogWarning($"Spine animation '{animationName}' was not found on '{name}'.", this);

            return false;
        }
    }
}
