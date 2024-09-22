using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Dre0Dru.SceneManagement
{
    //TODO cancellation tokens?
    public interface ISceneLoader
    {
        int LoadedScenesCount { get; }

        IEnumerable<string> LoadedScenesNames { get; }

        void SetSceneActive(string sceneName);

        UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, IProgress<float> progress = null,
            bool makeActive = false);

        UniTask UnloadSceneAsync(string sceneName, UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.None,
            IProgress<float> progress = null);
    }
}
