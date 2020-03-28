
Public Class TheSilverAssassinPig
    Inherits Kaiju
    Implements KaijuBaseInterface

    Public count = 0

    Public Sub New()
        strength = 1
        agility = 79
        toughness = 15
        intelligence = 5

        name = "The Silver Assassin Pig"
        teamName = "The Silver Assassin Pig"


        'setup action types
        Dim fightType As New ActionType("hurt", "health", "enemy")
        Dim stunType As New ActionType("hurt", "stun", "enemy")

        Dim strengthUp As New ActionType("help", "strength", "self")
        Dim meditationType As New ActionType("help", "intelligence", "self")

        'define actions
        action1 = New ActionDefinition("agility", 0.35, "Predator Rush", fightType)
        action2 = New ActionDefinition("agility", 0.5, "Shadow of Nyx", strengthUp) 'can temp boost stack???????
        action3 = New ActionDefinition("strength", 1, "quick Strike", fightType)



        'define ultimate actions and ultimate
        Dim ultAction1 = New ActionDefinition("agility", 1, "Shadow of Nyx", fightType)
        Dim ultAction2 = New ActionDefinition("agility", 1, "quick Strike", stunType)
        ultimate = New UltimateActionDefinition("Beast Strike", ultAction1, ultAction2)

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
                    count = 0
                    returnDecision.ultimate = ultimate
                End If
            ElseIf enemy.currentState = Constants.State.waiting Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    count = 0
                    returnDecision.action = action1
                End If
            ElseIf enemy.currentState = Constants.State.defending Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    count = 0
                    returnDecision.action = action1
                End If
            ElseIf enemy.currentState = Constants.State.attacking Then
                returnDecision.newState = Constants.State.defending
            Else enemy.currentState = Constants.State.vulnerable
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing     'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            End If
        Else
            If enemy.currentState = Constants.State.attacking Then
                returnDecision.newState = Constants.State.defending
            ElseIf enemy.currentState = Constants.State.stunned Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    count = 0
                    returnDecision.action = action1
                End If
            ElseIf enemy.currentState = Constants.State.vulnerable Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    count = 0
                    returnDecision.action = action1
                End If
            ElseIf enemy.currentState = Constants.State.waiting Then
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    count = 0
                    returnDecision.action = action1
                End If
            Else enemy.currentState = Constants.State.defending
                If count < 2 Then 'while enemy stunned boost up for 3 turns then attack
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                    count += 1
                Else
                    returnDecision.newState = Constants.State.attacking
                    count = 0
                    returnDecision.action = action1
                End If
            End If
        End If
        If returnDecision Is Nothing Then
            Console.WriteLine("Monster {0} had a null decision to return. Fix your AI.", name)
        End If

        Return returnDecision

    End Function
End Class
