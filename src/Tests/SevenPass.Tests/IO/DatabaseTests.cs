﻿using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SevenPass.IO;
using SevenPass.IO.Models;

namespace SevenPass.Tests.IO
{
    [TestFixture]
    public class DatabaseTests
    {
        [Test]
        public async Task Should_decrypt_database()
        {
            using (var kdbx = TestFiles.Read("IO.Demo7Pass.kdbx"))
            {
                // Headers
                var result = await FileFormat.Headers(kdbx);
                Assert.AreEqual(FileFormats.Supported, result.Format);

                var headers = result.Headers;
                Assert.NotNull(headers);

                // Master Key
                var password = new PasswordData
                {
                    Password = "demo",
                };
                var masterKey = await password
                    .GetMasterKey(headers);

                // Decrypt
                using (var decrypted = await FileFormat
                    .Decrypt(kdbx, masterKey, headers))
                {
                    // Start bytes
                    await FileFormat.VerifyStartBytes(
                        decrypted, headers);
                    
                    // Parse content
                    var doc = FileFormat.ParseContent(
                        decrypted, headers.UseGZip);

                    Assert.NotNull(doc);

                    var root = doc.Root;
                    Assert.NotNull(root);
                    Assert.AreEqual("KeePassFile", root.Name.LocalName);
                }
            }
        }
    }
}