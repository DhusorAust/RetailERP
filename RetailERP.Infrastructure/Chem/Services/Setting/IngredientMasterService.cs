using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Setting;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Setting
{
    public class IngredientMasterService : IIngredientMasterService
    { 
        private readonly DapperContext<Itm_IngredientMaster> _context;
        private readonly SqlConnection _connection;
        public IngredientMasterService(DapperContext<Itm_IngredientMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"";

        public async Task<GridEntity<Itm_IngredientMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = $@"";
            string orderBy = "IngredientID desc";

             
            if (status == Status.EDIT)
            {
                condition = @" Where isnull(A.Approved,0)=0 ";
            }
            else if (status == Status.APPROVED)
            {
                condition = @" Where isnull(A.Approved,0)=1 ";
            }

            sql = $@"
                Select A.IngredientID,A.IngredientCode,A.IngredientName,B.CategoryName,C.TypeName,A.IngredientStatus,A.NaturalIndexPct,A.MOQInKg,A.IsFragrance,A.IsAllergenApplicable,A.IsActive,A.Approved
                From itm_IngredientMaster A
                Left Join bas_IngredientCategory B on A.IngredientCategoryID=B.IngredientCategoryID
                Left Join bas_IngredientType C on A.IngredientTypeID=C.IngredientTypeID
                {condition}";

            return await _context.GetKendoGridDataAsync<Itm_IngredientMaster>(options, sql, orderBy, "");
        }

        public async Task<Itm_IngredientMaster> GetNew()
        {
            var query = $@"  
                        Select SupplierID as id, SupplierName as text from bas_Supplier;
                        Select FlagID as id, FlagName as text from ERP_FlagTable where FlagID in (1,2);
                        Select INCIID as id, INCIName as text,CASNo as valueA from bas_INCIMaster  where Approved=1 and IsAllergen=0; 
                        Select INCIID as id, INCIName as text from bas_INCIMaster  where Approved=1 and IsAllergen=1; 
                        Select SupplierID as id, SupplierName as text from bas_Supplier;
                        Select FlagID as id, FlagName as text from ERP_FlagTable where FlagID in (3,4);
                        Select CountryID as id, CountryName as text from bas_Country;";

            var data = new Itm_IngredientMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);

                data.IngredientManufacturerList = records.Read<DropDownList>().ToList();
                data.IngredientStatusList = records.Read<DropDownList>().ToList();
                data.INCIBreakDownList = records.Read<DropDownList>().ToList();
                data.AllergensBreakDownList = records.Read<DropDownList>().ToList();
                data.SupplierList = records.Read<DropDownList>().ToList();
                data.LeadTimeUnitList = records.Read<DropDownList>().ToList();
                data.CountryList = records.Read<DropDownList>().ToList(); 
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
                data.ErrNo = 1;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }

        public async Task<Itm_IngredientMaster> GetEdit(int id)
        {
            var query = $@"


                Select A.*,B.IngredientManufacturerIds
                from itm_IngredientMaster A 
                LEFT JOIN (
                            Select IngredientID,STRING_AGG(ManufacturerID,',')  as IngredientManufacturerIds 
                            from itm_IngredientManufacturer A 
                            where A.IngredientID={id} and A.IsDeleted=0
                            Group by A.IngredientID
                )B on A.IngredientID=B.IngredientID
                where A.IngredientID={id}; 

                Select B.*,C.INCIName 
                from itm_IngredientMaster A
                INNER JOIN itm_INCIBreakdown B on A.IngredientID=B.IngredientID and B.IsDeleted=0
                LEFT JOIN bas_INCIMaster C on B.INCIID=C.INCIID
                where a.IngredientID={id};

                Select B.*,C.INCIName  
                from itm_IngredientMaster A
                INNER JOIN itm_AllergensBreakdown B on A.IngredientID=B.IngredientID and B.IsDeleted=0
                LEFT JOIN bas_INCIMaster C on B.INCIID=C.INCIID
                where a.IngredientID={id};

                Select b.*,C.SupplierName 
                from itm_IngredientMaster A
                INNER JOIN itm_IngredientSupplier B on A.IngredientID=B.IngredientID and B.IsDeleted=0
                LEFT JOIN bas_Supplier C on B.SupplierID=C.SupplierID
                where a.IngredientID={id};

 
                Select b.*  
                from itm_IngredientMaster A
                INNER JOIN itm_IngredientManufacturer B on A.IngredientID=B.IngredientID and B.IsDeleted=0
                where a.IngredientID={id};


                Select B.*,C.CountryName 
                from itm_IngredientMaster A
                INNER JOIN itm_IngredientRegulatoryRestriction B on A.IngredientID=B.IngredientID and B.IsDeleted=0
                LEFT JOIN bas_Country C on B.CountryID=C.CountryID
                where a.IngredientID={id};  

                Select SupplierID as id, SupplierName as text from bas_Supplier;
                Select FlagID as id, FlagName as text from ERP_FlagTable where FlagID in (1,2);
                Select INCIID as id, INCIName as text from bas_INCIMaster  where Approved=1 and IsAllergen=0; 
                Select INCIID as id, INCIName as text from bas_INCIMaster  where Approved=1 and IsAllergen=1; 
                Select SupplierID as id, SupplierName as text from bas_Supplier;
                Select FlagID as id, FlagName as text from ERP_FlagTable where FlagID in (3,4);
                Select CountryID as id, CountryName as text from bas_Country;
                ";

            var data = new Itm_IngredientMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Itm_IngredientMaster>();
                 
                data.INCIBreakdowns = records.Read<Itm_INCIBreakdown>().ToList();
                data.AllergensBreakdowns = records.Read<Itm_AllergensBreakdown>().ToList();
                data.IngredientSuppliers = records.Read<Itm_IngredientSupplier>().ToList();
                data.IngredientManufacturers = records.Read<Itm_IngredientManufacturer>().ToList();
                data.IngredientRegulatoryRestrictions = records.Read<Itm_IngredientRegulatoryRestriction>().ToList();

                data.IngredientManufacturerList = records.Read<DropDownList>().ToList();
                data.IngredientStatusList = records.Read<DropDownList>().ToList();
                data.INCIBreakDownList = records.Read<DropDownList>().ToList();
                data.AllergensBreakDownList = records.Read<DropDownList>().ToList();
                data.SupplierList = records.Read<DropDownList>().ToList();
                data.LeadTimeUnitList = records.Read<DropDownList>().ToList();
                data.CountryList = records.Read<DropDownList>().ToList();



                foreach (Itm_INCIBreakdown items in data.INCIBreakdowns)
                {
                    items.objINCIBreakDownList.id = items.INCIID;
                    items.objINCIBreakDownList.text = items.INCIName; 
                     
                }

                foreach (Itm_AllergensBreakdown items in data.AllergensBreakdowns)
                {
                    items.objAllergensBreakDownList.id = items.INCIID;
                    items.objAllergensBreakDownList.text = items.INCIName;

                }

                foreach (Itm_IngredientSupplier items in data.IngredientSuppliers)
                {
                    items.objSupplierList.id = items.SupplierID;
                    items.objSupplierList.text = items.SupplierName;

                    items.objLeadTimeUnitList.id = items.LeadTimeUnitID;
                    items.objLeadTimeUnitList.text = items.LeadTimeUnit;
                }

                foreach (Itm_IngredientRegulatoryRestriction items in data.IngredientRegulatoryRestrictions)
                {
                    items.objCountryList.id = items.CountryID;
                    items.objCountryList.text = items.CountryName;

                } 
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
                data.ErrNo = 1;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }
        public async Task<Itm_IngredientMaster> GetDocumentList(int id, string idname)
        {
            var query = $@"


                Select A.*
                from itm_IngredientMaster A 
                
                where A.IngredientID={id}; 

                    Select * From itm_IngredientDocument where IngredientID={id} and DocumentType='{idname}'

                ";

            var data = new Itm_IngredientMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Itm_IngredientMaster>();
                 
                data.IngredientDocuments = records.Read<Itm_IngredientDocument>().ToList();
               
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
                data.ErrNo = 1;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }

        public async Task<Itm_IngredientMaster> SaveAsync(Itm_IngredientMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_Itm_INCIBreakdown> TTList1 = GetINCIBreakdown(entity.INCIBreakdowns);
                DataSet Childs1 = CommonFunction.ToDataSet(TTList1);
                var doChilds1 = Childs1 == null ? null : Childs1.GetXml();

                List<TT_Itm_AllergensBreakdown> TTList2 = GetAllergensBreakdown(entity.AllergensBreakdowns);
                DataSet Childs2 = CommonFunction.ToDataSet(TTList2);
                var doChilds2 = Childs2 == null ? null : Childs2.GetXml();

                List<TT_Itm_IngredientSupplier> TTList3 = GetIngredientSupplier(entity.IngredientSuppliers);
                DataSet Childs3 = CommonFunction.ToDataSet(TTList3);
                var doChilds3 = Childs3 == null ? null : Childs3.GetXml();

                //List<TT_Itm_IngredientManufacturer> TTList4 = GetIngredientManufacturer(entity.IngredientManufacturers);
                //DataSet Childs4 = CommonFunction.ToDataSet(TTList4);
                //var doChilds4 = Childs4 == null ? null : Childs4.GetXml();

                List<TT_Itm_IngredientRegulatoryRestriction> TTList5 = GetIngredientRegulatoryRestriction(entity.IngredientRegulatoryRestrictions);
                DataSet Childs5 = CommonFunction.ToDataSet(TTList5);
                var doChilds5 = Childs5 == null ? null : Childs5.GetXml();

               
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@dsxmlu1", doChilds1, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu2", doChilds2, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu3", doChilds3, DbType.Xml, ParameterDirection.Input);
              //  parameters.Add("@dsxmlu4", doChilds4, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@dsxmlu5", doChilds5, DbType.Xml, ParameterDirection.Input);
                parameters.Add("@IngredientID", entity.IngredientID);
                parameters.Add("@IngredientCode", entity.IngredientCode);
                parameters.Add("@IngredientName", entity.IngredientName);
                parameters.Add("@IngredientDescription", entity.IngredientDescription);
                parameters.Add("@IngredientCategoryID", entity.IngredientCategoryID);
                parameters.Add("@IngredientTypeID", entity.IngredientTypeID);
                parameters.Add("@NaturalIndexPct", entity.NaturalIndexPct);
                parameters.Add("@NaturalOriginIndexPct", entity.NaturalOriginIndexPct);
                parameters.Add("@OrganicIndexPct", entity.OrganicIndexPct);
                parameters.Add("@OrganicOriginIndexPct", entity.OrganicOriginIndexPct);
                parameters.Add("@MOQInKg", entity.MOQInKg);
                parameters.Add("@LeadTime", entity.LeadTime);
                parameters.Add("@LeadTimeUnit", entity.LeadTimeUnit);
                parameters.Add("@IngredientStatus", entity.IngredientStatus);
                parameters.Add("@IngredientManufacturerIds", entity.IngredientManufacturerIds);
                parameters.Add("@IsFragrance", entity.IsFragrance);
                parameters.Add("@IsAllergenApplicable", entity.IsAllergenApplicable);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_IngredientMaster", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = "Save operation failed.";
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

        public List<TT_Itm_INCIBreakdown> GetINCIBreakdown(List<Itm_INCIBreakdown> Childs)
        {
            var childList = new List<TT_Itm_INCIBreakdown>();
            foreach (var item in Childs)
            {
                var objItem = new TT_Itm_INCIBreakdown
                {
                    INCIBreakdownID = item.INCIBreakdownID,
                    INCIID = item.objINCIBreakDownList.id,
                    INCIPercentage = item.INCIPercentage,
                    Remarks = item.Remarks,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                };
                childList.Add(objItem);
            }
            return childList;
        }

        public List<TT_Itm_AllergensBreakdown> GetAllergensBreakdown(List<Itm_AllergensBreakdown> Childs)
        {
            var childList = new List<TT_Itm_AllergensBreakdown>();
            foreach (var item in Childs)
            {
                var objItem = new TT_Itm_AllergensBreakdown
                {
                    AllergenBreakdownID = item.AllergenBreakdownID,
                    INCIID = item.objAllergensBreakDownList.id, 
                    AllergenPercentage = item.AllergenPercentage,
                    Remarks = item.Remarks,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                };
                childList.Add(objItem);
            }
            return childList;
        }

        public List<TT_Itm_IngredientSupplier> GetIngredientSupplier(List<Itm_IngredientSupplier> Childs)
        {
            var childList = new List<TT_Itm_IngredientSupplier>();
            foreach (var item in Childs)
            {
                var objItem = new TT_Itm_IngredientSupplier
                {
                    IngredientSupplierID = item.IngredientSupplierID,
                    SupplierID = item.objSupplierList.id,
                    PricePerKg = item.PricePerKg,
                    MOQInKg = item.MOQInKg,
                    LeadTime = item.LeadTime,
                    LeadTimeUnitID = item.objLeadTimeUnitList.id, 
                    Remarks = item.Remarks,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                };
                childList.Add(objItem);
            }
            return childList;
        }

        //public List<TT_Itm_IngredientManufacturer> GetIngredientManufacturer(List<Itm_IngredientManufacturer> Childs)
        //{
        //    var childList = new List<TT_Itm_IngredientManufacturer>();
        //    foreach (var item in Childs)
        //    {
        //        var objItem = new TT_Itm_IngredientManufacturer
        //        {
        //            IngredientManufacturerID = item.IngredientManufacturerID,
        //            ManufacturerID = item.ManufacturerID,
        //            Remarks = item.Remarks,
        //            RowSLNo = item.RowSLNo,
        //            IsDeleted = item.IsDeleted
        //        };
        //        childList.Add(objItem);
        //    }
        //    return childList;
        //}

        public List<TT_Itm_IngredientRegulatoryRestriction> GetIngredientRegulatoryRestriction(List<Itm_IngredientRegulatoryRestriction> Childs)
        {
            var childList = new List<TT_Itm_IngredientRegulatoryRestriction>();
            foreach (var item in Childs)
            {
                var objItem = new TT_Itm_IngredientRegulatoryRestriction
                {
                    IngredientRegulatoryRestrictionID = item.IngredientRegulatoryRestrictionID,
                    CountryID = item.objCountryList.id, 
                    RestrictionNote = item.RestrictionNote,
                    RowSLNo = item.RowSLNo,
                    IsDeleted = item.IsDeleted
                };
                childList.Add(objItem);
            }
            return childList;
        }
 
    }
}
