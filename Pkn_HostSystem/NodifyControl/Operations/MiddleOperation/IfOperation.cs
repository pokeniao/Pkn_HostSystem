using Microsoft.VisualBasic;
using Pkn_HostSystem.NodifyControl.Nodes;
using Pkn_HostSystem.NodifyControl.OperationModels.Models.Core;
using Pkn_HostSystem.NodifyControl.Operations.Core;
using Pkn_HostSystem.NodifyControl.Views.NodeOperation;
using System.CodeDom;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.MiddleOperation
{
    public class IfOperation(IfOperationNode node) : BaseOperation<IfOperationNode>(node, new IfOperationUserControl())
    {
        protected override async Task OnExecute()
        {
            try
            {
                string modelExpression = node.Model.Expression;
                if (modelExpression == null)
                {
                    throw new Exception("表达式为Null");
                }

                int i;
                modelExpression = modelExpression.Replace(" ", "").Replace("\r", "").Replace("\n", "");
                while ((i = modelExpression.IndexOf("string.Count(")) != -1)
                {
                    string s = modelExpression.Substring(i + 13);
                    int indexOf = s.IndexOf(")");
                    if (indexOf == -1)
                    {
                        throw new Exception("`string.Count(`格式错误,找不到`)`");
                    }

                    string s2 = s.Substring(0, indexOf);
                    string paramValue = replace1(s2);
                    int paramValueLength = paramValue.Length;
                    modelExpression = modelExpression.Replace($"string.Count({s2})", paramValueLength.ToString());
                }

                while ((i = modelExpression.IndexOf("string.Equals(")) != -1)
                {
                    string s = modelExpression.Substring(i + 14);
                    int indexOf = s.IndexOf(")");
                    string s2 = s.Substring(0, indexOf);
                    string[] strings = s2.Split(',');
                    if (strings.Length != 2)
                    {
                        throw new Exception("`string.Equals(,)`格式错误,没传入2个值");
                    }

                    string paramValue1 = replace1(strings[0]);
                    string paramValue2 = replace1(strings[1]);
                    modelExpression =
                        modelExpression.Replace($"string.Equals({s2})", paramValue1.Equals(paramValue2).ToString());
                }

                modelExpression = runBracket(modelExpression);

                if (bool.Parse(modelExpression))
                {
                    node.Output[0].Enabled = true;
                    node.Output[1].Enabled = false;
                }
                else
                {
                    node.Output[0].Enabled = false;
                    node.Output[1].Enabled = true;
                }

                Log.Info(modelExpression, $"{node.NodeName}:{node.Id}");
            }
            catch (Exception e)
            {
                Log.Error(e.Message, $"{node.NodeName}:{node.Id}");
            }
        }

        private string replace1(string message)
        {
            int indexOf = message.IndexOf("[");
            int indexOf2 = message.IndexOf("]");

            //判断是否是嵌入的值
            if (indexOf == -1 && indexOf2 == -1)
            {
                return message;
            }


            string substring = message.Substring(indexOf + 1);
            string s = substring.Substring(0, substring.IndexOf("]"));


            foreach (var operationModel in node.InputParams)
            {
                if (operationModel.Name == s)
                {
                    return GetParamValue(operationModel);
                }
            }

            return null;
        }

        private string runBracket(string message)
        {
            //查询是否有括号
            int lastIndexOf = message.LastIndexOf('(');
            string substring = message.Substring(lastIndexOf + 1);
            int indexOf = substring.IndexOf(')');
            if (lastIndexOf != -1 && indexOf != -1)
            {
                //得到最里层括号内的内容
                string s = substring.Substring(0, indexOf);
                string bracket = run(s);
                //得到结果后
                string replace = message.Replace($"({s})", bracket);

                //接着检查是否有括号,决定是否递归  
                lastIndexOf = replace.LastIndexOf('(');
                substring = replace.Substring(lastIndexOf + 1);
                indexOf = substring.IndexOf(')');
                if (lastIndexOf != -1 && indexOf != -1)
                {
                    return runBracket(replace);
                }
                else if ((indexOf + lastIndexOf) == -2)
                {
                    return run(replace);
                }
                else
                {
                    throw new Exception("括号不完整");
                }
            }
            else if ((indexOf + lastIndexOf) == -2)
            {
                //找到最里层
                return run(message);
            }
            else
            {
                throw new Exception("括号不完整");
            }
        }

        private string run(string message)
        {
            string[] strings = [">=", "==", "<=", ">", "<", "!=", "&&", "||"];
            Dictionary<int, string> dictionary = new();

            for (int i = 0; i < strings.Length; i++)
            {
                int indexOf = message.IndexOf(strings[i]);
                //> 和< 的情况
                if (i == 3 || i == 4)
                {
                    char c = message[indexOf + 1];
                    if (c == '=')
                    {
                        indexOf = -1;
                    }
                }

                if (indexOf != -1)
                {
                    dictionary.Add(indexOf, strings[i]);
                }
            }


            //排序 从左到右
            Dictionary<int, string> dictionary1 =
                dictionary.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            //先执行第一个逻辑
            KeyValuePair<int, string>[] keyValuePairs = dictionary1.ToArray();
            string substring;
            string substring2;
            bool tryParse;
            bool tryParse2;
            string replace;
            double a;
            double b;
            bool e;
            bool f;
            if (keyValuePairs.Length == 0)
            {
                return message;
            }


            switch (keyValuePairs[0].Value)
            {
                case ">=":
                    substring = message.Substring(0, keyValuePairs[0].Key);

                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 2);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2);
                    }

                    //尝试转换成double类型

                    tryParse = double.TryParse(replace1(substring), out a);
                    tryParse2 = double.TryParse(replace1(substring2), out b);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception(">=比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}>={substring2}", (a >= b).ToString());
                    return run(replace);
                case "==":
                    substring = message.Substring(0, keyValuePairs[0].Key);

                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 2);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2);
                    }

                    //尝试转换成double类型
                    tryParse = double.TryParse(replace1(substring), out a);
                    tryParse2 = double.TryParse(replace1(substring2), out b);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception("==比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}=={substring2}", (a == b).ToString());
                    return run(replace);
                case "<=":
                    substring = message.Substring(0, keyValuePairs[0].Key);
                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 2);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2);
                    }

                    //尝试转换成double类型
                    tryParse = double.TryParse(replace1(substring), out a);
                    tryParse2 = double.TryParse(replace1(substring2), out b);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception("<=比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}<={substring2}", (a <= b).ToString());
                    return run(replace);
                case ">":
                    substring = message.Substring(0, keyValuePairs[0].Key);
                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 1,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 1);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 1);
                    }

                    //尝试转换成double类型
                    tryParse = double.TryParse(replace1(substring), out a);
                    tryParse2 = double.TryParse(replace1(substring2), out b);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception(">比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}>{substring2}", (a > b).ToString());
                    return run(replace);

                case "<":
                    substring = message.Substring(0, keyValuePairs[0].Key);
                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 1,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 1);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 1);
                    }

                    //尝试转换成double类型
                    tryParse = double.TryParse(replace1(substring), out a);
                    tryParse2 = double.TryParse(replace1(substring2), out b);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception("<比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}<{substring2}", (a < b).ToString());
                    return run(replace);
                case "!=":
                    substring = message.Substring(0, keyValuePairs[0].Key);
                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 2);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2);
                    }

                    //尝试转换成double类型
                    tryParse = double.TryParse(replace1(substring), out a);
                    tryParse2 = double.TryParse(replace1(substring2), out b);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception("!=比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}!={substring2}", (a != b).ToString());
                    return run(replace);
                case "&&":
                    substring = message.Substring(0, keyValuePairs[0].Key);
                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 2);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2);
                    }

                    //尝试转换成bool类型
                    tryParse = bool.TryParse(replace1(substring), out e);
                    tryParse2 = bool.TryParse(replace1(substring2), out f);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception("&&比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}&&{substring2}", (e && f).ToString());
                    return run(replace);
                case "||":
                    substring = message.Substring(0, keyValuePairs[0].Key);
                    if (keyValuePairs.Length > 1)
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2,
                            keyValuePairs[1].Key - keyValuePairs[0].Key + 2);
                    }
                    else
                    {
                        substring2 = message.Substring(keyValuePairs[0].Key + 2);
                    }

                    //尝试转换成bool类型
                    tryParse = bool.TryParse(replace1(substring), out e);
                    tryParse2 = bool.TryParse(replace1(substring2), out f);
                    if (!(tryParse && tryParse2))
                    {
                        throw new Exception("&&比较的格式错误");
                    }

                    //替换字符串
                    replace = message.Replace($"{substring}||{substring2}", (e || f).ToString());
                    return run(replace);
                case "True":
                    return "True";
                case "False":
                    return "False";
            }

            return null;
        }
    }
}