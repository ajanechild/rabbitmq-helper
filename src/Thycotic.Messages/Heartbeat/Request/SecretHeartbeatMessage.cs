﻿using Thycotic.Messages.Common;
using Thycotic.SharedTypes.PasswordChangers;

namespace Thycotic.Messages.Heartbeat.Request
{
    /// <summary>
    /// Secret heartbeat message
    /// </summary>
    public class SecretHeartbeatMessage : BasicConsumableBase
    {
        /// <summary>
        /// Gets or sets the password information provider.
        /// </summary>
        /// <value>
        /// The password information provider.
        /// </value>
        public IVerifyCredentialsInfo VerifyCredentialsInfo { get; set; }

        /// <summary>
        /// Gets or sets the Secret Id
        /// </summary>
        public int SecretId { get; set; }

    }
}
