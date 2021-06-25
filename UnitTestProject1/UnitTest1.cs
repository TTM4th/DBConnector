using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]//月別利用額データの取得テスト
        public void GetMoneyTableData()
        {
            var testObj = new DBConnector.Accessor.MoneyUsedDataAccessor("2020-06");
            testObj.GetMonetarydata();
            foreach(DBConnector.Entity.MoneyUsedData data in testObj.MoneyUsedDataEntitiesFromTable)
            {
                Console.WriteLine($"{data.ID}|{data.Date}|{data.Price}|{data.Classification}");
            }
        }

        [TestMethod]//テーブル存在確認メソッドの動作テスト
        public void TableExistCheck()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            Console.WriteLine($"Is Exist Table ? ANS={testObj.IsExistMonetaryTable("TEST")}");
        }

        [TestMethod]//DB上にある月別利用額テーブル名の取得テスト
        public void GetMonthlyTableNameTest()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            var names = testObj.MonthlyTableNames();
            foreach(string name in names)
            {
                Console.WriteLine($"{name}");
            }
        }

        [TestMethod]//月別利用額テーブルの作成テスト
        public void CreateTable()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            testObj.CreateTable("TEST");
        }

        [TestMethod]//月別利用額テーブルの消去テスト
        public void InitializeTable()
        {
            var testObj = new DBConnector.Controller.MoneyUsedDataTableManager();
            testObj.InitializeTable("TEST");
        }

        [TestMethod]//月別利用額テーブルのデータ挿入更新テスト
        public void UpdateTable()
        {
            string tableName = "TEST";
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

        [TestMethod]//月単位の合計利用金額を取得するテスト
        public void GetMontlySumTEST()
        {
            Console.WriteLine(DBConnector.Accessor.MoneyUsedDataAccessor.GetMonthlyPrice(2021, 04));
        }

        [TestMethod]//月単位の残額を取得するテスト
        public void GetMontlyBalanceTEST()
        {
            var obj = new DBConnector.Accessor.MonthlyFundAccessor();
            Console.WriteLine(obj.GetMonthFirstBalance(2021, 05));
        }


    }
}
