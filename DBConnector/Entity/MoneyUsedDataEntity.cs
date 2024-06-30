using System;

namespace DBConnector.Entity
{
    public class MoneyUsedDataEntity
    {
        /// <summary>
        /// 空のデータを作成する際のコンストラクタ
        /// </summary>
        public MoneyUsedDataEntity() { }

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
