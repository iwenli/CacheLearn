/*----------------------------------------------------------------
 *  Copyright (C) 2017 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：RedisCache
 *  所属项目：
 *  创建用户：张玉龙(HouWeiya)
 *  创建时间：2018/4/20 15:18:05
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

using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheLearn.Cache
{
    /// <summary>  
    /// Redis缓存服务器  
    /// 服务器和客户端下载：  
    ///  https://github.com/MSOpenTech/redis/releases  
    ///  https://github.com/ServiceStack/ServiceStack.Redis  
    /// </summary>  
    public class RedisCache : IDataCache
    {
        private static RedisClient _redis = null;
        public static RedisClient redis
        {
            get
            {
                if (_redis == null) _redis = new RedisClient("127.0.0.1", 6379);//要开启服务器才能连接  
                return _redis;
            }
        }

        ~RedisCache()
        {
            if (_redis != null) _redis.Shutdown();  
        }

        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <typeparam name="T">类型（对象必须可序列化，否则可以作为object类型取出再类型转换，不然会报错）</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <returns></returns>  
        public T Get<T>(string key)
        {
            return redis.Get<T>(key);
        }
        public T Get<T>(string key, string depFile)
        {
            string timeKey = key + "_time";
            if (redis.Exists(timeKey) > 0 && redis.Exists(key) > 0)
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
            return redis.Set<T>(key, value);
        }
        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return redis.Set<T>(key, value, expiresAt);
        }

        public bool Set<T>(string key, T value, int expiresSecond)
        {
            return redis.Set<T>(key, value, DateTime.Now.AddSeconds(expiresSecond));
        }

        public bool Set<T>(string key, T value, string depFile)
        {
            bool ret1 = redis.Set<T>(key, value);
            if (ret1 && File.Exists(depFile))
            {
                FileInfo fi = new FileInfo(depFile);
                DateTime lastWriteTime = fi.LastWriteTime;
                return redis.Set<DateTime>(key + "_time", lastWriteTime);
            }
            return false;
        }

        public int Delete(string key)
        {
            return (int)redis.Del(key);
        }
        public int Delete(string[] keys)
        {
            return (int)redis.Del(keys);
        }
        public void Dispose()
        {
            if (_redis != null) _redis.Shutdown();//调用Dispose释放memcached客户端连接  
        }

    }
}
