using DynamicData;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.Stations;
using Station1 = Pkn_HostSystem.Service.Stations.Station1;

namespace Pkn_HostSystem.Static
{
    public static class StationManager
    {


        public static void InitStation()
        {
            GlobalManager.StationDictionary.AddOrUpdate(new EachStation<Station1>() { Header = "扫码过站", });

            GlobalManager.StationDictionary.AddOrUpdate(new EachStation<Station2>() { Header = "生产信息上传", });
        }
        /// <summary>
        /// 记录工位日志
        /// </summary>
        /// <param name="logMethod"></param>
        /// <param name="station"></param>
        /// <param name="message"></param>
        public static void StationLog(StationLogEnum logMethod, InfoAndErrorEnum logInfoAndErrorEnum, string station,
            string message)
        {
            if (station == null)
            {
                return;
            }

            bool hasValue = GlobalManager.StationDictionary.Lookup(station).HasValue;

            if (!hasValue)
            {
                return;
            }

            dynamic value = GlobalManager.StationDictionary.Lookup(station).Value;

            switch (logMethod)
            {
                case StationLogEnum.UserLog:
                    if (logInfoAndErrorEnum == InfoAndErrorEnum.Info)
                    {
                        value.UserLog.InfoToRichTextBox(message);
                        value.DevLog.InfoToRichTextBox(message);
                    }
                    else
                    {
                        value.UserLog.ErrorToRichTextBox(message);
                        value.ErrorLog.ErrorToRichTextBox(message);
                        value.DevLog.ErrorToRichTextBox(message);
                    }

                    break;
                case StationLogEnum.DevLog:
                    if (logInfoAndErrorEnum == InfoAndErrorEnum.Info)
                    {
                        value.DevLog.InfoToRichTextBox(message);
                    }
                    else
                    {
                        value.ErrorLog.ErrorToRichTextBox(message);
                        value.DevLog.ErrorToRichTextBox(message);
                    }

                    break;
                case StationLogEnum.ErrorLog:
                    if (logInfoAndErrorEnum == InfoAndErrorEnum.Info)
                    {
                        value.ErrorLog.InfoToRichTextBox(message);
                    }
                    else
                    {
                        value.ErrorLog.ErrorToRichTextBox(message);
                        value.DevLog.ErrorToRichTextBox(message);
                    }
                    break;
            }
        }


        public static void TraceContextStart(string station)
        {
            if (station == null)
            {
                return;
            }
            bool hasValue = GlobalManager.StationDictionary.Lookup(station).HasValue;

            if (!hasValue)
            {
                return;
            }
            dynamic value = GlobalManager.StationDictionary.Lookup(station).Value;
            TraceContext.Param = value;
        }
    }
}