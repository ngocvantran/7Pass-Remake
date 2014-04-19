using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using NUnit.Framework;
using SevenPass.IO;

namespace SevenPass.Tests
{
    [TestFixture]
    public class PasswordDataTests
    {
        [Test]
        public async Task Should_support_binary_keyfile()
        {
            using (var input = TestFiles.Read("Demo7Pass.bin"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                Assert.AreEqual(
                    "96-4B-38-32-86-FB-A5-15-81-E8-44-76-25-76-67-1B-A0-2F-62-DC-98-34-A0-D4-95-4A-6D-00-23-D8-79-E8",
                    BitConverter.ToString(data.GetMasterKey().ToArray()));
            }
        }

        [Test]
        public async Task Should_support_hex_keyfile()
        {
            using (var input = TestFiles.Read("Demo7Pass.hex"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                Assert.AreEqual(
                    "96-4B-38-32-86-FB-A5-15-81-E8-44-76-25-76-67-1B-A0-2F-62-DC-98-34-A0-D4-95-4A-6D-00-23-D8-79-E8",
                    BitConverter.ToString(data.GetMasterKey().ToArray()));
            }
        }

        [Test]
        public async Task Should_support_password_and_keyfile()
        {
            using (var input = TestFiles.Read("Demo7Pass.key"))
            {
                var data = new PasswordData
                {
                    Password = "demo",
                };
                await data.AddKeyFile(input);

                Assert.AreEqual(
                    "F7-8B-E1-03-50-F0-4C-C8-7D-3F-70-30-EE-DE-60-E3-37-99-C6-66-53-16-97-5B-B4-42-F8-B8-03-BD-6B-AA",
                    BitConverter.ToString(data.GetMasterKey().ToArray()));
            }
        }

        [Test]
        public void Should_support_password_only()
        {
            var data = new PasswordData
            {
                Password = "demo",
            };

            Assert.AreEqual(
                "17-59-39-01-89-83-AB-CA-A4-02-9B-A8-1E-61-AD-BA-A6-3F-3F-AB-E3-8B-ED-84-A9-ED-D0-3B-EA-58-7A-DE",
                BitConverter.ToString(data.GetMasterKey().ToArray()));
        }

        [Test]
        public async Task Should_support_random_keyfile()
        {
            using (var input = TestFiles.Read("Demo7Pass.txt"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                Assert.AreEqual(
                    "F6-25-B4-EF-8C-CB-B1-6F-FC-D9-6B-F0-74-2A-5B-F5-5E-33-9F-82-6E-C1-03-CF-A1-3B-1D-97-D3-86-0C-2E",
                    BitConverter.ToString(data.GetMasterKey().ToArray()));
            }
        }

        [Test]
        public async Task Should_support_xml_keyfile()
        {
            using (var input = TestFiles.Read("Demo7Pass.key"))
            {
                var data = new PasswordData();
                await data.AddKeyFile(input);

                Assert.AreEqual(
                    "2E-6F-69-54-FA-AD-F3-6C-27-B7-2D-93-DA-06-A8-F5-41-CC-54-C4-D4-E8-4D-B8-86-C0-E0-6E-99-AB-8E-48",
                    BitConverter.ToString(data.GetMasterKey().ToArray()));
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