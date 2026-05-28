using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

namespace RetailERP.Web.Controllers
{
    public class CustomWebDocumentViewerController : WebDocumentViewerController
    {
        public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
            : base(controllerService) { }
    } 
}    
