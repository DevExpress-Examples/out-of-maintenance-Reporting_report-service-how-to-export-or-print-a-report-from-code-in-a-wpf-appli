Imports System.Printing

Namespace T148944.Service
    Public Interface IPrintDialogService
        ReadOnly Property PrintQueue() As PrintQueue
        Function ShowDialog() As Boolean
    End Interface
End Namespace
