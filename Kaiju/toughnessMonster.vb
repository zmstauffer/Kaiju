Public Class toughnessMonster
    Inherits Kaiju
    Implements KaijuBaseInterface

    Public Sub New()
        strength = 20
        agility = 20
        toughness = 40
        intelligence = 20
        name = "Toughness Monster"
        teamName = "Testing"

        'setup action types
        Dim fightType As New ActionType("hurt", "health", "enemy")

        'define actions
        action1 = New ActionDefinition("toughness", 1, "Toughness Strike", fightType)
        action2 = New ActionDefinition("strength", 1, "NONE", fightType)
        action3 = New ActionDefinition("strength", 1, "NONE", fightType)

        Dim ultAction1 = New ActionDefinition("toughness", 1, "Ult Strike", fightType)
        ultimate = New UltimateActionDefinition("NONE", ultAction1, ultAction1)

    End Sub

    Public Overrides Function action(ByVal enemy As KaijuBaseInterface) As Decision
        Dim returnDecision As New Decision

        If superReady Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = Nothing
            returnDecision.ultimate = ultimate
            Return returnDecision
        End If

        returnDecision.newState = Constants.State.attacking
        returnDecision.action = action1

        If returnDecision Is Nothing Then
            Console.WriteLine("Monster {0} had a null decision to return. Fix your AI.", name)
        End If

        Return returnDecision

    End Function
End Class
