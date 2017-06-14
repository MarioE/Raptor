using System.ComponentModel;
using System.IO;
using JetBrains.Annotations;
using Terraria;

namespace Raptor.Hooks.Events.Net
{
    /// <summary>
    ///     Provides data for the <see cref="NetHooks.GetData" /> event.
    /// </summary>
    [PublicAPI]
    public class GetDataEventArgs : HandledEventArgs
    {
        private readonly int _index;
        private readonly int _length;
        private readonly MessageBuffer _messageBuffer;

        internal GetDataEventArgs(MessageBuffer messageBuffer, int index, int length)
        {
            _messageBuffer = messageBuffer;
            _index = index;
            _length = length;
        }

        /// <summary>
        ///     Gets the packet type.
        /// </summary>
        public PacketTypes PacketType => (PacketTypes)_messageBuffer.readBuffer[_index];

        /// <summary>
        ///     Gets a binary reader for the data.
        /// </summary>
        /// <returns>The binary reader.</returns>
        [NotNull]
        public BinaryReader GetReader() => new BinaryReader(
            new MemoryStream(_messageBuffer.readBuffer, _index + 1, _length - 1));
    }
}
