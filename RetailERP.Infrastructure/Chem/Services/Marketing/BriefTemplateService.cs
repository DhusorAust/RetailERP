using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Marketing
{
    public class BriefTemplateService : IBriefTemplateService
    {
        private readonly DapperContext<Mkt_BriefTemplateMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public BriefTemplateService(DapperContext<Mkt_BriefTemplateMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"
           Select id=BrandID,text= BrandName from bas_Brand where IsActive=1
            Select id=ProductID,text=ProductName from bas_CS_ProductMaster where IsActive=1
            Select id=ProductTypeID,text=ProductTypeName from bas_CS_ProductType where IsActive=1
            Select id=ServiceOptionID,text=ServiceOption from bas_ServiceOption where IsActive=1
            Select id=TextureID,text=TextureName from bas_Texture where IsActive=1
            Select id=ColorID,text=ColorName from bas_Color where IsActive=1
            Select id=FragranceID,text=FragranceName from bas_Fragrance where IsActive=1
            Select id=GenderID,text=GenderName from bas_Gender where IsActive=1
            Select id=CountryID,text=CountryName from bas_Country where IsActive=1
            Select id=IngredientID,text=IngredientName,valueA=IngredientCode from itm_IngredientMaster where IsActive=1
            Select Top 10 id=ei.EmployeeID,text=CONCAT(ei.EmployeeNo, '-', ei.EmployeeName),valueA=ei.DepartmentID,valueB=ei.SectionID,valueC=ei.SubSectionID,
                valueD=d.DepartmentName,valueE=s.SectionName,valueF=ss.SubSectionName
            From {DBSource.PDLHRM}..emp_EmploymentInfo ei
                Left Join {DBSource.PDLHRM}..bas_Department d On d.DepartmentID = ei.DepartmentID
                Left Join {DBSource.PDLHRM}..bas_Section s On s.SectionID = ei.SectionID
                Left Join {DBSource.PDLHRM}..bas_SubSection ss On ss.SubSectionID = ei.SubSectionID
            Where ei.EmpStatusID = 1 AND ei.UnitID = 2
            Order By CONCAT(ei.EmployeeNo, '-', ei.EmployeeName);

           
            Select UOMID As id, UOM As [text] From bas_UOM Where IsActive = 1 AND Approved = 1;
            Select PackagingLevelID As id, PackagingLevelName As [text] From bas_PackagingLevel Where IsActive = 1;
            Select PackagingTypeID As id, PackagingName As [text] From bas_PackagingType Where IsActive = 1;
            Select MaterialID As id, MaterialName As [text] From bas_Material Where IsActive = 1;
            Select FinishID As id, FinishName As [text] From bas_Finish Where IsActive = 1;
            ";
        public async Task<GridEntity<Mkt_BriefTemplateMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "BTMasterID Desc";
            string condition = "";

            if (status == Status.EDIT)
            {
                condition = $@" AND ISNULL(brm.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition = $@" AND ISNULL(brm.Approved,0) = 1 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" AND 1 = 0 ";
            }
            else
            {
                condition = $@" ";
            }

            sql = $@"
                Select 
	                brm.*, b.BrandName, pm.ProductName, pt.ProductTypeName
                From 
	                Mkt_BriefTemplateMaster brm 
	                Left Join bas_Brand b On b.BrandID = brm.BrandID 
	                Left Join bas_CS_ProductMaster pm On pm.ProductID = brm.ProductID 
	                Left Join bas_CS_ProductType pt On pt.ProductTypeID = brm.ProductTypeID 
                WHERE  
                    ISNULL(brm.IsActive,0) = 1 {condition} ";

            return await _context.GetKendoGridDataAsync<Mkt_BriefTemplateMaster>(options, sql, orderBy, "");
        }

        public async Task<Mkt_BriefTemplateMaster> GetNew()
        {
            var query = $@" {CommonQuery}; ";
            var data = new Mkt_BriefTemplateMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);

                data.BrandList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.ProductTypeList = records.Read<DropDownList>().ToList();
                data.ServiceOptionList = records.Read<DropDownList>().ToList();
                data.TextureTypeList = records.Read<DropDownList>().ToList();
                data.ColorIList = records.Read<DropDownList>().ToList();
                data.FragranceList = records.Read<DropDownList>().ToList();
                data.GenderList = records.Read<DropDownList>().ToList();
                data.CountryList = records.Read<DropDownList>().ToList();
                data.IngredientList = records.Read<DropDownList>().ToList();
                data.EmployeeList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();
                data.PakagingLevelList = records.Read<DropDownList>().ToList();
                data.PackagingTypeList = records.Read<DropDownList>().ToList();
                data.MaterialTypeList = records.Read<DropDownList>().ToList();
                data.FinishTypeList = records.Read<DropDownList>().ToList();

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }

        public async Task<Mkt_BriefTemplateMaster> GetEdit(int id)
        {
            var query =
               $@"
                -- Master
                SELECT 
                    brm.*, b.BrandName, pm.ProductName, pt.ProductTypeName
                FROM 
                    Mkt_BriefTemplateMaster brm
                    Left Join bas_Brand b On b.BrandID = brm.BrandID
                    Left Join bas_CS_ProductMaster pm On pm.ProductID = brm.ProductID
                    Left Join bas_CS_ProductType pt On pt.ProductTypeID = brm.ProductTypeID
                WHERE brm.BTMasterID = {id};

                Select * From Mkt_BriefTemplateIngredient Where BTMasterID = {id} And IsNull(IsDeleted,0) = 0;
                Select * From Mkt_BriefTemplateReference Where BTMasterID = {id} And IsNull(IsDeleted,0) = 0;
                Select * From Mkt_BriefTemplateComplaince Where BTMasterID = {id} And IsNull(IsDeleted,0) = 0;
                  Select a.*, ProductTestName=pt.ProductTestName From Mkt_BriefTemplateProductTest a
                 left join bas_ProductTest pt on pt.ProductTestID = a.ProductTestID
                    Where a.BTMasterID = {id} And IsNull(a.IsDeleted,0) = 0;
                Select * From Mkt_BriefTemplateConsumer Where BTMasterID = {id} And IsNull(IsDeleted,0) = 0;
                Select * From Mkt_BriefTemplatePackaging Where BTMasterID = {id} And IsNull(IsDeleted,0) = 0;

                {CommonQuery}; ";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Mkt_BriefTemplateMaster data = await records.ReadFirstOrDefaultAsync<Mkt_BriefTemplateMaster>();
                data.IngredientChilds = records.Read<Mkt_BriefTemplateIngredient>().ToList();
                data.ReferenceChilds = records.Read<Mkt_BriefTemplateReference>().ToList();
                data.ComplainceChilds = records.Read<Mkt_BriefTemplateComplaince>().ToList();
                data.ProductTestChilds = records.Read<Mkt_BriefTemplateProductTest>().ToList();
                data.ConsumerChilds = records.Read<Mkt_BriefTemplateConsumer>().ToList();
                data.PackagingChilds = records.Read<Mkt_BriefTemplatePackaging>().ToList();
                data.BrandList = records.Read<DropDownList>().ToList();
                data.ProductList = records.Read<DropDownList>().ToList();
                data.ProductTypeList = records.Read<DropDownList>().ToList();
                data.ServiceOptionList = records.Read<DropDownList>().ToList();
                data.TextureTypeList = records.Read<DropDownList>().ToList();
                data.ColorIList = records.Read<DropDownList>().ToList();
                data.FragranceList = records.Read<DropDownList>().ToList();
                data.GenderList = records.Read<DropDownList>().ToList();
                data.CountryList = records.Read<DropDownList>().ToList();
                data.IngredientList = records.Read<DropDownList>().ToList();
                data.EmployeeList = records.Read<DropDownList>().ToList();
                data.UOMList = records.Read<DropDownList>().ToList();
                data.PakagingLevelList = records.Read<DropDownList>().ToList();
                data.PackagingTypeList = records.Read<DropDownList>().ToList();
                data.MaterialTypeList = records.Read<DropDownList>().ToList();
                data.FinishTypeList = records.Read<DropDownList>().ToList();
                await EnsureProjectLeaderInList(data.EmployeeList, data.ProjectLeaderBy);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }

        public async Task<List<DropDownList>> GetEmployeeList(string searchKey, int employeeId)
        {
            searchKey = (searchKey ?? string.Empty).Trim();
            var searchText = $"%{searchKey}%";
            var shouldSearch = string.IsNullOrEmpty(searchKey) || searchKey.Length >= 4;

            string query = $@"
                Select Top 10 id=ei.EmployeeID,text=CONCAT(ei.EmployeeNo, '-', ei.EmployeeName),valueA=ei.DepartmentID,valueB=ei.SectionID,valueC=ei.SubSectionID,
                    valueD=d.DepartmentName,valueE=s.SectionName,valueF=ss.SubSectionName
                From {DBSource.PDLHRM}..emp_EmploymentInfo ei
                    Left Join {DBSource.PDLHRM}..bas_Department d On d.DepartmentID = ei.DepartmentID
                    Left Join {DBSource.PDLHRM}..bas_Section s On s.SectionID = ei.SectionID
                    Left Join {DBSource.PDLHRM}..bas_SubSection ss On ss.SubSectionID = ei.SubSectionID
                Where ei.EmpStatusID = 1 AND ei.UnitID = 2
                    And (
                        @ShouldSearch = 0
                        Or @SearchKey = ''
                        Or ei.EmployeeNo Like @SearchText
                        Or ei.EmployeeName Like @SearchText
                    )
                Order By CONCAT(ei.EmployeeNo, '-', ei.EmployeeName);";

            try
            {
                await _connection.OpenAsync();
                var data = (await _connection.QueryAsync<DropDownList>(query, new
                {
                    SearchKey = searchKey,
                    SearchText = searchText,
                    ShouldSearch = shouldSearch ? 1 : 0
                })).ToList();

                await EnsureProjectLeaderInList(data, employeeId);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }

        private async Task EnsureProjectLeaderInList(List<DropDownList> employeeList, int employeeId)
        {
            if (employeeId <= 0 || employeeList.Any(x => x.id == employeeId)) return;

            string query = $@"
                Select Top 1 id=ei.EmployeeID,text=CONCAT(ei.EmployeeNo, '-', ei.EmployeeName),valueA=ei.DepartmentID,valueB=ei.SectionID,valueC=ei.SubSectionID,
                    valueD=d.DepartmentName,valueE=s.SectionName,valueF=ss.SubSectionName
                From {DBSource.PDLHRM}..emp_EmploymentInfo ei
                    Left Join {DBSource.PDLHRM}..bas_Department d On d.DepartmentID = ei.DepartmentID
                    Left Join {DBSource.PDLHRM}..bas_Section s On s.SectionID = ei.SectionID
                    Left Join {DBSource.PDLHRM}..bas_SubSection ss On ss.SubSectionID = ei.SubSectionID
                Where ei.EmployeeID = @EmployeeID;";

            var selectedEmployee = await _connection.QueryFirstOrDefaultAsync<DropDownList>(query, new { EmployeeID = employeeId });
            if (selectedEmployee != null)
            {
                employeeList.Insert(0, selectedEmployee);
            }
        }
        public async Task<Mkt_BriefTemplateMaster> SaveAsync(Mkt_BriefTemplateMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                var ingredientData = CommonFunction.ToDataSet(GetIngredientChilds(entity.IngredientChilds ?? new List<Mkt_BriefTemplateIngredient>()))?.GetXml();
                var referenceData = CommonFunction.ToDataSet(GetReferenceChilds(entity.ReferenceChilds ?? new List<Mkt_BriefTemplateReference>()))?.GetXml();
                var complainceData = CommonFunction.ToDataSet(GetComplainceChilds(entity.ComplainceChilds ?? new List<Mkt_BriefTemplateComplaince>()))?.GetXml();
                var productTestData = CommonFunction.ToDataSet(GetProductTestChilds(entity.ProductTestChilds ?? new List<Mkt_BriefTemplateProductTest>()))?.GetXml();
                var consumerData = CommonFunction.ToDataSet(GetConsumerChilds(entity.ConsumerChilds ?? new List<Mkt_BriefTemplateConsumer>()))?.GetXml();
                var packagingData = CommonFunction.ToDataSet(GetPackagingChilds(entity.PackagingChilds ?? new List<Mkt_BriefTemplatePackaging>()))?.GetXml();

                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@BTMasterID", entity.BTMasterID);
                parameters.Add("@ProjectName", entity.ProjectName);
                parameters.Add("@BrandID", entity.BrandID);
                parameters.Add("@ProjectLeaderBy", entity.ProjectLeaderBy);
                parameters.Add("@CompletionDate", entity.CompletionDate);
                parameters.Add("@ProductID", entity.ProductID);
                parameters.Add("@OrderQuantity", entity.OrderQuantity);
                parameters.Add("@TargetPerQtyCost", entity.TargetPerQtyCost);
                parameters.Add("@ServiceOptionID", entity.ServiceOptionID);
                parameters.Add("@MaxBulkPrice", entity.MaxBulkPrice);
                parameters.Add("@ProductTypeID", entity.ProductTypeID);
                parameters.Add("@ProductPerformance", entity.ProductPerformance);
                parameters.Add("@TextureID", entity.TextureID);
                parameters.Add("@ColorID", entity.ColorID);
                parameters.Add("@FragranceID", entity.FragranceID);
                parameters.Add("@ProductClaims", entity.ProductClaims);
                parameters.Add("@AdditionalComments", entity.AdditionalComments);
                parameters.Add("@Certification", entity.Certification);
                parameters.Add("@IsActive", entity.IsActive == 0 ? 1 : entity.IsActive);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@dsxmlu1", ingredientData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu2", referenceData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu3", complainceData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu4", productTestData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu5", consumerData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu6", packagingData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_Mkt_BriefTemplateMaster", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");

                if (entity.ResultId > 0 && entity.ErrorNo == 0)
                {
                    entity.Message = string.IsNullOrWhiteSpace(entity.Message) ? "Save operation successful." : entity.Message;
                }
                else
                {
                    entity.Message = string.IsNullOrWhiteSpace(entity.Message) ? "Save operation failed." : entity.Message;
                }
            }
            catch (Exception ex)
            {
                entity.Message = ex.Message;
            }
            finally
            {
                parameters = null;
            }
            return entity;
        }

        public List<TT_Mkt_BriefTemplateIngredient> GetIngredientChilds(List<Mkt_BriefTemplateIngredient> childs)
        {
            List<TT_Mkt_BriefTemplateIngredient> data = new List<TT_Mkt_BriefTemplateIngredient>();
            foreach (Mkt_BriefTemplateIngredient item in childs)
            {
                data.Add(new TT_Mkt_BriefTemplateIngredient
                {
                    BTIngredientID = item.BTIngredientID,
                    BTMasterID = item.BTMasterID,
                    IngredientID = item.IngredientID,
                    IngredientRequirementID = item.IngredientRequirementID,
                    RequirmentLevel = item.RequirmentLevel,
                    Remarks = item.Remarks,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                });
            }
            return data;
        }

        public List<TT_Mkt_BriefTemplateReference> GetReferenceChilds(List<Mkt_BriefTemplateReference> childs)
        {
            List<TT_Mkt_BriefTemplateReference> data = new List<TT_Mkt_BriefTemplateReference>();
            foreach (Mkt_BriefTemplateReference item in childs)
            {
                data.Add(new TT_Mkt_BriefTemplateReference
                {
                    BTReferenceID = item.BTReferenceID,
                    BTMasterID = item.BTMasterID,
                    ReferenceProductName = item.ReferenceProductName,
                    ReferenceBrand = item.ReferenceBrand,
                    ReferenceLink = item.ReferenceLink,
                    Remarks = item.Remarks,
                    IsActive = item.IsActive == 0 ? 1 : item.IsActive,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                });
            }
            return data;
        }

        public List<TT_Mkt_BriefTemplateComplaince> GetComplainceChilds(List<Mkt_BriefTemplateComplaince> childs)
        {
            List<TT_Mkt_BriefTemplateComplaince> data = new List<TT_Mkt_BriefTemplateComplaince>();
            foreach (Mkt_BriefTemplateComplaince item in childs)
            {
                data.Add(new TT_Mkt_BriefTemplateComplaince
                {
                    BTComplainceID = item.BTComplainceID,
                    BTMasterID = item.BTMasterID,
                    CountryID = item.CountryID,
                    IsDeleted = item.IsDeleted
                });
            }
            return data;
        }

        public List<TT_Mkt_BriefTemplateProductTest> GetProductTestChilds(List<Mkt_BriefTemplateProductTest> childs)
        {
            List<TT_Mkt_BriefTemplateProductTest> data = new List<TT_Mkt_BriefTemplateProductTest>();
            foreach (Mkt_BriefTemplateProductTest item in childs)
            {
                data.Add(new TT_Mkt_BriefTemplateProductTest
                {
                    BTProductTestID = item.BTProductTestID,
                    BTMasterID = item.BTMasterID,
                    ProductTestID = item.ProductTestID,
                    IsTested = item.IsTested,
                    Remarks = item.Remarks,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                });
            }
            return data;
        }

        public List<TT_Mkt_BriefTemplateConsumer> GetConsumerChilds(List<Mkt_BriefTemplateConsumer> childs)
        {
            List<TT_Mkt_BriefTemplateConsumer> data = new List<TT_Mkt_BriefTemplateConsumer>();
            foreach (Mkt_BriefTemplateConsumer item in childs)
            {
                data.Add(new TT_Mkt_BriefTemplateConsumer
                {
                    BTConsumerID = item.BTConsumerID,
                    BTMasterID = item.BTMasterID,
                    GenderID = item.GenderID,
                    MinAge = item.MinAge,
                    MaxAge = item.MaxAge,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                });
            }
            return data;
        }

        public List<TT_Mkt_BriefTemplatePackaging> GetPackagingChilds(List<Mkt_BriefTemplatePackaging> childs)
        {
            List<TT_Mkt_BriefTemplatePackaging> data = new List<TT_Mkt_BriefTemplatePackaging>();
            foreach (Mkt_BriefTemplatePackaging item in childs)
            {
                data.Add(new TT_Mkt_BriefTemplatePackaging
                {
                    BTPackagingID = item.BTPackagingID,
                    BTMasterID = item.BTMasterID,
                    PakagingLevelID = item.PakagingLevelID,
                    PackagingTypeID = item.PackagingTypeID,
                    PackagingStyle = item.PackagingStyle,
                    PackageSize = item.PackageSize,
                    UOMID = item.UOMID,
                    MaterialID = item.MaterialID,
                    FinishID = item.FinishID,
                    PackagingColorID = item.PackagingColorID,
                    PrintColorID = item.PrintColorID,
                    LabelDescription = item.LabelDescription,
                    DesignDescription = item.DesignDescription,
                    Certification = item.Certification,
                    Remarks = item.Remarks,
                    FilePath = item.FilePath,
                    RowSlNo = item.RowSlNo,
                    IsDeleted = item.IsDeleted
                });
            }
            return data;
        }
    }
}
