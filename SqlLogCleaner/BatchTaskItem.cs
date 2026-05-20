using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlLogCleaner
{
    // 批量任务携带实体，增加了 Grid 实际映射索引属性
    public class BatchTaskItem
    {
        public int GridRowIndex { get; set; }
        public string DbName { get; set; }
        public string LogFileName { get; set; }
        public string TotalSizeMB { get; set; }
        public string ChosenStrategy { get; set; }
    }
}
