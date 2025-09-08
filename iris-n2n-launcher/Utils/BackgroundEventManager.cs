using System.Collections.Concurrent;
using Timer = System.Timers.Timer;

namespace iris_n2n_launcher.Utils
{
    public sealed class BackgroundEventManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, (Timer timer, Action action)> _events = new();
        private readonly object _lock = new();
        private bool _isRunning = false;
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

            lock (_lock)
            {
                if (_events.ContainsKey(eventName))
                {
                    RemoveEvent(eventName);
                }

                var timer = new Timer
                {
                    Interval = intervalMilliseconds,
                    // AutoReset 默认为 true, 这将使计时器在每个间隔后持续触发。
                    AutoReset = true
                };

                // 使用 Elapsed 事件，它在后台线程上触发
                timer.Elapsed += (sender, e) =>
                {
                    try
                    {
                        if (_isRunning)
                        {
                            action();
                        }
                    }
                    catch (Exception ex)
                    {
                        logHelper.Error(ex);
                    }
                };

                _events[eventName] = (timer, action);

                if (_isRunning)
                {
                    timer.Start();
                }
            }
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void RemoveEvent(string eventName)
        {
            lock (_lock)
            {
                if (_events.TryRemove(eventName, out var eventData))
                {
                    eventData.timer.Stop();
                    eventData.timer.Dispose();
                }
            }
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public void ClearAllEvents()
        {
            lock (_lock)
            {
                foreach (var (timer, action) in _events.Values)
                {
                    timer.Stop();
                    timer.Dispose();
                }
                _events.Clear();
            }
        }

        /// <summary>
        /// 开始所有事件
        /// </summary>
        public void StartAllEvents()
        {
            lock (_lock)
            {
                if (!_isRunning)
                {
                    _isRunning = true;
                    foreach (var eventData in _events.Values)
                    {
                        eventData.timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 停止所有事件
        /// </summary>
        public void StopAllEvents()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    _isRunning = false;
                    foreach (var eventData in _events.Values)
                    {
                        eventData.timer.Stop();
                    }
                }
            }
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
            ClearAllEvents();
        }
    }
}