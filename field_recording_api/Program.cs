using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using field_recording_api.Models.DataAccess;
using field_recording_api.DataAccess;
using field_recording_api.Utilities;
using field_recording_api.DataAccess.FieldRecording;
using field_recording_api.Helpers.JWT;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Options;
using field_recording_api.DataAccess.SIIS;
using Microsoft.Extensions.DependencyInjection;
using field_recording_api.Services.Interface;
using field_recording_api.Services.Implement;
using field_recording_api.Services.MasterDataServices;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IFollowUpService, FollowUpService>();
builder.Services.AddTransient<IPaymentService, PaymentService>();
builder.Services.AddTransient<IMasterDataServices, MasterDataServices>();


//builder.Services.AddControllers();

builder.Services.AddControllers().AddJsonOptions(options =>
{
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
});







builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Field Recording Mobile API",
        Version = "v1",
        Description = "Field Recording Mobile API blueprint<br>" +
            "Version: v0.1" + "<br>",
        //"Environment: " + this.EnvironmentName,
        //Contact = new OpenApiContact
        //{
        //    Name = "dev_team",
        //    Email = "nipon.jt@summitcapital.co.th;kitsana.w@summitcapital.co.th",
        //}
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.Last());

    var securityScheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
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

    c.MapType<FileContentResult>(() => new OpenApiSchema
    {
        Type = "file"
    });
});

JWT.Version = typeof(Program).Assembly.GetName().Name.ToString();

var address = builder.Configuration.GetSection("ConnectionStrings:FieldRecordingConnection").Get<string>();
if (address.Split(";")[0].Length > 0)
{
    JWT.BuildVersion = address.Split(";")[0].Split("=").Last();
}

builder.Services.AddProblemDetails(options =>
options.OnBeforeWriteDetails = (ctx, problem) => {
    problem.Extensions["traceId"] = ctx.TraceIdentifier;
});

builder.Services.AddHealthChecks();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        );

});

//Multi Regigter Interface and Service 
Assembly.GetExecutingAssembly()
.GetTypes()
.Where(a => a.Name.EndsWith("Services") && !a.IsAbstract && !a.IsInterface)
.Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
.ToList()
.ForEach(typesToRegister =>
{
    typesToRegister.serviceTypes.ForEach(typeToRegister => builder.Services.AddScoped(typeToRegister, typesToRegister.assignedType));
});

//Automapper
builder.Services.AddAutoMapper(typeof(Program));

//log4net
builder.Services.AddLog4net();

//Context
//builder.Services.AddScoped<IDbContext, FieldRecordingContext>();
//builder.Services.AddScoped<IDbContext, SiisUatContext>();
builder.Services.AddScoped(typeof(IRepositoryEfcore<>), typeof(EfCoreRepository<>));
builder.Services.AddTransient<IUnitOfWorkDB, unitOfWorkDB>();

builder.Services.AddTransient<CoreRepository>();

//builder.Services.Configure<ConnectionStringsModel>(builder.Configuration.GetSection("ConnectionStrings:FieldRecordingConnection"));

builder.Services.AddScoped<DBHelpers>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton(builder.Configuration.GetSection("ConnectionStrings").Get<ConnectionStringsModel>());

builder.Services.AddDbContext<FieldRecordingContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:FieldRecordingConnection").Get<string>());
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //options.SuppressConsumesConstraintForFormFileParameters = true;
    //options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressModelStateInvalidFilter = true;
});

//builder.Services.AddDbContext<SiisUatContext>(opt =>
//{
//    opt.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:SIIS").Get<string>());
//});

//Add compression

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});



//End Add compression ///////////////////////////////////////////////////////////////

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Field Recording API");
    });
}

//app.UseMiddleware<TraceIdMiddleware>();

app.UseResponseCompression();

app.UseProblemDetails();

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
