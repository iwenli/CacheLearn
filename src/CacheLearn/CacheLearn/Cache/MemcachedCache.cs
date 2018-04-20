/*----------------------------------------------------------------
 *  Copyright (C) 2017 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：MemcachedCache
 *  所属项目：
 *  创建用户：张玉龙(HouWeiya)
 *  创建时间：2018/4/20 15:35:53
 *  
 *  功能描述：
 *          1、
 *          2、 
 * 
 *  修改标识：  
 *  修改描述：
 *  待 完 善：
 *          1、 
----------------------------------------------------------------*/

using Memcached.ClientLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLearn.Cache
{
    public class MemcachedCache : IDataCache
    {
        private MemcachedClient _mc = null;
        protected MemcachedClient mc
        {
            get
            {
                if (_mc == null) _mc = new MemcachedClient();//初始化一个客户端   
                return _mc;
            }
        }
        /// <summary>  
        /// 如果默认不是本地服务，可以额外指定memcached服务器地址  
        /// </summary>  
        public static string[] serverList
        {
            get;
            set;
        }
        private static MemcachedCache _instance = null;
        /// <summary>  
        /// 单例实例对象，外部只能通过MemcachedHelper.instance使用memcached  
        /// </summary>  
        public static MemcachedCache instance
        {
            get
            {
                if (_instance == null)
                {
                    if (serverList != null && serverList.Length > 0)
                        _instance = new MemcachedCache(serverList);
                    else _instance = new MemcachedCache();
                }

                return _instance;
            }
        }
        SockIOPool pool;
        private void start(params string[] servers)
        {
            string[] serverlist;
            if (servers == null || servers.Length < 1)
            {
                serverlist = new string[] { "127.0.0.1:11011" }; //服务器列表，可多个  
            }
            else
            {
                serverlist = servers;
            }
            pool = SockIOPool.GetInstance();

            //根据实际情况修改下面参数  
            pool.SetServers(serverlist);
            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 5;
            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;
            pool.MaintenanceSleep = 30;
            pool.Failover = true;
            pool.Nagle = false;
            pool.Initialize(); // initialize the pool for memcache servers        
        }
        public MemcachedCache(string[] servers)
        {
            start(servers);
        }
        public MemcachedCache()
        {
            start();
        }
        ~MemcachedCache()
        {
            //if (pool != null) pool.Shutdown();  
        }

        public T Get<T>(string key)
        {
            object data = mc.Get(key);
            if (data is T) return (T)data;
            else return default(T);
        }
        public T Get<T>(string key, string depFile)
        {
            string timeKey = key + "_time";
            if (mc.KeyExists(timeKey) && mc.KeyExists(key))
            {
                DateTime obj_time = Get<DateTime>(timeKey);
                T obj_cache = Get<T>(key);
                if (File.Exists(depFile))
                {
                    FileInfo fi = new FileInfo(depFile);
                    if (obj_time != fi.LastWriteTime)
                    {
                        Delete(key);
                        Delete(timeKey);
                        return default(T);
                    }
                    else return obj_cache;
                }
                else
                {
                    throw new Exception("文件(" + depFile + ")不存在！");
                }
            }
            else return default(T);

        }
        public bool Set<T>(string key, T value)
        {
            return mc.Set(key, value);
        }
        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return mc.Set(key, value, expiresAt);
        }

        public bool Set<T>(string key, T value, int expiresSecond)
        {
            return mc.Set(key, value, DateTime.Now.AddSeconds(expiresSecond));
        }

        public bool Set<T>(string key, T value, string depFile)
        {
            bool ret1 = mc.Set(key, value);
            if (ret1 && File.Exists(depFile))
            {
                FileInfo fi = new FileInfo(depFile);
                DateTime lastWriteTime = fi.LastWriteTime;
                return mc.Set(key + "_time", lastWriteTime);
            }
            return false;
        }


        public int Delete(string key)
        {
            return mc.Delete(key) ? 1 : 0;
        }
        public int Delete(string[] keys)
        {
            int i = 0;
            foreach (var key in keys)
            {
                mc.Delete(key);
                i++;
            }
            return i;
        }
        //在应用程序退出之前，调用Dispose释放memcached客户端连接  
        public void Dispose()
        {
            if (pool != null) pool.Shutdown();
        }
    }
}
