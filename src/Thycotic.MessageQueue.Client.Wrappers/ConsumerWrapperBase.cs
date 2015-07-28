﻿using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Thycotic.Logging;
using Thycotic.MessageQueue.Client.QueueClient;
using Thycotic.Messages.Common;

namespace Thycotic.MessageQueue.Client.Wrappers
{
    /// <summary>
    /// Base consumer wrapper
    /// </summary>
    /// <typeparam name="TConsumable">The type of the request.</typeparam>
    /// <typeparam name="TConsumer">The type of the handler.</typeparam>
    public abstract class ConsumerWrapperBase<TConsumable, TConsumer> : IConsumerWrapperBase
        where TConsumable : IConsumable
    {
        /// <summary>
        /// Retrieve the IModel this consumer is associated
        /// with, for use in acknowledging received messages, for
        /// instance.
        /// </summary>
        public ICommonModel CommonModel { get; private set; }

        private readonly ICommonConnection _connection;
        private readonly IExchangeNameProvider _exchangeNameProvider;

        private bool _terminated;

        private readonly ILogWriter _log = Log.Get(typeof(TConsumer));
        private bool _disposed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerWrapperBase{TConsumable, TConsumer}"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="exchangeNameProvider">The exchange name provider.</param>
        protected ConsumerWrapperBase(ICommonConnection connection, IExchangeNameProvider exchangeNameProvider)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(exchangeNameProvider != null);

            _connection = connection;
            _exchangeNameProvider = exchangeNameProvider;
            _connection.ConnectionCreated += (sender, args) => CommonModel = CreateModel();
        }

        /// <summary>
        /// Creates the model.
        /// </summary>
        /// <returns></returns>
        protected void CreateModel()
        {
            try
            {
                var exchangeName = _exchangeNameProvider.GetCurrentExchange();

                var routingKey = this.GetRoutingKey(typeof(TConsumable));

                var queueName = this.GetQueueName(exchangeName, typeof(TConsumer), typeof(TConsumable));

                    const int retryAttempts = -1; //forever
                    const int retryDelayGrowthFactor = 1;

                var model = _connection.OpenChannel(retryAttempts, DefaultConfigValues.ReOpenDelay, retryDelayGrowthFactor);

                _log.Debug(string.Format("Channel opened for {0}", queueName));

                    //TODO: Re-enable when Memory Mq honors this -dkk
                    //const int prefetchSize = 0;
                    //const int prefetchCount = 1;
                    //const bool global = false;

                    //model.BasicQos(prefetchSize, prefetchCount, global);

                    model.ModelShutdown += RecoverConnection;

                model.ExchangeDeclare(exchangeName, DefaultConfigValues.ExchangeType);

                model.QueueDeclare(queueName, true, false, false, null);
                model.QueueBind(queueName, exchangeName, routingKey);

                    const bool noAck = false; //since this consumer will send an acknowledgement
                    var consumer = this;

                model.BasicConsume(queueName, noAck, consumer); //we will ack, hence no-ack=false

                    CommonModel = model;
                }
            catch (Exception ex)
            {
                _log.Error(string.Format("Could not create model because {0}", ex.Message), ex);
                throw;
            }
        }

        /// <summary>
        /// Starts the consuming process.
        /// </summary>
        public void StartConsuming()
        {
                try
                {
                //forcing the connection to initialized causes the 
                //ConnectionCreated to fire and as a results the model will be recreated
                _connection.ForceInitialize();
                }
                catch (Exception ex)
                {
                    //if there is an issue opening the channel, clean up and rethrow
                _log.Error(string.Format("Failed to connect because {0}", ex.Message));

                _log.Info("Sleeping before reconnecting");

                    Task.Delay(DefaultConfigValues.ReOpenDelay).ContinueWith(task => StartConsuming());
                }
            }

        private void RecoverConnection(object model, ModelShutdownEventArgs reason)
        {
                if (_terminated)
                {
                    return;
                }

                _log.Warn(string.Format("Channel closed because {0}", reason.ReplyText));

                Task.Delay(DefaultConfigValues.ReOpenDelay).ContinueWith(task =>
                {
                _log.Debug("Reopening channel...");
                    StartConsuming();
                });
            }

        /// <summary>
        /// Starts the handle task.
        /// </summary>
        /// <param name="consumerTag">The consumer tag.</param>
        /// <param name="deliveryTag">The delivery tag.</param>
        /// <param name="redelivered">if set to <c>true</c> [redelivered].</param>
        /// <param name="exchange">The exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        protected abstract Task StartHandleTask(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            ICommonModelProperties properties, byte[] body);

        /// <summary>
        /// Called each time a message arrives for this consumer.
        /// </summary>
        /// <param name="consumerTag"></param>
        /// <param name="deliveryTag"></param>
        /// <param name="redelivered"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        /// <remarks>
        /// Be aware that acknowledgement may be required. See IModel.BasicAck.
        /// </remarks>
        public void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            ICommonModelProperties properties, byte[] body)
        {
            StartHandleTask(consumerTag, deliveryTag, redelivered, exchange, routingKey, properties, body);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _terminated = true;

            if (CommonModel == null || !CommonModel.IsOpen) return;

            _log.Debug("Closing channel...");
            CommonModel.Dispose();
            _log.Debug("Channel closed");

            _log.Debug("Closing connection...");
            _connection.Dispose();
            _log.Debug("Connection closed");

            _disposed = true;
        }
    }
}
