using System;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using SevenPass.IO.Crypto;
using Xunit;

namespace SevenPass.Tests.IO.Crypto
{
    public class Rc4RandomGeneratorTests
    {
        private readonly IBuffer _key;

        public Rc4RandomGeneratorTests()
        {
            _key = CryptographicBuffer.DecodeFromBase64String(
                "SfJI2VqUv500zHC3qgkJF5yCTiVmGDdIVdLxcp2uPf8=");
        }

        [Fact]
        public void Should_generate_different_bytes_after_each_request()
        {
            var generator = new Rc4RandomGenerator(_key);
            var first32 = generator.GetRandomBytes(32);
            var second32 = generator.GetRandomBytes(32);

            Assert.False(CryptographicBuffer
                .Compare(first32, second32));
        }

        [Fact]
        public void Should_generate_different_bytes_if_not_same_key()
        {
            var generator = new Rc4RandomGenerator(_key);
            var first32 = generator.GetRandomBytes(32);

            var generator2 = new Rc4RandomGenerator(
                CryptographicBuffer.GenerateRandom(32));
            var second32 = generator2.GetRandomBytes(32);

            Assert.False(CryptographicBuffer
                .Compare(first32, second32));
        }

        [Fact]
        public void Should_generate_random_bytes()
        {
            var generator = new Rc4RandomGenerator(_key);
            var output = generator.GetRandomBytes(32);

            Assert.Equal(
                "aoKkKycKsRrRIVOhRrH/afAz6lx+EaHBm6icFs9v2Kg=",
                CryptographicBuffer.EncodeToBase64String(output));
        }

        [Fact]
        public void Should_generate_same_bytes_if_same_key()
        {
            var generator = new Rc4RandomGenerator(_key);
            var first32 = generator.GetRandomBytes(32);

            var generator2 = new Rc4RandomGenerator(_key);
            var second32 = generator2.GetRandomBytes(32);

            Assert.True(CryptographicBuffer
                .Compare(first32, second32));
        }

        [Fact]
        public void Should_return_empty_array_if_size_is_zero()
        {
            var generator = new Rc4RandomGenerator(_key);

            var output = generator.GetRandomBytes(0);
            Assert.Equal(0U, output.Length);
        }
    }
}