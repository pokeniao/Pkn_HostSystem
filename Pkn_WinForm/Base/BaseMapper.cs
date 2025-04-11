using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pkn_WinForm.Base
{
    /// <summary>
    /// 模块块封装人 -- 破壳鸟(潘智高)
    /// 2025-02-28
    /// 参考:MybatisPlus
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseMapper<T>
    {
        //获取类型
        public Type type { get; set; }
        //获取属性

        public PropertyInfo[] propertyInfos;

        public string tableName;
        public string primaryKey = "id";
        public string primaryKeyProperty = "id";
        public List<string> incrementyProperty = new List<string>();
        public string sql { get; set; }
        //读取配置文件
        private static string url = ConfigurationManager.ConnectionStrings["url"].ToString();

        //private static string url = "Server=POKENIAO;DataBase=LearnSQL;Uid=sa;Pwd=pan1zhi2gao3";

        protected BaseMapper()
        {
            type = typeof(T);
            propertyInfos = type.GetProperties();
            //设置表名
            tableName = type.Name;

            IEnumerable<CustomAttributeData> customAttributes = type.CustomAttributes;
            foreach (CustomAttributeData attribute in customAttributes)
            {
                Type attributeType = attribute.AttributeType;
                string name = attributeType.Name;
                if (name == "TableNameAttribute")
                {
                    TableNameAttribute tableNameAttribute = (TableNameAttribute)type.GetCustomAttributes(typeof(TableNameAttribute), false)[0];
                    tableName = tableNameAttribute.TableName;
                }
            }
            //设置主键
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                IEnumerable<CustomAttributeData> attributes = propertyInfo.CustomAttributes;
                foreach (CustomAttributeData attribute in attributes)
                {
                    Type attributeType = attribute.AttributeType;
                    string name = attributeType.Name;
                    if (name == "PrimaryKeyAttribute")
                    {
                        PrimaryKeyAttribute primaryKeyAttribute = (PrimaryKeyAttribute)propertyInfo.GetCustomAttributes(typeof(PrimaryKeyAttribute), false)[0];
                        primaryKey = primaryKeyAttribute.PrimaryKeyName;
                        primaryKeyProperty = propertyInfo.Name;
                    }

                    if (name == "IncremntAttribute")
                    {
                        incrementyProperty.Add(propertyInfo.Name);
                    }
                }
            }
        }

        #region 插入一行数据
        /// <summary>
        /// 查入单行用
        /// </summary>
        /// <param name="pojo"></param>
        /// <returns></returns>
        public int insert(T pojo)
        {
            StringBuilder sql = new StringBuilder();
            sql = insertLogic(pojo, sql);

            this.sql = sql.ToString();
            return ExecuteNonQuery(sql.ToString());
        }

        #endregion

        #region 插入逻辑

        public StringBuilder insertLogic(T pojo, StringBuilder sql)
        {
            Type pojoType = pojo.GetType();
            sql.Append("insert into ");
            sql.Append(tableName);
            sql.Append(" (");
            //获取所有的属性;
            PropertyInfo[] properties = pojoType.GetProperties();

            foreach (var propertie in properties)
            {
                if (propertie.GetValue(pojo) != null && !incrementyProperty.Contains(propertie.Name))
                {
                    sql.Append(propertie.Name);
                    sql.Append(",");
                }
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append(") values(");
            foreach (var propertie2 in properties)
            {
                if (propertie2.GetValue(pojo) != null && !incrementyProperty.Contains(propertie2.Name))
                {
                    sql.Append("'");
                    sql.Append(propertie2.GetValue(pojo));
                    sql.Append("',");
                }
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append(");");
            return sql;
        }


        #endregion

        #region 插入通过List集合
        /// <summary>
        /// 超过1W5数据用
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public async Task<int> insertBig(List<T> lists)
        {
            StringBuilder sql = new StringBuilder();
            List<Task<int>> tasks = new List<Task<int>>();  // 用于存储任务列表
            int num = 0;
            for (int i = 0; i < lists.Count; i++)
            {
                sql = insertLogic(lists[i], sql);
                if (i % 5000 == 0 && i != 0)
                {
                    var currentSql = sql.ToString();  // 保存当前的 SQL
                    tasks.Add(Task.Run(() => sendExecuteNonQueryWork(currentSql)));
                    sql.Clear();  // 清空 sql
                }
            }
            if (sql.Length != 0)
            {
                var currentSql = sql.ToString();
                tasks.Add(Task.Run(() => sendExecuteNonQueryWork(currentSql)));
            }
            //等待所有任务结束
            // Task.WaitAll(tasks.ToArray());
            //异步等待所有控件
            await Task.WhenAll(tasks.ToArray());
            // 收集并输出结果
            foreach (var task in tasks)
            {
                num += task.Result;
            }

            return num;
        }
        private int sendExecuteNonQueryWork(string sql)
        {
            return ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 异步用
        /// </summary>
        /// <param name="lists"></param>
        public void insertAsync(List<T> lists)
        {
            StringBuilder sql = new StringBuilder();
            foreach (T pojo in lists)
            {
                sql = insertLogic(pojo, sql);
            }
            ExecuteNonQueryAsync(sql.ToString());
        }
        /// <summary>
        /// 插入多行用
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public int insertByList(List<T> lists)
        {
            StringBuilder sql = new StringBuilder();
            foreach (T pojo in lists)
            {
                sql = insertLogic(pojo, sql);
            }
            this.sql = sql.ToString();
            return ExecuteNonQuery(sql.ToString());
        }
        #endregion

        #region 插入通过sql

        public int insert(string sql)
        {
            return ExecuteNonQuery(sql);
        }

        #endregion

        #region 更新通过id

        public int updateById(T pojo)
        {
            Type pojoType = pojo.GetType();
            StringBuilder sql = new StringBuilder();
            sql.Append("update ");
            sql.Append(tableName);
            sql.Append(" set ");
            //获取所有的属性;
            PropertyInfo[] properties = pojoType.GetProperties();

            foreach (var propertie in properties)
            {

                if (propertie.GetValue(pojo) != null && !incrementyProperty.Contains(propertie.Name))
                {
                    sql.Append(propertie.Name);
                    sql.Append("='");
                    sql.Append(propertie.GetValue(pojo));
                    sql.Append("',");
                }
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append($" where {primaryKey} =");

            PropertyInfo property = pojoType.GetProperty(primaryKeyProperty);
            sql.Append(property.GetValue(pojo));
            this.sql = sql.ToString();
            return ExecuteNonQuery(sql.ToString());
        }

        #endregion

        #region 更新数据通过where

        public int updateByWhere(T pojo, string where)
        {
            Type pojoType = pojo.GetType();
            StringBuilder sql = new StringBuilder();
            sql.Append("update ");
            sql.Append(tableName);
            sql.Append(" set ");
            //获取所有的属性;
            PropertyInfo[] properties = pojoType.GetProperties();

            foreach (var propertie in properties)
            {
                if (propertie.GetValue(pojo) != null && !incrementyProperty.Contains(propertie.Name))
                {
                    sql.Append(propertie.Name);
                    sql.Append("='");
                    sql.Append(propertie.GetValue(pojo));
                    sql.Append("',");
                }
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append(" ");
            sql.Append(where);
            this.sql = sql.ToString();
            return ExecuteNonQuery(sql.ToString());
        }

        #endregion

        #region 更新通过sql

        public int update(string sql)
        {
            return ExecuteNonQuery(sql);
        }

        #endregion

        #region 删除通过sql

        public int delete(string sql)
        {
            return ExecuteNonQuery(sql);
        }

        #endregion

        #region 删除通过id,主键必须为id

        public int deleteById(long id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ");
            sql.Append(tableName);
            sql.Append($" where {primaryKey} =");
            sql.Append(id);
            this.sql = sql.ToString();
            return ExecuteNonQuery(sql.ToString());
        }

        #endregion

        #region 删除通过where
        public int deleteByWhere(string where)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ");
            sql.Append(type.Name);
            sql.Append(" ");
            sql.Append(where);
            this.sql = sql.ToString();
            return ExecuteNonQuery(sql.ToString());
        }

        #endregion


        #region 删除全部
        public int deleteAll()
        {
            string sql = $"delete from {tableName}";

            this.sql = sql;
            return ExecuteNonQuery(sql);
        }


        #endregion

        #region 查询通过id,返回单个,需要id为主键

        public T selectById(long id)
        {
            object pojo = null;
            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var property in propertyInfos)
            {
                sql.Append(property.Name);
                sql.Append(",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" from ");
            sql.Append(type.Name);
            sql.Append($" where {primaryKey} = ");
            sql.Append(id.ToString());
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql.ToString();
                    sqlCommand.Connection = sqlConnection;
                    using (SqlDataReader result = sqlCommand.ExecuteReader())
                    {
                        if (result.Read())
                        {
                            pojo = Activator.CreateInstance(type);//创建对象
                            foreach (var property in propertyInfos)
                            {
                                if (result[property.Name].ToString().Length != 0)
                                {
                                    property.SetValue(pojo, result[property.Name]);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"发生错误{e.Message}");
                    return default(T);
                }

                this.sql = sql.ToString();
                return (T)pojo;
            }
        }

        #endregion

        #region 查询通过id,返回多个,需要id为主键
        public List<T> selectBatchIds(List<long> ids)
        {
            List<T> listPojo;
            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var property in propertyInfos)
            {
                sql.Append(property.Name);
                sql.Append(",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" from ");
            sql.Append(type.Name);
            sql.Append($" where {primaryKey} in (");
            foreach (long id in ids)
            {
                sql.Append(id.ToString());
                sql.Append(",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql.ToString();
                    sqlCommand.Connection = sqlConnection;
                    using (SqlDataReader result = sqlCommand.ExecuteReader())
                    {
                        listPojo = new List<T>();
                        while (result.Read())
                        {
                            object pojo = Activator.CreateInstance(type);//创建对象
                            foreach (var property in propertyInfos)
                            {
                                if (result[property.Name].ToString().Length != 0)
                                {
                                    property.SetValue(pojo, result[property.Name]);
                                }
                            }
                            listPojo.Add((T)pojo);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"发生错误{e.Message}");
                    return default(List<T>);
                }

                this.sql = sql.ToString();
                return listPojo;
            }
        }


        #endregion

        #region 查询通过where,返回List
        public List<T> selectByWhere(string where)
        {
            List<T> listPojo;
            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var property in propertyInfos)
            {
                sql.Append(property.Name);
                sql.Append(",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" from ");
            sql.Append(tableName);
            sql.Append(" ");
            sql.Append(where);
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql.ToString();
                    sqlCommand.Connection = sqlConnection;
                    using (SqlDataReader result = sqlCommand.ExecuteReader())
                    {
                        listPojo = new List<T>();
                        while (result.Read())
                        {
                            object pojo = Activator.CreateInstance(type);//创建对象
                            foreach (var property in propertyInfos)
                            {
                                if (result[property.Name].ToString().Length != 0)
                                {
                                    property.SetValue(pojo, result[property.Name]);
                                }
                            }
                            listPojo.Add((T)pojo);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"发生错误{e.Message}");
                    return default(List<T>);
                }

                this.sql = sql.ToString();
                return listPojo;
            }
        }



        #endregion

        #region 查询通过sql,返回DataTable
        public DataTable select(string sql)
        {
            return SqlDataAdapter(sql);
        }

        #endregion

        #region 查询总数量
        public int Count()
        {
            string sql = $"select count({primaryKey}) from {tableName};";
            this.sql = sql;
            return ExecuteScalar(sql);
        }
        #endregion

        #region 查询指定条件的数量
        public int Count(string where)
        {
            string sql = $"select count({primaryKey}) from {tableName} {where};";
            this.sql = sql;
            return ExecuteScalar(sql);
        }
        #endregion

        #region ADO增删改查ExecuteNonQuery
        public int ExecuteNonQuery(string sql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                int result;
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql;
                    sqlCommand.Connection = sqlConnection;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"执行ExecuteNonQuery错误:{e.Message}");
                    return -1;
                }

                return result;
            }
        }

        #endregion

        #region 增删改查异步ExecuteNonQueryAsync

        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                int result;
                try
                {
                    await sqlConnection.OpenAsync();  // 异步打开连接
                    using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                    {
                        // 异步执行 SQL 命令
                        result = await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"执行ExecuteNonQueryAsync错误:{e.Message}");
                    return -1;
                }
                return result;
            }
        }


        #endregion

        #region 查询ExecuteScalar只返回一个结果的

        public int ExecuteScalar(string sql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                object result;
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql;
                    sqlCommand.Connection = sqlConnection;


                    result = sqlCommand.ExecuteScalar();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"执行ExecuteScalar错误:{e.Message}");
                    return -1;
                }
                return (int)result;
            }
        }

        #endregion

        #region 返回一个结果集ExecuteReader
        public T ExecuteReader(string sql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql;
                    sqlCommand.Connection = sqlConnection;


                    //提交查询返回一个结果集,注意result是需要关闭的
                    using (SqlDataReader result = sqlCommand.ExecuteReader())
                    {
                        //Read()每次读取一行数据,当数据被读取完后结束
                        while (result.Read())
                        {
                            //需要通过result["列名"]索引到内容
                            Console.WriteLine(result["name"]);
                        }
                        //可以跳转到下个结果集
                        // if (result.NextResult())
                        // {
                        //     while (result.Read())
                        //     {
                        //         //需要通过result["列名"]索引到内容
                        //         Console.WriteLine(result["name"]);
                        //     }
                        // }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"执行ExecuteReader错误:{e.Message}");
                    return default(T);
                }
                return default(T);
            }
        }
        #endregion

        #region 返回一个DateTable,在内存产生数据库里面一个表
        public DataTable SqlDataAdapter(string sql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(url))
            {
                try
                {
                    sqlConnection.Open(); //打开连接
                    SqlCommand sqlCommand = new SqlCommand();
                    sqlCommand.CommandText = sql;
                    sqlCommand.Connection = sqlConnection;


                    //创建一个数据适配器对象
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    //创建数据集对象DateSet
                    DataSet dataSet = new DataSet();
                    //填充数据
                    sqlDataAdapter.Fill(dataSet);
                    //遍历表,获取行
                    // foreach (var row in dataSet.Tables[0].Rows)
                    // {
                    //     row["列名"]
                    //
                    // }
                    return dataSet.Tables[0];
                }
                catch (Exception e)
                {
                    Console.WriteLine($"执行SqlDataAdapter错误:{e.Message}");
                    return null;
                }

            }
        }
        #endregion

    }

    /// <summary>
    /// 在POJO中
    /// 通过[PrimaryKey]标记主键
    /// 通过[TableName]标记表明
    /// 通过[Incremnt]标记递增,标记递增后不会继续增加
    /// </summary>

    #region Attribute
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : System.Attribute
    {
        public string PrimaryKeyName { get; }

        public PrimaryKeyAttribute(string primaryKeyName)
        {
            this.PrimaryKeyName = primaryKeyName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : System.Attribute
    {
        public string TableName { get; }

        public TableNameAttribute(string TableName)
        {
            this.TableName = TableName;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IncremntAttribute : System.Attribute
    {

    }
    #endregion

}