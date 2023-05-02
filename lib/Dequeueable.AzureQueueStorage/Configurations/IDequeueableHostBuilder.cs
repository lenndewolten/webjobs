﻿namespace Dequeueable.AzureQueueStorage.Configurations
{
    /// <summary>
    /// Interface to builds and setup the dequeueable host
    /// </summary>
    public interface IDequeueableHostBuilder
    {
        /// <summary>
        /// This makes sure only a single instance of the function is executed at any given time (even across host instances).
        /// A blob lease is used behind the scenes to implement the lock./>
        /// </summary>
        /// <param name="options">Action to configure the <see cref="SingletonOptions"/></param>
        /// <returns><see cref="IDequeueableHostBuilder"/></returns>
        IDequeueableHostBuilder AsSingleton(Action<SingletonOptions>? options = null);
        /// <summary>
        /// The application will run as a job, from start to finish, and will automatically shutdown when the messages are executed.
        /// </summary>
        /// <param name="options">Action to configure the <see cref="HostOptions"/></param>
        /// <returns><see cref="IDequeueableHostBuilder"/></returns>
        IDequeueableHostBuilder RunAsJob(Action<HostOptions>? options = null);
        /// <summary>
        /// The application will run as a listener, the queue will periodically be polled for new message.
        /// </summary>
        /// <param name="options">Action to configure the <see cref="ListenerOptions"/></param>
        /// <returns><see cref="IDequeueableHostBuilder"/></returns>
        IDequeueableHostBuilder RunAsListener(Action<ListenerOptions>? options = null);
    }
}