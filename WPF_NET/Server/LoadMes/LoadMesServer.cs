using System.Collections.ObjectModel;
using RestSharp;
using WPF_NET.Base;
using WPF_NET.Models;

namespace WPF_NET.Server.LoadMes;

public class LoadMesServer
{
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;

    private LogBase<LoadMesServer> log;

    public LoadMesServer(ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList)
    {
        this.mesPojoList = mesPojoList;
        log = new LogBase<LoadMesServer>();
    }

    /// <summary>
    /// 查找是否存在
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public LoadMesAddAndUpdateWindowModel SelectByName(string Name)
    {
        foreach (LoadMesAddAndUpdateWindowModel item in mesPojoList)
        {
            if (Name == item.Name)
            {
                return item;
            }
        }

        return null;
    }
    /// <summary>
    /// 包装Request请求
    /// </summary>
    /// <param name="name"></param>
    public string PackRequest(string name)
    {
        LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel = SelectByName(name);
        if (loadMesAddAndUpdateWindowModel == null)
        {
            return null;
        }

        List<ConditionItem> conditionItems = loadMesAddAndUpdateWindowModel.Condition.ToList();

        string request = loadMesAddAndUpdateWindowModel.Request;

        foreach (ConditionItem item in conditionItems)
        {
            string itemKey = item.Key;
            string itemValue = item.Value;

            int i = request.IndexOf($"[{itemKey}]");
            if (i != -1)
            {
                int keyLen = itemKey.Length;
                string requestA = request.Substring(0, i);
                string requestB = request.Substring(i + keyLen+2);
                request = requestA + itemValue + requestB;
            }
        }
        
        log.Info($"request 值为: {request}");
        return request;
    }
    /// <summary>
    /// 手动发送HTTP消息
    /// </summary>
    public bool runJog()
    {
        foreach (LoadMesAddAndUpdateWindowModel item in mesPojoList)
        {
            string request = PackRequest(item.Name);
            if (request != null)
            {
                RestClient client = new RestClient(item.HttpPath);

            }
        }
        return true;
    }

}