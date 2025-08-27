
using StackExchange.Redis;

try
{
    // Connect to the local Redis server
    var redis = await ConnectionMultiplexer.ConnectAsync("localhost");

    // Get a Redis database instance
    var db = redis.GetDatabase();

    // Create a TokenBucket with:
    var bucket = new TokenBucket(
        db,
        "token_bucket",
        capacity: 10,
        refillTokens: 10,
        refillInterval: TimeSpan.FromSeconds(30)
    );

    // Start a background task to refill the bucket at the specified interval
    var refillTask = Task.Run(async () =>
    {
        while (true)
        {
            await bucket.RefillAsync(); // Add tokens to the bucket
            await Task.Delay(bucket.RefillInterval); // Wait for the next refill
        }
    });

    // Number of worker threads to simulate
    int threadCount = 50;
    var tasks = new Task[threadCount];
    for (int i = 0; i < threadCount; i++)
    {
        int threadId = i;
        // Each worker tries to consume tokens 10 times
        tasks[i] = Task.Run(async () =>
        {
            for (int j = 0; j < 1000; j++)
            {
                // Try to consume 1 token from the bucket
                bool success = await bucket.TryConsumeAsync(1);
                // Format threadId as two digits (e.g., 01, 02, ...)
                Console.WriteLine($"Thread {threadId:D2}: {(success ? "Acquired token" : "")}");
                await Task.Delay(1000); // Wait 1 second before next attempt
            }
        });
    }

    // Wait for all worker threads to finish
    await Task.WhenAll(tasks);
    // Clean up the Redis connection
    redis.Dispose();
}
catch (RedisConnectionException ex)
{
    Console.WriteLine("Error: Could not connect to Redis. Is the Redis server running on localhost:6379?");
    Console.WriteLine($"Details: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}