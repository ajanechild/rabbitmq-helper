﻿using System;
using System.Diagnostics.Contracts;

namespace Thycotic.MessageQueue.Client.QueueClient
{
    /// <summary>
    /// Interface for a Memory Mq connection
    /// </summary>
    [ContractClass(typeof(CommonConnectionContract))]
    public interface ICommonConnection : IDisposable
    {

        /// <summary>
        /// Forces the initialization.
        /// </summary>
        bool ForceInitialize();

        /// <summary>
        /// Opens the channel.
        /// </summary>
        /// <param name="retryAttempts">The retry attempts.</param>
        /// <param name="retryDelayMs">The retry delay ms.</param>
        /// <param name="retryDelayGrowthFactor">The retry delay growth factor.</param>
        /// <returns></returns>
        ICommonModel OpenChannel(int retryAttempts, int retryDelayMs, float retryDelayGrowthFactor);

        /// <summary>
        /// Gets or sets the connection created.
        /// </summary>
        /// <value>
        /// The connection created.
        /// </value>
        EventHandler ConnectionCreated { get; set; }

    }

    /// <summary>
    /// Contract for ICommonConnection
    /// </summary>
    [ContractClassFor(typeof(ICommonConnection))]
    public abstract class CommonConnectionContract : ICommonConnection
    {
        /// <summary>
        /// Forces the initialization.
        /// </summary>
        public bool ForceInitialize()
        {
            return default(bool);
        }

        /// <summary>
        /// Opens the channel.
        /// </summary>
        /// <param name="retryAttempts">The retry attempts.</param>
        /// <param name="retryDelayMs">The retry delay ms.</param>
        /// <param name="retryDelayGrowthFactor">The retry delay growth factor.</param>
        /// <returns></returns>
        public ICommonModel OpenChannel(int retryAttempts, int retryDelayMs, float retryDelayGrowthFactor)
        {
            Contract.Ensures(Contract.Result<ICommonModel>() != null);

            return default(ICommonModel);
        }

        /// <summary>
        /// Gets or sets the connection created.
        /// </summary>
        /// <value>
        /// The connection created.
        /// </value>
        public EventHandler ConnectionCreated { get; set; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}