using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace ssbot;

/// <summary>
/// redis扩展
/// </summary>
public static class RedisExtension
{
    /// <summary>
    /// 大小写敏感 允许中文的序列化设置
    /// </summary>
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
    
    static RedisExtension()
    {
        JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
    
    
    /// <summary>
    /// 获取指定key的缓存，找不到则返回null
    /// </summary>
    /// <param name="cache">分布式缓存</param>
    /// <param name="key">缓存key</param>
    /// <param name="token">取消令牌</param>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken token = default)
    {
        var s = await cache.GetStringAsync(key, token);
        return s is null ? default : JsonSerializer.Deserialize<T>(s, JsonSerializerOptions);
    }

    /// <summary>
    /// 设置缓存
    /// </summary>
    /// <param name="cache">分布式缓存</param>
    /// <param name="key">缓存key</param>
    /// <param name="data">缓存数据</param>
    /// <param name="expire">过期时间</param>
    /// <param name="token">取消令牌</param>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T data,
        TimeSpan? expire = null, CancellationToken token = default) => cache.SetStringAsync(key,
        JsonSerializer.Serialize(data, JsonSerializerOptions),
        expire is null ? new () : new () { SlidingExpiration = expire }, token);

    /// <summary>
    /// 获取指定key的缓存，如果没有则进行设置
    /// </summary>
    /// <param name="cache">分布式缓存</param>
    /// <param name="key">缓存key</param>
    /// <param name="addAction">获得要设置的数据的委托</param>
    /// <param name="expire">过期时间</param>
    /// <param name="token">取消令牌</param>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public static async Task<T> GetOrAddAsync<T>(this IDistributedCache cache, string key, Func<T> addAction,
        TimeSpan? expire = null, CancellationToken token = default)
    {
        var r = await cache.GetAsync<T>(key, token);
        if (r is not null) return r;
        r = addAction();
        await cache.SetAsync(key, r, expire, token);
        return r;
    }
}