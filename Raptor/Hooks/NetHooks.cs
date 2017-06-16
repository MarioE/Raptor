using System;
using JetBrains.Annotations;
using Raptor.Hooks.Events.Net;
using Terraria;
using Terraria.Localization;

namespace Raptor.Hooks
{
    /// <summary>
    ///     Holds the net hooks.
    /// </summary>
    [PublicAPI]
    public static class NetHooks
    {
        /// <summary>
        ///     Invoked when data is being received.
        /// </summary>
        public static event EventHandler<GetDataEventArgs> GetData;

        /// <summary>
        ///     Invoke when data is received.
        /// </summary>
        public static event EventHandler<GotDataEventArgs> GotData;

        /// <summary>
        ///     Invoked when data is being sent.
        /// </summary>
        public static event EventHandler<SendDataEventArgs> SendData;

        /// <summary>
        ///     Invoke when data is sent.
        /// </summary>
        public static event EventHandler<SentDataEventArgs> SentData;

        internal static bool InvokeGetData(object messageBuffer, int index, int length)
        {
            var args = new GetDataEventArgs((MessageBuffer)messageBuffer, index, length);
            GetData?.Invoke(null, args);
            return args.Handled;
        }

        internal static void InvokeGotData(object messageBuffer, int index, int length)
        {
            GotData?.Invoke(null, new GotDataEventArgs((MessageBuffer)messageBuffer, index, length));
        }

        internal static bool InvokeSendData(int packetType, object text, int number1, float number2, float number3,
            float number4, int number5, int number6, int number7)
        {
            var args = new SendDataEventArgs(packetType, (NetworkText)text, number1, number2, number3, number4, number5,
                number6, number7);
            SendData?.Invoke(null, args);
            return args.Handled;
        }

        internal static void InvokeSentData(int packetType, object text, int number1, float number2, float number3,
            float number4, int number5, int number6, int number7)
        {
            SentData?.Invoke(null,
                new SentDataEventArgs(packetType, (NetworkText)text, number1, number2, number3, number4, number5,
                    number6, number7));
        }
    }
}
