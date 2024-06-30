using DBConnector.Entity;
using System.Collections.Generic;

namespace DBConnector.Model
{
    public class MenuFormModel
    {
        /// <summary>
        /// 選択用の月別金額管理テーブル名一覧
        /// </summary>
        public IEnumerable<string> MonthlyTableNames { get; set; }

        /// <summary>
        /// 現在月の手持ち金額
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// SumByClassビューへ渡すための区分別集計値情報
        /// </summary>
        public IDictionary<string, decimal> MoneyUsedData { get; set; }
    }
}
