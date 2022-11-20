// See https://aka.ms/new-console-template for more information

using Ductus.FluentDocker.Builders;

Console.WriteLine("Hello, World!");

var svc = new Builder()
    .UseContainer()
    .UseCompose()
    .FromFile("./docker-compose.yml")
    .RemoveOrphans()
    .Build().Start();

Task.Delay(2000);

svc.Stop();
svc.Remove(true);