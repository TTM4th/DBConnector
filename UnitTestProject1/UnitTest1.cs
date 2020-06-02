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

        [TestMethod]
        public void TableExistCheck()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            Console.WriteLine($"Is Exist Table ? ANS={testObj.IsExistMonetaryTable("20_6")}");
        }

        [TestMethod]
        public void CreateTable()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            testObj.CreateTable("20_6");
        }

    }
}
