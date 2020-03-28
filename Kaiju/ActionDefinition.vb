Public Class ActionDefinition
    Public primaryStat As String
    Public statPower As Single
    Public name As String
    Public myType As ActionType

    Sub New()

    End Sub

    Sub New(ByVal stat As String, ByVal power As Single, ByVal attackName As String, ByVal myType As ActionType)
        'do validation on entries
        If attackName = "" Then
            Console.WriteLine("An action definition didn't have a name and failed for: {0}", Me.name)
            shutdown()
        End If

        If stat <> "strength" And stat <> "agility" And stat <> "toughness" And stat <> "intelligence" Then
            Console.WriteLine("An action definition didn't have a correct stat for: {0}", attackName)
            shutdown()
        End If

        If power <= 0 Or power > 1 Then
            Console.WriteLine("Power for action {0} needs to be between 0.01 and 1", attackName)
            shutdown()
        End If

        If myType Is Nothing Then
            Console.WriteLine("Action {0} doesn't have a correct actionType.", attackName)
            shutdown()
        End If

        With Me
            .name = attackName
            .primaryStat = stat
            .statPower = power
            .myType = myType
        End With

    End Sub

End Class

Public Class ActionType
    Public dmg As String
    Public affectedStat As String
    Public target As String

    Sub New(ByVal attackDmg As String, ByVal stat As String, ByVal myTarget As String)
        'validate entries
        If attackDmg <> "help" And attackDmg <> "hurt" Then
            Console.WriteLine("Invalid ActionType attempted. Must be help or hurt.")
            shutdown()
        End If

        If stat <> "strength" And stat <> "agility" And stat <> "toughness" And stat <> "intelligence" And stat <> "health" And stat <> "stun" Then
            Console.WriteLine("Invalid affectedStat attempted.")
            shutdown()
        End If

        If myTarget <> "self" And myTarget <> "enemy" Then
            Console.WriteLine("Invalid target for attackType")
            shutdown()
        End If

        With Me
            .dmg = attackDmg
            .affectedStat = stat
            .target = myTarget
        End With

    End Sub

End Class

Public Class UltimateActionDefinition 

    Public name As String
    Public action1 As ActionDefinition
    Public action2 As ActionDefinition

    Sub New(ByVal n As String, ByRef att1 As ActionDefinition, ByRef att2 As ActionDefinition)

        If att1 Is Nothing Or att2 Is Nothing Or n = "" Then
            Console.WriteLine("Error creating ultimate.")
            shutdown()
        End If

        name = n
        action1 = att1
        action2 = att2
    End Sub

End Class

Public Class Decision

    Public newState As Constants.State
    Public action As ActionDefinition
    Public ultimate As UltimateActionDefinition

End Class