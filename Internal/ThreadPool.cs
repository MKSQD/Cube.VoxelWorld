using System;
using System.Threading;
using System.Collections.Generic;

namespace Cube.Voxelworld {
    public sealed class ThreadPool : IDisposable {
        struct Task {
            public Action action;
            public int priority;
        }

        public ThreadPool() {
            int size = Math.Max(Environment.ProcessorCount - 1, 1);

            _workers = new LinkedList<Thread>();
            for (var i = 0; i < size; ++i) {
                var worker = new Thread(Worker) {
                    Name = string.Concat("Worker ", i)
                };
                worker.Start();
                _workers.AddLast(worker);
            }
        }

        public void Dispose() {
            var waitForThreads = false;
            lock (_tasks) {
                if (!_disposed) {
                    GC.SuppressFinalize(this);

                    _disallowAdd = true;
                    while (_tasks.Count > 0) {
                        Monitor.Wait(_tasks);
                    }

                    _disposed = true;
                    Monitor.PulseAll(_tasks);
                    waitForThreads = true;
                }
            }
            if (waitForThreads) {
                foreach (var worker in _workers) {
                    worker.Join();
                }
            }
        }

        public void QueueTask(Action action, int priority) {
            lock (this._tasks) {
                if (_disallowAdd)
                    throw new InvalidOperationException("This Pool instance is in the process of being disposed, can't add anymore");

                if (_disposed)
                    throw new ObjectDisposedException("This Pool instance has already been disposed");

                Task task;
                task.action = action;
                task.priority = priority;

                var node = _tasks.First;
                while (node != null) {
                    if (node.Value.priority > priority)
                        break;

                    node = node.Next;
                }

                if (node != null) {
                    _tasks.AddBefore(node, task);
                } else {
                    _tasks.AddLast(task);
                }

                Monitor.PulseAll(_tasks);
            }
        }

        void Worker() {
            Action task = null;
            while (true) {
                lock (_tasks) {
                    while (true) {
                        if (_disposed)
                            return;

                        if (null != _workers.First && object.ReferenceEquals(Thread.CurrentThread, _workers.First.Value) && _tasks.Count > 0) {
                            task = _tasks.First.Value.action;
                            _tasks.RemoveFirst();
                            _workers.RemoveFirst();
                            Monitor.PulseAll(_tasks);
                            break;
                        }
                        Monitor.Wait(_tasks);
                    }
                }

                task();
                lock (_tasks) {
                    _workers.AddLast(Thread.CurrentThread);
                }
                task = null;
            }
        }

        readonly LinkedList<Thread> _workers;
        readonly LinkedList<Task> _tasks = new LinkedList<Task>();
        bool _disallowAdd;
        bool _disposed;
    }
}