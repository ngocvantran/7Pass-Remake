using System;
using SevenPass.Services.Databases;
using Xunit;

namespace SevenPass.Tests.Services
{
    public class RegisteredDbsServiceTests
    {
        private readonly RegisteredDbsService _service;

        public RegisteredDbsServiceTests()
        {
            _service = new RegisteredDbsService();
        }

        [Fact]
        public void ListAsync_should_list_registered_databases()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RegisterAsync_should_infer_database_name_from_file_name()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RegisterAsync_should_provide_unique_database_ID()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RetrieveAsync_should_retrieve_registered_databases()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void RetrieveAsync_should_return_null_for_unknown_ID()
        {
            throw new NotImplementedException();
        }
    }
}