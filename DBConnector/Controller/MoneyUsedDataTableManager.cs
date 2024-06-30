using DBConnector.Data;
using System.Collections.Generic;
using System.Linq;

namespace DBConnector.Controller
{
    public class MoneyUsedDataTableManager
    {
        private readonly IMoneyUsedData _data;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoneyUsedDataTableManager(IMoneyUsedData data)
        {
            _data = data;
        }

        /// <summary>
        /// 最新年月のテーブルの月別利用総額を取得する
        /// </summary>
        /// <returns>最新年月テーブルの月別利用総額</returns>
        public decimal LastMonthlySumPrice()
        {
            var yeardate = this.MonthlyTableNames().First<string>().Split('-');
            return this.GetMonthlySumPrice(yeardate[0], yeardate[1]);
        }

        public decimal GetMonthlySumPrice(string v1, string v2)
        {
            return _data.LoadMonthlySumPrice(v1, v2);
        }

        public IEnumerable<string> MonthlyTableNames()
        {
            return _data.MonthlyTableNames();
        }

        public void CreateTable(string v)
        {
           _data.CreateTable(v);
        }

        public bool IsExistMonetaryTable(string v)
        {
            return _data.IsExistMonetaryTable(v);
        }
    }
}
