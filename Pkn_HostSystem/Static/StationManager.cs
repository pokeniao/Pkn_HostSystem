using DynamicData;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Service.LoadMes.Decorator;
using Pkn_HostSystem.Service.Stations;
using Station1 = Pkn_HostSystem.Service.Stations.Station1;

namespace Pkn_HostSystem.Static
{
    public static class StationManager
    {

        public static void InitStation()
        {
            GlobalManager.StationDictionary.AddOrUpdate(new EachStation<Station1>() { Header = "VOC检测",CreateDecoratorFunc = (loadMesService) => new Station1LoadMesServiceDecorator(loadMesService)});
        }

        /// <summary>
        /// 记录工位日志
        /// </summary>
        /// <param name="logMethod"></param>
        /// <param name="station"></param>
        /// <param name="message"></param>
        public static void StationLog(StationLogEnum logMethod, InfoAndErrorEnum logInfoAndErrorEnum, string station,
            string message , bool baseNeed = true)
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
                        value.UserLog.InfoToRichTextBox(message,false);

                        value.DevLog.InfoToRichTextBox(message, baseNeed);
                    }
                    else
                    {
                        value.UserLog.ErrorToRichTextBox(message,false);
                        value.ErrorLog.ErrorToRichTextBox(message,false);
                        value.DevLog.ErrorToRichTextBox(message, baseNeed);
                    }

                    break;
                case StationLogEnum.DevLog:
                    if (logInfoAndErrorEnum == InfoAndErrorEnum.Info)
                    {
                        value.DevLog.InfoToRichTextBox(message, baseNeed);
                    }
                    else
                    {
                        value.ErrorLog.ErrorToRichTextBox(message, false);
                        value.DevLog.ErrorToRichTextBox(message, baseNeed);
                    }

                    break;
                case StationLogEnum.ErrorLog:
                    if (logInfoAndErrorEnum == InfoAndErrorEnum.Info)
                    {
                        value.ErrorLog.InfoToRichTextBox(message, baseNeed);
                    }
                    else
                    {
                        value.ErrorLog.ErrorToRichTextBox(message, false);
                        value.DevLog.ErrorToRichTextBox(message, baseNeed);
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
            TraceContext.UpdateParam("EachStation", value);
        }
    }
}