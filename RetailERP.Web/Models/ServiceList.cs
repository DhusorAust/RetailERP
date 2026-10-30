
using RetailERP.Infrastructure.Chem.Interfaces.Common;
using RetailERP.Infrastructure.Chem.Interfaces.ExcelExport;
using RetailERP.Infrastructure.Chem.Interfaces.Security;
using RetailERP.Infrastructure.Chem.Services.Common;
using RetailERP.Infrastructure.Chem.Services.ExcelExport;
using RetailERP.Infrastructure.Chem.Services.Security;
using Microsoft.Extensions.DependencyInjection;


namespace RetailERP.Web.Models
{
    public class ServiceList
    {
        public static void RegisterServiceList(IServiceCollection services)
        {
            #region
            services.AddScoped<IHelperService, HelperService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            #endregion

            #region Security
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUserListService, UserListService>();
            services.AddScoped<IMenuCreateService, MenuCreateService>();
            services.AddScoped<IMenuPermissionRoleService, MenuPermissionRoleService>();
            services.AddScoped<IMenuPermissionUserService, MenuPermissionUserService>();
            services.AddScoped<IMenuReportCreateService, MenuReportCreateService>();
            services.AddScoped<IReportPermissionRoleService, ReportPermissionRoleService>();
            services.AddScoped<IReportPermissionUserService, ReportPermissionUserService>();
            services.AddScoped<IChangePasswordService, ChangePasswordService>();
            services.AddScoped<IUserViewPermissionService, UserViewPermissionService>();
            #endregion Security
           
         
            #region Excel Export
            services.AddScoped<IExcelExportService, ExcelExportService>();
            #endregion

          
        } 
    }
}
