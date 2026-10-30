using RetailERP.Core.Common;
using RetailERP.Infrastructure.Chem.Interfaces.Common;
using ClosedXML.Excel;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Common
{
    public class HelperService : IHelperService
    {
        private readonly DapperContext<DropDownList> _context;
        private readonly SqlConnection _connection;

        private readonly DapperContext<AuditTrail> _contextArticle;
        public HelperService(DapperContext<DropDownList> context, DapperContext<AuditTrail> contextArticle)
        {
            _context = context;
            _connection = _context.Connection;
            _contextArticle = contextArticle;
        }
        //Insert Audit Trail
        public AuditTrail SaveAuditTrail(AuditTrail entity)
        {
            try
            {
                string strQuery = $@"
                Insert Into ERPDB..AuditTrail(AuditTrailDate, AuditTrailType, AuditTrailDescription, 
                UserID, ProjectID, MenuID, ViewPage)
                Values('{entity.AuditTrailDate}', '{entity.AuditTrailType}', '{entity.AuditTrailDescription}',
                {entity.UserID},{entity.ProjectID},{entity.MenuID}, '{entity.ViewPage}') ";
                SqlCommand cmd = new SqlCommand(strQuery, _connection);
                _connection.Open();
                int i = cmd.ExecuteNonQuery();
                _connection.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return entity;
        }

        #region
        public async Task<List<DropDownList>> GetIngredientBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        {
            var condition = "";
            var search = "";
            var orderBy = " ORDER BY a.text DESC";

            if (!string.IsNullOrEmpty(searchKey))
            {
                search = searchKey;
                condition = "  and (a.valueA like '%" + search + "%' or a.id like '%" + search + "%' or a.text like '%" + search + "%' or a.valueB like '%" + search + "%' or a.valueC like '%" + search + "%')";
            }
            else if (!string.IsNullOrEmpty(itemName))
            {
                search = itemName;
            }

            if (!string.IsNullOrEmpty(search))
            {
                orderBy = $@" ORDER BY (
                                CASE 
                                    WHEN a.text LIKE N'{search}' or a.id LIKE N'{search}' or a.valueB LIKE N'{search}' or a.valueC LIKE N'{search}' THEN 1 
                                    WHEN a.text LIKE N'%{search}' or a.id LIKE N'%{search}' or a.valueB LIKE N'%{search}' or a.valueC LIKE N'%{search}' THEN 2 
                                    WHEN a.text LIKE N'{search}%' or a.id LIKE N'{search}%' or a.valueB LIKE N'{search}%' or a.valueC LIKE N'{search}%' THEN 3 
                                    ELSE 4 
                                END
                            ), a.text DESC
                ";
            }

            var query = $@"
                               
                SELECT a.* FROM(
                    SELECT
                        im.IngredientID AS id, im.IngredientName AS [text], im.IngredientCode AS valueA, c.INCIName AS valueB,
                        s.SupplierName AS valueC, isup.MOQInKg AS valueD, isup.LeadTime AS valueE, ltu.FlagName AS valueF, 
                        isup.LeadTimeUnitID AS valueG
                        ,valueH=im.NaturalIndexPct
						,valueI=im.NaturalOriginIndexPct
						,valueJ=im.OrganicIndexPct
						,valueK=im.OrganicOriginIndexPct
                    FROM 
                         itm_IngredientMaster im
                        LEFT JOIN itm_IngredientSupplier isup ON isup.IngredientID = im.IngredientID
                        LEFT JOIN bas_Supplier s ON s.SupplierID = isup.SupplierID
                        LEFT JOIN ERP_FlagTable ltu ON ltu.FlagID = isup.LeadTimeUnitID
                         left join (select a.IngredientID,INCIName=STRING_AGG(c.INCIName,',') from itm_IngredientMaster a
						inner join itm_INCIBreakdown b on b.IngredientID=a.IngredientID and b.IsDeleted=0 
						inner join bas_INCIMaster c on c.INCIID=b.INCIID and c.IsActive=1
						where a.IsActive=1
						group by a.IngredientID) c on c.IngredientID=im.IngredientID
                    WHERE
                        im.IsActive = 1 AND im.Approved = 1  
                )a
                WHERE 1 = 1 {condition}
                {orderBy}
                OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY; ";

            var dataList = await _context.GetDataAsync<DropDownList>(query);
            return dataList;
        }
        #endregion


        #region Product
        public async Task<List<DropDownList>> GetAllProductBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        {
            var condition = "";
            var search = "";
            var orderBy = " Order By a.text \r\n";

            if (!string.IsNullOrEmpty(searchKey))
            {
                itemName = "";
                search = searchKey;

                condition = "  and (pm.ProductName like '%" + search + "%' or pm.ProductID like '%" + search + "%' or pc.ProductCategoryName like '%" + search + "%' or pt.ProductTypeName like '%" + search + "%')";
            }

            if (!string.IsNullOrEmpty(itemName))
            {
                search = itemName;
                orderBy = $@" Order By (
                                CASE 
                                    WHEN a.text LIKE N'{search}' or a.id LIKE N'{search}' or a.valueB LIKE N'{search}' or a.valueD LIKE N'{search}' THEN 1 
                                    WHEN a.text LIKE N'%{search}' or a.id LIKE N'%{search}' or a.valueB LIKE N'%{search}' or a.valueD LIKE N'%{search}' THEN 2 
                                    WHEN a.text LIKE N'{search}%' or a.id LIKE N'{search}%' or a.valueB LIKE N'{search}%' or a.valueD LIKE N'{search}%' THEN 3 
                                    ELSE 4 
                                END
                            )
                ";
            } 

            var query = $@"
                Select * From
                (
                    Select 
	                    pc.ProductCategoryID As valueA, pc.ProductCategoryName As valueB, pt.ProductTypeID As valueC, 
	                    pt.ProductTypeName As valueD, pm.ProductID As id, pm.ProductName As [text]
                    From 
				        bas_CS_ProductMaster pm
	                    join bas_CS_ProductCategory pc on pc.ProductCategoryID=pm.ProductCategoryID
	                    Inner Join bas_CS_ProductType pt On pt.ProductTypeID = pm.ProductTypeID
	                    --Inner Join bas_CS_ProductMaster pm On pm.ProductCategoryID = pc.ProductCategoryID AND pm.ProductTypeID = pt.ProductTypeID
                    Where 
	                    pm.IsActive = 1 AND pm.Approved = 1 {condition} 
				)a                
                {orderBy}
                OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY; ";

            var dataList = await _context.GetDataAsync<DropDownList>(query);
            return dataList;
        }
        #endregion Product  

        #region Others 
        public async Task<string> ExcelFileConvertBase64String(DataTable dataTable)
        {
            try
            {
                string base64String;
                using (var wb = new XLWorkbook())
                {
                    var sheet = wb.AddWorksheet(dataTable, "Employee Records");

                    // Apply font color to columns 1 to data count
                    sheet.Columns(1, dataTable.Columns.Count).Style.Font.FontColor = XLColor.Black;

                    // Adjust column widths based on content
                    sheet.Columns().AdjustToContents();

                    using (var ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);
                        // Convert the Excel workbook to a base64-encoded string
                        base64String = Convert.ToBase64String(ms.ToArray());
                    }
                }

                // Return a CreatedResult with the base64-encoded Excel data
                return base64String;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion Others   

        //    public async Task<List<DropDownList>> GetAllBuyerBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //    {
        //        var condition = "";
        //        var search = "";
        //        var orderBy = " order by a.text \r\n";
        //        if (!string.IsNullOrEmpty(searchKey))
        //        {
        //            itemName = "";
        //            search = searchKey;

        //            condition = "  and (c.BuyerName like '%" + search + "%' or c.BuyerID like '%" + search + "%' or c.BuyerAddress like '%" + search + "%' )";
        //        }

        //        if (!string.IsNullOrEmpty(itemName))
        //        {
        //            search = itemName;
        //            orderBy = $@" order by (
        //                            CASE 
        //                                WHEN a.text LIKE N'{search}' or a.id LIKE N'{search}' or a.valueA LIKE N'{search}' THEN 1 
        //                                WHEN a.text LIKE N'%{search}' or a.id LIKE N'%{search}' or a.valueA LIKE N'%{search}' THEN 2 
        //                                WHEN a.text LIKE N'{search}%' or a.id LIKE N'{search}%' or a.valueA LIKE N'{search}%' THEN 3 
        //                                ELSE 4 
        //                            END
        //                        )
        //            ";
        //        }


        //        var query =
        //        $@"
        //            select * from(
        //	SELECT 
        //		distinct c.BuyerID id, c.BuyerName [text], c.BuyerAddress valueA
        //	FROM   
        //		bas_BuyerNSupplier c
        //	WHERE 1=1 {condition}
        //)a                
        //            {orderBy}
        //            OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //        ";

        //        var dataList = await _context.GetDataAsync<DropDownList>(query);
        //        return dataList;
        //    }
        //    public async Task<List<DropDownList>> GetAllVehicleBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //    {
        //        var condition = "";
        //        var search = "";
        //        var orderBy = " order by a.text \r\n";
        //        if (!string.IsNullOrEmpty(searchKey))
        //        {
        //            itemName = "";
        //            search = searchKey;
        //            condition = "  and (c.VehicleName like '%" + search + "%' or c.VehicleID like '%" + search + "%' or c.MobileNo like '%" + search + "%' )";
        //        }
        //        if (!string.IsNullOrEmpty(itemName))
        //        {
        //            search = itemName;
        //        }

        //        if (!string.IsNullOrEmpty(search))
        //        {
        //            orderBy = $@" order by (
        //                            CASE 
        //                                WHEN a.text LIKE N'{search}' or a.id LIKE N'{search}' or a.valueA LIKE N'{search}' THEN 1 
        //                                WHEN a.text LIKE N'%{search}' or a.id LIKE N'%{search}' or a.valueA LIKE N'%{search}' THEN 2 
        //                                WHEN a.text LIKE N'{search}%' or a.id LIKE N'{search}%' or a.valueA LIKE N'{search}%' THEN 3 
        //                                ELSE 4 
        //                            END
        //                        )
        //            ";
        //        }


        //        var query =
        //        $@"
        //            select * from(
        //	select 
        //                 distinct id=VehicleID, [text]=VehicleName, ids=VehicleName, valueA=MobileNo, valueB=Descriptions
        //                from bas_Vehicle c
        //                where IsActive=1 {condition}
        //)a                
        //            {orderBy}
        //            OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //        ";

        //        var dataList = await _context.GetDataAsync<DropDownList>(query);
        //        return dataList;
        //    }
        //    public async Task<List<DropDownList>> GetAllTransportBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //    {
        //        var condition = "";
        //        var search = "";
        //        var orderBy = " order by a.text \r\n";
        //        if (!string.IsNullOrEmpty(searchKey))
        //        {
        //            itemName = "";
        //            search = searchKey;
        //            condition = "  and (c.TransportName like '%" + search + "%' or c.TransportID like '%" + search + "%' or c.TransportAddress like '%" + search + "%' )";
        //        }
        //        if (!string.IsNullOrEmpty(itemName))
        //        {
        //            search = itemName;
        //        }

        //        if (!string.IsNullOrEmpty(search))
        //        {
        //            orderBy = $@" order by (
        //                            CASE 
        //                                WHEN a.text LIKE N'{search}' or a.id LIKE N'{search}' or a.valueA LIKE N'{search}' THEN 1 
        //                                WHEN a.text LIKE N'%{search}' or a.id LIKE N'%{search}' or a.valueA LIKE N'%{search}' THEN 2 
        //                                WHEN a.text LIKE N'{search}%' or a.id LIKE N'{search}%' or a.valueA LIKE N'{search}%' THEN 3 
        //                                ELSE 4 
        //                            END
        //                        )
        //            ";
        //        }


        //        var query =$@"
        //                select * from(
        //        select distinct 
        //                        id=TransportID, [text]=TransportName, ids=TransportName, valueA=TransportAddress, valueB=MobileNo, valueC=[Zone]
        //                    from bas_Transport c
        //                    where IsActive=1 {condition}
        //                )a                
        //                {orderBy}
        //                OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;";

        //        var dataList = await _context.GetDataAsync<DropDownList>(query);
        //        return dataList;
        //    }       
        //    public async Task<List<DropDownList>> GetAllImpLCBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //    {
        //        var condition = "";
        //        var search = "";
        //        var orderBy = " order by a.text \r\n";
        //        if (!string.IsNullOrEmpty(searchKey))
        //        {
        //            itemName = "";
        //            search = searchKey;

        //            condition = "  and (c.BuyerName like '%" + search + "%' or c.BuyerID like '%" + search + "%' or c.BuyerAddress like '%" + search + "%' )";
        //        }

        //        if (!string.IsNullOrEmpty(itemName))
        //        {
        //            search = itemName;
        //            orderBy = $@" order by (
        //                            CASE 
        //                                WHEN a.text LIKE N'{search}' or a.id LIKE N'{search}' or a.valueA LIKE N'{search}' THEN 1 
        //                                WHEN a.text LIKE N'%{search}' or a.id LIKE N'%{search}' or a.valueA LIKE N'%{search}' THEN 2 
        //                                WHEN a.text LIKE N'{search}%' or a.id LIKE N'{search}%' or a.valueA LIKE N'{search}%' THEN 3 
        //                                ELSE 4 
        //                            END
        //                        )
        //            ";
        //        }


        //        var query =
        //        $@"
        //            select * from(
        //	SELECT 
        //		distinct c.BuyerID id, c.BuyerName [text], c.BuyerAddress valueA
        //	FROM   
        //		bas_BuyerNSupplier c
        //	WHERE 1=1 {condition}
        //)a                
        //            {orderBy}
        //            OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //        ";

        //        var dataList = await _context.GetDataAsync<DropDownList>(query);
        //        return dataList;
        //    }
        #region Employee Information 
        //public async Task<List<Emp_EmploymentInfo>> GetEmployeeByEmployeeID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string employeeID)
        //{
        //    var condition = "";
        //    var orderBy = " order by ei.EmployeeNo, ei.EmployeeName\r\n";
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        employeeName = "";
        //        condition = "  and (ei.EmployeeNo like '%" + searchKey + "%' or ei.EmployeeName like '%" + searchKey + "%' or ei.DepartmentName like '%" + searchKey + "%' or ei.DesignationName like '%" + searchKey + "%' or ei.SectionName like '%" + searchKey + "%')";
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{searchKey}' or ei.EmployeeName = N'{searchKey}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{searchKey}' or ei.EmployeeName = N'%{searchKey}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{searchKey}%' or ei.EmployeeName = N'{searchKey}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }
        //    else if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{employeeName}' or ei.EmployeeName = N'{employeeName}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{employeeName}' or ei.EmployeeName = N'%{employeeName}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}%' or ei.EmployeeName = N'{employeeName}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }

        //    if (employeeID != null)
        //    {
        //        condition += $@" And ei.EmployeeID In({employeeID})";
        //    }
        //    var query =
        //    $@"
        //        -- Details
        //        select 
        //         ei.EmployeeID, ei.EmployeeNo, ei.EmployeeName,EmployeeNameWithID='['+ ei.EmployeeNo +'] '+ ei.EmployeeName, ei.UnitFullName, ei.DepartmentID, ei.DepartmentName, 
        //            ei.DesignationID, ei.DesignationName, ei.SectionID, ei.SectionName, ei.SubSectionID, ei.SectionName
        //        from {DBSource.PDLHRM}..vw_EmployeeInfo ei
        //        where 1 = 1
        //        {condition}
        //        {orderBy}
        //        OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //    ";

        //    var dataList = await _context.GetDataAsync<Emp_EmploymentInfo>(query);
        //    return dataList;
        //}
        //public async Task<List<Emp_EmploymentInfo>> GetEmployeeByUnitID(string searchKey, int pageIndex, int pageSize, string employeeName, string unitID,string deptIds,string secIds,string empTypeIds)
        //{
        //    var condition = "";
        //    var orderBy = " order by ei.EmployeeNo, ei.EmployeeName\r\n";
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        employeeName = "";
        //        condition = "  and (ei.EmployeeNo LIKE '%" + searchKey + "%' or ei.EmployeeID LIKE '%" + searchKey + "%' or ei.EmployeeName LIKE '%" + searchKey + "%' or ei.DepartmentName LIKE '%" + searchKey + "%' or ei.DesignationName LIKE '%" + searchKey + "%' or ei.SectionName LIKE '%" + searchKey + "%')";
        //    }
        //    else if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}' or ei.EmployeeName LIKE N'{employeeName}' or ei.EmployeeID LIKE N'{employeeName}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{employeeName}' or ei.EmployeeName LIKE N'%{employeeName}' or ei.EmployeeID LIKE N'%{employeeName}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}%' or ei.EmployeeName LIKE N'{employeeName}%' or ei.EmployeeID LIKE N'{employeeName}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }

        //    if (!string.IsNullOrEmpty(unitID))
        //    {
        //        condition += $@" And ei.UnitID In({unitID})";
        //    }
        //    if (!string.IsNullOrEmpty(deptIds))
        //    {
        //        condition += $@" And ei.DepartmentID In({deptIds})";
        //    }
        //    if (!string.IsNullOrEmpty(secIds))
        //    {
        //        condition += $@" And ei.SectionID In({secIds})";
        //    }
        //    if (!string.IsNullOrEmpty(empTypeIds) && empTypeIds != "undefined")
        //    {
        //        condition += $@" And ei.EmployeeTypeID In({empTypeIds})";
        //    }

        //    var query =
        //    $@"
        //        -- Details
        //        select 
        //         ei.EmployeeID, ei.EmployeeNo, ei.EmployeeName,EmployeeNameWithID='['+ ei.EmployeeNo +'] '+ ei.EmployeeName, ei.UnitFullName, ei.DepartmentID, ei.DepartmentName, 
        //            ei.DesignationID, ei.DesignationName, ei.SectionID, ei.SectionName, ei.SubSectionID, ei.SectionName   
        //        from {DBSource.PDLHRM}..vw_EmployeeInfo ei
        //        where EmpStatusID = 1
        //        {condition}
        //        {orderBy}
        //        OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //    ";

        //    var dataList = await _context.GetDataAsync<Emp_EmploymentInfo>(query);
        //    return dataList;
        //}
        //public async Task<List<Emp_EmploymentInfo>> GetEmployeeByDepartmentID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string departmentID)
        //{
        //    var condition = "";
        //    var orderBy = " order by ei.EmployeeNo, ei.EmployeeName\r\n";
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        employeeName = "";
        //        condition = "  and (ei.EmployeeNo like '%" + searchKey + "%' or ei.EmployeeName like '%" + searchKey + "%' or ei.DepartmentName like '%" + searchKey + "%' or ei.DesignationName like '%" + searchKey + "%' or ei.SectionName like '%" + searchKey + "%')";
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{searchKey}' or ei.EmployeeName = N'{searchKey}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{searchKey}' or ei.EmployeeName = N'%{searchKey}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{searchKey}%' or ei.EmployeeName = N'{searchKey}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }
        //    else if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{employeeName}' or ei.EmployeeName = N'{employeeName}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{employeeName}' or ei.EmployeeName = N'%{employeeName}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}%' or ei.EmployeeName = N'{employeeName}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }

        //    if (departmentID != null)
        //    {
        //        condition += $@" And ei.DepartmentID In({departmentID})";
        //    }

        //    var query =
        //    $@"
        //        -- Details
        //        select 
        //         ei.EmployeeID, ei.EmployeeNo, ei.EmployeeName, ei.UnitFullName, ei.DepartmentID, ei.DepartmentName, 
        //            ei.DesignationID, ei.DesignationName, ei.SectionID, ei.SectionName, ei.SubSectionID, ei.SectionName
        //        from {DBSource.PDLHRM}..vw_EmployeeInfo ei
        //        where 1 = 1
        //        {condition}
        //        {orderBy}
        //        OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //    ";

        //    var dataList = await _context.GetDataAsync<Emp_EmploymentInfo>(query);
        //    return dataList;
        //}
        //public async Task<List<Emp_EmploymentInfo>> GetEmployeeByDepartmentNUnitID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string departmentID, string unitID)
        //{
        //    var condition = "";
        //    var orderBy = " order by ei.EmployeeNo, ei.EmployeeName\r\n";
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        employeeName = "";
        //        condition = "  and (ei.EmployeeNo like '%" + searchKey + "%' or ei.EmployeeName like '%" + searchKey + "%' or ei.DepartmentName like '%" + searchKey + "%' or ei.DesignationName like '%" + searchKey + "%' or ei.SectionName like '%" + searchKey + "%')";
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{searchKey}' or ei.EmployeeName = N'{searchKey}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{searchKey}' or ei.EmployeeName = N'%{searchKey}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{searchKey}%' or ei.EmployeeName = N'{searchKey}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }
        //    else if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{employeeName}' or ei.EmployeeName = N'{employeeName}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{employeeName}' or ei.EmployeeName = N'%{employeeName}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}%' or ei.EmployeeName = N'{employeeName}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }

        //    if (departmentID != null)
        //    {
        //        condition += $@" And ei.DepartmentID In({departmentID})";
        //    }
        //    if (unitID != null)
        //    {
        //        condition += $@" And ei.UnitID In({unitID})";
        //    }

        //    var query =
        //    $@"
        //        -- Details
        //        select 
        //         ei.EmployeeID, ei.EmployeeNo, ei.EmployeeName, ei.UnitFullName, ei.DepartmentID, ei.DepartmentName, 
        //            ei.DesignationID, ei.DesignationName, ei.SectionID, ei.SectionName, ei.SubSectionID, ei.SectionName
        //        from {DBSource.PDLHRM}..vw_EmployeeInfo ei
        //        where 1 = 1
        //        {condition}
        //        {orderBy}
        //        OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY; ";

        //    var dataList = await _context.GetDataAsync<Emp_EmploymentInfo>(query);
        //    return dataList;
        //}
        //public async Task<List<Emp_EmploymentInfo>> GetEmployeeBySectionID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string sectionID)
        //{
        //    var condition = "";
        //    var orderBy = " order by ei.EmployeeNo, ei.EmployeeName\r\n";
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        employeeName = "";
        //        condition = "  and (ei.EmployeeNo like '%" + searchKey + "%' or ei.EmployeeName like '%" + searchKey + "%' or ei.DepartmentName like '%" + searchKey + "%' or ei.DesignationName like '%" + searchKey + "%' or ei.SectionName like '%" + searchKey + "%')";
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{searchKey}' or ei.EmployeeName = N'{searchKey}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{searchKey}' or ei.EmployeeName = N'%{searchKey}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{searchKey}%' or ei.EmployeeName = N'{searchKey}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }
        //    else if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{employeeName}' or ei.EmployeeName = N'{employeeName}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{employeeName}' or ei.EmployeeName = N'%{employeeName}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}%' or ei.EmployeeName = N'{employeeName}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }

        //    if (sectionID != null)
        //    {
        //        condition += $@" And ei.SectionID In({sectionID})";
        //    }

        //    var query =
        //    $@"
        //        -- Details
        //        select 
        //         ei.EmployeeID, ei.EmployeeNo, ei.EmployeeName, ei.UnitFullName, ei.DepartmentID, ei.DepartmentName, 
        //            ei.DesignationID, ei.DesignationName, ei.SectionID, ei.SectionName, ei.SubSectionID, ei.SectionName
        //        from {DBSource.PDLHRM}..vw_EmployeeInfo ei
        //        where 1 = 1
        //        {condition}
        //        {orderBy}
        //        OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //    ";

        //    var dataList = await _context.GetDataAsync<Emp_EmploymentInfo>(query);
        //    return dataList;
        //}
        //public async Task<List<Emp_EmploymentInfo>> GetEmployeeListByAll(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string unitID, string departmentID, string sectionID)
        //{
        //    var condition = "";
        //    var orderBy = " order by ei.EmployeeNo, ei.EmployeeName\r\n";
        //    if (!string.IsNullOrEmpty(searchKey))
        //    {
        //        employeeName = "";
        //        condition = "  and (ei.EmployeeNo like '%" + searchKey + "%' or ei.EmployeeName like '%" + searchKey + "%' or ei.DepartmentName like '%" + searchKey + "%' or ei.DesignationName like '%" + searchKey + "%' or ei.SectionName like '%" + searchKey + "%')";
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{searchKey}' or ei.EmployeeName = N'{searchKey}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{searchKey}' or ei.EmployeeName = N'%{searchKey}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{searchKey}%' or ei.EmployeeName = N'{searchKey}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }
        //    else if (!string.IsNullOrEmpty(employeeName))
        //    {
        //        orderBy = $@" order by (
        //                                CASE 
        //                                    WHEN ei.EmployeeNo = N'{employeeName}' or ei.EmployeeName = N'{employeeName}' THEN 1 
        //                                    WHEN ei.EmployeeNo LIKE N'%{employeeName}' or ei.EmployeeName = N'%{employeeName}' THEN 2 
        //                                    WHEN ei.EmployeeNo LIKE N'{employeeName}%' or ei.EmployeeName = N'{employeeName}%' THEN 3 
        //                                    ELSE 4 
        //                                END
        //                    )";
        //    }

        //    if (unitID != null)
        //    {
        //        condition += $@" And ei.UnitID In({unitID})";
        //    }
        //    if (departmentID != null)
        //    {
        //        condition += $@" And ei.DepartmentID In({departmentID})";
        //    }
        //    if (sectionID != null)
        //    {
        //        condition += $@" And ei.SectionID In({sectionID})";
        //    }
        //    var query =
        //    $@"
        //        -- Details
        //        select 
        //         ei.EmployeeName, ei.EmployeeNo, ei.DepartmentName, ei.DesignationName, ei.SectionName,
        //         ei.UnitFullName, ei.EmployeeID
        //        from {DBSource.PDLHRM}..vw_EmployeeInfo ei
        //        where 1 = 1
        //        {condition}
        //        {orderBy}
        //        OFFSET ({pageSize} * ({pageIndex} - 1)) ROWS FETCH NEXT {pageSize} ROWS ONLY;
        //    ";

        //    var dataList = await _context.GetDataAsync<Emp_EmploymentInfo>(query);
        //    return dataList;
        //}

        #endregion
    }
}
