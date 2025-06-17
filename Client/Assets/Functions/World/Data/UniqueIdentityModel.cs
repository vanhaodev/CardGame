using System.Threading;
using System.Threading.Tasks;

namespace Functions.World.Data
{
    /// <summary>
    /// UniqueId
    /// </summary>
    [System.Serializable]
    public class UniqueIdentityModel
    {
        public UniqueIdentityUIntModel ItemId;
        public UniqueIdentityUIntModel CardId;
    }

    [System.Serializable]
    public class UniqueIdentityIntModel
    {
        public int Value = 0; // Giữ nguyên public
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<int> GetValue()
        {
            await _semaphore.WaitAsync();
            try
            {
                Value += 1;
                return Value;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
    [System.Serializable]
    public class UniqueIdentityUIntModel
    {
        public uint Value = 0; // Giữ nguyên public
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<uint> GetValue()
        {
            await _semaphore.WaitAsync();
            try
            {
                Value += 1;
                return Value;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

}