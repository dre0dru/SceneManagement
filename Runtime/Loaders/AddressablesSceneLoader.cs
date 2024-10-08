#if ADDRESSABLES_SUPPORT
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Dre0Dru.SceneManagement
{
    public class AddressablesSceneLoader : ISceneLoader
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly HashSet<string> _buildSettingsScenesNames;
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> _addressablesScenes;

        public int LoadedScenesCount => _sceneLoader.LoadedScenesCount;

        public IEnumerable<string> LoadedScenesNames => _sceneLoader.LoadedScenesNames;

        [RequiredMember]
        public AddressablesSceneLoader()
        {
            _sceneLoader = new SceneLoader();
            _buildSettingsScenesNames = GetBuildSettingsScenes();
            _addressablesScenes = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
        }

        public void SetSceneActive(string sceneName)
        {
            _sceneLoader.SetSceneActive(sceneName);
        }

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode,
            IProgress<float> progress = null, bool makeActive = false)
        {
            if (IsBuildSettingsScene(sceneName))
            {
                await _sceneLoader.LoadSceneAsync(sceneName, loadSceneMode, progress, makeActive);
                return;
            }

            var handle = Addressables.LoadSceneAsync(sceneName, loadSceneMode);
            _addressablesScenes[sceneName] = handle;
            await handle.ToUniTask(progress);

            if (makeActive)
            {
                SetSceneActive(sceneName);
            }
        }

        public UniTask UnloadSceneAsync(string sceneName, UnloadSceneOptions unloadSceneOptions =
 UnloadSceneOptions.None,
            IProgress<float> progress = null)
        {
            if (IsBuildSettingsScene(sceneName))
            {
                return _sceneLoader.UnloadSceneAsync(sceneName, unloadSceneOptions, progress);
            }

            var sceneHandle = _addressablesScenes[sceneName];

            _addressablesScenes.Remove(sceneName);
            
            return Addressables.UnloadSceneAsync(sceneHandle, true).ToUniTask(progress);
        }

        private HashSet<string> GetBuildSettingsScenes()
        {
            var result = new HashSet<string>();

            var scenesCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < scenesCount; i++)
            {
                result.Add(SceneManager.GetSceneByBuildIndex(i).name);
            }

            return result;
        }

        private bool IsBuildSettingsScene(string sceneName)
        {
            return _buildSettingsScenesNames.Contains(sceneName);
        }
    }
}

#endif
