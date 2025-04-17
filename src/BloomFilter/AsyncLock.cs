using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace BloomFilter;

/// <summary>
/// AsyncLock
/// </summary>
internal readonly struct AsyncLock : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Releaser _releaser;
    private readonly Task<Releaser> _releaserTask;

    public int MaxCount { get; }

    public AsyncLock() : this(1)
    {
    }

    public AsyncLock(int maxCount = 1)
    {
        MaxCount = maxCount;
        _semaphore = new SemaphoreSlim(maxCount);
        _releaser = new Releaser(_semaphore);
        _releaserTask = Task.FromResult(_releaser);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Releaser Acquire()
    {
        _semaphore.Wait();
        return _releaser;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<Releaser> AcquireAsync(bool continueOnCapturedContext = false)
    {
        var acquireAsync = _semaphore.WaitAsync();
        return Return(acquireAsync, continueOnCapturedContext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => _semaphore.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task<Releaser> Return(Task acquireAsync, bool continueOnCapturedContext)
    {
        return acquireAsync.Status == TaskStatus.RanToCompletion
            ? _releaserTask
            : WaitForAcquireAsync(acquireAsync, continueOnCapturedContext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<Releaser> WaitForAcquireAsync(Task acquireAsync, bool continueOnCapturedContext)
    {
        await acquireAsync.ConfigureAwait(continueOnCapturedContext);
        return _releaser;
    }

    public readonly struct Releaser(SemaphoreSlim? semaphore) : IDisposable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => semaphore?.Release();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetRemainingCount()
    {
        return MaxCount - _semaphore.CurrentCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCurrentCount()
    {
        return _semaphore.CurrentCount;
    }
}