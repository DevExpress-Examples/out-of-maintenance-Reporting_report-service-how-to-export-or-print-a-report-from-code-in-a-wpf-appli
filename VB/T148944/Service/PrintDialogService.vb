Imports System.Printing
Imports System.Windows.Controls
Imports DevExpress.Mvvm.UI

Namespace T148944.Service
    Public Class PrintDialogService
        Inherits ServiceBase
        Implements IPrintDialogService

        Private ReadOnly dialog As New PrintDialog()

        Public ReadOnly Property PrintQueue() As PrintQueue Implements IPrintDialogService.PrintQueue
            Get
                Return dialog.PrintQueue
            End Get
        End Property

        Public Function ShowDialog() As Boolean Implements IPrintDialogService.ShowDialog
            Return dialog.ShowDialog() = True
        End Function
    End Class
End Namespace
