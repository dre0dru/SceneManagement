using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
#if VCONTAINER_SUPPORT
using VContainer;
using VContainer.Unity;
#endif

namespace Dre0Dru.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        public int LoadedScenesCount => SceneManager.sceneCount;

        public IEnumerable<string> LoadedScenesNames
        {
            get
            {
                var loadedScenesCount = LoadedScenesCount;
                var scenesNames = new string[loadedScenesCount];

                for (int i = 0; i < loadedScenesCount; i++)
                {
                    scenesNames[i] = SceneManager.GetSceneAt(i).name;
                }

                return scenesNames;
            }
        }
        
        [RequiredMember]
        public SceneLoader()
        {
        
        }

        public void SetSceneActive(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode,
            IProgress<float> progress = null, bool makeActive = false)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode).ToUniTask(progress);

            if (makeActive)
            {
                SetSceneActive(sceneName);
            }
        }

        #if VCONTAINER_SUPPORT
        public async UniTask LoadSceneAsyncWithExtraBindings(string sceneName, LoadSceneMode loadSceneMode,
            Action<IContainerBuilder> extraBindings,
            IProgress<float> progress = null, bool makeActive = false)
        {
            using (LifetimeScope.Enqueue(extraBindings))
            {
                await LoadSceneAsync(sceneName, loadSceneMode, progress, makeActive);
            }
        }
        #endif

        public UniTask UnloadSceneAsync(string sceneName, UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.None, IProgress<float> progress = null)
        {
            return SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.None).ToUniTask(progress);
        }
    }
}
