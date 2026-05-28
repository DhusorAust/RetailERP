using RetailERP.Infrastructure.Chem.Interfaces.ChemicalStore;
using RetailERP.Infrastructure.Chem.Interfaces.Common;
using RetailERP.Infrastructure.Chem.Interfaces.ExcelExport;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Infrastructure.Chem.Interfaces.RnD;
using RetailERP.Infrastructure.Chem.Interfaces.Security;
using RetailERP.Infrastructure.Chem.Interfaces.Setting;
using RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore;
using RetailERP.Infrastructure.Chem.Services.ChemicalStore;
using RetailERP.Infrastructure.Chem.Services.Common;
using RetailERP.Infrastructure.Chem.Services.ExcelExport;
using RetailERP.Infrastructure.Chem.Services.Marketing;
using RetailERP.Infrastructure.Chem.Services.RnD;
using RetailERP.Infrastructure.Chem.Services.Security;
using RetailERP.Infrastructure.Chem.Services.Setting;
using RetailERP.Infrastructure.Chem.Services.Setting.Store.ChemicalStore;
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

            #region Setting
            
            services.AddScoped<IINCIMasterService, INCIMasterService>(); 
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<IBankBranchService, BankBranchService>();
            services.AddScoped<IBuyerBankAccountService, BuyerBankAccountService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IProductMasterService, ProductMasterService>();
            services.AddScoped<IIngredientMasterService, IngredientMasterService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IUOMService, UOMService>();
            services.AddScoped<ISeasonService, SeasonService>();

            #endregion Setting   

            #region Chemical Store  
            services.AddScoped<IRequisitionService, RequisitionService>();
            services.AddScoped<IIndentService, IndentService>();
            services.AddScoped<IRequirementService, RequirementService>();
            services.AddScoped<IReceiveService, ReceiveService>();
            services.AddScoped<IIssueService, IssueService>();

            #endregion Setting

            #region Marketing  
            services.AddScoped<IBuyerRefService, BuyerRefService>();
            services.AddScoped<ISDRFService, SDRFService>();
            services.AddScoped<IPOEntryService, POEntryService>();
            services.AddScoped<IBriefTemplateService, BriefTemplateService>();
            #endregion Marketing

            #region RnD  
            services.AddScoped<IStabilityTestService, StabilityTestService>();
            services.AddScoped<IFormulationService, FormulationService>();
            services.AddScoped<IFeedbackService, FeedbackService>();

            #endregion RnD

            #region Production
            services.AddScoped<ICMPD_ProductionEntry, CMPD_ProductionEntry>();
            services.AddScoped<IFILL_ProductionEntryService, FILL_ProductionEntryService>();


            services.AddScoped<IPACK_ProductionEntryService, PACK_ProductionEntryService>();
            #endregion
            #region SMS

            #endregion

            #region Excel Export
            services.AddScoped<IExcelExportService, ExcelExportService>();
            #endregion

          
        } 
    }
}
