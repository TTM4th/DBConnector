using System;
using System.Data.SQLite;

namespace DBConnector.Accessor
{
    /// <summary>
    /// 月別月初残高テーブルのAcceserクラス
    /// </summary>
    public class MonthlyFundAccessor
    {

        #region 各種変数・プロパティ
        /// <summary>
        /// 接続用Connectionオブジェクト
        /// </summary>
        private SQLiteConnection Connection;

        /// <summary>
        /// 月別月初残高テーブル名
        /// </summary>
        private string TableName { get { return "MonthlyFund"; } }

        # endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonthlyFundAccessor() { }

        /// <summary>
        /// 指定した年月の最新残高を取得する
        /// </summary>
        /// <param name="year">取得したい年</param>
        /// <param name="month">取得したい月</param>
        /// <returns></returns>
        public decimal GetMonthFirstBalance(int year,int month)
        {
            decimal gotBalance;
            Connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString };
            Connection.Open();
            //SqlCommand command = new SqlCommand(GetBalanceProcedureName,Connection);
            SQLiteCommand command = new SQLiteCommand(Connection);
            command.CommandText = "SELECT COUNT([Price]) " +
                                    "FROM [MonthlyFund] " +
                                    $"WHERE [Year] = {year} " +
                                    $"AND[Month] = {month} " +
                                    "ORDER BY [MonthlyFund].ID DESC";
            if (Convert.ToInt32(command.ExecuteScalar()) == 0)
            {
                var pickedDate = new DateTime(year, month, 1);//初日を想定しているので１日には月を入れている
                //pickedDate.AddMonths(-1);
                command.CommandText = "SELECT [Price] "+
                                        "FROM [MonthlyFund] "+
                                        $"WHERE[Year] = {pickedDate.Year} "+
                                        $"AND[Month] = {pickedDate.Month} "+
                                        "ORDER BY [MonthlyFund].ID DESC LIMIT 1";
                decimal prevBalance = DBNull.Value.Equals(command.ExecuteScalar()) ? 0 : Convert.ToDecimal(command.ExecuteScalar());
                decimal newBalance = prevBalance - MoneyUsedDataAccessor.GetMonthlyPrice(pickedDate.AddMonths(-1).Year, pickedDate.AddMonths(-1).Month);
                gotBalance = newBalance;
                command.CommandText = $"INSERT INTO [MonthlyFund]([Year],[Month],[Price]) VALUES ({year},{month},{newBalance})";
                command.ExecuteNonQuery();
            }
            else { 
            command.CommandText= "SELECT [Price] " +
                                    "FROM [MonthlyFund] " +
                                    $"WHERE [Year] = {year} " +
                                    $"AND[Month] = {month} " +
                                    "ORDER BY [MonthlyFund].ID DESC LIMIT 1";
            gotBalance = Convert.ToDecimal(command.ExecuteScalar());
            }
            Connection.Close();
            return gotBalance;
        }



    }
}
