Imports System.IO
Imports System.Printing
Imports System.ServiceModel
Imports System.Threading.Tasks
Imports DevExpress.DocumentServices.ServiceModel
Imports DevExpress.DocumentServices.ServiceModel.Client
Imports DevExpress.DocumentServices.ServiceModel.DataContracts
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.POCO
Imports DevExpress.XtraPrinting
Imports T148944.Service

Namespace T148944.ViewModel
    Public Class MainViewModel
        Private ReadOnly ReportName As String = "Reports.SampleReport, Reports"
        Private ReadOnly ReportServiceAddress As String = "http://localhost:64790/DemoReportService.svc"

        Private privateIsBusy As Boolean
        Public Overridable Property IsBusy() As Boolean
            Get
                Return privateIsBusy
            End Get
            Protected Set(ByVal value As Boolean)
                privateIsBusy = value
            End Set
        End Property
        Protected Sub OnIsBusyChanged()
            Me.RaiseCanExecuteChanged(Sub(x) x.Print())
            Me.RaiseCanExecuteChanged(Sub(x) x.Export())
        End Sub

        Public Overridable Property ReportParameter() As String

        Protected Overridable ReadOnly Property MessageBoxService() As IMessageBoxService
            Get
                Return Nothing
            End Get
        End Property
        Protected Overridable ReadOnly Property SaveFileDialogService() As ISaveFileDialogService
            Get
                Return Nothing
            End Get
        End Property
        Protected Overridable ReadOnly Property PrintDialogService() As IPrintDialogService
            Get
                Return Nothing
            End Get
        End Property

        Public Sub New()
            ReportParameter = "Report Parameter: default value"
        End Sub

        Public Sub Print()
            If PrintDialogService.ShowDialog() Then
                IsBusy = True
                ExportReport(New XpsExportOptions()).ContinueWith(AddressOf ExportToXpsCompleted, TaskScheduler.FromCurrentSynchronizationContext())
            End If
        End Sub

        Private Sub ExportToXpsCompleted(ByVal task As Task(Of Byte()))
            IsBusy = False
            If TaskIsFauledOrCancelled(task, "Print") Then
                Return
            End If
            Using jobInfo As PrintSystemJobInfo = PrintDialogService.PrintQueue.AddJob("Print Job Name")
                jobInfo.JobStream.Write(task.Result, 0, task.Result.Length)
            End Using
        End Sub

        Public Function CanPrint() As Boolean
            Return Not IsBusy
        End Function

        Public Sub Export()
            SaveFileDialogService.Filter = "PDF files (*.pdf)|*.pdf"
            If SaveFileDialogService.ShowDialog() Then
                IsBusy = True
                ExportReport(New PdfExportOptions()).ContinueWith(AddressOf ExportToPdfCompleted, TaskScheduler.FromCurrentSynchronizationContext())
            End If
        End Sub

        Private Sub ExportToPdfCompleted(ByVal task As Task(Of Byte()))
            IsBusy = False
            If TaskIsFauledOrCancelled(task, "Export") Then
                Return
            End If
            Using stream As Stream = SaveFileDialogService.OpenFile()
                stream.Write(task.Result, 0, task.Result.Length)
            End Using
        End Sub

        Public Function CanExport() As Boolean
            Return Not IsBusy
        End Function

        Private Function ExportReport(ByVal exportOptions As ExportOptionsBase) As Task(Of Byte())
            Dim clientFactory As New ReportServiceClientFactory(New EndpointAddress(ReportServiceAddress))
            Dim reportParameters() As ReportParameter = { _
                New ReportParameter() With {.Path = "stringParameter", .Value = ReportParameter} _
            }

            Return Task.Factory.ExportReportAsync(clientFactory.Create(), ReportName, exportOptions, reportParameters, Nothing)
        End Function

        Private Function TaskIsFauledOrCancelled(ByVal task As Task, ByVal caption As String) As Boolean
            If task.IsFaulted Then
                MessageBoxService.Show(task.Exception.Message, caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error)
                Return True
            End If

            If task.IsCanceled Then
                MessageBoxService.Show("Operation has been cancelled", caption, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation)
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
