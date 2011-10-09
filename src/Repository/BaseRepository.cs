using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;

namespace Snippets.Repository
{
    public abstract class BaseRepository<TEntity>
        where TEntity : Entity
    {
        #region Protected constructors

        protected BaseRepository(ObjectCache cache,
                                 bool        cacheEnabled,
                                 TimeSpan    cacheAbsoluteExpiration)
        {
            _cache                   = cache;
            _cacheEnabled            = cacheEnabled;
            _cacheAbsoluteExpiration = cacheAbsoluteExpiration;
        }

        #endregion

        #region Protected methods

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected bool TryGetCache(out TEntity     cacheValue,
                                   params object[] parameters)
        {
            // Create cache key unique to the calling method and the specified parameters.
            string cacheKey = CreateCacheKey(new StackFrame(1, false).GetMethod(),
                                             parameters);

            return TryGetCacheCore(cacheKey,
                                   out cacheValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected bool TryGetCache(out IEnumerable<TEntity> cacheValue,
                                   params object[]          parameters)
        {
            // Create cache key unique to the calling method and the specified parameters.
            string cacheKey = CreateCacheKey(new StackFrame(1, false).GetMethod(),
                                             parameters);

            return TryGetCacheCore(cacheKey,
                                   out cacheValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected bool TrySetCache(TEntity         cacheValue,
                                   params object[] parameters)
        {
            // Create cache key unique to the calling method and the specified parameters.
            string cacheKey = CreateCacheKey(new StackFrame(1, false).GetMethod(),
                                             parameters);

            return TrySetCacheCore(cacheKey,
                                   cacheValue);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected bool TrySetCache(IEnumerable<TEntity> cacheValue,
                                   params object[]      parameters)
        {
            // Create cache key unique to the calling method and the specified parameters.
            string cacheKey = CreateCacheKey(new StackFrame(1, false).GetMethod(),
                                             parameters);

            return TrySetCacheCore(cacheKey,
                                   cacheValue);
        }

        /// <summary>
        /// <para>Creates the default CacheItemPolicy object to use.</para>
        /// <para>Concrete implementations can override this method to customise caching policy on a Repository class level.</para>
        /// </summary>
        /// <returns></returns>
        protected virtual CacheItemPolicy CreateCachePolicy()
        {
            return new CacheItemPolicy
                       {
                           AbsoluteExpiration = DateTimeOffset.UtcNow.Add(_cacheAbsoluteExpiration),
                       };
        }

        #endregion

        #region Private static methods

        private bool TryGetCacheCore<T>(string cacheKey,
                                        out T  cacheValue)
            where T : class
        {
            // Fail if cache is not available, or caching is disabled
            if (_cache == null || !_cacheEnabled)
            {
                cacheValue = null;
                return false;
            }

            // Fail if cache does not contain an entry matching cacheKey
            if (!_cache.Contains(cacheKey))
            {
                cacheValue = null;
                return false;
            }

            // Get cached value
            object c = _cache[cacheKey];

            // If cached value is DBNull.Value, substitute with null
            cacheValue = c == DBNull.Value ? null : c as T;

            // Return true to indicate a value was found and retrieved from the cache
            return true;
        }

        private bool TrySetCacheCore<T>(string cacheKey,
                                        T      cacheValue)
            where T : class
        {
            // Fail if cache is not available or caching is disabled
            if (_cache == null || !_cacheEnabled)
            {
                return false;
            }

            // An exception occurs if the value null is added to the cache.
            // null is substituted with DBNull.Value instead to workaround this limitation.
            // When retrieving values from the cache, DBNull.Value must be substituted with null.
            _cache.Set(cacheKey,
                       cacheValue ?? (object)DBNull.Value,
                       CreateCachePolicy());

            // Return true to indicate the value was added to the cache
            return true;
        }

        /// <summary>
        /// Creates a caching key for the specified.
        /// </summary>
        /// <param name="method">The class method used to query the data.</param>
        /// <param name="parameters">The parameters passed to the class method to query the data.</param>
        /// <returns>A caching key unique to the specified query and parameters.</returns>
        /// <remarks>
        /// <para>Format of the key is: CallingClassFullName.CallingMethodName(CallingMethodParameterSignature):Encoded-String-Of-Parameters-Passed-To-Calling-Method</para>
        /// <para>e.g. JLTA.WebCIR.Infrastructure.Persistence.ClientRepository.GetClientByUserIdAndClientId(System.decimal clientId):AAEAAAD/////AQAAAAAAAAAQAQAAAAEAAAAKCw==</para>
        /// </remarks>
        private string CreateCacheKey(MethodBase      method,
                                      params object[] parameters)
        {
            ParameterInfo[] parameterInfos      = method.GetParameters();
            string          parameterSignatures = string.Empty;

            if (parameterInfos.Length > 0)
            {
                parameterSignatures = parameterInfos.Select(p => p.ParameterType.ToString() + " " + p.Name)
                                                    .Aggregate((current, next) => current + ", " + next);
            }

            string cacheKey = string.Format("{0}.{1}({2}):{3}",
                                            method.DeclaringType,
                                            method.Name,
                                            parameterSignatures,
                                            EncodeParameters(parameters));

            return cacheKey;
        }

        /// <summary>
        /// Creates a Base-64 string encoding of the specified <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string EncodeParameters(object[] parameters)
        {
            if (parameters == null)
            {
                return "null";
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                // If the current parameter is an includePaths parameter
                var includePathsParameter = parameters[i] as Expression<Func<TEntity, object>>[];
                if (includePathsParameter != null)
                {
                    parameters[i] = includePathsParameter.Select(p => p.ToPathString())
                                                         .ToList();
                }
            }

            using (var ms = new MemoryStream())
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, parameters);
                byte[] bytes = ms.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        #endregion

        #region Private fields

        private readonly ObjectCache _cache;
        private readonly bool        _cacheEnabled;
        private readonly TimeSpan    _cacheAbsoluteExpiration;

        #endregion
    }
}
