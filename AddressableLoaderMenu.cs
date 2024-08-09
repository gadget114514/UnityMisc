/*
AddressableWrapper

MIT License

Copyright (c) 2021 kyubuns

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace KyubunsSandbox.Editor
{
    public static class AddressableLoaderMenu
    {
        private static string PlayerPrefsKey => $"{PlayerSettings.productName}.Addressable";
        private const int Default = 0;
        private const int Fast = 1;
        private const int Slow = 2;

        [InitializeOnEnterPlayMode]
        private static void InitAddressableLoader(EnterPlayModeOptions options)
        {
            var v = EditorPrefs.GetInt(PlayerPrefsKey, 0);
            if (v == Default) AddressableWrapper.AddressableLoader = new AddressableWrapper.DefaultAddressableLoader();
            if (v == Fast) AddressableWrapper.AddressableLoader = new EditorOptimizedAddressable();
            if (v == Slow) AddressableWrapper.AddressableLoader = new EditorSlowAddressable();
        }

        public class EditorOptimizedAddressable : AddressableWrapper.IAddressableLoader
        {
            public UniTask<IDisposableAsset<TObject>> LoadAssetAsync<TObject>(string address, CancellationToken cancellationToken) where TObject : UnityEngine.Object
            {
                return LoadAssetInternal<TObject>(address, cancellationToken);
            }

            public static UniTask<IDisposableAsset<TObject>> LoadAssetInternal<TObject>(string address, CancellationToken cancellationToken) where TObject : UnityEngine.Object
            {
                foreach (var entity in UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.groups.SelectMany(x => x.entries))
                {
                    if (entity.address == address)
                    {
                        return UniTask.FromResult<IDisposableAsset<TObject>>(new DisposableAsset<TObject>((TObject) entity.TargetAsset, () => { }));
                    }
                }

                throw new Exception($"{address} is not found");
            }
        }

        public class EditorSlowAddressable : AddressableWrapper.IAddressableLoader
        {
            public async UniTask<IDisposableAsset<TObject>> LoadAssetAsync<TObject>(string address, CancellationToken cancellationToken) where TObject : UnityEngine.Object
            {
                await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, 1f)));
                return await EditorOptimizedAddressable.LoadAssetInternal<TObject>(address, cancellationToken);
            }
        }

        [MenuItem("Addressable/Default")]
        public static void AddressableDefault() => EditorPrefs.SetInt(PlayerPrefsKey, Default);

        [MenuItem("Addressable/Fast")]
        public static void AddressableFast() => EditorPrefs.SetInt(PlayerPrefsKey, Fast);

        [MenuItem("Addressable/Slow")]
        public static void AddressableSlow() => EditorPrefs.SetInt(PlayerPrefsKey, Slow);

        [MenuItem("Addressable/Default", true)]
        public static bool AddressableDefaultValidate()
        {
            var v = EditorPrefs.GetInt(PlayerPrefsKey, 0);
            Menu.SetChecked("Addressable/Default", v == Default);
            Menu.SetChecked("Addressable/Fast", v == Fast);
            Menu.SetChecked("Addressable/Slow", v == Slow);
            return true;
        }
    }
}
