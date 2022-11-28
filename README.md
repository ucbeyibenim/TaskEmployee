# Introduction

TaskEmployee is small web site, that is created using asp.net core mvc technology. The main goal of the project is integration with external party.


The web site has a home page, create page, edit page and view page. The home page contains:

1) Browse File control
2) button to execute the import 
3) grid/table (described below).



User can select the csv file from external system by using Browse File control.


When user selects the file and clicks on Import button the program should parse the file, get data and insert to the database. The page reports on how many rows were successfully processed.


We can see the added rows on the grid page.


The data will be sorted by surname ascending in the grid table when we open the page.


Grdid has sorting, searching and editing functionality.


Bootstrap 5 was used  for the grid.



The project is divided into 2 parts:

1) TaskEmployee
2) TaskEmployee.DataAccess

# TaskEmployee:

This part contains the main settings of the project:

Controllers
Properties
ViewModels
Add project files.-
wwwroot
Program.cs
appsettings.Development.json
appsettings.json




These packages are used in this part:
      <Project Sdk="Microsoft.NET.Sdk.Web">

        <PropertyGroup>
          <TargetFramework>net6.0</TargetFramework>
          <Nullable>enable</Nullable>
          <ImplicitUsings>enable</ImplicitUsings>
        </PropertyGroup>

        <ItemGroup>
          <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="7.0.0">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
          </PackageReference>
          <PackageReference Include="PagedList" Version="1.17.0" />
          <PackageReference Include="PagedList.Mvc" Version="4.5.0" />
        </ItemGroup>

        <ItemGroup>
          <ProjectReference Include="..\TaskEmployee.DataAcess\TaskEmployee.DataAcess.csproj" />
        </ItemGroup>

      </Project>







# appsettings.json:        

        {
          "Logging": {
            "LogLevel": {
              "Default": "Information",
              "Microsoft.AspNetCore": "Warning"
            }
          },
          "AllowedHosts": "*",
          "ConnectionStrings": {
          "TaskEmployeeDb": "server=DESKTOP-5ROIMS8\\SQLEXPRESS;database=TaskEmployee;Trusted_connection=true; Encrypt=false"
          }
        }



The part that needs to be changed:

                          "ConnectionStrings": {
                           "TaskEmployeeDb": "server=CHANGE\\SQLEXPRESS;database=CHANGEDBNAME;Trusted_connection=true; Encrypt=false"
                          }




# Program.cs:
                using Microsoft.EntityFrameworkCore;
                using TaskEmployee.DataAcess;
                using TaskEmployee.DataAcess.Models;
                using TaskEmployee.Models;

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddControllersWithViews();
                builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
                builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TaskEmployeeDb")));
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();



This is our connection string:

                options.UseSqlServer(builder.Configuration.GetConnectionString("TaskEmployeeDb")));





# TaskEmployee.DataAccess:
This TaskEmployee.DataAccess part only works with the database

In this part we have:
1) Migrations
    1) Employee.cs
    2) EmployeeRepository.cs
    3) ErrorViewModel.cs
    4) IEmployeeRepository.cs
    5) PageInfo.cs
    6) PagingHtmlHelpers.cs
    7) UploadedDataInfo.cs
2) Models
3) AppDbContext.cs



These packages are used in this part:
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <TargetFramework>net6.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
          </PropertyGroup>

          <ItemGroup>
            <PackageReference Include="CsvHelper" Version="30.0.1" />
            <PackageReference Include="EPPlus" Version="6.1.1" />
            <PackageReference Include="Microsoft.AspNetCore.Html.Abstractions" Version="2.2.0" />
            <PackageReference Include="Microsoft.AspNetCore.Http.Features" Version="5.0.17" />
            <PackageReference Include="Microsoft.AspNetCore.Mvc.ViewFeatures" Version="2.2.0" />
            <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.0" />
            <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.0">
              <PrivateAssets>all</PrivateAssets>
              <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
            </PackageReference>
          </ItemGroup>

        </Project>
        
        
        
        
Package Csv is for working with csv files:

                  <PackageReference Include="CsvHelper" Version="30.0.1" />
  
 
 
 Package Epplus is for working with xlsx files. 
 
                        <PackageReference Include="EPPlus" Version="6.1.1" />


For working with Database:

                <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.0" />
                <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.0">
                
                
