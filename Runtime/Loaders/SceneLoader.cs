using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

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

        public UniTask UnloadSceneAsync(string sceneName, UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.None, IProgress<float> progress = null)
        {
            return SceneManager.UnloadSceneAsync(sceneName, unloadSceneOptions).ToUniTask(progress);
        }
    }
}
