using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetMoneyTableData()
        {
            var testObj = new DBConnector.Accessor.MoneyUsedDataAccessor("19_1");
            testObj.GetMonetarydata();
            foreach(DBConnector.Entity.MoneyUsedData data in testObj.MoneyUsedDataEntitiesFromTable)
            {
                Console.WriteLine($"{data.ID}|{data.Date}|{data.Price}|{data.Classification}");
            }
        }
    }
}
