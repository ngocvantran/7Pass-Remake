using System;

namespace SevenPass.Services.Picker
{
    public enum FilePickTargets
    {
        /// <summary>
        /// Open file picker triggered by add database.
        /// </summary>
        Databases,

        /// <summary>
        /// Open file picker triggered by add keyfile page.
        /// </summary>
        KeyFile,

        /// <summary>
        /// Save file picker triggered by save attachment page.
        /// </summary>
        Attachments,
    }
}