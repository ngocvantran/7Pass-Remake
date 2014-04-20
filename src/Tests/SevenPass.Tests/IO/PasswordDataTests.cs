using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using NUnit.Framework;
using SevenPass.IO;

namespace SevenPass.Tests.IO
{
    [TestFixture]
    public class PasswordDataTests
    {
        [Test]
        public async Task Should_support_binary_keyfile()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.bin"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                var master = data.GetMasterKey();
                Assert.AreEqual(
                    "964b383286fba51581e844762576671ba02f62dc9834a0d4954a6d0023d879e8",
                    CryptographicBuffer.EncodeToHexString(master));
            }
        }

        [Test]
        public async Task Should_support_hex_keyfile()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.hex"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                var master = data.GetMasterKey();
                Assert.AreEqual(
                    "964b383286fba51581e844762576671ba02f62dc9834a0d4954a6d0023d879e8",
                    CryptographicBuffer.EncodeToHexString(master));
            }
        }

        [Test]
        public async Task Should_support_password_and_keyfile()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.key"))
            {
                var data = new PasswordData
                {
                    Password = "demo",
                };
                await data.AddKeyFile(input);

                var master = data.GetMasterKey();
                Assert.AreEqual(
                    "f78be10350f04cc87d3f7030eede60e33799c6665316975bb442f8b803bd6baa",
                    CryptographicBuffer.EncodeToHexString(master));
            }
        }

        [Test]
        public void Should_support_password_only()
        {
            var data = new PasswordData
            {
                Password = "demo",
            };

            var master = data.GetMasterKey();
            Assert.AreEqual(
                "175939018983abcaa4029ba81e61adbaa63f3fabe38bed84a9edd03bea587ade",
                CryptographicBuffer.EncodeToHexString(master));
        }

        [Test]
        public async Task Should_support_random_keyfile()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.txt"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                var master = data.GetMasterKey();
                Assert.AreEqual(
                    "f625b4ef8ccbb16ffcd96bf0742a5bf55e339f826ec103cfa13b1d97d3860c2e",
                    CryptographicBuffer.EncodeToHexString(master));
            }
        }

        [Test]
        public async Task Should_support_xml_keyfile()
        {
            using (var input = TestFiles.Read("IO.Demo7Pass.key"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                var master = data.GetMasterKey();
                Assert.AreEqual(
                    "2e6f6954faadf36c27b72d93da06a8f541cc54c4d4e84db886c0e06e99ab8e48",
                    CryptographicBuffer.EncodeToHexString(master));
            }
        }

        [Test]
        public async Task Should_transform_master_key()
        {
            var data = new PasswordData
            {
                Password = "demo",
            };

            var seed = CryptographicBuffer.DecodeFromHexString(
                "9525F6992BEB739CBAA73AE6E050627FCAFF378D3CD6F6C232D20AA92F6D0927");

            var masterKey = await data.GetMasterKey(seed, 6000);
            Assert.AreEqual(
                "87730050341ff55c46421f2f2a5f4e1e018d0443d19cacc8682f128f1874d0a4",
                CryptographicBuffer.EncodeToHexString(masterKey));
        }
    }
}