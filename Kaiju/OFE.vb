
Public Class OFE
    Inherits Kaiju
    Implements KaijuBaseInterface

    Public count = 0

    Public Sub New()
        strength = 50
        agility = 10
        toughness = 30
        intelligence = 10
        name = "Gozer the Stay Puft Marshmallow Man"
        teamName = "OFE"


        'setup action types
        Dim fightType As New ActionType("hurt", "health", "enemy")
        Dim stunType As New ActionType("hurt", "stun", "enemy")

        Dim strengthUp As New ActionType("help", "strength", "self")
        Dim meditationType As New ActionType("help", "intelligence", "self")

        'define actions
        action1 = New ActionDefinition("strength", 0.45, "Flaming Marshmallow", fightType)
        action2 = New ActionDefinition("strength", 1, "Absorb", strengthUp) 'can temp boost stack???????
        action3 = New ActionDefinition("agility", 0.35, "Sticky", stunType)



        'define ultimate actions and ultimate
        Dim ultAction1 = New ActionDefinition("strength", 1, "Flaming Marshmallow", fightType)
        Dim ultAction2 = New ActionDefinition("strength", 1, "Flaming Marshmallow", fightType)
        ultimate = New UltimateActionDefinition("Smores Smash", ultAction1, ultAction2)

    End Sub

    Public Overrides Function action(ByVal enemy As KaijuBaseInterface) As Decision
        Dim returnDecision As New Decision

        'put your AI here. Make sure to assign your decision to returnDecision
        'to check yourself
        If superReady = True Then

            If enemy.currentState = Constants.State.stunned Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = Nothing     'THIS IS IMPORTANT
                    returnDecision.ultimate = ultimate
                End If
            ElseIf enemy.currentState = Constants.State.waiting Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing     'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.defending Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing     'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.attacking Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing     'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            Else enemy.currentState = Constants.State.vulnerable
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing     'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            End If
        Else
            If enemy.currentState = Constants.State.stunned Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action1
                End If
            ElseIf enemy.currentState = Constants.State.waiting Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            ElseIf enemy.currentState = Constants.State.defending Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action2
            ElseIf enemy.currentState = Constants.State.attacking Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action3
            Else enemy.currentState = Constants.State.vulnerable
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            End If
        End If
        'If enemy.currentState = Constants.State.waiting Then
        'returnDecision.newState = Constants.State.attacking
        'returnDecision.action = action1
        'ElseIf enemy.currentState = Constants.State.vulnerable Then
        ' returnDecision.newState = Constants.State.attacking
        ' returnDecision.action = action1
        ' ElseIf enemy.currentState = Constants.State.defending Then
        ' returnDecision.newState = Constants.State.attacking
        'returnDecision.action = action3
        'ElseIf enemy.currentState = Constants.State.stunned Then
        'If enemy.stunLevel > 50 Then
        '    'do ultimate
        '    '*************************************************************************************
        '    'check ultimate meter 
        '    If Ultimate meter = full Then
        '            returnDecision.newState = Constants.State.attacking
        '        returnDecision.action = Nothing     'THIS IS IMPORTANT
        '        returnDecision.ultimate = ultimate

        '    Else returnDecision.newState = Constants.State.attacking
        '        returnDecision.action = action2
        '    End If
        '    '*************************************************************************************
        'End If
        ' If count < 5 Then 'while enemy stunned boost up for 4 turns then attack
        'returnDecision.newState = Constants.State.attacking
        'returnDecision.action = action2
        ' count += 1
        ' Else
        'returnDecision.newState = Constants.State.attacking
        'returnDecision.action = action1
        'End If

        'ElseIf enemy.stunLevel <= 50 Then
        'do ultimate
        ' returnDecision.newState = Constants.State.attacking
        ' returnDecision.action = action2

        'Else
        'returnDecision.newState = Constants.State.attacking
        'returnDecision.action = action1
        'End If

        If returnDecision Is Nothing Then
            Console.WriteLine("Monster {0} had a null decision to return. Fix your AI.", name)
        End If

        Return returnDecision

    End Function
End Class
