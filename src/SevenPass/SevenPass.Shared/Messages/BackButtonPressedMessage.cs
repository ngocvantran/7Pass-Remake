using System;

namespace SevenPass.Messages
{
    public sealed class BackButtonPressedMessage
    {
        /// <summary>
        /// Gets or sets value indicating whether the message has been handled.
        /// </summary>
        public bool IsHandled { get; set; }
    }
}