
Public Class Bombers
    Inherits Kaiju
    Implements KaijuBaseInterface

    Public Sub New()
        strength = 40
        agility = 20
        toughness = 30
        intelligence = 10
        name = "Billy"
        teamName = "Robot Overlords"

        'setup action types
        Dim fightType As New ActionType("hurt", "health", "enemy")
        Dim stunType As New ActionType("hurt", "stun", "enemy")

        'define actions
        action1 = New ActionDefinition("strength", 0.2, "Jab", fightType)
        action2 = New ActionDefinition("strength", 0.6, "Uppercut", fightType)
        action3 = New ActionDefinition("strength", 1, "Headbutt", fightType)

        'define ultimate actions and ultimate
        Dim ultAction1 = New ActionDefinition("strength", 1, "Zenith Charge", fightType)
        Dim ultAction2 = New ActionDefinition("toughness", 1, "Electrical Overload", stunType)
        ultimate = New UltimateActionDefinition("Zenith Charge", ultAction1, ultAction2)

    End Sub

    Public Overrides Function action(ByVal enemy As KaijuBaseInterface) As Decision
        Dim returnDecision As New Decision

        If superReady Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = Nothing
            returnDecision.ultimate = ultimate
            Return returnDecision
        End If

        If currentState = Constants.State.vulnerable Then
            returnDecision.newState = Constants.State.defending
        ElseIf currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.defending
        ElseIf enemy.currentState = Constants.State.stunned Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        ElseIf enemy.currentState = Constants.State.vulnerable Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action3
        ElseIf enemy.currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action2
        Else
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        End If

        If returnDecision Is Nothing Then
            Console.WriteLine("Monster {0} had a null decision to return. Fix your AI.", name)
        End If

        Return returnDecision

    End Function
End Class
