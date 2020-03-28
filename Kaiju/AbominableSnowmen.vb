Public Class AbominableSnowmen
    Inherits Kaiju
    Implements KaijuBaseInterface
    Private enemyBaseHealth As Integer
    Private baseHealth As Integer
    Private startAgility As Integer
    Private firstRun As Boolean

    Public Sub New()
        strength = 1
        agility = 39
        toughness = 30
        intelligence = 30
        name = "Frostbite"
        teamName = "Abominable Snowmen"

        'add any variables you want to keep track of here
        firstRun = True

        'setup action types  
        Dim fightType As New ActionType("hurt", "health", "enemy")
        Dim healType As New ActionType("help", "health", "self")
        Dim stunType As New ActionType("hurt", "stun", "enemy")
        Dim agilityBoost As New ActionType("help", "agility", "self")

        'define actions
        action1 = New ActionDefinition("agility", 0.2, "Bloodlust", agilityBoost)
        action2 = New ActionDefinition("agility", 1, "Snowball", fightType)
        action3 = New ActionDefinition("intelligence", 1, "Healing Frost", healType)

        'define ultimate actions and ultimate
        Dim ultAction1 = New ActionDefinition("agility", 1, "Ice Lance", fightType)
        Dim ultAction2 = New ActionDefinition("intelligence", 1, "Brain Freeze", stunType)
        ultimate = New UltimateActionDefinition("Icy Blast", ultAction1, ultAction2)
    End Sub

    Public Overrides Function action(ByVal enemy As KaijuBaseInterface) As Decision

        If firstRun Then
            enemyBaseHealth = enemy.health
            baseHealth = health
            firstRun = False
            startAgility = agility
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
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.waiting Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.attacking And (enemy.health < (enemyBaseHealth / 10) Or enemy.stunLevel >= 50) Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            ElseIf enemy.currentState = Constants.State.defending And enemy.health <= (enemyBaseHealth / 20) Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = Nothing 'THIS IS IMPORTANT
                returnDecision.ultimate = ultimate
            End If

            If returnDecision IsNot Nothing Then
                Return returnDecision
            End If
        End If

        'healing first			
        If health < baseHealth / 2 Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action3
            Return returnDecision
        End If

        If enemy.currentState = Constants.State.waiting Then
            returnDecision.newState = Constants.State.attacking
            returnDecision.action = action1
        ElseIf enemy.currentState = Constants.State.vulnerable Or enemy.currentState = Constants.State.stunned Or enemy.currentState = Constants.State.waiting Or enemy.currentState = Constants.State.defending Then
            If health <= baseHealth / 2 Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action3
            Else
                If enemy.currentState = Constants.State.stunned And startAgility = agility Then
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action1
                Else
                    returnDecision.newState = Constants.State.attacking
                    returnDecision.action = action2
                End If
            End If
        ElseIf enemy.currentState = Constants.State.defending Then
            If health <= baseHealth * 0.95 Then
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action3
            Else
                returnDecision.newState = Constants.State.attacking
                returnDecision.action = action1
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

