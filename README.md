## Project Running Steps

- ```git clone https://github.com/EmreCanCakir/Bemay.git```
- ```cd PathToProjectDirectory```
- ```dotnet clean```
- ```dotnet restore --no-cache```
- ```dotnet build --no-restore```
- ```cd Bemay```
- ```dotnet run --no-build```

**If you got that error:**

 Failed executing DbCommand (9ms) [Parameters=[], CommandType='Text', CommandTimeout='60']
      CREATE DATABASE [Bemay_DB];
Unhandled exception. Microsoft.Data.SqlClient.SqlException (0x80131904): Database 'Bemay_DB' already exists. Choose a different database name.

**You should command out those lines from program.cs:**

```
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!dbContext.Database.CanConnect()) 
    {
        dbContext.Database.Migrate();
    }
}
```
