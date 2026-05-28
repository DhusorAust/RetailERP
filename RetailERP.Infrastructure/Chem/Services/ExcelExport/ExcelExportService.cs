using RetailERP.Core.Chem.ExcelExport;
using RetailERP.Core.Common;
using RetailERP.Infrastructure.Chem.Interfaces.ExcelExport;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.ExcelExport
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly DapperContext<ExcelExportParam> _context;
        private readonly SqlConnection _connection;

        public ExcelExportService(DapperContext<ExcelExportParam> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"";
        public async Task<ExcelExportParam> GetNew(int userId)
        {
            var query =
                $@"
                -- Report Menu
                select b.MenuID as id,b.MenuHead as text from ERPDB.dbo.MenuPermissionRole a
                inner join ERPDB.dbo.Menu b on a.MenuID=b.MenuID
                inner join ERPDB.dbo.UserRole c on c.UserRoleID=a.UserRoleID
                inner join ERPDB.dbo.UserRolePermission d on d.UserRoleID=c.UserRoleID 
                                            and d.UserID={userId} and d.ProjectID=4
                Where d.UserID={userId} and b.ProjectID=4 and ParentMenuID=1642
                order by MenuHead;

                --unit
                Select UnitID as id, UnitName as text from bas_UnitInfo where IsActive=1 order by UnitID; 

                --Buyer
                Select BuyerID as id, BuyerName as text from bas_BuyerNSupplier where IsActive=1 order by BuyerName;
                    
                --Nego Bank
                select BankID as id, ShortBankName as text from bas_Bank where IsActive=1 ;

                --Zone
                SELECT distinct UPPER(Zone) as id, UPPER(Zone) as text from bas_BuyerNSupplier where IsActive=1 and Zone is not null
                and Zone in ('EAST','EXPORT','EXTENSION','NORTH','OE','PDL','SOUTH','UNIT2')
                ORDER BY UPPER(Zone);

                --Certificate Type
                Select CertificateTypeID as id, CertificateName as text from bas_CertificateType where IsActive=1 order by CertificateName;

                -- Yarn Type
                SELECT V.id, V.text FROM (VALUES ('C', 'C'),('K', 'K')) AS V(id, text);

                ";

            ExcelExportParam data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data.ReportMenuList = records.Read<DropDownList>().ToList();
                data.UnitList = records.Read<DropDownList>().ToList();
                data.BuyerList = records.Read<DropDownList>().ToList();
                data.NegoBankList = records.Read<DropDownList>().ToList();
                data.ZoneList = records.Read<DropDownList2>().ToList();
                data.CertTypeList = records.Read<DropDownList>().ToList();
                data.YarnTypeList = records.Read<DropDownList2>().ToList();
               
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
            return data;
        }

        public async Task<DataTable> GetExportExcelData(ExcelExportParam reportParemaeters)
        {
            string query = "";
            var data = new DataTable();

            if (reportParemaeters.ReportMenuID == 1585) //LCRegisterBook
            {
                query = LCRegisterBookQuery(reportParemaeters);
            }
            else if (reportParemaeters.ReportMenuID == 1797) //LCMonitoring Delivery
            {
                query = LCRegisterBookQuery(reportParemaeters);
            }
            else
            {
                throw new Exception("Invalid Report Menu ID");
            }
            //Get Data Table
            try
            {
                data = await _context.SqlDataTable(query);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return data;
        }
        public string LCRegisterBookQuery(ExcelExportParam reportParemaeters)
        {
            string sql = $@"";
            string condition = "";

            if (reportParemaeters.UnitID > 0)
            {
                condition += @$" And Ld.UnitID={reportParemaeters.UnitID}";
            }
            if (reportParemaeters.BuyerID > 0)
            {
                condition += @$" And Ld.BuyerID={reportParemaeters.BuyerID}";
            }
            if (reportParemaeters.BuyerBankID > 0)
            {
                condition += @$" And ld.LCOPBankID={reportParemaeters.BuyerBankID}";
            }
            if (reportParemaeters.NegBankID > 0)
            {
                condition += @$" And ld.DocBankID={reportParemaeters.NegBankID}";
            }
            if (reportParemaeters.CertificateTypeID > 0)
            {
                condition += @$" And cc.CertificateTypeID={reportParemaeters.CertificateTypeID}";
            }

            if (!string.IsNullOrEmpty(reportParemaeters.ZoneID))
            {
                condition += @$" And b.Zone LIKE '%{reportParemaeters.ZoneID}%'";
            }

            if (reportParemaeters.DateFrom != null)
            {
                var fromDate = reportParemaeters.DateFrom?.ToString("dd-MMM-yyyy");
                var todate = reportParemaeters.DateTo?.ToString("dd-MMM-yyyy");

                condition += @$" And (ld.LCEntryDate Between '{fromDate}' And '{todate}')";
            }

        sql = $@"SELECT 
            ld.LCNo, ld.FileNo,ld.LCEntryDate, ld.LCDate, im.PINo, im.PIDate, u.UnitName,  b.BuyerName,
            SUBSTRING(b.[Zone], 1, 3)ZoneName,  c.CountName,  b_Doc.BankName As DocBankName,b_Doc.ShortBankName as Bank,  ld.ShipmentDate, 
            (CASE WHEN ISNULL(ld.Tenor, 0) = 0 THEN 'At Sight' ELSE CAST(ld.Tenor AS VARCHAR) + ' Days Deferred' END) AS Tenor,
            (CASE WHEN ld.LcStatus = 'CL' THEN 'L/C Cancelled' ELSE '' END) AS LCStatus,
            (CASE WHEN ld.LCStatus = 'IN' THEN CAST(ld.LCValue AS VARCHAR(100)) ELSE '0' END) AS LCValue, 
            id.QtyBag, id.Qty, id.Rate, (Isnull(id.Qty, 0) * Isnull(id.Rate, 0)) AS PIValue  
            FROM 
            exp_LCDetails ld
            Inner Join exp_PIInfoD id ON id.LCID = ld.LCID
            Inner Join exp_PIInfoM im ON im.PIID = id.PIID
            Inner Join bas_CountCertificate cc On cc.CountID = id.CountID
            Inner Join bas_Count c On c.CountID = id.CountID
            Left Join bas_BuyerNSupplier b On b.BuyerID = ld.BuyerID
            Left Join bas_Bank b_Doc On b_Doc.BankID = ld.DocBankID
            Left Join bas_UnitInfo u On u.UnitID = im.UnitID
            Where Isnull(ld.IsCancel, 0) = 0 AND Isnull(id.IsDeleted, 0) = 0 
            {condition}
            ORDER BY  FileNo ASC";

            return sql;
        }

        public string LCMonitoring_Delivery(ExcelExportParam reportParemaeters)
        {
            string sql = $@"";
            string condition = "";

            if (reportParemaeters.UnitID > 0)
            {
                condition += @$" And Ld.UnitID={reportParemaeters.UnitID}";
            }
            if (reportParemaeters.BuyerID > 0)
            {
                condition += @$" And Ld.BuyerID={reportParemaeters.BuyerID}";
            }
            if (reportParemaeters.DateFrom != null)
            {
                var fromDate = reportParemaeters.DateFrom?.ToString("dd-MMM-yyyy");
                var todate = reportParemaeters.DateTo?.ToString("dd-MMM-yyyy");

                condition += @$" And (ld.LCEntryDate Between '{fromDate}' And '{todate}')";
            }

            sql = $@"SELECT 
			A.LCID, A.FileNo, A.LCNo, A.LcDate, A.BuyerID, A.BuyerName, A.LCBag, A.LCKg, A.LCValue, B.InvoiceMasterID, B.InvoiceBag, B.InvoiceQty,
			C.DODate, (CASE WHEN A.LCKg - D.LocalChallanQty <> 0 THEN CAST(DATEDIFF(DAY, C.DoDate, GETDATE()) + 1 AS VARCHAR) + ' Days' ELSE NULL END) AS Remarks,
			B.InvoiceAmount, B.MaturityDate, B.PRCAmount, B.PRCCnt, B.PRCDifferentP, C.DOMasterID, C.DONo, C.DOStatus, C.DOBag, C.DOQty, D.ExportChallanBag,
			D.ExportChallanQty, D.LocalChallanBag, D.LocalChallanQty, A.LCStatus, A.Remarks1,A.UnitName
		FROM 
		(
			SELECT 
				Ld.LCID, Ld.FileNo, Ld.LcNo, Ld.LCDate, CASE WHEN Ld.LcStatus = 'CL' THEN 'L/C Cancelled' END AS LCStatus,
				Ld.BuyerID, b.BuyerName,
				(SELECT SUM(pid.QtyBag) FROM exp_PIInfoD pid WHERE pid.LCID = Ld.LCID) AS LCBag,
				(SELECT SUM(pid.Qty) FROM exp_PIInfoD pid WHERE pid.LCID = Ld.LCID) AS LCKg,
				Ld.LCValue, Ld.Remarks1,U.UnitName
			FROM 
				exp_LCDetails Ld
				Left Join bas_BuyerNSupplier b ON b.BuyerID = Ld.BuyerID
				Left Join bas_UnitInfo U ON U.UnitID = Ld.UnitID
			WHERE
				1 = 1
				{condition}
		) A
		LEFT JOIN 
		(
			SELECT 
				LCID, COUNT(InvoiceMasterID) AS InvoiceMasterID, SUM(ISNULL(InvoiceBag, 0)) AS InvoiceBag, SUM(ISNULL(InvoiceQty, 0)) AS InvoiceQty,
				SUM(ISNULL(InvoiceAmount, 0)) AS InvoiceAmount, MAX(MaturityDate) AS MaturityDate, SUM(ISNULL(PaymentAmount, 0)) AS PRCAmount,
				SUM(ISNULL((CASE WHEN PaymentAmount <> 0 THEN 1 ELSE 0 END), 0)) AS PRCCnt,
				ROUND(((SUM(ISNULL(InvoiceAmount, 0)) - SUM(ISNULL(PaymentAmount, 0))) / NULLIF(SUM(ISNULL(InvoiceAmount, 0)), 0)) * 100, 2) AS PRCDifferentP
			FROM 
			(
				SELECT 
					M.LCID, M.InvoiceMasterID, M.InvoiceNo, M.MaturityDate, ISNULL(M.PaymentAmt, 0) AS PaymentAmount, SUM(ISNULL(D.QtyBag, 0)) AS InvoiceBag,
					SUM(ISNULL(D.Qty, 0)) AS InvoiceQty, SUM(ISNULL(D.Amount, 0)) AS InvoiceAmount
				FROM 
					exp_InvoiceM M
					INNER JOIN exp_InvoiceD D ON M.InvoiceMasterID = D.InvoiceMasterID
				WHERE 
					M.InvoiceDate >= '2016-01-01'
				GROUP BY 
					M.LCID, M.InvoiceMasterID, M.InvoiceNo, M.MaturityDate, ISNULL(M.PaymentAmt, 0)
			) AS Sub
			GROUP BY LCID
		) B ON A.LCID = B.LCID
		LEFT JOIN 
		(
			SELECT 
				Dm.DOMasterID, Dm.DONo, Dm.DODate, Dm.LCID, CASE WHEN ISNULL(Dm.CloseStatus, 0) = 1 THEN 'D/O Cancelled' END AS DOStatus,
				SUM(ISNULL(Dd.QtyBag, 0)) AS DOBag, SUM(ISNULL(Dd.Qty, 0)) AS DOQty
			FROM 
				exp_DOMaster Dm
				INNER JOIN exp_DOChild Dd ON Dm.DOMasterID = Dd.DOMasterID
			WHERE 
				ISNULL(Dm.IsLocal, 0) = 0
			GROUP BY 
				Dm.DOMasterID, Dm.DONo, Dm.DODate, Dm.LCID, Dm.CloseStatus
		) C ON A.LCID = C.LCID
		LEFT JOIN 
		(
			SELECT 
				Dm.LCID,
				SUM(CASE WHEN ISNULL(Dm.IsLocal, 0) = 0 THEN Dd.QtyBag ELSE 0 END) AS ExportChallanBag,
				SUM(CASE WHEN ISNULL(Dm.IsLocal, 0) = 0 THEN Dd.Qty ELSE 0 END) AS ExportChallanQty,
				SUM(CASE WHEN Dm.IsLocal = 1 THEN Dd.QtyBag ELSE 0 END) AS LocalChallanBag,
				SUM(CASE WHEN Dm.IsLocal = 1 THEN Dd.Qty ELSE 0 END) AS LocalChallanQty
			FROM 
				YS_DeliveryChallanM Dm
				INNER JOIN YS_DeliveryChallanD Dd ON Dm.ChallanMasterID = Dd.ChallanMasterID
			WHERE 
				--Dm.ChallanType = 'YO' AND 
				ISNULL(Dm.CloseStatus, 0) = 0
				AND Dm.LCID IS NOT NULL
			GROUP BY Dm.LCID
		) D ON A.LCID = D.LCID 
		ORDER BY 
			A.FileNo ASC";

            return sql;
        }
    }
}
