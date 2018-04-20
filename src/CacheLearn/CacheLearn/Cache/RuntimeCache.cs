/*----------------------------------------------------------------
 *  Copyright (C) 2017 天下商机（txooo.com）版权所有
 * 
 *  文 件 名：RuntimeCache
 *  所属项目：
 *  创建用户：张玉龙(HouWeiya)
 *  创建时间：2018/4/20 15:48:24
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CacheLearn.Cache
{
    public class RuntimeCache : IDataCache
    {

        #region 删除缓存  
        /// <summary>  
        /// 删除缓存  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        public int Delete(string CacheKey)
        {
            HttpRuntime.Cache.Remove(CacheKey);
            return 1;
        }
        public int Delete(string[] CacheKeys)
        {
            int i = 0;
            foreach (var key in CacheKeys)
            {
                HttpRuntime.Cache.Remove(key);
                i++;
            }
            return i;
        }
        #endregion

        #region 获取缓存，依赖时间  
        /// <summary>  
        /// 获取缓存，依赖时间  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <returns></returns>  
        public T Get<T>(string CacheKey)
        {
            object obj_time = HttpRuntime.Cache[CacheKey + "_time"];
            object obj_cache = HttpRuntime.Cache[CacheKey];
            if (obj_time != null && obj_cache != null)
            {
                if (Convert.ToDateTime(obj_time) < DateTime.Now)
                {
                    Delete(CacheKey);
                    Delete(CacheKey + "_time");
                    return default(T);
                }
                else return (T)obj_cache;
            }
            else
            {
                Delete(CacheKey);
                Delete(CacheKey + "_time");
                return default(T);
            }
        }
        #endregion

        #region 获取缓存，依赖文件  
        /// <summary>  
        /// 获取缓存，依赖文件  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <param name="depFile">依赖的文件</param>  
        /// <returns></returns>  
        public T Get<T>(string CacheKey, string depFile)
        {
            object obj_time = HttpRuntime.Cache[CacheKey + "_time"];
            object obj_cache = HttpRuntime.Cache[CacheKey];
            if (File.Exists(depFile))
            {
                FileInfo fi = new FileInfo(depFile);

                if (obj_time != null && obj_cache != null)
                {
                    if (Convert.ToDateTime(obj_time) != fi.LastWriteTime)
                    {
                        Delete(CacheKey);
                        Delete(CacheKey + "_time");
                        return default(T);
                    }
                    else return (T)obj_cache;
                }
                else
                {
                    Delete(CacheKey);
                    Delete(CacheKey + "_time");
                    return default(T);
                }
            }
            else
            {
                throw new Exception("文件(" + depFile + ")不存在！");
            }
        }
        #endregion

        #region 简单的插入缓存  
        /// <summary>  
        /// 简单的插入缓存  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        public bool Set<T>(string CacheKey, T objObject)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject);
            return true;
        }
        #endregion

        #region 有过期时间的插入缓存数据  
        public bool Set<T>(string CacheKey, T objObject, DateTime expiresAt)
        {
            HttpRuntime.Cache.Insert(CacheKey, objObject, null, expiresAt, Cache.NoSlidingExpiration);
            HttpRuntime.Cache.Insert(CacheKey + "_time", expiresAt, null, expiresAt, Cache.NoSlidingExpiration);//存储过期时间  
            return true;
        }
        #endregion

        #region 插入缓存数据，指定缓存多少秒  
        public bool Set<T>(string CacheKey, T objObject, int seconds)
        {
            DateTime expiresAt = DateTime.Now.AddSeconds(seconds);
            HttpRuntime.Cache.Insert(CacheKey, objObject, null, expiresAt, Cache.NoSlidingExpiration);
            HttpRuntime.Cache.Insert(CacheKey + "_time", expiresAt, null, expiresAt, Cache.NoSlidingExpiration);//存储过期时间  
            return true;
        }
        #endregion

        #region 依赖文件的缓存，文件没改不会过期  
        /// <summary>  
        /// 依赖文件的缓存，文件没改不会过期  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        /// <param name="depfilename">依赖文件，可调用 DataCache 里的变量</param>  
        public bool Set<T>(string CacheKey, T objObject, string depfilename)
        {
            //缓存依赖对象  
            System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(depfilename);
            DateTime absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            TimeSpan slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                CacheKey,
                objObject,
                dep,
                absoluteExpiration, //从不过期  
                slidingExpiration, //禁用可调过期  
                System.Web.Caching.CacheItemPriority.Default,
                null);
            if (File.Exists(depfilename))
            {
                FileInfo fi = new FileInfo(depfilename);
                DateTime lastWriteTime = fi.LastWriteTime;
                HttpRuntime.Cache.Insert(CacheKey + "_time", lastWriteTime, null, absoluteExpiration, slidingExpiration);//存储文件最后修改时间  
            }
            return true;
        }
        #endregion
    }
}
