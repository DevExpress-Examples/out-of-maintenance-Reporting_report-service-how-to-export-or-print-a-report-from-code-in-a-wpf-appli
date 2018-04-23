using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Printing.Service;
using DevExpress.XtraReports.Service;
using DevExpress.XtraReports.UI;

namespace T148944.Web {
    [SilverlightFaultBehavior]
    public class DemoReportService : DevExpress.XtraReports.Service.ReportService {

    }
}
