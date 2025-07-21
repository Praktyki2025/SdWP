namespace SdWP.Test;
using SdWP.Data;

[TestClass]
public class DbConnectionTest
{
    [TestMethod]
    public void TestDatabaseConnection()
    {
        var db = DbContextInitializer.Create();
        Assert.IsTrue(db.Database.CanConnect());
    }
}
