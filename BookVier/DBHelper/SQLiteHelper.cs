using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace BookVier.DBHelper
{
    class SQLiteHelper
    {
        private string _dbName = "";
        private SQLiteConnection _SQLiteConn = null;     //连接对象
        private SQLiteTransaction _SQLiteTrans = null;   //事务对象
        private bool _IsRunTrans = false;        //事务运行标识
        private string _SQLiteConnString = null; //连接字符串
        private bool _AutoCommit = false; //事务自动提交标识
        /// <summary>
        /// 数据读取定义
        /// </summary>
        private SQLiteDataReader dataReader;
        /// <summary>
        /// SQL命令定义
        /// </summary>
        private SQLiteCommand dbCommand;

        public string SQLiteConnString
        {
            set { this._SQLiteConnString = value; }
            get { return this._SQLiteConnString; }
        }

        public SQLiteHelper(string dbPath)
        {
            this._dbName = dbPath;
            this._SQLiteConnString = "Data Source=" + dbPath;
        }

        /// <summary>
        /// 新建数据库文件
        /// </summary>
        /// <param name="dbPath">数据库文件路径及名称</param>
        /// <returns>新建成功，返回true，否则返回false</returns>
        static public Boolean NewDbFile(string dbPath)
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("新建数据库文件" + dbPath + "失败：" + ex.Message);
            }
        }


        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public SQLiteDataReader ExecuteQuery(string queryString)
        {
            try
            {
                this._SQLiteConn = new SQLiteConnection(_SQLiteConnString);
                _SQLiteConn.Open();
                dbCommand = _SQLiteConn.CreateCommand();
                dbCommand.CommandText = queryString;
                dataReader = dbCommand.ExecuteReader();
            }
            catch (Exception e)
            {
                throw e;
            }

            return dataReader;
        }

        public int ExecuteNonQuery(string sql)
        {
            try
            {
                this._SQLiteConn = new SQLiteConnection(_SQLiteConnString);
                _SQLiteConn.Open();
                SQLiteCommand command = new SQLiteCommand(sql, _SQLiteConn);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            return 0;
        }


        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbPath">指定数据库文件</param>
        /// <param name="tableName">表名称</param>
        static public void NewTable(string dbPath, string sql, string tableName)
        {

            SQLiteConnection sqliteConn = new SQLiteConnection("data source=" + dbPath);
            if (sqliteConn.State != System.Data.ConnectionState.Open)
            {
                sqliteConn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = sqliteConn;
                cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master where type='table' and name='tableIndex'";
                if (0 == Convert.ToInt32(cmd.ExecuteScalar()))
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

            }
            sqliteConn.Close();
        }


        public SQLiteCommand getCommand(string sql)
        {
            this._SQLiteConn = new SQLiteConnection(_SQLiteConnString);
            _SQLiteConn.Open();
            SQLiteCommand command = new SQLiteCommand(sql, _SQLiteConn);
            return command;
        }

        /// <summary>
        /// 打开当前数据库的连接
        /// </summary>
        /// <returns></returns>
        public Boolean OpenDb()
        {
            try
            {
                this._SQLiteConn = new SQLiteConnection(this._SQLiteConnString);
                this._SQLiteConn.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库：" + _dbName + "的连接失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 打开指定数据库的连接
        /// </summary>
        /// <param name="dbPath">数据库路径</param>
        /// <returns></returns>
        public Boolean OpenDb(string dbPath)
        {
            try
            {
                string sqliteConnString = "Data Source=" + dbPath;

                this._SQLiteConn = new SQLiteConnection(sqliteConnString);
                this._dbName = dbPath;
                this._SQLiteConnString = sqliteConnString;
                this._SQLiteConn.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库：" + dbPath + "的连接失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseDb()
        {
            if (this._SQLiteConn != null && this._SQLiteConn.State != ConnectionState.Closed)
            {
                if (this._IsRunTrans && this._AutoCommit)
                {
                    this.Commit();
                }
                this._SQLiteConn.Close();
                this._SQLiteConn = null;
            }
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        public void BeginTransaction()
        {
            this._SQLiteConn.BeginTransaction();
            this._IsRunTrans = true;
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        /// <param name="isoLevel">事务锁级别</param>
        public void BeginTransaction(IsolationLevel isoLevel)
        {
            this._SQLiteConn.BeginTransaction(isoLevel);
            this._IsRunTrans = true;
        }

        /// <summary>
        /// 提交当前挂起的事务
        /// </summary>
        public void Commit()
        {
            if (this._IsRunTrans)
            {
                this._SQLiteTrans.Commit();
                this._IsRunTrans = false;
            }
        }
    }
}
