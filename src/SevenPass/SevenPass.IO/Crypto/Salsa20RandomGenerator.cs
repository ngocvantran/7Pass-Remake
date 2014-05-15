using System;
using Windows.Storage.Streams;

namespace SevenPass.IO.Crypto
{
    public class Salsa20RandomGenerator : IRandomGenerator
    {
        public Salsa20RandomGenerator(IBuffer key) {}

        public IBuffer GetRandomBytes(uint size)
        {
            throw new NotImplementedException();
        }
    }
}