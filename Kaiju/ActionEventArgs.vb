Public Class ActionEventArgs
    Inherits EventArgs

    Public action As ActionDefinition

    Public Sub New(ByRef m As Kaiju, ByRef act As ActionDefinition)
        action = act
    End Sub
End Class
