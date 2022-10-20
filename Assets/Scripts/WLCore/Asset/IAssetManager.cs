// IAssetManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/24

using System;

namespace Unity.Core
{
    public interface IAssetManager
    {
        T Load<T>(string res, string key) where T : UnityEngine.Object;

        UnityEngine.Object Load(string res, string key);

        void LoadAsync<T>(string res, string key, Action<T> action) where T : UnityEngine.Object;
        void LoadAsync(string res, string key, Action<UnityEngine.Object> action);

        void LoadAllAsync(string res, Action<UnityEngine.Object[]> action);
        void CancelLoadAsync();
        UnityEngine.Object[] LoadAll(string res);

        void Unload(string res, bool unload = false);
        void UnloadAll(bool unload = false);

        bool Contains(string res, string key);
        void LoadDependencieMF(string name);
        void CleanDependencieMF();
    }
}
