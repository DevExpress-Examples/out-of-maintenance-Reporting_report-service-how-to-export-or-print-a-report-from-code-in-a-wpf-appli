using System.IO;
using System.Printing;
using System.ServiceModel;
using System.Threading.Tasks;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraPrinting;
using T148944.Service;

namespace T148944.ViewModel {
    public class MainViewModel {
        readonly string ReportName = "Reports.SampleReport, Reports";
        readonly string ReportServiceAddress = "http://localhost:64790/DemoReportService.svc";

        public virtual bool IsBusy { get; protected set; }
        protected void OnIsBusyChanged() {
            this.RaiseCanExecuteChanged(x => x.Print());
            this.RaiseCanExecuteChanged(x => x.Export());
        }

        public virtual string ReportParameter { get; set; }

        protected virtual IMessageBoxService MessageBoxService { get { return null; } }
        protected virtual ISaveFileDialogService SaveFileDialogService { get { return null; } }
        protected virtual IPrintDialogService PrintDialogService { get { return null; } }

        public MainViewModel() {
            ReportParameter = "Report Parameter: default value";
        }

        public void Print() {
            if(PrintDialogService.ShowDialog()) {
                IsBusy = true;
                ExportReport(new XpsExportOptions())
                    .ContinueWith(ExportToXpsCompleted, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        void ExportToXpsCompleted(Task<byte[]> task) {
            IsBusy = false;
            if(TaskIsFauledOrCancelled(task, "Print"))
                return;
            using(PrintSystemJobInfo jobInfo = PrintDialogService.PrintQueue.AddJob("Print Job Name")) {
                jobInfo.JobStream.Write(task.Result, 0, task.Result.Length);
            }
        }

        public bool CanPrint() {
            return !IsBusy;
        }

        public void Export() {
            SaveFileDialogService.Filter = "PDF files (*.pdf)|*.pdf";
            if(SaveFileDialogService.ShowDialog()) {
                IsBusy = true;
                ExportReport(new PdfExportOptions())
                    .ContinueWith(ExportToPdfCompleted, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        void ExportToPdfCompleted(Task<byte[]> task) {
            IsBusy = false;
            if(TaskIsFauledOrCancelled(task, "Export"))
                return;
            using(Stream stream = SaveFileDialogService.OpenFile()) {
                stream.Write(task.Result, 0, task.Result.Length);
            }
        }

        public bool CanExport() {
            return !IsBusy;
        }

        Task<byte[]> ExportReport(ExportOptionsBase exportOptions) {
            ReportServiceClientFactory clientFactory = new ReportServiceClientFactory(new EndpointAddress(ReportServiceAddress));
            ReportParameter[] reportParameters = new ReportParameter[] {
                new ReportParameter() { Path = "stringParameter", Value = ReportParameter }
            };

            return Task.Factory.ExportReportAsync(clientFactory.Create(), ReportName, exportOptions, reportParameters, null);
        }

        bool TaskIsFauledOrCancelled(Task task, string caption) {
            if(task.IsFaulted) {
                MessageBoxService.Show(task.Exception.Message, caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return true;
            }

            if(task.IsCanceled) {
                MessageBoxService.Show("Operation has been cancelled", caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
                return true;
            }

            return false;
        }
    }
}
