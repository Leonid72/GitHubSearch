namespace GitHub.API.Exception
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>  
        /// Extension methods for registering application services into the ASP.NET Core DI container.  
        /// Keeps Program.cs clean by encapsulating dependency registrations.  
        /// </summary>  
        public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient();
            // Db 
            services.AddDbContext<GitHub.Infrastructure.Data.AppDbContext>(opt =>
                opt.UseSqlite(config.GetConnectionString("Default")));

            // DI  
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<AuthService>();
            services.AddScoped<IGitHubSearchService, GitHubSearchService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddTransient<ErrorHandlingMiddleware>();
            services.AddMemoryCache();
            services.AddSingleton<IGenericCacheService, GenericCacheService>();
            // Swagger / OpenAPI configuration  
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "GitHub API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme."
                                  + "\n\nEnter ONLY your token below. The word 'Bearer' is added automatically."
                });

                // Apply Bearer auth to all operations (you can also do per-endpoint)  
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                      {
                            new OpenApiSecurityScheme
                       {
                           Reference = new OpenApiReference
                           {
                           Type = ReferenceType.SecurityScheme,
                           Id = "Bearer"
                           }
                       },
                           Array.Empty<string>()
                       }
                });
            });

            return services;
        }
    }
}
