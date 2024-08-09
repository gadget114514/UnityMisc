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
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace KyubunsSandbox
{
    public static class AddressableWrapper
    {
        private static IAddressableLoader _addressableLoader;

        public static IAddressableLoader AddressableLoader
        {
            get => _addressableLoader ?? (_addressableLoader = new DefaultAddressableLoader());
            set => _addressableLoader = value;
        }

        [MustUseReturnValue]
        public static UniTask<IDisposableAsset<TObject>> Load<TObject>(string address, CancellationToken cancellationToken) where TObject : UnityEngine.Object
        {
            return AddressableLoader.LoadAssetAsync<TObject>(address, cancellationToken);
        }

        public interface IAddressableLoader
        {
            UniTask<IDisposableAsset<TObject>> LoadAssetAsync<TObject>(string address, CancellationToken cancellationToken) where TObject : UnityEngine.Object;
        }

        public class DefaultAddressableLoader : IAddressableLoader
        {
            public async UniTask<IDisposableAsset<TObject>> LoadAssetAsync<TObject>(string address, CancellationToken cancellationToken) where TObject : UnityEngine.Object
            {
                var handle = Addressables.LoadAssetAsync<TObject>(address);
                await handle.ToUniTask(cancellationToken: cancellationToken);
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return new DisposableAsset<TObject>(handle.Result, () => Addressables.Release(handle));
                }
                throw new Exception($"Failed to load {address} {handle.Status}");
            }
        }
    }

    public interface IDisposableAsset<out T> : IDisposable
    {
        T Value { get; }
    }

    public class DisposableAsset<T> : IDisposableAsset<T>
    {
        public T Value { get; }

        private bool _disposed;
        private readonly Action _dispose;

        public DisposableAsset(T value, Action dispose)
        {
            Value = value;
            _dispose = dispose;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _dispose();
        }
    }
}
