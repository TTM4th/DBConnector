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

            //まずは指定した年月の月初残高の存在有無を確認する
            SQLiteCommand command = new SQLiteCommand(Connection);
            command.CommandText = BuildPickUpMonthlyFundQuery(year,month,false);
            if (Convert.ToInt32(command.ExecuteScalar()) == 0)
            {
                InsertFromPreviousMonth(new DateTime(year, month,1),Connection);//初日を想定しているので日付には1日を入れている
            }
            
            //月初日の残額を取得する
            command.CommandText = BuildPickUpMonthlyFundQuery(year, month,true);
            gotBalance = Convert.ToDecimal(command.ExecuteScalar());
            
            Connection.Close();
            return gotBalance;
        }

        /// <summary>
        /// 引数のDateTime構造体さかのぼって、前月の利用金額と前月の月初残額から引数の月初金額を計算して挿入する
        /// </summary>
        /// <param name="pickedDate">挿入したい年月のDateTime構造体</param>
        /// <param name="connection">接続に利用しているconnectionオブジェクト</param>
        private void InsertFromPreviousMonth(DateTime pickedDate,SQLiteConnection connection) {
            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = BuildPickUpMonthlyFundQuery(pickedDate.AddMonths(-1).Year, pickedDate.AddMonths(-1).Month, true);
            //前月の月初めの金額情報
            decimal prevBalance = DBNull.Value.Equals(command.ExecuteScalar()) ? 0 : Convert.ToDecimal(command.ExecuteScalar());
            decimal newBalance = prevBalance - MoneyUsedDataAccessor.GetMonthlyPrice(pickedDate.AddMonths(-1).Year, pickedDate.AddMonths(-1).Month);
            this.InsertMonthlyFundRecord(pickedDate.Year,pickedDate.Month,newBalance,connection);
        }

        /// <summary>
        /// 月初日残額情報を引き出すクエリ文を作成する。
        /// </summary>
        /// <param name="year">引き出したい年（西暦）</param>
        /// <param name="month">引き出したい月</param>
        /// <param name="isSingle">引き出したいレコードは単一の場合はtrue,それ以外はfalse</param>
        private string BuildPickUpMonthlyFundQuery(int year, int month, bool isSingle)
        {
            string returnQuery = "SELECT [Price] " +
                                 "FROM [MonthlyFund] " +
                                 $"WHERE [Year] = {year} " +
                                 $"AND[Month] = {month} " +
                                 $"ORDER BY [{TableName}].ID DESC";
            if (isSingle) { return returnQuery + " LIMIT 1"; }
            else { return returnQuery; }
        }

        /// <summary>
        /// MonthlyFundに引数で指定した年月の初日の残額情報を挿入する
        /// </summary>
        /// <param name="year">指定する年月（西暦四ケタ）</param>
        /// <param name="month">指定する月</param>
        /// <param name="firstBalance">第1、第2引数で指定した年月に挿入する金額情報</param>
        /// <param name="connection">接続に利用しているconnectionオブジェクト</param>
        private void InsertMonthlyFundRecord(int year, int month, decimal firstBalance, SQLiteConnection connection)
        {
            SQLiteCommand command = new SQLiteCommand(connection)
            {
                CommandText = $"INSERT INTO [{TableName}]([Year],[Month],[Price]) VALUES ({year},{month},{firstBalance})"
            };
            command.ExecuteNonQuery();
        }



        ///// <summary>
        ///// MonthlyFundに入っているレコードから一番最新の年と月を取得する
        ///// </summary>
        ///// <param name="connection">接続に利用しているconnectionオブジェクト</param>
        ///// <returns>整数型の西暦四ケタ,月を取得する。初期状態の場合は両方とも０を返す</returns>
        //private (int year,int month) PickUpMostRecentYearAndMonth(SQLiteConnection connection)
        //{
        //    SQLiteCommand command = new SQLiteCommand(connection)
        //    {
        //        CommandText = "SELECT MonthlyFund.Year,MonthlyFund.Month " +
        //        "FROM MonthlyFund ORDER BY MonthlyFund.Year DESC,MonthlyFund.Month DESC LIMIT 1"
        //    };

        //    (int year, int month) returnTuple; 

        //    SQLiteDataReader sdr = command.ExecuteReader();

        //    if (sdr.HasRows) {
        //        sdr.Read();
        //        returnTuple = ((int)sdr["Year"], (int)sdr["Month"]);
        //    }
        //    else {
        //        //初期状態で1行も存在していない場合
        //        returnTuple=(0, 0);
        //    }

        //    sdr.Close();

        //    return returnTuple;
        //}


    }
}
