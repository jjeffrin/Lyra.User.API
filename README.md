# Lyra.User.API

This API handles authentication requests used by the [Lyra web application](https://lyra.jjeffr.in/). 

## Links

Swagger can be accessed [here](https://auth.lyra.jjeffr.in/swagger/index.html)

API [Health](https://auth.lyra.jjeffr.in/health) can also be checked. 

## How it works?

This is a .NET Core Web API created using .NET 7. It interacts with an Azure SQL Database using Entity Framework Core. This API can be used to register/login users by providing JWT access token as a HttpOnly cookie.
