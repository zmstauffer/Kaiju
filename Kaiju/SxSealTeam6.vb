Public Class SxSealTeam6
    Inherits Kaiju
    Implements KaijuBaseInterface

    Public Sub New()
        strength = 40
        agility = 15
        toughness = 35
        intelligence = 10
        name = "TS-3000"
        teamName = "SxSeal Team 6"

        'setup action types
        Dim fightType As New ActionType("hurt", "health", "enemy")
        Dim stunType As New ActionType("hurt", "stun", "enemy")

        Dim toughType As New ActionType("help", "toughness", "self")
        Dim meditationType As New ActionType("help", "intelligence", "self")

        'define actions
        action1 = New ActionDefinition("strength", 0.5, "Tusk Slash", fightType)
        action2 = New ActionDefinition("toughness", 0.25, "Blubber Quake", stunType)
        action3 = New ActionDefinition("toughness", 0.33, "Double Hammer Fin Slap", meditationType)



        'define ultimate actions and ultimate
        Dim ultAction1 = New ActionDefinition("toughness", 1.0, "Ultra Mega Belly Flop", stunType)
        Dim ultAction2 = New ActionDefinition("strength", 1.0, "Ultra Mega Sulfur Jet", fightType)
        ultimate = New UltimateActionDefinition("Ultra Mega Sulfur Jet", ultAction1, ultAction2)

    End Sub

    Public Overrides Function action(ByVal enemy As KaijuBaseInterface) As Decision
        Dim returnDecision As New Decision

        If superReady = True Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = Nothing
            returnDecision.ultimate = ultimate
            Return returnDecision
        End If
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If enemy.currentState = Constants.State.attacking Then
            returnDecision.newState = Constants.State.defending
            '''''''''''''''''''''''''
        ElseIf currentState = Constants.State.defending And enemy.health >= 1.25 * health Then
            If enemy.currentState = Constants.State.stunned Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            Else
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action2
            End If
        ElseIf currentState = Constants.State.defending And enemy.health < 1.25 * health Then
            If enemy.currentState = Constants.State.stunned Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            Else
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action2
            End If
        ElseIf enemy.currentState = Constants.State.defending And enemy.health >= health Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action3
        ElseIf enemy.currentState = Constants.State.defending And enemy.health < health Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
            ''''''''''''''''''''''''
        ElseIf currentState = Constants.State.vulnerable Then
            returnDecision.newState = Constants.State.defending
        ElseIf currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.defending
        ElseIf enemy.currentState = Constants.State.stunned Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        ElseIf enemy.currentState = Constants.State.vulnerable Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        ElseIf enemy.currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        Else
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        End If
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        If returnDecision Is Nothing Then
            Console.WriteLine("Monster {0} had a null decision to return. Fix your AI.", name)
        End If

        Return returnDecision

    End Function
End Class
