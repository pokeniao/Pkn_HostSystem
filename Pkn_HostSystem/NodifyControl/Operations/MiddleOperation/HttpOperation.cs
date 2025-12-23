using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views;
using RestSharp;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;


namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class HttpOperation(HttpOperationNode node)
        : BaseOperation<HttpOperationNode>(node, new HttpOperationUserControl())
    {
        protected override async Task OnExecute(CancellationTokenSource cts)
        {

            try
            {

                var client = new RestClient(node.Model.HttpPath);
                RestRequest request = null;

                switch (node.Model.HttpMethod)
                {
                    case "GET":
                        //获取请求体
                        string Api = DynMessage(node.Model.ApiPath);
                        request = new RestRequest(Api, Method.Get);
                        break;
                    case "POST":
                        request = PostPutRequest();
                        break;
                    case "PUT":
                        request = PostPutRequest();
                        break;
                    case "DELETE":
                        request = new RestRequest(node.Model.ApiPath, Method.Delete);
                        break;
                }

                //添加请求头
                foreach (var header in node.Model.HttpHeaders)
                {
                    request?.AddHeader(header.Key, header.Value);
                }

                //发送请求
                RestResponse response = await client.ExecuteAsync(request, cts.Token);
                //判断
                if (response.IsSuccessStatusCode)
                {
                    //判断是否是JSON格式,如果是转成输出
                    string formatJson = JsonTool<Object>.TryFormatJson(response.Content, out bool isJson);
                    Log.Info($"[{TraceContext.Name}]--返回消息--成功--消息体:\r\n{formatJson}", $"{node.NodeName}:{node.Id}");

                    node.OutputParams[0].ParamValue = "True";
                    node.OutputParams[1].ParamValue = formatJson;

                }
                else
                {

                    string? errorMessage = response.ErrorMessage == null ? response.Content : response.ErrorMessage;

                    //判断是否是JSON格式,如果是转成输出item.Response = JsonTool<Object>.TryFormatJson(item.Response, out bool isJson);

                    Log.Error($"[{TraceContext.Name}]--返回消息--失败--消息体:\r\n{errorMessage}", $"{node.NodeName}:{node.Id}");

                    node.OutputParams[0].ParamValue = "False";
                    node.OutputParams[1].ParamValue = errorMessage;

                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, $"{node.NodeName}:{node.Id}");
            }

        }


        private RestRequest PostPutRequest()
        {
            RestRequest request = null;
            switch (node.Model.HttpMethod)
            {
                case "POST":
                    request = new RestRequest(node.Model.ApiPath, Method.Post);
                    break;
                case "PUT":
                    request = new RestRequest(node.Model.ApiPath, Method.Put);
                    break;
            }

            //添加请求体
            string message;
            switch (node.Model.ContentType)
            {
                case "application/json":
                    //调用动态添加
                    message = DynMessage(node.Model.HttpBody);
                    //会自动设置 Content-Type: application/json，并把内容当作 JSON 处理。
                    request.AddStringBody(message, DataFormat.Json);
                    break;
                case "application/xml":
                    //调用动态添加
                    message = DynMessage(node.Model.HttpBody);
                    //表示数据格式是 XML。
                    request.AddStringBody(message, DataFormat.Xml);
                    break;
                case "application/x-www-form-urlencoded":
                    //更具fromBody来添加
                    ObservableCollection<HttpItem> fromBodys = node.Model.FromBodys;
                    foreach (HttpItem httpItem in fromBodys)
                    {
                        request.AddParameter(httpItem.Key, DynMessage(httpItem.Value));
                    }
                    break;
                case "multipart/form-data":
                    //调用动态添加
                    message = DynMessage(node.Model.HttpBody);
                    //用于文件上传等需要分块传输数据的场景。
                    request.AddStringBody(message, DataFormat.None);
                    break;
            }

            return request;
        }


        private string DynMessage(string message)
        {
            if (message == null)
            {
                return "";
            }

            //获取输入
            ObservableCollectionExtended<OperationModel> inputParams = node.InputParams;
            //通过正则表达式匹配对应数量的[]格式的字符 , 为了让顺序属于按[]出现的顺序来处理
            MatchCollection matches = Regex.Matches(message, @"\[.*?\]");
            foreach (Match match in matches)
            {
                //获取到匹配的内容
                foreach (var operationModel in inputParams)
                {
                    var itemKey = operationModel.Name;
                    //检查是否存在
                    var i = match.Value.IndexOf($"[{itemKey}]");

                    if (i == -1)
                    {
                        continue;
                    }
                    message = StaticMessage(message, itemKey, GetParamValue(operationModel));
                }
            }
            return message;
        }


        private string StaticMessage(string request, string itemKey, string itemValue)
        {
            var i = request.IndexOf($"[{itemKey}]");

            string messageBefore = request;
            if (i != -1)
            {
                var keyLen = itemKey.Length;
                var requestA = request.Substring(0, i);
                var requestB = request.Substring(i + keyLen + 2);
                request = requestA + itemValue + requestB;
            }

            //防止堆栈溢出,重复嵌套调用
            if (request == messageBefore)
            {
                Log.Error($"[{TraceContext.Name}]--进行嵌入后,前后一样,避免循环嵌套堆栈溢出,退出嵌套");
                return request;
            }

            return request.IndexOf($"[{itemKey}]") != -1 ? StaticMessage(request, itemKey, itemValue) : request;
        }
    }
}