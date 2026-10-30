using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace RetailERP.Core.Common
{ 
    #region Static Class
    public static class AppGlobalFields
    {
        public const string NEW = "<<--System Value-->>";
        public const string ProjectID = "1";
        public const string AdminMessage = "Please contact with IT department.";
    }
    public static class DBSource
    {  
        public const string RetailERP = "RetailERP";
        public const string PDLHRM = "ERPDB";
        public const string ERPUSERDB = "ERPDB";
    }
    
    #endregion Static Class

    #region Enum Variable
    public enum Status
    {
        ALL = 1,
        NEW = 2,
        EDIT = 3,
        PENDING = 4,
        COMPLETED = 5,
        PARTIALLY_COMPLETED = 6,
        APPROVED = 7,
        PROPOSED_FOR_APPROVAL = 8,
        UN_APPROVE = 9,
        ACKNOWLEDGE = 10,
        PROPOSED_FOR_ACKNOWLEDGE = 11,
        UN_ACKNOWLEDGE = 12,
        ACKNOWLEDGE_ACCEPTENCE = 13,
        PROPOSED_FOR_ACKNOWLEDGE_ACCEPTENCE = 14,
        REJECT = 15,
        REVISE = 16,
        CHECK = 17,
        CHECK_REJECT = 18,
        ACTIVE = 19,
        IN_ACTIVE = 20,
        RECEIVED = 21,
        ISSUED = 22,
        CANCEL = 23,
        RUNNING = 24,
        CLOSE = 25,

        FILE = 26,
        INVOICE = 27,
        GP_NO = 28,
        INT_CHALLAN = 29,

        RETURN = 29,
        OTHERS = 29,

        LIST_1 = 40,
        LIST_2 = 41,
        LIST_3 = 42,
        LIST_4 = 43,
        LIST_5 = 44,
        LIST_6 = 45,
    }
    #endregion Enum Variable

    #region Dropdown List
    public class DropDownList
    {
        public int? id { get; set; } 
        public int? itemId { get; set; } 
        public string ids { get; set; }
        public int? virtualID { get; set; }
        public string text { get; set; }
        public string desc { get; set; }
        public string additionalValue { get; set; }
        public int? additionalID { get; set; }
        public string valueA { get; set; }
        public string valueB { get; set; }
        public string valueC { get; set; }
        public string valueD { get; set; }
        public string valueE { get; set; }
        public string valueF { get; set; }
        public string valueG { get; set; }
        public string valueH { get; set; }
        public string valueI { get; set; }
        public string valueJ { get; set; }
        public string valueK { get; set; }
        public string valueL { get; set; }
        public string valueM { get; set; }
        public DateTime? dateA { get; set; }
        public DateTime? dateB { get; set; }
        
    }
    public class DropDownList2
    {
        public string id { get; set; }
        public string itemId { get; set; }
        public string ids { get; set; }
        public int? virtualID { get; set; }
        public string text { get; set; }
        public string desc { get; set; }
        public string additionalValue { get; set; }
        public int? additionalID { get; set; }
        public string valueA { get; set; }
        public string valueB { get; set; }
        public string valueC { get; set; }
        public string valueD { get; set; }
        public string valueE { get; set; }
        public string valueF { get; set; }
        public string valueG { get; set; }
        public string valueH { get; set; }
        public string valueI { get; set; }
        public DateTime? dateA { get; set; }
        public DateTime? dateB { get; set; }

    }
    #endregion Dropdown List

    #region Base Class 
    public class BaseEntity
    {
        [Write(false)]
        public int SaveOption { get; set; } = 0;
        [Write(false)]
        public int IdentityValue { get; set; } = 0;
        [Write(false)]
        public int ErrNo { get; set; } = 0;

        [Write(false)]
        public int ResultId { get; set; }
        [Write(false)]
        public int NoofRows { get; set; }
        [Write(false)]
        public string Message { get; set; }
        [Write(false)]
        public string ExceptionError { get; set; }
        [Write(false)]
        public int ErrorNo { get; set; }
        [Write(false)]
        public string ReturnValue { get; set; }
        [Write(false)]
        public int IsValid { get; set; }
        [Write(false)]
        public int IsOperations { get; set; }
        [Write(false)]
        public int IsLogin { get; set; }
        [Write(false)]
        public int isCompleted { get; set; }
        [Write(false)]
        public string redirectUrl { get; set; }
        [Write(false)]
        public string UserBy { get; set; }
        [Write(false)]
        public string UserName { get; set; }
        [Write(false)]
        public int ReferenceResultId { get; set; }
    }  
    public class AuditTrail
    {
        public int AuditTrailID { get; set; }
        public DateTime? AuditTrailDate { get; set; }
        public string AuditTrailType { get; set; }
        public string AuditTrailDescription { get; set; }
        public int? UserID { get; set; }
        public int? ProjectID { get; set; }
        public int? MenuID { get; set; }
        public string ViewPage { get; set; }
        public int IsActive { get; set; } = 0;
    }
    public class FileUploadModel
    {
        public FileUploadModel()
        {
            IsName = false;
            Is64Byte = false;
            PrimaryKeyID = "";
        }
        public IFormFile Files { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string FileFormatName { get; set; }
        public string HTMLTextBoxID { get; set; }
        public string PrimaryKeyID { get; set; }
        public string SaveResult { get; set; }
        public bool IsName { get; set; }
        public bool Is64Byte { get; set; }
        public byte[] ImageByteData { get; set; }
        public string SqlQuery { get; set; }
        public string Usql { get; set; }
        public string Dsql { get; set; }
        public int? IsDeleted { get; set; } = 0;
        public string Condition { get; set; }
        public string PreviousFilePath { get; set; }
        public bool IsMultiple { get; set; } = false;

    }
    #endregion Base Class 

    #region Common Class  
    public class Emp_EmploymentInfo
    {  
        public int? EmployeeID { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNameWithID { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int? DesignationID { get; set; }
        public string DesignationName { get; set; }
        public int? SectionID { get; set; }
        public string SectionName { get; set; }
        public int? SubSectionID { get; set; }  
        public string SubSectionName { get; set; } 
        public string UnitFullName { get; set; } 
    }   
    public class DeserializeObjectModel
    {
        public string Id1 { get; set; } 
        public string Id2 { get; set; }
        public string Id3 { get; set; }
    }
    public class Parameter
    {
        public string RowID { get; set; }
        public string RequirementMasterID { get; set; }
        public string ReceiveMasterID { get; set; }
        public string IssueMasterID { get; set; }
        public string SaveOption { get; set; }
        public string id { get; set; }
        public string MenuID { get; set; }
        public string PageName { get; set; }

    }

    #endregion Common
}
