using System;
using System.Data.SQLite;
using System.Text;

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
            
            //月初日の残額を取得する
            command.CommandText = BuildPickUpMonthlyFundQuery(Convert.ToString(year), Convert.ToString(month));
            gotBalance = Convert.ToDecimal(command.ExecuteScalar());
            
            Connection.Close();
            return gotBalance;
        }

        public bool IsExistFirstBalance(int year, int month)
        {
            bool isexist = true;
            using (var connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString }) {
                connection.Open();
                //まずは指定した年月の月初残高の存在有無を確認する
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = BuildPickUpMonthlyFundQuery(Convert.ToString(year), Convert.ToString(month));
                if (command.ExecuteScalar() is null)
                {
                    isexist = false;
                }
                connection.Close();
            }
            return isexist;
        }
        /// <summary>
        /// 前月の利用金額と前月の月初残額から引数の月初金額を計算して挿入する
        /// </summary>
        /// <param name="pickedDate">挿入したい年月のDateTime構造体</param>
        /// <param name="connection">接続に利用しているconnectionオブジェクト</param>
        public void InsertFromPreviousMonth(int year, int month) {
            using (var connection = new SQLiteConnection { ConnectionString = Properties.Settings.Default.ConnectionString })
            {
                connection.Open();
                DateTime pickedDate = new DateTime(year, month, 1);
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = $" SELECT [Price] FROM[{TableName}] ORDER BY[{TableName}].ID DESC LIMIT 1";
                //前月の月初めの金額情報
                decimal prevBalance = DBNull.Value.Equals(command.ExecuteScalar()) ? 0 : Convert.ToDecimal(command.ExecuteScalar());
                //前月の集計
                var lastSumGetter = new Controller.MoneyUsedDataTableManager();
                decimal newBalance = prevBalance - lastSumGetter.LastMonthlySumPrice();
                this.InsertMonthlyFundRecord(pickedDate.Year, pickedDate.Month, newBalance, connection);
                connection.Close();
            }
        }

        /// <summary>
        /// 月初日残額情報を引き出すクエリ文を作成する。
        /// </summary>
        /// <param name="year">引き出したい年（西暦）</param>
        /// <param name="month">引き出したい月</param>
        private string BuildPickUpMonthlyFundQuery(string year, string month)
        {
            var query = new StringBuilder();
            query.AppendLine("SELECT [Price] ");
            query.AppendLine($"FROM [{TableName}] ");
            query.AppendLine($"WHERE [Year] = {year} ");
            query.AppendLine($"AND[Month] = {month} ");
            query.AppendLine($"ORDER BY [{TableName}].ID DESC LIMIT 1");
            return query.ToString();
            //string returnQuery = "SELECT [Price] " +
            //                     $"FROM [{TableName}] " +
            //                     $"WHERE [Year] = {year} " +
            //                     $"AND[Month] = {month} " +
            //                     $"ORDER BY [{TableName}].ID DESC LIMIT 1";
            //return returnQuery; 
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



    }
}
