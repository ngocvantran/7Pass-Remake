using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SevenPass.IO
{
    public class PasswordData
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Adds the specified key file.
        /// </summary>
        /// <param name="input">The keyfile stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="input"/> parameter cannot be <c>null</c>.
        /// </exception>
        public async Task AddKeyFile(IRandomAccessStream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the raw master key.
        /// </summary>
        /// <returns>The raw master key data.</returns>
        public IBuffer GetMasterKey()
        {
            throw new NotImplementedException();
        }
    }
}