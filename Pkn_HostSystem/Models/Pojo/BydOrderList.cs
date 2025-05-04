namespace Pkn_HostSystem.Models.Pojo
{
    public class BydOrderList
    {
        /// <summary>
        /// 计划结束时间
        /// </summary>
        public string endTime { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }

        /// <summary>
        /// 产线编号
        /// </summary>
        public string lineCode { get; set; }
        /// <summary>
        /// 产线名称
        /// </summary>
        public string lineName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string materialCode { get; set; }
        public string materialName { get; set; }
        public string materialVersion { get; set; }
        /// <summary>
        /// 工单编码
        /// </summary>
        public string orderCode { get; set; }
        /// <summary>
        /// 工单数量
        /// </summary>
        public string orderQty { get; set; }
       
        public string orderSources { get; set; }
        /// <summary>
        /// 工单状态编码
        /// </summary>
        public string orderStateCode { get; set; }
        /// <summary>
        /// 工单状态
        /// </summary>
        public string orderStatus { get; set; }
        /// <summary>
        /// 工单类型
        /// </summary>
        public string orderType { get; set; }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string productCode { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string productName { get; set; }
        /// <summary>
        /// 产品版本
        /// </summary>
        public string productVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string routeCode { get; set; }
        /// <summary>
        /// 排程编码
        /// </summary>
        public string scheduleCode { get; set; }
        /// <summary>
        /// 排程计划结束时间
        /// </summary>
        public string scheduleEditTime { get; set; }
        /// <summary>
        /// 排程主键
        /// </summary>
        public string scheduleId { get; set; }
        /// <summary>
        /// 排程数量，产线需执行的计划数量
        /// </summary>
        public string scheduleQty { get; set; }
        /// <summary>
        /// 排程状态编码(0-新建,1-下达,2-开
        /// 线,3-生产,4-暂停,5-取消,6-完成,7-
        /// 部分派工)
        /// </summary>
        public string scheduleStateCode { get; set; }
        /// <summary>
        /// 排程状态
        /// </summary>
        public string scheduleStatus { get; set; }
        /// <summary>
        /// 计划开始时间(yyyy-MM-dd
        /// HH:mm:ss）
        /// </summary>
        public string startTime { get; set; }
        //车间编号
        public string workshopCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string workshopName { get; set; }



    }
}