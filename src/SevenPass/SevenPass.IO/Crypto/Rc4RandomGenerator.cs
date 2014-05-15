using System;
using Windows.Storage.Streams;

namespace SevenPass.IO.Crypto
{
    public class Rc4RandomGenerator : IRandomGenerator
    {
        public Rc4RandomGenerator(IBuffer key) {}

        public IBuffer GetRandomBytes(uint size)
        {
            throw new NotImplementedException();
        }
    }
}