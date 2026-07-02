using System.Collections.Concurrent;
using Timer = System.Timers.Timer;

namespace iris_n2n_launcher.Utils
{
    public sealed class BackgroundEventManager : IDisposable
    {
        private sealed class EventData : IDisposable
        {
            public required Timer Timer { get; init; }
            public required Action Action { get; init; }
            public ManualResetEventSlim Completed { get; } = new(true);
            public int IsExecuting;
            public bool IsDisposed;

            public void Dispose()
            {
                IsDisposed = true;
                Timer.Stop();
                Timer.Dispose();
                Completed.Dispose();
            }

            public void DisposeTimer()
            {
                IsDisposed = true;
                Timer.Stop();
                Timer.Dispose();
            }
        }

        private readonly ConcurrentDictionary<string, EventData> _events = new();
        private readonly object _lock = new();
        private bool _isRunning = false;
        private bool _disposed = false;
        private static readonly LogHelper logHelper = LogHelper.Instance;

        public BackgroundEventManager() { }

        /// <summary>
        /// 添加一个循环事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">要执行的动作。注意：此动作在后台线程上执行，如需操作UI元素，请确保使用Invoke/Dispatcher等方式切换回UI线程。</param>
        /// <param name="intervalMilliseconds">循环间隔(毫秒)</param>
        public void AddEvent(string eventName, Action action, int intervalMilliseconds)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentException("Event name cannot be null or empty", nameof(eventName));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (intervalMilliseconds <= 0)
                throw new ArgumentException("Interval must be greater than 0", nameof(intervalMilliseconds));

            EventData? oldEventData = null;

            lock (_lock)
            {
                ThrowIfDisposed();

                if (_events.TryRemove(eventName, out oldEventData))
                {
                    oldEventData.IsDisposed = true;
                    oldEventData.Timer.Stop();
                }

                var timer = new Timer
                {
                    Interval = intervalMilliseconds,
                    // 回调完成后再启动下一轮，避免上一次执行过慢导致重入。
                    AutoReset = false
                };

                var eventData = new EventData
                {
                    Timer = timer,
                    Action = action
                };

                // 使用 Elapsed 事件，它在后台线程上触发
                timer.Elapsed += (sender, e) => ExecuteEvent(eventData);

                _events[eventName] = eventData;

                if (_isRunning)
                {
                    timer.Start();
                }
            }

            DisposeEvent(oldEventData);
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void RemoveEvent(string eventName)
        {
            EventData? eventData = null;

            lock (_lock)
            {
                if (_events.TryRemove(eventName, out eventData))
                {
                    eventData.IsDisposed = true;
                    eventData.Timer.Stop();
                }
            }

            DisposeEvent(eventData);
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public void ClearAllEvents()
        {
            List<EventData> events;

            lock (_lock)
            {
                events = [.. _events.Values];
                _events.Clear();

                foreach (var eventData in events)
                {
                    eventData.IsDisposed = true;
                    eventData.Timer.Stop();
                }
            }

            foreach (var eventData in events)
            {
                DisposeEvent(eventData);
            }
        }

        /// <summary>
        /// 开始所有事件
        /// </summary>
        public void StartAllEvents()
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                if (!_isRunning)
                {
                    _isRunning = true;
                    foreach (var eventData in _events.Values)
                    {
                        if (!eventData.IsDisposed)
                        {
                            eventData.Timer.Start();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 停止所有事件
        /// </summary>
        public void StopAllEvents()
        {
            List<EventData> events;

            lock (_lock)
            {
                if (!_isRunning)
                {
                    return;
                }

                _isRunning = false;
                events = [.. _events.Values];

                foreach (var eventData in events)
                {
                    eventData.Timer.Stop();
                }
            }

            WaitForEvents(events);
        }

        /// <summary>
        /// 获取所有事件名称
        /// </summary>
        public string[] GetEventNames()
        {
            return [.. _events.Keys];
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            StopAllEvents();
            ClearAllEvents();
        }

        private void ExecuteEvent(EventData eventData)
        {
            if (!_isRunning || eventData.IsDisposed)
            {
                return;
            }

            if (Interlocked.Exchange(ref eventData.IsExecuting, 1) == 1)
            {
                return;
            }

            eventData.Completed.Reset();

            try
            {
                if (_isRunning && !eventData.IsDisposed)
                {
                    eventData.Action();
                }
            }
            catch (Exception ex)
            {
                logHelper.Error(ex);
            }
            finally
            {
                Interlocked.Exchange(ref eventData.IsExecuting, 0);
                eventData.Completed.Set();

                if (_isRunning && !eventData.IsDisposed)
                {
                    try
                    {
                        eventData.Timer.Start();
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
            }
        }

        private static void DisposeEvent(EventData? eventData)
        {
            if (eventData == null)
            {
                return;
            }

            if (eventData.Completed.Wait(TimeSpan.FromSeconds(2)))
            {
                eventData.Dispose();
            }
            else
            {
                eventData.DisposeTimer();
            }
        }

        private static void WaitForEvents(IEnumerable<EventData> events)
        {
            foreach (var eventData in events)
            {
                eventData.Completed.Wait(TimeSpan.FromSeconds(2));
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(BackgroundEventManager));
            }
        }
    }
}
