using Polly;

static void GuardPersistApplicationData()
{
    const int RETRY_ATTEMPTS = 5;
    for (var i = 0; i < RETRY_ATTEMPTS; i++)
    {
        try
        {
            Thread.Sleep(i * 100);
            // Here comes the call, we *actually* care about.
            PersistApplicationData();
            // Successful call => exit loop.
            break;
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

static void PersistApplicationData()
{
    // ...
}



// Retry Forever
Policy.Handle<IOException>().Or<UnauthorizedAccessException>()
    .RetryForever(e => Console.WriteLine(e.Message))
    .Execute(PersistApplicationData);

// Retry n Times
Policy.Handle<Exception>()
    .Retry(10, (e, i) => Console.WriteLine($"Error '{e.Message}' at retry #{i}"))
    .Execute(PersistApplicationData);

// Wait and Retry
Policy.Handle<Exception>()
    .WaitAndRetry(5, count => TimeSpan.FromSeconds(count))
    .Execute(PersistApplicationData);

// Infinite Wait and Retry 
Policy.Handle<Exception>()
    .WaitAndRetryForever(count => TimeSpan.FromSeconds(count))
    .Execute(PersistApplicationData);

// Circuit Breaker
// The last policy we want to take a look at is slightly different from those we got to know so far. CircuitBreaker acts like its real-world prototype,
// which interrupts the flow of electricity. The software counterpart of fault current or short circuits are exceptions, and this policy can be configured
// in a way that a certain amount of exceptions “break” the application’s flow. This has the effect that the “protected” code (PersistApplicationData)
// simply will not get called any more, as soon as a given threshold of exceptions has been reached. Additionally, an interval can be specified,
// after which the CircuitBreaker recovers and the application flow is restored again.
var policy = Policy.Handle<IOException>().Or<UnauthorizedAccessException>()
    .CircuitBreaker(5, TimeSpan.FromMinutes(2));
// ...
policy.Execute(PersistApplicationData);