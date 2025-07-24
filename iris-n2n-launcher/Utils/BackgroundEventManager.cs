using System.Collections.Concurrent;
using Timer = System.Windows.Forms.Timer;

namespace iris_n2n_launcher.Utils
{
    public sealed class BackgroundEventManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, (Timer timer, Action action)> _events = new();
        private readonly object _lock = new();
        private bool _isRunning = false;

        public BackgroundEventManager() { }

        /// <summary>
        /// 添加一个循环事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="action">要执行的动作</param>
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

                var timer = new System.Windows.Forms.Timer
                {
                    Interval = intervalMilliseconds
                };

                timer.Tick += (sender, e) =>
                {
                    try
                    {
                        if (_isRunning)
                        {
                            action();
                        }
                    }
                    catch
                    {
                        // 忽略所有错误
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
                foreach (var eventData in _events.Values)
                {
                    eventData.timer.Stop();
                    eventData.timer.Dispose();
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
            return _events.Keys.ToArray();
        }

        public void Dispose()
        {
            ClearAllEvents();
        }
    }
}
