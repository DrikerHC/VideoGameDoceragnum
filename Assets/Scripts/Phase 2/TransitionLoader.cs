using System;
using System.Collections;
using UnityEngine;

namespace Doceragnum
{
    public class TransitionLoader : MonoBehaviour
    {
        public static TransitionLoader Instance;
        private Animator _transition;

        public event Action OnTransitionDone;

        private void Awake()
        {
            Instance = this;
            _transition = GetComponent<Animator>();
        }

        public void BeginTransition()
        {
            Player.Instance.IsLocked = true;
            StartCoroutine(TransitionStartCoroutine());
        }

        private IEnumerator TransitionStartCoroutine()
        {
            _transition.SetTrigger("Start");
            yield return new WaitForSeconds(1f);
            OnTransitionDone.Invoke();
        }

        public void EndTransition()
        {
            StartCoroutine(TransitionEndCoroutine());
        }

        private IEnumerator TransitionEndCoroutine()
        {
            Player.Instance.IsLocked = true;
            _transition.SetTrigger("End");
            yield return new WaitForSeconds(1f);
            Player.Instance.IsLocked = false;
            PauseMenu.Instance.CanBeOpened = true;
        }
    }
}