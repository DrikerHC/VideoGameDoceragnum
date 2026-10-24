using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SceneController
{

    // Manages additive scene loading and unloading using a slot-based system.
    // Inherits from a Singleton to ensure only one instance handles scene transitions.
    public class SceneController : Singleton<SceneController>
    {
        // Prevents multiple scene transition operations from running concurrently
        private bool isBusy;

        // Tracks which scene name is currently occupying a specific slot key (e.g., "Menu" -> "MainMenuScene")
        private readonly Dictionary<string, string> loadedScenesBySlots = new();

        // A builder class used to configure and queue up a multi-scene transition plan.
        public class SceneTransitionPlanner
        {
            // Configuration properties populated via Fluent API chaining
            public Dictionary<string, string> ScenesToLoad { get; } = new();
            public List<string> SlotToUnload { get; } = new();
            public string ActiveSceneName { get; private set; } = string.Empty;
            public bool ClearUnusedAssets { get; private set; }
            public bool Overlay { get; private set; }

            // Action to execute once the entire transition completes successfully
            private Action onComplete;

            // Queues a scene to be loaded into a specific slot
            public SceneTransitionPlanner Load(string slotKey, string sceneName, bool setActive = true)
            {
                ScenesToLoad[slotKey] = sceneName;
                if (setActive)
                {
                    ActiveSceneName = sceneName;
                }

                return this; // Returns builder instance for method chaining
            }

            // Queues a specific slot to be unloaded during the transition.
            public SceneTransitionPlanner Unload(string slotKey)
            {
                if (slotKey != null) SlotToUnload.Add(slotKey);
                return this;
            }

            // Forces Unity to release unused assets from RAM during the transition.
            public SceneTransitionPlanner WithClearUnusedAssets()
            {
                ClearUnusedAssets = true;
                return this;
            }

            // Flags the transition to use a black loading screen overlay fade.
            public SceneTransitionPlanner WithOverlay()
            {
                Overlay = true;
                return this;
            }

            // Assigns a callback method to run when all loads and unloads finish.
            public SceneTransitionPlanner OnComplete(Action callback)
            {
                onComplete = callback;
                return this;
            }

            // Dispatches the configured plan to the SceneController singleton to begin execution.
            public Coroutine Execute()
            {
                return Instance.ExecutePlan(this);
            }

            // Internal helper to fire the completion callback safely
            internal void InvokeOnComplete()
            {
                onComplete?.Invoke();
            }

            // Internal helper to clean up references and avoid memory leaks
            internal void RemoveCallback()
            {
                if (onComplete != null)
                {
                    onComplete = null;
                }
            }
        }

        // Entry point to construct a brand-new scene transition batch.
        public SceneTransitionPlanner NewPlan()
        {
            return new SceneTransitionPlanner();
        }

        // Kicks off the asynchronous scene manipulation coroutine
        private Coroutine ExecutePlan(SceneTransitionPlanner plan)
        {
            if (isBusy)
            {
                Debug.LogWarning("Scene is already busy processing a transition!");
                return null;
            }
            isBusy = true;
            return StartCoroutine(ChangeScene(plan));
        }

        // The master workflow execution sequence for the transition plan.
        // Handles the orderly timing of visual fades, unloading, sweeping memory, and loading.
        private IEnumerator ChangeScene(SceneTransitionPlanner plan)
        {
            try
            {
                // Step 1: Cover up the screen if an overlay is requested
                if (plan.Overlay)
                {
                    yield return LoadingOverlay.Instance.FadeInBlack();
                    yield return new WaitForSeconds(0.5f); // Soft buffer for visual comfort
                }

                // Step 2: Clear out explicitly requested slots
                foreach (var slotKey in plan.SlotToUnload) yield return UnloadSlot(slotKey);
                
                // Step 3: Garbage collect assets if flag was set
                if (plan.ClearUnusedAssets) yield return ClearUnused();

                // Step 4: Handle new scene allocations
                foreach (var (slotKey, sceneName) in plan.ScenesToLoad)
                {
                    // Implicit safety: If the target slot is currently occupied, evict the old occupant first
                    if (loadedScenesBySlots.ContainsKey(slotKey))
                    {
                        yield return UnloadSlot(slotKey);
                    }

                    yield return loadAdditiveScene(slotKey, sceneName, plan.ActiveSceneName == sceneName);
                }

                // Step 5: Reveal the new scenes
                if (plan.Overlay)
                {
                    yield return LoadingOverlay.Instance.FadeOutBlack();
                }

                // Step 6: Notify developers/systems listening to this plan's completion
                plan.InvokeOnComplete();
                plan.RemoveCallback();
            }
            finally
            {
                // Always reset busy flag, even if an unexpected error occurs inside the try block
                isBusy = false;
            }
        }

        // Asynchronously loads a scene additively, waiting for activation to safely sync it.
        private IEnumerator loadAdditiveScene(string slotKey, string sceneName, bool isActive)
        {
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadOperation == null)
            {
                yield break;
            }

            // Prevent scene from instantly initializing visuals/audio before it finishes raw loading
            loadOperation.allowSceneActivation = false;
            
            // Unity pauses async progress at 0.9f when allowSceneActivation is false
            while (loadOperation.progress < 0.9f)
            {
                yield return null;
            }
            
            // Allow the scene to cross the finish line and initialize
            loadOperation.allowSceneActivation = true;

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            // Handle designating this scene as primary if specified
            if (isActive)
            {
                var newScene = SceneManager.GetSceneByName(sceneName);
                if (newScene.IsValid() && newScene.isLoaded)
                {
                    SceneManager.SetActiveScene(newScene);
                }
            }

            // Map the slot successfully to the freshly loaded scene name
            loadedScenesBySlots[slotKey] = sceneName;
        }

        // Yields until Unity completes a comprehensive asset garbage collection sweep.
        private IEnumerator ClearUnused()
        {
            var unloadOperation = Resources.UnloadUnusedAssets();
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }

        // Asynchronously discards a scene associated with a slot identifier and cleans up the tracking dictionary.
        private IEnumerator UnloadSlot(string slotKey)
        {
            // Early break if the slot doesn't exist or is empty
            if (!loadedScenesBySlots.TryGetValue(slotKey, out var sceneName) || string.IsNullOrEmpty(sceneName))
            {
                yield break;
            }

            var unloadOperation = SceneManager.UnloadSceneAsync(sceneName);
            if (unloadOperation == null)
            {
                yield break;
            }

            while (!unloadOperation.isDone)
            {
                yield return null;
            }

            // Safely clear the tracking reference now that the scene is physically purged from memory
            loadedScenesBySlots.Remove(slotKey);
        }
    }
}