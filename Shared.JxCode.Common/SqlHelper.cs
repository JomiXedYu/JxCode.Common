/*
 * @Moudule     : SqlHelper
 * @Date        : 2020/12/04
 * @Author      : jx
 * @Link        : https://github.com/JomiXedYu
 * @Description : 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace JxCode.Common
{
    public class SqlTable<T> : IEnumerable<T> where T : new()
    {
        public string Name { get; private set; }
        protected SqlHelper parent;
        public SqlTable(string name, SqlHelper parent)
        {
            this.Name = name;
            this.parent = parent;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return parent.QueryAll<T>("select * from " + Name).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return parent.QueryAll<T>("select * from " + Name).GetEnumerator();
        }
    }
    public abstract class SqlHelper
    {
        protected DbConnection conn;

        public Type CommandType { get; protected set; }
        public Type ConnectionType { get; protected set; }
        public Type AdapterType { get; protected set; }

        public string DataBaseName { get; protected set; }

        public string SqlType { get; protected set; }

        #region factory
        /// <summary>
        /// 使用用户名和密码创建SQLServer对象
        /// </summary>
        /// <param name="host"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SqlHelper CreateBySQLServer(string host, string database, string username, string password)
        {
            return new SqlServer(host, database, username, password);
        }
        /// <summary>
        /// 使用Windows身份创建SQLServer对象
        /// </summary>
        /// <param name="host"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public static SqlHelper CreateBySQLServer(string host, string database)
        {
            return new SqlServer(host, database);
        }
        /// <summary>
        /// 使用默认Windows身份创建SQLServer对象
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static SqlHelper CreateBySQLServer(string database)
        {
            return new SqlServer(null, database);
        }
        /// <summary>
        /// 创建SQLite对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SqlHelper CreateBySQLite(string path)
        {
            return new Sqlite(path);
        }
        /// <summary>
        /// 创建MySQL对象
        /// </summary>
        /// <param name="host"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SqlHelper CreateByMySql(string host, string database, string username, string password)
        {
            return new MySql(host, database, username, password);
        }
        #endregion

        public virtual void Open()
        {
            this.conn.Open();
        }
        public virtual void Close()
        {
            this.conn.Close();
        }
        /// <summary>
        /// 表数量
        /// </summary>
        public abstract int TableCount { get; }
        /// <summary>
        /// 检测表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract bool HasTable(string tableName);

        protected DbCommand CreateCmd(string sql)
        {
            DbCommand cmd = Activator.CreateInstance(this.CommandType) as DbCommand;
            cmd.CommandText = sql;
            cmd.Connection = this.conn;
            return cmd;
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int NonQuery(string sql)
        {
            return CreateCmd(sql).ExecuteNonQuery();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="act"></param>
        public void Query(string sql, Action<DbDataReader> act)
        {
            using (var reader = CreateCmd(sql).ExecuteReader())
            {
                while (reader.Read())
                {
                    act.Invoke(reader);
                }
            }
        }
        /// <summary>
        /// 可打断的查询，返回true继续，返回false打断
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="act"></param>
        /// 
        public void QueryBreak(string sql, Func<DbDataReader, bool> act)
        {
            using (var reader = CreateCmd(sql).ExecuteReader())
            {
                while (reader.Read())
                {
                    if (!act.Invoke(reader))
                    {
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 查询一行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="act"></param>
        public void QueryLine(string sql, Action<DbDataReader> act)
        {
            using (var reader = CreateCmd(sql).ExecuteReader())
            {
                if (reader.Read())
                {
                    act.Invoke(reader);
                }
            }
        }
        /// <summary>
        /// 用字典查询一行
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Dictionary<string, string> QueryLineDict(string sql)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            QueryLine(sql, r =>
            {
                for (int i = 0; i < r.FieldCount; i++)
                {
                    ret.Add(r.GetName(i), r.GetString(i));
                }
            });
            return ret;
        }
        /// <summary>
        /// 用类型查询一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QueryLineT<T>(string sql) where T : new()
        {
            T t = new T();
            QueryLine(sql, r =>
            {
                for (int i = 0; i < r.FieldCount; i++)
                {
                    SetValue(t, r.GetName(i), r.GetValue(i));
                }
            });
            return t;
        }
        /// <summary>
        /// 用数据结构查询所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public IList<T> QueryAll<T>(string sql) where T : new()
        {
            List<T> list = new List<T>();
            Query(sql, r =>
            {
                T t = new T();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    SetValue(t, r.GetName(i), r.GetValue(i));
                }
                list.Add(t);
            });
            return list;
        }

        protected void SetValue<T, Value>(T t, string name, Value value)
        {
            Type type = t.GetType();

            FieldInfo fi = null;
            PropertyInfo pi = null;

            if ((fi = type.GetField(name)) != null)
            {
                fi.SetValue(t, value);
            }
            else if ((pi = type.GetProperty(name)) != null)
            {
                pi.SetValue(t, value, null);
            }
        }

        public SqlTable<T> GetTable<T>(string tableName) where T : new()
        {
            return new SqlTable<T>(tableName, this);
        }

        public DataTable QueryDataTable(string sql)
        {
            DbDataAdapter sq = Activator.CreateInstance(this.AdapterType) as DbDataAdapter;
            sq.SelectCommand = CreateCmd(sql);
            DataSet ds = new DataSet();
            sq.Fill(ds);
            return ds.Tables[0];
        }
        //未测试
        public class MySql : SqlHelper
        {
            public MySql(string host, string database, string username, string password)
            {
                this.SqlType = "MySql";
                this.DataBaseName = database;

                var ass = Assembly.Load("MySql.Data.SqlClient");
                this.ConnectionType = ass.GetType("MySql.Data.SQLiteConnection");
                this.CommandType = ass.GetType("MySql.Data.SQLiteCommand");
                this.AdapterType = ass.GetType("MySql.Data.");
                this.conn = Activator.CreateInstance(this.ConnectionType) as DbConnection;
                this.conn.ConnectionString =
                    string.Format("data source={0};database={1};userId={2};password={3};",
                    host, database, username, password);
            }

            public override int TableCount
            {
                get
                {
                    int count = 0;
                    Query("select table_name " +
                        "from information_schema.tables " +
                        "where table_schema = '" + this.DataBaseName + "'", r =>
                        {
                            count++;
                        });
                    return count;
                }
            }

            public override bool HasTable(string tableName)
            {
                bool exist = false;
                QueryBreak("select table_name " +
                    "from information_schema.tables " +
                    "where table_schema = '" + this.DataBaseName + "'", r =>
                    {
                        if (r.GetString(0) == tableName)
                        {
                            exist = true;
                            return false;
                        }
                        return true;
                    });
                return exist;
            }
        }
        public class SqlServer : SqlHelper
        {
            protected SqlServer(string database)
            {
                this.SqlType = "SqlServer";
                this.DataBaseName = database;

                var ass = Assembly.Load("System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                this.ConnectionType = ass.GetType("System.Data.SqlClient.SqlConnection");
                this.CommandType = ass.GetType("System.Data.SqlClient.SqlCommand");
                this.AdapterType = ass.GetType("System.Data.SqlClient.SqlDataAdapter");
                this.conn = Activator.CreateInstance(this.ConnectionType) as DbConnection;
            }
            public SqlServer(string host, string database) : this(database)
            {
                if (host == null)
                {
                    host = System.Environment.MachineName + "\\SQLEXPRESS";
                }
                this.conn.ConnectionString =
                    string.Format("data source={0};Integrated Security=true;database={1};",
                    host, database);
            }
            public SqlServer(string host, string database, string username, string password) : this(database)
            {
                this.conn.ConnectionString =
                    string.Format("data source={0};Integrated Security=false;database={1};user Id={2};password={3};",
                    host, database, username, password);

            }
            public override int TableCount
            {
                get
                {
                    int count = 0;
                    Query("select name from sysobjects where xtype='U'", r => count++);
                    return count;
                }
            }

            public override bool HasTable(string tableName)
            {
                bool exist = false;
                QueryBreak("select name from sysobjects where xtype='U'", r =>
                {
                    if (r.GetString(0) == tableName)
                    {
                        exist = true;
                        return false;
                    }
                    return true;
                });
                return exist;
            }
        }
        public class Sqlite : SqlHelper
        {
            public Sqlite(string path)
            {
                this.SqlType = "SQLite";
                this.DataBaseName = path;

                var ass = Assembly.Load("System.Data.SQLite");
                this.ConnectionType = ass.GetType("System.Data.SQLite.SQLiteConnection", true, true);
                this.CommandType = ass.GetType("System.Data.SQLite.SQLiteCommand", true, true);
                this.AdapterType = ass.GetType("System.Data.SQLite.SQLiteDataAdapter", true, true);
                this.conn = Activator.CreateInstance(this.ConnectionType) as DbConnection;
                this.conn.ConnectionString = "data source=" + path;
            }
            public override int TableCount
            {
                get
                {
                    int count = 0;
                    Query("select name from sqlite_master where type = 'table'", r => count++);
                    return count;
                }
            }

            public override bool HasTable(string tableName)
            {
                bool exist = false;
                Query("select name from sqlite_master where type = 'table'", r =>
                {
                    if (r.GetString(0) == tableName)
                    {
                        exist = true;
                    }
                });

                return exist;
            }
        }
    }
}
