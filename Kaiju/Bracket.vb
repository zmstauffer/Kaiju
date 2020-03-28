Public Class Bracket
    Public monster1 As KaijuBaseInterface
    Public monster2 As KaijuBaseInterface
    Public winner As KaijuBaseInterface

    Sub New(ByRef m1, ByRef m2, ByRef win)
        monster1 = m1
        monster2 = m2
        winner = win
    End Sub

End Class
