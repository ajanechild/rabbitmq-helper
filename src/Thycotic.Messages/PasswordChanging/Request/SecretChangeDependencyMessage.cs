﻿using Thycotic.AppCore.Federator;
using Thycotic.Messages.Common;

namespace Thycotic.Messages.PasswordChanging.Request
{
    /// <summary>
    /// Secret heartbeat message
    /// </summary>
    public class SecretChangeDependencyMessage : IBasicConsumable
    {
        //TODO: Uncomment
        ///// <summary>
        ///// Gets or sets the password information provider.
        ///// </summary>
        ///// <value>
        ///// The password information provider.
        ///// </value>
        //public IDependencyChangerInfo PasswordInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the Secret Id
        /// </summary>
        public int SecretId { get; set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        /// <value>
        /// The retry count.
        /// </value>
        public int RetryCount { get; set; }
    }
}
