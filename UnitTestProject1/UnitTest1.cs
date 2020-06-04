using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void InitializeTable()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            testObj.InitializeTable("20_6");
        }

        [TestMethod]
        public void UpdateTable()
        {
            string tableName = "20_6";
            var testObj = new DBConnector.Accessor.MoneyUsedDataAccessor(tableName);
            var testManger = new DBConnector.Controller.MoneyUsedDataTableManager();

            if (testManger.IsExistMonetaryTable(tableName)) { testManger.InitializeTable(tableName); }
            else { testManger.CreateTable(tableName); }

            var uploadObj = new List<DBConnector.Entity.MoneyUsedData>() {};
            uploadObj.Add(new DBConnector.Entity.MoneyUsedData { ID = 1,Date="1999/01/02",Price=-15000,Classification="?" });
            uploadObj.Add(new DBConnector.Entity.MoneyUsedData { ID = 2, Date = "1999/01/02", Price = 108, Classification = "2" });

            testObj.UploadMonetaryData(uploadObj);
            testObj.GetMonetarydata();
            foreach (DBConnector.Entity.MoneyUsedData data in testObj.MoneyUsedDataEntitiesFromTable)
            {
                Console.WriteLine($"{data.ID}|{data.Date}|{data.Price}|{data.Classification}");
            }

        }
    }
}
