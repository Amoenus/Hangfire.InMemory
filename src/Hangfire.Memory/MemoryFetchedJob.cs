﻿using System.Collections.Concurrent;
using Hangfire.Storage;

namespace Hangfire.Memory
{
    internal class MemoryFetchedJob : IFetchedJob
    {
        private readonly IMemoryDispatcher _dispatcher;

        public MemoryFetchedJob(IMemoryDispatcher dispatcher, string queueName, string jobId)
        {
            _dispatcher = dispatcher;

            QueueName = queueName;
            JobId = jobId;
        }

        public void Dispose()
        {
        }

        public void RemoveFromQueue()
        {
        }

        public void Requeue()
        {
            // TODO: We can do this as a fire-and-forget operation
            _dispatcher.QueryAndWait(state =>
            {
                if (!state._queues.TryGetValue(QueueName, out var queue))
                {
                    // TODO: Refactor this to unify creation of a queue
                    state._queues.Add(QueueName, queue = new BlockingCollection<string>());
                }

                queue.Add(JobId);
                return true;
            });
        }

        public string QueueName { get; }
        public string JobId { get; }
    }
}