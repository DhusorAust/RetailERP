/// <reference path="../../common/js/utilitycommon.js" />

$.ajaxSetup({
    async: false
});

var statusCons;
var masterData;
var menuId, pageName;
var ToolbarId;
var DivToolbarDom, DivDetailsDom, ToolbarDom, FormDom;
var childsActionBtn = true;
var childsToolsBar = false;
var savedFilter = null;
var IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = false;
var isApprovePage = false;
var IngredientManufacturerList = [];
var FormData_File = new FormData();
var FileDataArray = [];
var UploadFiles = "";
var UploadFileNames = "";

$(document).ready(function () {
    Helper.Initialize();
});

var Manager = { 
    GetNew: function () {
        debugger;
        $.get(`/api/IngredientMaster/new`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();
            debugger
            masterData = data;
            KendoDropDownHelper.MultiSelectCombo([], "cmbIngredientManufacturerID", "text", "id");
            KendoDropDownHelper.MultiSelectCombo(data.IngredientManufacturerList, "cmbIngredientManufacturerID", "text", "id");

            FormHelper.SetFormData(FormDom, masterData);
            GridManager.InitChildTable_INCIBreakdown([]);
            GridManager.TblChildId_AllergensBreakdown([]);
            GridManager.TblChildId_IngredientSupplier([]);
            GridManager.TblChildId_IngredientRegulatoryRestriction([]);
            GridManager.TblChildId_RptGrid([]);

        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },

    GetDetails: function (id) {
        $.get(`/api/IngredientMaster/edit?id=${id}`, function (data) {
            DivDetailsDom.fadeIn();
            DivToolbarDom.fadeOut();

            masterData = data;
            KendoDropDownHelper.MultiSelectCombo([], "cmbIngredientManufacturerID", "text", "id");
            KendoDropDownHelper.MultiSelectCombo(data.IngredientManufacturerList, "cmbIngredientManufacturerID", "text", "id");
            KendoDropDownHelper.MultiSelectCombo(data.IngredientManufacturerList, "cmbIngredientManufacturerID", "text", "id");

            FormHelper.SetFormData(FormDom, masterData);

            if (masterData.IngredientManufacturerIds) {
                IngredientManufacturerList = [];
                const array = masterData.IngredientManufacturerIds.split(',');
                IngredientManufacturerList = array.flat().join(',');
                $("#cmbIngredientManufacturerID").data("kendoMultiSelect").value(masterData.IngredientManufacturerIds.split(','));
            }

            GridManager.InitChildTable_INCIBreakdown(masterData.INCIBreakdowns);
            GridManager.TblChildId_AllergensBreakdown(masterData.AllergensBreakdowns);
            if ((masterData.AllergensBreakdowns || []).length > 0) {
                Helper.SetAllergenApplicable(true);
            }
            GridManager.TblChildId_IngredientSupplier(masterData.IngredientSuppliers);
            GridManager.TblChildId_IngredientRegulatoryRestriction(masterData.IngredientRegulatoryRestrictions);
            GridManager.TblChildId_RptGrid(masterData.IngredientRegulatoryRestrictions);

            $("#btnPrintIndent").show();
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },
    GetDocumentList(idname, indentId) {
        debugger
        $.get(`/api/IngredientMaster/GetDocumentList?id=${indentId}&idname=${idname}`, function (data) {
            debugger
            GridManager.TblChildId_RptGrid(data.IngredientDocuments);
        }).catch(function (err) {
            MessageHelper.Warning("Error = ", err.response.data.Message);
        });
    },

    Save: function (IsApproved) {

        if (Helper.IsFrmValid()) {
            debugger

            var model = FormHelper.FormElToJson(FormDom);
            model.INCIBreakdowns = JSON.parse(JSON.stringify($(TblChildId_INCIBreakdown).data("kendoGrid").dataSource.data()));
            model.AllergensBreakdowns = JSON.parse(JSON.stringify($(TblChildId_AllergensBreakdown).data("kendoGrid").dataSource.data()));
            model.IngredientSuppliers = JSON.parse(JSON.stringify($(TblChildId_IngredientSupplier).data("kendoGrid").dataSource.data()));
            model.IngredientRegulatoryRestrictions = JSON.parse(JSON.stringify($(TblChildId_IngredientRegulatoryRestriction).data("kendoGrid").dataSource.data()));
            model.IsAllergenApplicable = model.AllergensBreakdowns.length > 0 ? 1 : model.IsAllergenApplicable;
            model.Approved = IsApproved;
            model.IngredientManufacturerIds = IngredientManufacturerList;

            //Child Validation
            if (!Helper.IsFrmChildsValid(model)) {
                return;
            }

            $.post("/api/IngredientMaster/save", model, function (data) {
                if (parseInt(data.ResultId) > 0) {
                    MessageHelper.Success("Save Operations", data.Message);
                    $('#IngredientID').val(data.ResultId);
                    // Upload file 
                    Manager.FileUpload(data);
                    Helper.BackToList();
                } else {
                    if (data.IsLogin == 1) {
                        window.open(data.redirectUrl, "_blank");
                    } else {
                        MessageHelper.Warning("Save Operations", data.Message);
                    }
                }
            });
        }
    },
    FileUpload: function (data) {
        //TIN 
        debugger
        var UploadFiles = "";
        var UploadFileNames = "";
        var fileDateArray = [];
        var usqll = [];
        var conditionn = [];

        if ($("#inputSDSFilePath").val() != "") {
            UploadFiles = $("#inputSDSFilePath").get(0).files;
            UploadFileNames = $("#inputSDSFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputSDSFilePath";
            FilePath = "\\Files\\IngredientMaster\\SDS\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'SDS',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_SDS_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }
       
        if ($("#inputCOAFilePath").val() != "") {
            UploadFiles = $("#inputCOAFilePath").get(0).files;
            UploadFileNames = $("#inputCOAFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputCOAFilePath";
            FilePath = "\\Files\\IngredientMaster\\COA\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'COA',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_COA_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputINCIFilePath").val() != "") {
            UploadFiles = $("#inputINCIFilePath").get(0).files;
            UploadFileNames = $("#inputINCIFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputINCIFilePath";
            FilePath = "\\Files\\IngredientMaster\\INCI\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'INCI',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_INCI_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputIFRAFilePath").val() != "") {
            UploadFiles = $("#inputIFRAFilePath").get(0).files;
            UploadFileNames = $("#inputIFRAFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputIFRAFilePath";
            FilePath = "\\Files\\IngredientMaster\\IFRA\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'IFRA',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_IFRA_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputAllergenStatementFilePath").val() != "") {
            UploadFiles = $("#inputAllergenStatementFilePath").get(0).files;
            UploadFileNames = $("#inputAllergenStatementFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputAllergenStatementFilePath";
            FilePath = "\\Files\\IngredientMaster\\AllergenStatement\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'AllergenStatement',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_AllergenStatement_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputRegulatoryFilePath").val() != "") {
            UploadFiles = $("#inputRegulatoryFilePath").get(0).files;
            UploadFileNames = $("#inputRegulatoryFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputRegulatoryFilePath";
            FilePath = "\\Files\\IngredientMaster\\Regulatory\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'Regulatory',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_Regulatory_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputImpurityFilePath").val() != "") {
            UploadFiles = $("#inputImpurityFilePath").get(0).files;
            UploadFileNames = $("#inputImpurityFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputImpurityFilePath";
            FilePath = "\\Files\\IngredientMaster\\Impurity\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'Impurity',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_Impurity_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputTDSFilePath").val() != "") {
            UploadFiles = $("#inputTDSFilePath").get(0).files;
            UploadFileNames = $("#inputTDSFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputTDSFilePath";
            FilePath = "\\Files\\IngredientMaster\\TDS\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'TDS',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_TDS_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputISO16128StatementFilePath").val() != "") {
            UploadFiles = $("#inputISO16128StatementFilePath").get(0).files;
            UploadFileNames = $("#inputISO16128StatementFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputISO16128StatementFilePath";
            FilePath = "\\Files\\IngredientMaster\\ISO16128Statement\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'ISO16128Statement',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_ISO16128Statement_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputCosmosFilePath").val() != "") {
            UploadFiles = $("#inputCosmosFilePath").get(0).files;
            UploadFileNames = $("#inputCosmosFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputCosmosFilePath";
            FilePath = "\\Files\\IngredientMaster\\Cosmos\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'Cosmos',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_Cosmos_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputHalalCerificateFilePath").val() != "") {
            UploadFiles = $("#inputHalalCerificateFilePath").get(0).files;
            UploadFileNames = $("#inputHalalCerificateFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputHalalCerificateFilePath";
            FilePath = "\\Files\\IngredientMaster\\HalalCerificate\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'HalalCerificate',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_HalalCerificate_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputAnimalFilePath").val() != "") {
            UploadFiles = $("#inputAnimalFilePath").get(0).files;
            UploadFileNames = $("#inputAnimalFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputAnimalFilePath";
            FilePath = "\\Files\\IngredientMaster\\Animal\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'Animal',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_Animal_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputPalmFilePath").val() != "") {
            UploadFiles = $("#inputPalmFilePath").get(0).files;
            UploadFileNames = $("#inputPalmFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputPalmFilePath";
            FilePath = "\\Files\\IngredientMaster\\Palm\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'Palm',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_Palm_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       
        if ($("#inputClinicalDataFilePath").val() != "") {
            UploadFiles = $("#inputClinicalDataFilePath").get(0).files;
            UploadFileNames = $("#inputClinicalDataFilePath");
            var HTMLTextBoxID = "", FilePath = "", PrimaryKeyID = 0, FileFormatName = "", Usql = "", Condition = "", Is64Byte = false;
            HTMLTextBoxID = "inputClinicalDataFilePath";
            FilePath = "\\Files\\IngredientMaster\\ClinicalData\\";
            PrimaryKeyID = data.ResultId;
           
            for (i = 0; i < UploadFiles.length; i++) {
                
                Usql = "insert into itm_IngredientDocument(IngredientID,DocumentType,FilePath) values (" + data.ResultId + ",'ClinicalData',";
                Condition = ")";
                var file = UploadFiles[i];//.files[0];
                var name = UploadFileNames[0];//.value; 
                FileFormatName = "IngredientMaster_ClinicalData_" + Date.now().toString() + Math.floor(Math.random() * 100000) +"_";

                var fileData = {
                    Name: name,
                    File: file,
                    Usql: Usql,
                    Condition: Condition
                };

                fileDateArray.push(fileData);
                usqll.push(Usql);
                conditionn.push(Condition);
            }
            FileUploadHelper.SetUploadedFileArray(fileDateArray, FilePath, PrimaryKeyID, FileFormatName, usqll, conditionn, Is64Byte, "" ?? "");
            FileUploadHelper.SaveFile(data.ResultId, "/api/helperC/FileUpload");
        }

       



    },
    RptIndent: function (id) {
        window.open(`/RptCommercialExport/csindent?id=${id}`, "_blank");
    },
}

var Helper = {
    BackToList: function () {
        DivDetailsDom.fadeOut();
        Helper.ResetForm();
        DivToolbarDom.fadeIn();
        GridManager.InitMasterTable();
    },
    ResetForm: function () {
        FormDom.trigger("reset");
        $('input[type="hidden"]').val("0");
    },
    IsFrmChildsValid: function (model) {

        debugger;

        var isValidItem = true;
        var TotalINCIPercentage = 0;
        var TotalAllergenPercentage = 0;

        // INCI Breakdown
        if (model.INCIBreakdowns.length === 0) {
            MessageHelper.Warning("Warning", "At least 1 INCI Breakdown is required.");
            return isValidItem = false;
        }

        $.each(model.INCIBreakdowns, function (i, obj) {
            if (obj.objINCIBreakDownList == null || obj.objINCIBreakDownList.id == null || obj.objINCIBreakDownList.id == 0) {
                MessageHelper.Warning("Warning", "INCI Breakdown is required.");
                isValidItem = false;
                return false;
            }

            if (parseFloat(obj.INCIPercentage || 0) <= 0) {
                MessageHelper.Warning("Warning", "INCI Percentage must be greater than zero.");
                isValidItem = false;
                return false;
            }

            TotalINCIPercentage += parseFloat(obj.INCIPercentage || 0);
        });

        if (!isValidItem) return false;

        if (TotalINCIPercentage > 100) {
            MessageHelper.Warning("Warning", "INCI Breakdown total can not be greater than 100.");
            return isValidItem = false;
        }

        // Allergens Breakdown
        $.each(model.AllergensBreakdowns, function (i, obj) {
            if (obj.objAllergensBreakDownList == null || obj.objAllergensBreakDownList.id == null || obj.objAllergensBreakDownList.id == 0) {
                MessageHelper.Warning("Warning", "Allergen Breakdown is required.");
                isValidItem = false;
                return false;
            }

            if (parseFloat(obj.AllergenPercentage || 0) <= 0) {
                MessageHelper.Warning("Warning", "Allergen Percentage must be greater than zero.");
                isValidItem = false;
                return false;
            }

            TotalAllergenPercentage += parseFloat(obj.AllergenPercentage || 0);
        });

        if (!isValidItem) return false;

        if (TotalAllergenPercentage > 100) {
            MessageHelper.Warning("Warning", "Allergen Breakdown total can not be greater than 100.");
            return isValidItem = false;
        }

        // Ingredient Supplier
        if (model.IngredientSuppliers.length === 0) {
            MessageHelper.Warning("Warning", "At least 1 Supplier is required.");
            return isValidItem = false;
        }

        $.each(model.IngredientSuppliers, function (i, obj) {
            if (obj.objSupplierList == null || obj.objSupplierList.id == null || obj.objSupplierList.id == 0) {
                MessageHelper.Warning("Warning", "Supplier is required.");
                isValidItem = false;
                return false;
            }

            if (parseFloat(obj.PricePerKg || 0) <= 0) {
                MessageHelper.Warning("Warning", "Price Per Kg must be greater than zero.");
                isValidItem = false;
                return false;
            }

            if (parseFloat(obj.MOQInKg || 0) <= 0) {
                MessageHelper.Warning("Warning", "Supplier MOQ In Kg must be greater than zero.");
                isValidItem = false;
                return false;
            }

            if (parseFloat(obj.LeadTime || 0) <= 0) {
                MessageHelper.Warning("Warning", "Supplier Lead Time must be greater than zero.");
                isValidItem = false;
                return false;
            }

            if (obj.objLeadTimeUnitList == null || obj.objLeadTimeUnitList.id == null || obj.objLeadTimeUnitList.id == 0 || obj.objLeadTimeUnitList.text == "") {
                MessageHelper.Warning("Warning", "Supplier Lead Time Unit is required.");
                isValidItem = false;
                return false;
            }
        });

        if (!isValidItem) return false;

        // Regulatory Restriction
        $.each(model.IngredientRegulatoryRestrictions, function (i, obj) {
            if (obj.objCountryList == null || obj.objCountryList.id == null || obj.objCountryList.id == 0) {
                MessageHelper.Warning("Warning", "Country is required in Regulatory Restriction.");
                isValidItem = false;
                return false;
            }

            if (obj.RestrictionNote == null || obj.RestrictionNote.trim() == "") {
                MessageHelper.Warning("Warning", "Restriction Note is required.");
                isValidItem = false;
                return false;
            }
        });

        if (!isValidItem) return false;

        return isValidItem;
    },


    IsFrmValid: function () {
        var isValidItem = true;
        var Elements = ["UnitID", "StoreID", "IndentDate", "IndentBy", "IndentTypeID"];
        if (!FormHelper.IsFormValidation(FormDom, Elements)) {
            isValidItem = false;
            ToastrMessageHelper.ValidationToastr("Please fill-up mandatory field.");
        }
        return isValidItem;
    },
    Initialize: function () {
        //Library Code
        if (!menuId)
            menuId = localStorage.getItem("menuId");
        if (!pageName)
            pageName = localStorage.getItem("pageName");

        var pageId = pageName + "-" + menuId;
        DivToolbarDom = $(pageConstants.DIV_TOOLBAR_ID + pageId);
        ToolbarId = pageConstants.TOOLBAR_ID + pageId;
        ToolbarDom = $(ToolbarId);
        TblMasterId = pageConstants.MASTER_TABLE_ID + pageId;

        TblChildId_INCIBreakdown = pageConstants.CHILD_TABLE_ID + pageId;
        TblChildId_AllergensBreakdown = pageConstants.CHILD_TABLE_ID1 + pageId;
        TblChildId_IngredientSupplier = pageConstants.CHILD_TABLE_ID2 + pageId;
        TblChildId_IngredientRegulatoryRestriction = pageConstants.CHILD_TABLE_ID3 + pageId;
        TblChildId_RptGrid = pageConstants.CHILD_TABLE_ID4 + pageId;

        FormDom = $(pageConstants.FORM_ID + pageId);
        DivDetailsDom = $(pageConstants.DIV_DETAILS_ID + pageId);



        //Development Code  
        $("#kt_aside_toggle").addClass('active');
        $("#kt_body").addClass('aside-minimize');


        $("#IsFragrance").kendoSwitch({ messages: { checked: "YES", unchecked: "NO" } });
        $("#IsAllergenApplicable").kendoSwitch({
            messages: {
                checked: "YES",
                unchecked: "NO"
            },
            checked: false,
            change: function () {
                Helper.ToggleAllergenAddNewButton();
            }
        });

       
        $("#divItemWindow").kendoWindow({
            width: "900px",
            height: "500px",
            title: "Report List",
            visible: false,
            modal: true,
            actions: ["Minimize", "Maximize", "Close"]
        });


        statusCons = statusConstants.EDIT;
        FormHelper.TogToolbarBtn(ToolbarDom.find("#btnEditList"), ToolbarDom);
        GridManager.InitMasterTable();


        // ToolbarDom means Toolbar Buttons 
        $("#btnNewAdd").on("click", function (e) {
            FormDom.find("#btnSave,#btnApproved").show();
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn($("#btnEditList"), ToolbarDom);
            Manager.GetNew();
            Helper.ToggleAllergenAddNewButton();
        });
        ToolbarDom.find("#btnEditList").on("click", function (e) {
            statusCons = statusConstants.EDIT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            Helper.ToggleAllergenAddNewButton();
        });
        ToolbarDom.find("#btnApprovedList").on("click", function (e) {
            statusCons = statusConstants.APPROVED;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            Helper.ToggleAllergenAddNewButton();
        });
        ToolbarDom.find("#btnRejectList").on("click", function (e) {
            statusCons = statusConstants.REJECT;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            Helper.ToggleAllergenAddNewButton();
        });
        ToolbarDom.find("#btnAllList").on("click", function (e) {
            statusCons = statusConstants.ALL;
            FormHelper.TogToolbarBtn(this, ToolbarDom);
            GridManager.InitMasterTable();
            Helper.ToggleAllergenAddNewButton();
        });

        //Button
        $("#btnSave").on("click", function (e) {
            Manager.Save(false);
        });

        $("#btnApproved").on("click", function (e) {
            Manager.Save(true);
        });

        $("#cmbIngredientManufacturerID").change(function (e) {

            var control = $("#cmbIngredientManufacturerID").data("kendoMultiSelect");
            var selectedColorItem = control.dataItems();

            IngredientManufacturerList = selectedColorItem.map(f => f.id).join(',');

        });


        $("#btnRevise").on("click", function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Revise this Requisition?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsSendForApproval = false, IsApproved = false, IsRevise = true, IsReject = false;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        });

        $("#btnReject").on("click", function (e) {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to Reject this Requisition?')
            ).then((result) => {
                if (result.isConfirmed) {
                    IsSendForApproval = false, IsApproved = false, IsRevise = false, IsReject = true;
                    Manager.Save(IsApproved, IsRevise, IsReject);
                }
            })
        });

        $("#btnBackToList").on("click", function () {
            Helper.BackToList();
        });
        $("#btnPrintPI").on("click", function () {
            var piid = $("#IndentMasterID").val();
            if (piid) {
                Manager.RptIndent(piid);
            }
        });
        
        $("#IsAllergenApplicable").on("change", function () {
            debugger;

            var isAllergenApplicable = false;

            var allergenSwitch = $("#IsAllergenApplicable").data("kendoSwitch");
            if (allergenSwitch) {
                isAllergenApplicable = allergenSwitch.check();
            }

            var grid = $(TblChildId_AllergensBreakdown).data("kendoGrid");
            if (grid) {
                if (isAllergenApplicable) {
                    grid.wrapper.find(".k-grid-add").show();
                } else {
                    grid.wrapper.find(".k-grid-add").hide();
                }
            }
        });

        $("#btnSDSFile,#btnCOAFile,#btnINCIFile,#btnIFRAFile,#btnAllergenStatementFile,#btnRegulatoryFile,#btnImpurityFile,#btnTDSFile,#btnISO16128StatementFile,#btnCosmosFile,#btnHalalCerificateFile,#btnAnimalFile,#btnPalmFile,#btnClinicalDataFile")
            .on("click", function (e) {
                var wnd = $("#divItemWindow").data("kendoWindow");
                wnd.center().open(); debugger;
                var idname = this.title;
                var indentId = $("#IngredientID").val();
                Manager.GetDocumentList(idname, indentId);
        });

        //Selected Row in the kendo Grid
        //KendoGridHelper.GridRowSelected(InitChildTable_INCIBreakdown);
    },
    ToggleAllergenAddNewButton: function () {debugger
        var isAllergenApplicable = $("#IsAllergenApplicable").data("kendoSwitch").check();
        var grid = $(TblChildId_AllergensBreakdown).data("kendoGrid");
        if (grid) {
            if (isAllergenApplicable) {
                grid.wrapper.find(".k-grid-add").show();
            } else {
                grid.wrapper.find(".k-grid-add").hide();
            }
        }
    },

    SetAllergenApplicable: function (isApplicable) {
        var allergenSwitch = $("#IsAllergenApplicable").data("kendoSwitch");
        if (allergenSwitch) {
            allergenSwitch.check(isApplicable);
        }

        $("#IsAllergenApplicable").prop("checked", isApplicable);
        Helper.ToggleAllergenAddNewButton();
    },

    GetIndentBy: function (data) {
        var columns = [
            { field: "text", title: "Name", width: "200" },
            { field: "valueD", title: "Department", width: "200" },
            { field: "valueE", title: "Section", width: "200" },
            { field: "valueF", title: "Sub Section", width: "300" },
        ];
        KendoDropDownHelper.MultiColumnComboBox(data, "#IndentBy", "text", "id", columns);
    },
}

var GridManager = {
    InitMasterTable: function () {
        if ($(TblMasterId).data("kendoGrid")) $(TblMasterId).kendoGrid('destroy').empty();
        dataSource = new kendo.data.DataSource({
            type: "json",
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            allowUnsort: true,
            pageSize: 50,
            transport: {
                read: {
                    type: "POST",
                    url: `/api/IngredientMaster/list/${statusCons}`,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                },
                parameterMap: function (options) {
                    return JSON.stringify(options);
                }
            },
            schema: {
                data: "Items", total: "TotalCount",
                model: {

                },
            }
        });

        var command;
        var columns;
        var search; 

        var command = [];
        if (statusCons == statusConstants.APPROVED) {
            command = [{ name: "view", text: "", iconClass: "k-icon k-i-eye", className: "k-success", click: GridHelper.HandleCommands }];
        } else {
            command = [{ name: "edit", text: "", iconClass: "k-icon k-i-edit", className: "k-success", click: GridHelper.HandleCommands }];
        }

        var columns = [
            { title: "", command: command, width: 50 },
            { field: "IngredientID", hidden: true },
            { field: "IngredientCode", title: "Ingredient Code", width: 120 },
            { field: "IngredientName", title: "Ingredient Name", width: 180 },
            { field: "CategoryName", title: "Category", width: 120 },
            { field: "TypeName", title: "Type", width: 140 },
            { field: "IngredientStatus", title: "Status", width: 100 },
            { field: "NaturalIndexPct", title: "Natural Index %", width: 110 },
            { field: "MOQInKg", title: "MOQ (Kg)", width: 90 },
            { field: "IsFragrance", title: "Fragrance", width: 80, template: "#= IsFragrance == 1 ? 'Yes' : 'No' #" },
            { field: "Approved", title: "Approved", width: 80, template: "#= Approved == true ? 'Yes' : 'No' #" }
        ];

        var height = $(window).height() - 120;
        KendoGridHelper.MasterGridLoad(TblMasterId, height + "px", search, columns, true)
    },

    InitChildTable_INCIBreakdown: function (data) {
        if ($(TblChildId_INCIBreakdown).data("kendoGrid")) $(TblChildId_INCIBreakdown).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete1", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 100,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "INCIBreakdownID", hidden: true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "IngredientID", hidden: true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    //field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 70,
                    field: "RowSLNo", title: "SL#",  template: function(dataItem) {
                        var grid = $(TblChildId_INCIBreakdown).data("kendoGrid");
                        if (!grid) return "";

                        var data = grid.dataSource.view();
                        return data.indexOf(dataItem) + 1;
                    },  width: 70,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objINCIBreakDownList", title: "INCI Breakdown", width: 280,
                    editor: GridHelper.cboINCIBreakDown,
                    template: "#=objINCIBreakDownList != null ? objINCIBreakDownList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "INCIPercentage", title: "% Input", width: 110,
                    footerTemplate: "Total % : <span id='sumINCIPercentage'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "CASNo", title: "CAS No", width: 110, editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },

            ];

        var schema = {
            model: {
                fields: {
                    objINCIBreakDownList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    CASNo: {
                        type: "string",
                        defaultValue: ""
                    },
                    INCIPercentage: {
                        type: "number",
                        defaultValue: 0,
                        validation: {
                            required: true,
                            min: 0.01
                        }
                    }
                }
            },
        };

        KendoGridHelper.ChildGridLoad(TblChildId_INCIBreakdown, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId_INCIBreakdown);

        var grid = $(TblChildId_INCIBreakdown).data("kendoGrid");
        grid.dataSource.unbind("change", GridHelper.SumINCIPercentage);
        grid.dataSource.bind("change", GridHelper.SumINCIPercentage);

        //grid.bind("save", GridHelper.GridDuplicateCheck);

        GridHelper.SumINCIPercentage();
    },
    TblChildId_AllergensBreakdown: function (data) {
        if ($(TblChildId_AllergensBreakdown).data("kendoGrid")) $(TblChildId_AllergensBreakdown).kendoGrid('destroy').empty();
        dataSource = data;
        gbGridSLNo = 0;

        var columns =
            [
                {
                    command: [{
                        name: "delete1", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 100,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "AllergenBreakdownID", hidden: true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "IngredientID", hidden: true,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    //field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 70,
                    field: "RowSLNo", title: "SL#", template: function(dataItem) {
                        var grid = $(TblChildId_AllergensBreakdown).data("kendoGrid");
                        if (!grid) return "";

                        var data = grid.dataSource.view();
                        return data.indexOf(dataItem) + 1;
                    }, width: 70,
                    editable: function () { return false; },
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "objAllergensBreakDownList", title: "Allergen Breakdown", width: 380,
                    editor: GridHelper.cboAllergensBreakDown,
                    template: "#=objAllergensBreakDownList != null ? objAllergensBreakDownList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "AllergenPercentage", title: "% Input",
                    footerTemplate: "Total % : <span id='sumAllergenPercentage'></span>",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
            ];

        var schema = {
            model: {
                fields: {
                    objAllergensBreakDownList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    AllergenPercentage: {
                        type: "number",
                        defaultValue: 0,
                        validation: {
                            required: true,
                            min: 0.01
                        }
                    }
                }
            },
        };


        KendoGridHelper.ChildGridLoad(TblChildId_AllergensBreakdown, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId_AllergensBreakdown);

        var grid = $(TblChildId_AllergensBreakdown).data("kendoGrid");
        grid.dataSource.unbind("change", GridHelper.SumAllergenPercentage);
        grid.dataSource.bind("change", GridHelper.SumAllergenPercentage);

        GridHelper.SumAllergenPercentage();

    },

    TblChildId_IngredientSupplier: function (data) {
        if ($(TblChildId_IngredientSupplier).data("kendoGrid")) $(TblChildId_IngredientSupplier).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete1", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 40,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IngredientSupplierID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    field: "IngredientID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                {
                    //field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 40,
                    field: "RowSLNo", title: "SL#", template: function (dataItem) {
                        var grid = $(TblChildId_IngredientSupplier).data("kendoGrid");
                        if (!grid) return "";

                        var data = grid.dataSource.view();
                        return data.indexOf(dataItem) + 1;
                    }, width: 40,
                    editable: function () { return false; },
                },
                {
                    field: "objSupplierList", title: "Supplier", width: 120,
                    editor: GridHelper.cboSupplier,
                    template: "#=objSupplierList != null ? objSupplierList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "PricePerKg", title: "Price Per kg", width: 60,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "MOQInKg", title: "MOQ In Kg", width: 60,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
                {
                    field: "LeadTime", title: "Lead Time", width: 60,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },

                {
                    field: "objLeadTimeUnitList", title: "Days/Weeks", width: 120,
                    editor: GridHelper.cboLeadTimeUnit,
                    template: "#=objLeadTimeUnitList != null ? objLeadTimeUnitList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
            ];

        var schema = {
            model: {
                fields: {

                    objSupplierList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                    objLeadTimeUnitList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },

                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId_IngredientSupplier, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId_IngredientSupplier);
    },

    TblChildId_IngredientRegulatoryRestriction: function (data) {
        if ($(TblChildId_IngredientRegulatoryRestriction).data("kendoGrid")) $(TblChildId_IngredientRegulatoryRestriction).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "delete1", text: "", iconClass: "k-icon k-i-delete", className: "k-success",
                        click: GridHelper.ChildCommandClick,
                    }], title: "Action &nbsp;", width: 100,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IngredientRegulatoryRestrictionID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                { field: "IngredientID", hidden: true },
                {
                    //field: "RowSLNo", title: "SL#", template: "#= ++gbGridSLNo #", width: 70,
                    field: "RowSLNo", title: "SL#", template: function (dataItem) {
                        var grid = $(TblChildId_IngredientRegulatoryRestriction).data("kendoGrid");
                        if (!grid) return "";

                        var data = grid.dataSource.view();
                        return data.indexOf(dataItem) + 1;
                    }, width: 70,
                    editable: function () { return false; },
                },
                {
                    field: "objCountryList", title: "Country Name", width: 250,
                    editor: GridHelper.cboCountry,
                    template: "#=objCountryList != null ? objCountryList.text : ''#",
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, false)
                },
                {
                    field: "RestrictionNote", title: "RestrictionNote", //width: 60, 
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, false)
                },
            ];

        var schema = {
            model: {
                fields: {
                    objCountryList: {
                        defaultValue: { id: null, text: "" },
                        validation: { required: true }
                    },
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId_IngredientRegulatoryRestriction, columns, data, schema, true, true, false);
        GridHelper.InitializeHelper();
        KendoGridHelper.EnterKeyUp(TblChildId_IngredientRegulatoryRestriction);

    },
    TblChildId_RptGrid: function (data) {
        if ($(TblChildId_RptGrid).data("kendoGrid")) $(TblChildId_RptGrid).kendoGrid('destroy').empty();
        dataSource = data;
        var columns =
            [
                {
                    command: [{
                        name: "download", text: "", iconClass: "k-icon k-i-download", className: "k-success",
                        click: function (e) {
                            e.preventDefault();
                            var tr = $(e.target).closest("tr");
                            newItem = this.dataItem(tr);
                            FileUploadHelper.DownloadFile(newItem.FilePath);
                        }, visible: function (dataItem) {
                            return dataItem.FilePath != null
                        }
                    },], title: "Action &nbsp;", width: 100,
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", true, true)
                },
                {
                    field: "IngredientDocumentID", hidden: true
                    , ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-left", "k-text-left", true, true)
                },
                { field: "IngredientID", hidden: true },
                
                
                {
                    field: "DocumentType", title: "DocumentType", //width: 60, 
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", false, false)
                },
                
                {
                    field: "FileName", title: "FileName", //width: 60, 
                    ...KendoGridHelper.GridCenterAlignAttributes("k-text-center", "k-text-center", "k-text-center", false, false)
                },
            ];

        var schema = {
            model: {
                fields: {
                    //objCountryList: {
                    //    defaultValue: { id: null, text: "" },
                    //    validation: { required: true }
                    //},
                }
            },
        };
        KendoGridHelper.ChildGridLoad(TblChildId_RptGrid, columns, data, schema, true, true, false);
        //GridHelper.InitializeHelper();
        //KendoGridHelper.EnterKeyUp(TblChildId_IngredientRegulatoryRestriction);
        var grid = $(TblChildId_RptGrid).data("kendoGrid");
            grid.wrapper.find(".k-grid-add").hide();

    },
}

var GridHelper = {
    // Actions
    HandleCommands: function (args) {
        args.preventDefault();
        var tr = $(args.currentTarget).closest("tr");
        var selectedItem = this.dataItem(tr);

        if (selectedItem != null) {
            if (args.data.commandName == "edit") {
                FormDom.find("#btnSave,#btnApproved").show();
                Manager.GetDetails(selectedItem.IngredientID);
            }
            if (args.data.commandName == "view") {
                FormDom.find("#btnSave,#btnApproved").hide();
                Manager.GetDetails(selectedItem.IngredientID);
            }
        }

        Helper.ToggleAllergenAddNewButton();
    },
    ChildCommandClick: function (args) {
        if (args.data.commandName == "delete1") {
            Swal.fire(
                SwalHelper.SwalConfirmationConfig('Are you sure want to DELETE this record.?')
            ).then((result) => {
                if (result.isConfirmed) {
                    var grid = $(args.currentTarget).closest(".k-grid").data("kendoGrid");
                    var tr = $(args.currentTarget).closest("tr");
                    var selectedItem = this.dataItem(tr);
                    grid.select(tr);
                    if (selectedItem != null) {
                        grid.dataSource.remove(selectedItem);
                    }
                }
            })
        }
    },

    InitializeHelper: function () {
        //Grid Function


    },
    SumINCIPercentage: function () {
        var TblChilds = $(TblChildId_INCIBreakdown).data("kendoGrid");
        var TblCDataSource = TblChilds.dataSource.data();
        var percentage = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            percentage += parseFloat(TblCDataSource[i].INCIPercentage || 0);
        }

        $("#sumINCIPercentage").text(kendo.toString(percentage, "0.##"));
        if (percentage > 100) {
            $("#sumINCIPercentage").css({ "color": "red", "font-weight": "bold" });
            MessageHelper.Error("Warning", "Already INCI Cross 100%!!");

        }
        else {
            $("#sumINCIPercentage").css({ "color": "", "font-weight": "" });
            
        }

        if (percentage >= 100) {
            TblChilds.wrapper.find(".k-grid-add").hide();
        }
        else {
            TblChilds.wrapper.find(".k-grid-add").show();
        }
    },
    SumAllergenPercentage: function () {
        var TblChilds = $(TblChildId_AllergensBreakdown).data("kendoGrid");
        var TblCDataSource = TblChilds.dataSource.data();
        var percentage = 0;

        for (var i = 0; i < TblCDataSource.length; i++) {
            percentage += parseFloat(TblCDataSource[i].AllergenPercentage || 0);
        }

        $("#sumAllergenPercentage").text(kendo.toString(percentage, "0.##"));


    },

    cboINCIBreakDown: function (container, options) {
        $('<input required data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoMultiColumnComboBox({
                dataSource: masterData.INCIBreakDownList,
                dataTextField: "text",
                dataValueField: "id",
                filter: "contains",
                autoBind: true,
                suggest: true,
                valuePrimitive: false,
                columns: [
                    { field: "text", title: "INCI Name", width: 220 },
                    { field: "valueA", title: "CAS No", width: 120 }
                ],
                change: function (e) {
                    debugger;
                    var item = this.dataItem();

                    if (item) {
                        var grid = $(TblChildId_INCIBreakdown).data("kendoGrid");
                        var data = grid.dataSource.data();

                        var isDuplicate = data.some(function (row) {
                            return row.uid !== options.model.uid
                                && row.objINCIBreakDownList
                                && row.objINCIBreakDownList.id == item.id;
                        });

                        if (isDuplicate) {
                            MessageHelper.Warning("Duplicate", "This INCI Breakdown already selected.");

                            options.model.set("CASNo", "");
                            options.model.set("INCIID", null);
                            options.model.set("objINCIBreakDownList", { id: null, text: "", valueA: "" });

                            this.value("");
                            this.text("");

                            return;
                        }

                        options.model.set("CASNo", item.valueA);
                        options.model.set("INCIID", item.id);
                        options.model.set("objINCIBreakDownList", {
                            id: item.id,
                            text: item.text,
                            valueA: item.valueA
                        });
                    }
                    else {
                        options.model.set("CASNo", "");
                        options.model.set("INCIID", null);
                        options.model.set("objINCIBreakDownList", { id: null, text: "", valueA: "" });
                    }
                }
            });
    },

    cboAllergensBreakDown: function (container, options) {
        $('<input data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container)
            .kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                dataSource: masterData.AllergensBreakDownList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);

                    var item = this.dataItem();

                    if (item) {
                        var grid = $(TblChildId_AllergensBreakdown).data("kendoGrid");
                        var data = grid.dataSource.data();

                        var isDuplicate = data.some(function (row) {
                            return row.uid !== options.model.uid
                                && row.objAllergensBreakDownList
                                && row.objAllergensBreakDownList.id == item.id;
                        });

                        if (isDuplicate) {
                            MessageHelper.Warning("Duplicate", "This Allergen Breakdown already selected.");

                            options.model.set("objAllergensBreakDownList", { id: null, text: "" });

                            this.value("");
                            this.text("");

                            return;
                        }

                        options.model.set("objAllergensBreakDownList", {
                            id: item.id,
                            text: item.text
                        });
                    }
                    else {
                        options.model.set("objAllergensBreakDownList", { id: null, text: "" });
                    }
                }
            });
    },

    cboSupplier: function (container, options) {
        $('<input id="cmbProduct" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.SupplierList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },

    cboLeadTimeUnit: function (container, options) {
        $('<input id="cmbProduct" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.LeadTimeUnitList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },


    cboCountry: function (container, options) {
        $('<input id="cmbProduct" data-text-field="text" data-value-field="id" data-bind="value:' + options.field + '"/>')
            .appendTo(container).kendoComboBox({
                filter: "contains",
                suggest: true,
                optionLabel: '',
                placeholder: "Select",
                // autoWidth: true,
                dataSource: masterData.CountryList,
                change: function (e) {
                    KendoGridHelper.RemoveUndefined(e);
                },
            });
    },



    GridCenterAlignAttributes: function (align_header = "", align_body = "", align_footer = "") {
        return {
            headerAttributes: { "class": align_header + " k_input_cell" },
            attributes: { "class": align_body + " k_input_cell" },
            footerAttributes: { "class": align_footer + " k_input_cell" }
        };
    },

    GridDuplicateCheck:  function (e) {
        if (e.values.objINCIBreakDownList) {

            var selectedId = e.values.objINCIBreakDownList.id;
            var data = grid.dataSource.data();

            var duplicate = data.some(function (item) {
                return item.uid !== e.model.uid
                    && item.objINCIBreakDownList
                    && item.objINCIBreakDownList.id == selectedId;
            });

            if (duplicate) {
                e.preventDefault();
                MessageHelper.Warning("Duplicate", "This item already exists.");

                setTimeout(function () {
                    e.model.set("objINCIBreakDownList", { id: null, text: "" });
                }, 0);
            }
        }
    }
}


