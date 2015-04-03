﻿using Thycotic.Discovery.Sources.Scanners;
using Thycotic.DistributedEngine.EngineToServerCommunication.Areas.Discovery.Response;
using Thycotic.DistributedEngine.Logic.EngineToServer;
using Thycotic.MessageQueue.Client;
using Thycotic.MessageQueue.Client.QueueClient;
using Thycotic.Messages.Areas.Discovery.Request;
using Thycotic.Messages.Common;

namespace Thycotic.DistributedEngine.Logic.Areas.Discovery
{
    /// <summary>
    /// Host Range Consumer
    /// </summary>
    public class HostRangeConsumer : IBasicConsumer<ScanHostRangeMessage>
    {
        private readonly IRequestBus _requestBus;
        private readonly IResponseBus _responseBus;
        private readonly IExchangeNameProvider _exchangeNameProvider;



        /// <summary>
        /// Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Retry Count
        /// </summary>
        public int RetryCount { get; set; }


        //private readonly ILogWriter _log = Log.Get(typeof(ChainMessage));

        /// <summary>
        /// Host Range Consumer
        /// </summary>
        /// <param name="exchangeNameProvider"></param>
        /// <param name="requestBus"></param>
        /// <param name="responseBus"></param>
        public HostRangeConsumer(IExchangeNameProvider exchangeNameProvider, IRequestBus requestBus, IResponseBus responseBus)
        {
            _requestBus = requestBus;
            _responseBus = responseBus;
            _exchangeNameProvider = exchangeNameProvider;
        }

        /// <summary>
        /// Scan Host Range
        /// </summary>
        /// <param name="request"></param>
        public void Consume(ScanHostRangeMessage request)
        {
            // do the scanning

            var scanner = ScannerFactory.GetDiscoveryScanner(request.DiscoveryScannerId);
            var result = scanner.ScanForHostRanges(request.Input);
            var response = new ScanHostRangeResponse
            {
                HostRangeItems = result.HostRangeItems,
                Success = result.Success,
                ErrorCode = result.ErrorCode,
                StatusMessages = { },
                Logs = result.Logs,
                ErrorMessage = result.ErrorMessage
            };

            // call back to server
            _responseBus.Execute(response);
        }
    }
}