using DynamicData;
using Pkn_HostSystem.Base.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

public class CsvHelper
{
    private readonly List<List<string>> _rows = new();
    private readonly string _filePath;
    private readonly Encoding _encoding;
    private LogBase<CsvHelper> Log =new LogBase<CsvHelper>();
    public CsvHelper(string filePath, Encoding encoding = null)
    {
        _filePath = filePath;
        _encoding = encoding ?? Encoding.UTF8;
    }

    // 加载 CSV 文件内容（可选）
    public void Load()
    {
        _rows.Clear();
        if (!File.Exists(_filePath)) return;

        foreach (var line in File.ReadAllLines(_filePath, _encoding))
        {
            var fields = ParseCsvLine(line);
            _rows.Add(fields);
        }
    }

    // 添加一行
    public void AddRow(params string[] fields)
    {
        _rows.Add(fields.ToList());
    }

    // 修改单元格
    public void UpdateCell(int rowIndex, int columnIndex, string newValue)
    {
        if (rowIndex >= 0 && rowIndex < _rows.Count &&
            columnIndex >= 0 && columnIndex < _rows[rowIndex].Count)
        {
            _rows[rowIndex][columnIndex] = newValue;
        }
    }

    // 导出 List<T> 到 CSV，支持是否带表头
    public void ExportFromObjects<T>(List<T> objects, bool includeHeader = true)
    {
        _rows.Clear();
        if (objects == null || objects.Count == 0) return;

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (includeHeader)
        {
            _rows.Add(props.Select(p => p.Name).ToList());
        }

        foreach (var obj in objects)
        {
            var row = props.Select(p => p.GetValue(obj)?.ToString() ?? "").ToList();
            _rows.Add(row);
        }
    }

    // 获取全部数据
    public List<List<string>> GetAllRows() => _rows;

    // 保存到文件
    public void Save()
    {
        using var writer = new StreamWriter(_filePath, false, _encoding);
        foreach (var row in _rows)
        {
            writer.WriteLine(string.Join(",", row.Select(EscapeCsv)));
        }
    }

    // 添加多行对象到已有内容末尾（按属性顺序追加）
    public void AddRowsFromObjects<T>(List<T> objects)
    {
        if (objects == null || objects.Count == 0) return;

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // 如果当前为空，自动加表头
        if (_rows.Count == 0)
        {
            _rows.Add(props.Select(p => p.Name).ToList());
        }

        foreach (var obj in objects)
        {
            var row = props.Select(p => p.GetValue(obj)?.ToString() ?? "").ToList();
            _rows.Add(row);
        }
    }

    //Json转
    public void AddRowFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;
        Dictionary<string, object>? dict;
        try
        {
             dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch (Exception e)
        {
            Log.Error($"{nameof(CsvHelper)}--本地保存失败,Json格式不正确:{e.Message}");
            return;
        }
        if (dict == null || dict.Count == 0) return;

        // 如果表头为空（CSV 没有 Load 或新建），就加表头
        if (_rows.Count == 0)
        {
            _rows.Add(dict.Keys.ToList());
        }

        // 若已有表头，按表头顺序写入值，找不到的列填空
        var header = _rows[0];
        var row = header.Select(h => dict.TryGetValue(h, out var value) ? value?.ToString() ?? "" : "").ToList();

        _rows.Add(row);
    }

    // 从 CSV 文件加载为对象列表
    public List<T> ImportToObjects<T>() where T : new()
    {
        var result = new List<T>();

        Load(); // 先加载文件内容
        if (_rows.Count < 1) return result;

        var headers = _rows[0];
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var propMap = new Dictionary<int, PropertyInfo>();
        for (int i = 0; i < headers.Count; i++)
        {
            var match = props.FirstOrDefault(p => p.Name == headers[i]);
            if (match != null)
                propMap[i] = match;
        }
        for (int rowIndex = 1; rowIndex < _rows.Count; rowIndex++)
        {
            var row = _rows[rowIndex];
            var obj = new T();

            foreach (var kvp in propMap)
            {
                int colIndex = kvp.Key;
                var prop = kvp.Value;

                if (colIndex >= row.Count) continue;

                string value = row[colIndex];

                try
                {
                    object convertedValue = ConvertToType(value, prop.PropertyType);
                    prop.SetValue(obj, convertedValue);
                }
                catch
                {
                    // 忽略转换失败的字段
                }
            }
            result.Add(obj);
        }
        return result;
    }

    // ====================== 私有辅助方法 ======================

    private static string EscapeCsv(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            field = field.Replace("\"", "\"\"");
            return $"\"{field}\"";
        }
        return field;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        result.Add(current.ToString());
        return result;
    }


    // 类型转换
    private object ConvertToType(string value, Type targetType)
    {
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(int)) return int.TryParse(value, out var i) ? i : 0;
        if (targetType == typeof(double)) return double.TryParse(value, out var d) ? d : 0.0;
        if (targetType == typeof(bool)) return bool.TryParse(value, out var b) ? b : false;
        if (targetType == typeof(DateTime)) return DateTime.TryParse(value, out var dt) ? dt : default;

        // 支持可空类型
        var underlying = Nullable.GetUnderlyingType(targetType);
        if (underlying != null)
            return ConvertToType(value, underlying);

        return Convert.ChangeType(value, targetType);
    }


}
