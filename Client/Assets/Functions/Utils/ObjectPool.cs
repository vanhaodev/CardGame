using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    // Dynamic pool - không giới hạn số lượng đối tượng
    public class DynamicObjectPool<T> where T : class
    {
        private readonly ConcurrentQueue<T> _pool = new ConcurrentQueue<T>();
        private readonly Func<T> _createFunc;
        private readonly Action<T> _resetAction;

        public DynamicObjectPool(Func<T> createFunc, Action<T> resetAction = null)
        {
            _createFunc = createFunc ??
                          throw new ArgumentNullException(nameof(createFunc), "Create function cannot be null.");
            _resetAction = resetAction;
        }

        public T Get()
        {
            if (_pool.TryDequeue(out T obj))
            {
                return obj;
            }

            return _createFunc();
        }

        public void Put(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Cannot add null object to the pool.");
            }

            _resetAction?.Invoke(obj);
            _pool.Enqueue(obj);
        }

        public void Clear()
        {
            while (_pool.TryDequeue(out _))
            {
            }
        }

        public bool IsContains(T obj)
        {
            return _pool.Contains(obj);
        }

        public int Count => _pool.Count;
    }

    // Fixed pool - giới hạn số lượng đối tượng, hỗ trợ chờ nếu đầy
    public class FixedObjectPool<T> where T : class
    {
        private readonly ConcurrentQueue<T> _pool;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _resetAction;
        private readonly int _maxObjects;
        private readonly SemaphoreSlim _semaphore;

        public FixedObjectPool(Func<T> createFunc, Action<T> resetAction, int maxObjects)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _resetAction = resetAction ?? throw new ArgumentNullException(nameof(resetAction));
            _maxObjects = maxObjects;
            _pool = new ConcurrentQueue<T>();
            _semaphore = new SemaphoreSlim(maxObjects, maxObjects);
        }

        public async Task<T> GetAsync()
        {
            // Đợi cho đến khi có chỗ trống trong pool
            await _semaphore.WaitAsync();

            // Lấy đối tượng từ pool, nếu không có thì tạo mới
            if (_pool.TryDequeue(out var obj))
            {
                return obj;
            }

            // Nếu pool rỗng, tạo một đối tượng mới
            return _createFunc();
        }

        public void Put(T obj)
        {
            // Reset đối tượng trước khi đưa vào pool
            _resetAction(obj);

            // Đưa đối tượng vào pool
            _pool.Enqueue(obj);

            // Giải phóng semaphore để cho phép thêm đối tượng
            _semaphore.Release();
        }

        public void Clear()
        {
            while (_pool.TryDequeue(out _))
            {
            }
        }

        public bool IsContains(T obj)
        {
            return _pool.Contains(obj);
        }
    }
}