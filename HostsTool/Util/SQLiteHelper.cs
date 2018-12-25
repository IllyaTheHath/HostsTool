using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace HostsTool.Util
{
    public class SQLiteHelper
    {
        private static String _datasource = @"data.db";
        private static SQLiteConnection _connection = null;
        private static Boolean _isOpen = false;

        /// <summary>
        /// 根据数据源设置连接对象
        /// </summary>
        /// <param name="datasource">数据源</param>
        public static void SetConnection(String datasource = null)
        {
            if (!String.IsNullOrWhiteSpace(datasource))
                _datasource = datasource;
            var connectionString = new SQLiteConnectionStringBuilder
            {
                Version = 3,
                DataSource = _datasource
            }.ConnectionString;
            _connection = new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// 释放连接对象
        /// </summary>
        public static void DisposeConnection()
        {
            if (_connection == null)
                return;
            CloseConnection();
            _connection.Dispose();
            GC.Collect();
        }

        /// <summary>
        /// 检查数据库是否存在
        /// </summary>
        /// <returns>是否存在</returns>
        public static Boolean CheckDbExist(String dbPath)
        {
            if (File.Exists(dbPath))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 创建数据库文件
        /// </summary>
        /// /// <exception cref="Exception"></exception>
        public static void CreateDB()
        {
            try
            {
                SQLiteConnection.CreateFile(_datasource);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 打开数据库连接
        /// </summary>
        private static void OpenConnection()
        {
            if (_connection == null)
                return;
            if (!_isOpen)
                _connection.Open();
            _isOpen = true;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        private static void CloseConnection()
        {
            if (_connection == null)
                return;
            if (_isOpen)
                _connection.Close();
            _isOpen = false;
        }

        /// <summary>
        /// 创建数据库操作命令
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">SQL语句参数</param>
        /// <returns>命令</returns>
        private static SQLiteCommand CreateCommand(String commandText, SQLiteParameter[] parameters)
        {
            SQLiteCommand cmd = new SQLiteCommand(commandText, _connection);
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }

        /// <summary>
        /// 创建SQL语句参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterType">参数类型</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns>SQL语句参数</returns>
        public static SQLiteParameter CreateParameter(String parameterName, DbType parameterType, Object parameterValue)
        {
            return new SQLiteParameter
            {
                DbType = parameterType,
                ParameterName = parameterName,
                Value = parameterValue
            };
        }

        /// <summary>
        /// 对SQLite数据库执行增删改操作
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">SQL语句参数</param>
        /// <returns>受影响的行数</returns>
        /// <exception cref="Exception"></exception>
        public static Int32 ExecuteNonQuery(String commandText, params SQLiteParameter[] parameters)
        {
            Int32 row = 0;
            try
            {
                OpenConnection();
                using (var cmd = CreateCommand(commandText, parameters))
                {
                    row = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return row;
        }

        /// <summary>
        /// 对SQLite数据库执行查询操作，返回关联的SQLiteDataReader实例
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">SQL语句参数</param>
        /// <returns>关联的SQLiteDataReader实例</returns>
        /// <exception cref="Exception"></exception>
        public static SQLiteDataReader ExecuteReader(String commandText, params SQLiteParameter[] parameters)
        {
            SQLiteDataReader reader = null;
            try
            {
                OpenConnection();
                using (var cmd = CreateCommand(commandText, parameters))
                {
                    reader = cmd.ExecuteReader();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return reader;
        }

        /// <summary>
        /// 对SQLite数据库执行查询操作，返回类型T的List集合
        /// </summary>
        /// <typeparam name="T">返回List集合中元素数据类型</typeparam>
        /// <param name="BuildObject">类型T的BuildObject函数</param>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">SQL语句参数</param>
        /// <returns>类型T的List集合</returns>
        /// <exception cref="Exception"></exception>
        public static List<T> ExecuteReader<T>(String commandText, params SQLiteParameter[] parameters)
            where T : class, new()
        {
            List<T> list = new List<T>();
            try
            {
                OpenConnection();
                using (var cmd = CreateCommand(commandText, parameters))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add((T)Activator.CreateInstance(typeof(T), reader));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return list;
        }

        /// <summary>
        /// 对SQLite数据库执行查询操作，返回第一个结果
        /// </summary>
        /// <typeparam name="T">返回结果类型</typeparam>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parameters">SQL语句参数</param>
        /// <returns>第一个结果</returns>
        /// <exception cref="Exception"></exception>
        public static T ExecuteScalar<T>(String commandText, params SQLiteParameter[] parameters)
        {
            T obj;
            try
            {
                OpenConnection();
                using (var cmd = CreateCommand(commandText, parameters))
                {
                    obj = (T)cmd.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseConnection();
            }
            return obj;
        }
    }
}
