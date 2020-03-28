Public Class Zoomy
    Inherits Kaiju
    Implements KaijuBaseInterface
    Private enemyBaseHealth As Integer
    Private baseHealth As Integer
    Private startAgility As Integer
    Private startStrength As Integer
    Private firstRun As Boolean

    Public Sub New()
        strength = 40
        agility = 20
        toughness = 30
        intelligence = 10
        name = "Zoomy"
        teamName = "Unaffiliated"

        'add any variables you want to keep track of here
        firstRun = True

        'setup action types  
        Dim fightType As New ActionType("hurt", "health", "enemy")
        Dim strengthBoost As New ActionType("help", "strength", "self")
        Dim stunType As New ActionType("hurt", "stun", "enemy")
        Dim agilityBoost As New ActionType("help", "agility", "self")

        'define actions
        action1 = New ActionDefinition("strength", 0.2, "Blood Lust", strengthBoost)
        action2 = New ActionDefinition("agility", 0.2, "Blood Rage", agilityBoost)
        action3 = New ActionDefinition("strength", 1, "Sucker Punch", fightType)

        'define ultimate actions and ultimate
        Dim ultAction1 = New ActionDefinition("agility", 1, "Left Hook", stunType)
        Dim ultAction2 = New ActionDefinition("strength", 1, "Right Hook", fightType)
        ultimate = New UltimateActionDefinition("Double Punch", ultAction1, ultAction2)
    End Sub

    Public Overrides Function action(ByVal enemy As KaijuBaseInterface) As Decision

        If firstRun Then
            enemyBaseHealth = enemy.health
            baseHealth = health
            firstRun = False
            startAgility = agility
            startStrength = strength
        End If

        Dim returnDecision As New Decision

        If currentState = Constants.State.vulnerable Then
            returnDecision.newState = Constants.State.defending
        ElseIf currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.defending
        End If

        If superReady Then
            If enemy.currentState = Constants.State.vulnerable Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.stunned Then
                If agility = startAgility Then
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                ElseIf strength = startStrength Then
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action1
                Else
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = Nothing 'THIS IS IMPORTANT
                    returnDecision.ultimate = ultimate
                End If
            ElseIf enemy.currentState = Constants.State.waiting Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.attacking And (enemy.health < (enemyBaseHealth / 10) Or enemy.stunLevel >= 50) Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.defending And enemy.health <= (enemyBaseHealth / 10) Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            End If

            If returnDecision IsNot Nothing Then
                Return returnDecision
            End If
        End If


        If enemy.currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action2
        ElseIf enemy.currentState = Constants.State.stunned Then
            If agility = startAgility Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action2
            ElseIf strength = startStrength Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            Else
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action3
            End If
        ElseIf enemy.currentState = Constants.State.vulnerable Or enemy.currentState = Constants.State.defending Then
            If agility = startAgility Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action2
            ElseIf strength = startStrength Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            Else
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action3
            End If
        ElseIf enemy.currentState = Constants.State.attacking Then
            If health <= baseHealth * 0.2 Then
                returnDecision.newState = Constants.State.defending
            Else
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
            End If
        Else
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action3
        End If

        If returnDecision Is Nothing Then
            Console.WriteLine("Monster {0} had a null decision to return. Fix your AI.", name)
        End If

        Return returnDecision
    End Function
End Class

