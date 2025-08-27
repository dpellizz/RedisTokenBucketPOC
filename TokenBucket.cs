using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class TokenBucket
{
    private readonly IDatabase _redis;
    private readonly string _bucketKey;
    private readonly int _capacity;
    private readonly int _refillTokens;
    private readonly TimeSpan _refillInterval;
    public TimeSpan RefillInterval => _refillInterval;

    public TokenBucket(IDatabase redis, string bucketKey, int capacity, int refillTokens, TimeSpan refillInterval)
    {
        _redis = redis;
        _bucketKey = bucketKey;
        _capacity = capacity;
        _refillTokens = refillTokens;
        _refillInterval = refillInterval;
    }

    public async Task<bool> TryConsumeAsync(int tokens)
    {
        string script = @"
            local tokens = tonumber(redis.call('get', KEYS[1]) or '-1')
            if tokens == -1 then
                tokens = tonumber(ARGV[1])
                redis.call('set', KEYS[1], tokens)
            end
            if tokens >= tonumber(ARGV[2]) then
                redis.call('decrby', KEYS[1], ARGV[2])
                return 1
            else
                return 0
            end
        ";
        var result = (int)(long)await _redis.ScriptEvaluateAsync(script, new RedisKey[] { _bucketKey }, new RedisValue[] { _capacity, tokens });
        return result == 1;
    }

    public async Task RefillAsync()
    {
        string script = @"
            local tokens = tonumber(redis.call('get', KEYS[1]) or '-1')
            if tokens == -1 then
                tokens = tonumber(ARGV[1])
            end
            tokens = math.min(tokens + tonumber(ARGV[2]), tonumber(ARGV[1]))
            redis.call('set', KEYS[1], tokens)
            return tokens
        ";
        await _redis.ScriptEvaluateAsync(script, new RedisKey[] { _bucketKey }, new RedisValue[] { _capacity, _refillTokens });
    }
}
