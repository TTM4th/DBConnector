using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnector.Entity
{
    public class MoneyUsedData
    {
        /// <summary>
        /// 空のデータを作成する際のコンストラクタ
        /// </summary>
        public MoneyUsedData() { }

        /// <summary>
        /// DataRowから作成する際のコンストラクタ
        /// </summary>
        /// <param name="row"></param>
        public MoneyUsedData(System.Data.DataRow row)
        {
            ID = Convert.ToInt32(row[nameof(ID)]);
            var rawdate = (DateTime)row[nameof(Date)];
            Date = rawdate.ToShortDateString();
            Price = (decimal)row[nameof(Price)];
            Classification = (string)row[nameof(Classification)];
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 日付
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 価格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 区分
        /// </summary>
        public string Classification { get; set; }
    }
}
