using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager; 
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Infrastructure.Chem.Interfaces.RnD;
using Dapper;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.RnD
{   
    public class FormulationService : IFormulationService
    {
        private readonly DapperContext<RnD_FormulationMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public FormulationService(DapperContext<RnD_FormulationMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"            
            Select BrandID As id, BrandName As [text] From bas_Brand Where IsActive = 1 AND Approved = 1;
            Select UnitID As id, UnitName As [text] From bas_UnitInfo Where IsActive = 1;
            Select FlagID As id, FlagName As [text] From ERP_FlagTable Where IsActive = 1;
            Select ProductID As id, ProductName As [text] 
            From bas_CS_ProductMaster 
            Where IsActive = 1 AND Approved = 1 AND ItemTypeID = 3;
            Select UOMID As id, UOM As [text] From bas_UOM Where IsActive = 1 AND Approved = 1;

            -- Child
            Select StepID As id, StepNo As [text] From bas_Step Where IsActive = 1; ";
        public async Task<GridEntity<RnD_FormulationMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.EDIT)
            {
                condition = $@" And fm.Approved = 0 And fm.Reject = 0 ";
            } 
            else if (status == Status.APPROVED)
            {
                condition = $@" And fm.Approved = 1 And fm.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" And fm.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }
             
            sql = $@"
                Select 
	                fm.*, b.BrandName, u.UnitName, ft.FlagName As FormulationStatusName, pm.ProductName, uom.UOM,
                    (ei.EmployeeNo + ' - ' + ei.EmployeeName) FormulationByName,
                    (Case When fm.IsLocked = 1 Then 'Locked' Else 'Unlocked' End)LockedStatus
                From 
	                rnd_FormulationMaster fm
	                Inner Join bas_Brand b On b.BrandID = fm.BrandID
	                Inner Join bas_UnitInfo u On u.UnitID = fm.UnitID
	                Inner Join ERP_FlagTable ft On ft.FlagID = fm.FormulationStatus
	                Inner Join bas_CS_ProductMaster pm On pm.ProductID = fm.ProductID
	                Inner Join bas_UOM uom On uom.UOMID = fm.BatchUOMID
                    left Join ERPDB..emp_EmploymentInfo ei On ei.EmployeeID = fm.FormulationBy
                Where fm.IsActive = 1 {condition} "; 
                    
                orderBy = "FormulationMasterID Desc";
            
            return await _context.GetKendoGridDataAsync<RnD_FormulationMaster>(options, sql, orderBy, "");
        }
        public async Task<RnD_FormulationMaster> GetNew()
        {
            var query = $@"{CommonQuery} ";

            RnD_FormulationMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query); 
                if(data != null)
                {
                    data.BrandList = records.Read<DropDownList>().ToList();
                    data.UnitList = records.Read<DropDownList>().ToList();
                    data.FormulationStatusList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.BatchUOMList = records.Read<DropDownList>().ToList();
                    data.StepList = records.Read<DropDownList>().ToList();
                    //data.IngredientList = records.Read<DropDownList>().ToList(); 
                }                
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
        public async Task<RnD_FormulationMaster> GetEdit(int id)
        {
            var query = $@" 
                -- Master
                Select * From rnd_FormulationMaster Where FormulationMasterID = {id};   

                ---- Childs
                --SELECT 
                --    fcd.*
                --INTO #tmpFormulationChild
                --FROM 
                --    rnd_FormulationChild fcd
                --WHERE 
                --    fcd.FormulationMasterID = {id} And fcd.IsDeleted = 0;
                --
                --;WITH cteInciList AS(
                --    SELECT
                --        a.IngredientID, STRING_AGG(a.INCIName, ', ') AS INCIName
                --    FROM(
                --        SELECT DISTINCT
                --            ib.IngredientID, inci.INCIName
                --        FROM 
                --            itm_INCIBreakdown ib
                --            INNER JOIN bas_INCIMaster inci ON inci.INCIID = ib.INCIID
                --    ) a
                --    GROUP BY a.IngredientID
                --)
                --
                --SELECT
                --    im.IngredientID, im.IngredientName, im.IngredientCode, il.INCIName,
                --    s.SupplierName, isup.MOQInKg, isup.LeadTime, ltu.FlagName, isup.LeadTimeUnitID
                --INTO #tmpIngredientData
                --FROM 
                --    itm_IngredientMaster im
                --    LEFT JOIN itm_IngredientSupplier isup ON isup.IngredientID = im.IngredientID
                --    LEFT JOIN bas_Supplier s ON s.SupplierID = isup.SupplierID
                --    LEFT JOIN ERP_FlagTable ltu ON ltu.FlagID = isup.LeadTimeUnitID
                --    LEFT JOIN cteInciList il ON il.IngredientID = im.IngredientID
                --WHERE
                --    im.IsActive = 1 AND im.Approved = 1  
                --    AND EXISTS(
                --        SELECT 1 FROM #tmpFormulationChild fc WHERE im.IngredientID = fc.IngredientID
                --    )
                --
                --SELECT
	            --    fc.*, s.StepNo, im.IngredientID, im.IngredientName, im.IngredientCode, im.INCIName,
                --    im.SupplierName, im.MOQInKg, im.LeadTime, LeadTimeUnit=im.FlagName
                --FROM 
	            --    #tmpFormulationChild fc
	            --    Inner Join bas_Step s On s.StepID = fc.StepID 
	            --    Inner Join #tmpIngredientData im On im.IngredientID = fc.IngredientID;
                --
                --DROP TABLE #tmpFormulationChild;
                --DROP TABLE #tmpIngredientData;
                
               Select a.*,c.INCIName,IngredientName,IngredientCode,LeadTimeUnit=FlagName,s.StepNo
			   from rnd_FormulationChild a
			   inner join bas_Step s on s.StepID=a.StepID
			   inner join itm_IngredientMaster b on b.IngredientID=a.IngredientID
			   inner join ERP_FlagTable f on f.FlagID=a.LeadTimeUOMID
			   left join (select a.IngredientID,INCIName=STRING_AGG(c.INCIName,',') from itm_IngredientMaster a
						inner join itm_INCIBreakdown b on b.IngredientID=a.IngredientID and b.IsDeleted=0 
						inner join bas_INCIMaster c on c.INCIID=b.INCIID and c.IsActive=1
						where a.IsActive=1
						group by a.IngredientID) c on c.IngredientID=a.IngredientID
                Where a.FormulationMasterID={id} And a.IsDeleted=0
                Order by a.RowSLNo; 

                SELECT
                    fm.* 
                FROM 
                    rnd_FormulationMethod fm 
                WHERE 
                    fm.FormulationMasterID = {id} And fm.IsDeleted = 0
                Order by fm.RowSLNo;

                {CommonQuery} ";

            RnD_FormulationMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<RnD_FormulationMaster>();
                data.Childs = records.Read<RnD_FormulationChild>().ToList();
                data.ChildsMethod = records.Read<RnD_FormulationMethod>().ToList();

                if (data != null)
                {
                    data.BrandList = records.Read<DropDownList>().ToList();
                    data.UnitList = records.Read<DropDownList>().ToList();
                    data.FormulationStatusList = records.Read<DropDownList>().ToList();
                    data.ProductList = records.Read<DropDownList>().ToList();
                    data.BatchUOMList = records.Read<DropDownList>().ToList();
                    data.StepList = records.Read<DropDownList>().ToList();

                    data.Childs.ForEach(child =>
                    {
                        child.objStepList.id = child.StepID;
                        child.objStepList.text = child.StepNo;

                        child.objIngredientList.id = child.IngredientID;
                        child.objIngredientList.text = child.IngredientName;
                        child.objIngredientList.valueB = child.INCIName;
                        child.objIngredientList.valueA = child.IngredientCode;
                        child.objIngredientList.valueG = child.LeadTimeUOMID?.ToString() ?? "";
                        child.objIngredientList.valueF = child.LeadTimeUnit;

                        child.objLeadTimeUOMList.id = child.LeadTimeUOMID;
                        child.objLeadTimeUOMList.text = child.LeadTimeUnit;
                    });
                } 
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
        public async Task<RnD_FormulationMaster> SaveAsync(RnD_FormulationMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_RnD_FormulationChild> TTList = GetChild(entity.Childs);
                List<TT_RnD_FormulationMethod> TTListB = GetChildB(entity.ChildsMethod);

                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();

                DataSet DetailsDataB = CommonFunction.ToDataSet(TTListB);
                var doDetailsDataB = DetailsDataB == null ? null : DetailsDataB.GetXml();

                parameters = new DynamicParameters();

                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu2", doDetailsDataB, DbType.Xml, ParameterDirection.Input);

                parameters.Add("@FormulationMasterID", entity.FormulationMasterID);
                parameters.Add("@FormulationDate", entity.FormulationDate);
                parameters.Add("@UnitID", entity.UnitID);
                parameters.Add("@BrandID", entity.BrandID);
                parameters.Add("@ProductID", entity.ProductID);
                parameters.Add("@BatchQty", entity.BatchQty);
                parameters.Add("@BatchUOMID", entity.BatchUOMID);
                parameters.Add("@FormulationBy", entity.FormulationBy);
                parameters.Add("@TotalPercentage", entity.TotalPercentage);
                parameters.Add("@TotalFormulationPrice", entity.TotalFormulationPrice);
                parameters.Add("@TotalNaturalIndexPct", entity.TotalNaturalIndexPct);
                parameters.Add("@TotalNaturalOriginIndexPct", entity.TotalNaturalOriginIndexPct);
                parameters.Add("@TotalOrganicIndexPct", entity.TotalOrganicIndexPct);
                parameters.Add("@TotalOrganicOriginIndexPct", entity.TotalOrganicOriginIndexPct);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@FormulationStatus", entity.FormulationStatus);
                parameters.Add("@Appearance", entity.Appearance);
                parameters.Add("@Colour", entity.Colour);
                parameters.Add("@Odour", entity.Odour);
                parameters.Add("@Viscosity", entity.Viscosity);
                parameters.Add("@PH", entity.PH);
                parameters.Add("@SG", entity.SG);

                parameters.Add("@IsLocked", entity.IsLocked);
                parameters.Add("@IsActive", entity.IsActive);

                parameters.Add("@ReviseReason", entity.ReviseReason);
                parameters.Add("@RejectReason", entity.RejectReason);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@Revise", entity.Revise);

                parameters.Add("@UserBy", entity.UserBy);


                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_rnd_Formulation", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = entity.Message+" Save operation failed.";
                }
            }
            catch (Exception ex)
            {
                entity.Message = ex.InnerException.ToString();
            }
            finally
            {
                parameters = null;
            }
            return entity;
        } 
        public List<TT_RnD_FormulationChild> GetChild(List<RnD_FormulationChild> Childs)
        {
            var TTList = new List<TT_RnD_FormulationChild>();
            foreach (var item in Childs)
            {
                var objItem = new TT_RnD_FormulationChild
                {
                    FormulationChildID = item.FormulationChildID,
                    StepID = item.objStepList.id,
                    IngredientID = item.objIngredientList.id,
                    IngredientPercentage = item.IngredientPercentage,
                    IngredientQtyKg = item.IngredientQtyKg,
                    IngredientQtyGM = item.IngredientQtyGM,
                    IngredientCost = item.IngredientCost,
                    IngredientPricePerQtyUsed = item.IngredientPricePerQtyUsed,
                    MOQInKg = item.MOQInKg,
                    LeadTime = item.LeadTime,
                    LeadTimeUOMID = item.objLeadTimeUOMList.id,
                    NaturalIndexPct = item.NaturalIndexPct,
                    NaturalOriginIndexPct = item.NaturalOriginIndexPct,
                    OrganicIndexPct = item.OrganicIndexPct,
                    OrganicOriginIndexPct = item.OrganicOriginIndexPct,
                    RegulatoryWarning = item.RegulatoryWarning,
                    RowSLNo = item.RowSLNo
                };
                TTList.Add(objItem);
            }
            
            
            return TTList;
        }
        public List<TT_RnD_FormulationMethod> GetChildB(List<RnD_FormulationMethod> Childs)
        {
            var TTList = new List<TT_RnD_FormulationMethod>();
            foreach (RnD_FormulationMethod item in Childs)
            {
                var objItem = new TT_RnD_FormulationMethod
                {
                    FormulationMethodID = item.FormulationMethodID,
                    Instruction = item.Instruction,
                    InstructionRemarks = item.InstructionRemarks,
                    RowSLNo = item.RowSLNo
                };
                TTList.Add(objItem);
            }
            return TTList;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
