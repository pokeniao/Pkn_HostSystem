using Pkn_HostSystem.Base.Log;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Pkn_HostSystem.Base;

public class CsvHelper
{
    private  List<List<string>> _rows = new();
    private readonly string _filePath;
    private readonly Encoding _encoding;
    private LogControl<CsvHelper> Log = new LogControl<CsvHelper>();

    public CsvHelper(string filePath, Encoding encoding = null)
    {
        _filePath = filePath;
        _encoding = encoding ?? Encoding.UTF8;
    }

    // 加载 CSV 文件内容（可选）
    public void Load()
    {
        _rows.Clear();
        //如果没有创建直接不用加载
        if (!File.Exists(_filePath)) return;

        //如果创建了,先读取一下 
        using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs, _encoding);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var fields = ParseCsvLine(line);
            _rows.Add(fields);
        }
        //读取完毕后,再判断一下是否被打开占用了
        try
        {
            //检查是否能正常打开,还是被占用了
            using (File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.None)) ;
        }
        catch (Exception e)
        {
            // 临时文件路径
            string tempFile  = _filePath + ".tmp";
            //被占用了,但临时文件还没有
            if (!File.Exists(tempFile)) return;

            //如果临时文件已经存在,读取临时文件
            using var fs2 = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader2 = new StreamReader(fs2, _encoding);
            string? line2;
            List<List<string>> _rows2 = new();
            while ((line2 = reader2.ReadLine()) != null)
            {
                var fields = ParseCsvLine(line2);
                _rows2.Add(fields);
            }
            //比较一下 那个内容长, 就取用那个的(防止加载的是旧缓存覆盖新缓存)
            if (_rows.Count < _rows2.Count)
            {
                _rows = _rows2;
            }
        }
    }
    //读取长度
    public int GetLineLength => _rows.Count;

    //读取指定行
    public List<string>? GetRow(int rowIndex) 
    {
        if (rowIndex >= 0 && rowIndex < _rows.Count)
        {
            return _rows[rowIndex];
        }
        return null;
    }
    //更具单元格值获取行号(倒序)
    public int GetRowIndexByCellValue(int columnIndex, string cellValue)
    {
        for (int i = _rows.Count -1; i >= 0 ; i--)
        {
            if (columnIndex >= 0 && columnIndex < _rows[i].Count &&
                _rows[i][columnIndex] == cellValue)
            {
                return i;
            }
        }
        return -1; // 未找到
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

    // 修改整行,替换整行数据(Json)
    public void UpdateRowFromJson(int rowIndex, string json)
    {
        if (rowIndex < 0 || rowIndex >= _rows.Count) return;
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
        // 若已有表头，按表头顺序写入值，找不到的列填空
        var header = _rows[0];
        var row = header.Select(h => dict.TryGetValue(h, out var value) ? value?.ToString() ?? "" : "").ToList();
        _rows[rowIndex] = row;
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
    public void Save(CancellationToken token)
    {
        //判断文件夹是否存在
        if (File.Exists(_filePath))
        {
            //存在临时文件中，避免直接覆盖原文件导致数据丢失
            string tempFile = _filePath + ".tmp";
            // 先写到临时文件
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                //创建临时文件写入流
            using (var writer = new StreamWriter(fs, _encoding))
            {
                foreach (var row in _rows)
                {
                    token.ThrowIfCancellationRequested(); // 检查取消,抛出异常
                    writer.WriteLine(string.Join(",", row.Select(EscapeCsv)));
                }
            }

            try
            {
                // 用临时文件替换目标文件（自动覆盖）
                File.Replace(tempFile, _filePath, null);
            }
            catch (IOException)
            {
                // 如果目标文件被占用，保留临时文件，提示用户稍后重试
                // 这样至少不会丢数据

                Log.Error($"文件正被占用，保存失败。请关闭相关程序后再重试。\n临时文件已保存到: {tempFile}");
            }
        }
        // 如果文件不存在，直接创建新文件
        else
        {
            using (var fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(fs, _encoding))
            {
                foreach (var row in _rows)
                {
                    token.ThrowIfCancellationRequested(); // 检查取消,抛出异常
                    writer.WriteLine(string.Join(",", row.Select(EscapeCsv)));// row.Select(EscapeCsv) 相当于  row.Select(x => EscapeCsv(x))
                }
            }

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
        //更具表头顺序,字段添加数据
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
            //将引号替换为两个引号
            field = field.Replace("\"", "\"\"");
            //又用引号包裹起来
            return $"\"{field}\"";
        }

        return field;
    }

    /// <summary>
    /// 解析 CSV 行，处理引号和逗号 ,并返回当前行为List<string>
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>(); // 存解析后的字段
        //一个单元格的数据
        var current = new StringBuilder(); // 当前字段内容
        bool inQuotes = false; // 是否在引号包裹的字段内
        //获取单个字符
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
                    current.Append(c); // 普通字符加入字段
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                //逗号是按列区分的分隔符
                else if (c == ',')
                {
                    result.Add(current.ToString()); //字段结束，加入列表
                    //清除掉当前单元格的数据
                    current.Clear();
                }
                else
                {
                    current.Append(c); // 普通字符加入字段
                }
            }
        }

        result.Add(current.ToString());
        return result;
    }


    /// <summary>
    /// 类型转换 , 将对应字符串转换为目标类型
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
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