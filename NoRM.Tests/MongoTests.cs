using System.Net;
using Xunit;

namespace NoRM.Tests
{
    public class MongoTests
    {
        [Fact]
        public void ConnectsToDefaultServerByDefault()
        {
            using (var mongo = new Mongo())
            {
                var endpoint = (IPEndPoint)mongo.ServerConnection().Client.Client.RemoteEndPoint;
                Assert.Equal("127.0.0.1", endpoint.Address.ToString());
                Assert.Equal(27017, endpoint.Port);
            }
        }

        [Fact]
        public void CreatesConnectionWithOverrideOptions()
        {
            using (var mongo = Mongo.ParseConnection("mongodb://localhost/NoRMTests?strict=true", "strict=false"))
            {
                Assert.Equal(false, mongo.ServerConnection().StrictMode);
            }
        }

        [Fact]
        public void GetsTheLastError()
        {
            using (var mongo = Mongo.ParseConnection("mongodb://localhost/NoRMTests?strict=false&pooling=false"))
            {
                mongo.GetCollection<FakeObject>().Insert(new FakeObject { Id = null });
                mongo.GetCollection<FakeObject>().Insert(new FakeObject { Id = null });
                var error = mongo.LastError();
                Assert.Equal(1d, error.Ok);
                Assert.Equal("E11000 duplicate key errorindex: NoRMTests.FakeObject.$_id_  dup key: { : null }", error.Err);

            }
        }
    }
}