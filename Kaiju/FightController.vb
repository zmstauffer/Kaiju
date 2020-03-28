Imports System.Timers

Public Class FightController
    Public monster1 As Kaiju
    Public monster2 As Kaiju

    Private pic As Graphics

    'monster specific timers
    Private m1StunDuration As Timer
    Private m1StunLevelChangeTimer As Timer
    Private m1StateChangeTimer As Timer

    Private m2StunDuration As Timer
    Private m2StunLevelChangeTimer As Timer
    Private m2StateChangeTimer As Timer

    Public Sub New(ByRef m1 As Kaiju, ByRef m2 As Kaiju, ByRef p As Graphics)
        monster1 = m1
        monster2 = m2
        pic = p

        'setup stun timers
        m1StunLevelChangeTimer = New Timer(Constants.stunTimer / monster1.toughness)
        AddHandler m1StunLevelChangeTimer.Elapsed, New ElapsedEventHandler(AddressOf m1StunTimerElapsed)
        m1StunLevelChangeTimer.AutoReset = True
        m1StunLevelChangeTimer.Start()

        m2StunLevelChangeTimer = New Timer(Constants.stunTimer / monster2.toughness)
        AddHandler m2StunLevelChangeTimer.Elapsed, New ElapsedEventHandler(AddressOf m2StunTimerElapsed)
        m2StunLevelChangeTimer.AutoReset = True
        m2StunLevelChangeTimer.Start()

        'setup state timers
        m1StateChangeTimer = New Timer()
        AddHandler m1StateChangeTimer.Elapsed, New ElapsedEventHandler(AddressOf m1StateChangeTimerElapsed)

        m2StateChangeTimer = New Timer()
        AddHandler m2StateChangeTimer.Elapsed, New ElapsedEventHandler(AddressOf m2StateChangeTimerElapsed)

        'clear screen and display monster cards
        Console.Clear()
        pic.drawMonsterCard(m1, 1)
        pic.drawVS()
        pic.drawMonsterCard(m2, 70)

    End Sub

    Public Function fight()

        While monster1.health > 0 And monster2.health > 0
            'lower any temp values if they have expired
            monster1.resetBaseValues()
            monster2.resetBaseValues()

            'roll for initiative
            Dim init1 As Single
            Dim init2 As Single

            calcInitiative(monster1)
            calcInitiative(monster2)

            If monster1.currentState = Constants.State.stunned Then
                init1 = 0
                init2 = 100
            ElseIf monster2.currentState = Constants.State.stunned Then
                init1 = 100
                init2 = 0
            Else
                init1 = rand(20) + monster1.initiative
                init2 = rand(20) + monster2.initiative
            End If

            'make monster decision
            Dim m1Decision As Decision = Nothing
            Dim m2Decision As Decision = Nothing

            If monster1.currentState = Constants.State.waiting Or monster1.currentState = Constants.State.defending Then
                m1Decision = monster1.action(monster2)
            End If
            If monster2.currentState = Constants.State.waiting Or monster2.currentState = Constants.State.defending Then
                m2Decision = monster2.action(monster1)
            End If

            'process decisions in initiative order
            If init1 >= init2 Then
                If monster1.currentState <> Constants.State.stunned Then processDecision(monster1, m1Decision)
                If monster2.currentState <> Constants.State.stunned Then processDecision(monster2, m2Decision)
            Else
                If monster2.currentState <> Constants.State.stunned Then processDecision(monster2, m2Decision)
                If monster1.currentState <> Constants.State.stunned Then processDecision(monster1, m1Decision)
            End If

                'check supermeters
                If monster1.superMeter >= 100 Then
                monster1.superReady = True
            End If

            If monster2.superMeter >= 100 Then
                monster2.superReady = True
            End If

            'update console
            If pic.needsUpdate Then
                Console.CursorVisible = False
                pic.updateMonsterCard(monster1, 1)
                pic.updateMonsterCard(monster2, 70)
                'pic.drawVS()
                pic.outputEventList()
            End If

        End While

        'turn off timers
        m1StunLevelChangeTimer.Stop()
        m2StunLevelChangeTimer.Stop()
        'return the winner
        If monster1.health > 0 Then
            Return monster1
        Else
            Return monster2
        End If

        Return Nothing
    End Function

    Private Sub processDecision(ByRef monster As Kaiju, ByRef dec As Decision)

        If dec Is Nothing Then Exit Sub

        monster.numDecisions += 1

        Select Case dec.newState
            Case Constants.State.defending
                monster.currentState = Constants.State.defending
                monster.lastAction = "Went defensive"
                'pic.addEvent(monster.name & "went defensive...")
            Case Constants.State.waiting
                monster.currentState = Constants.State.waiting
                monster.lastAction = "Changed to Waiting"
            Case Constants.State.attacking
                If dec.action IsNot Nothing Then
                    monster.currentState = Constants.State.vulnerable
                    monster.nextState = Constants.State.waiting
                    monster.lastAction = dec.action.name

                    If monster.name = monster1.name Then
                        m1StateChangeTimer.Interval = dec.action.statPower * Constants.vulnerableTimeMultiplier - (monster.intelligence * Constants.timeIntelligenceModifier)
                        m1StateChangeTimer.Enabled = True
                        monster.vulnerableTime = m1StateChangeTimer.Interval
                    ElseIf monster.name = monster2.name Then
                        m2StateChangeTimer.Interval = dec.action.statPower * Constants.vulnerableTimeMultiplier - (monster.intelligence * Constants.timeIntelligenceModifier)
                        m2StateChangeTimer.Enabled = True
                        monster.vulnerableTime = m2StateChangeTimer.Interval
                    End If

                    'do the action
                    If dec.action.myType.target = "self" Then
                        'affecting myself
                        attackSelf(monster, dec.action)
                    Else
                        'attacking enemy
                        attackEnemy(monster, dec.action)
                    End If

                ElseIf dec.ultimate IsNot Nothing Then
                    If monster.superReady Then
                        monster.currentState = Constants.State.vulnerable
                        monster.nextState = Constants.State.waiting
                        monster.lastAction = dec.ultimate.name

                        If monster.name = monster1.name Then
                            m1StateChangeTimer.Interval = (dec.ultimate.action1.statPower + dec.ultimate.action2.statPower) / 2 * Constants.vulnerableTimeMultiplier
                            m1StateChangeTimer.Enabled = True
                            monster.vulnerableTime = m1StateChangeTimer.Interval
                        ElseIf monster.name = monster2.name Then
                            m2StateChangeTimer.Interval = (dec.ultimate.action1.statPower + dec.ultimate.action2.statPower) / 2 * Constants.vulnerableTimeMultiplier
                            m2StateChangeTimer.Enabled = True
                            monster.vulnerableTime = m2StateChangeTimer.Interval
                        End If

                        'handle ultimate
                        If dec.ultimate.action1.myType.target = "self" Then
                            attackSelf(monster, dec.ultimate.action1)
                        Else
                            attackEnemy(monster, dec.ultimate.action1)
                        End If

                        If dec.ultimate.action2.myType.target = "self" Then
                            attackSelf(monster, dec.ultimate.action2)
                        Else
                            attackEnemy(monster, dec.ultimate.action2)
                        End If
                        monster.superMeter = 0
                        monster.superReady = False
                    Else
                        monster.currentState = Constants.State.waiting
                    End If

                Else
                    Console.WriteLine("Chose attack w/out specifying which attack. Shutting down.")
                    shutdown()
                End If

        End Select

        pic.needsUpdate = True

    End Sub

    Public Sub attackSelf(ByRef monster As Kaiju, ByVal action As ActionDefinition)

        Dim stat As Single = Nothing
        Select Case action.primaryStat
            Case "strength"
                stat = monster.baseStrength.value
            Case "agility"
                stat = monster.baseAgility.value
            Case "toughness"
                stat = monster.baseToughness.value
            Case "intelligence"
                stat = monster.baseIntelligence.value
            Case Else
                Console.WriteLine("incorrect primary stat for {0}.", action.name)
        End Select

        Dim power As Single = Math.Round(stat * action.statPower * stat / 10 * Constants.attackModifier)

        If action.myType.dmg = "help" Then
            Select Case action.myType.affectedStat
                Case "strength"
                    monster.baseStrength.time = Date.Now
                    monster.baseStrength.time = monster.baseStrength.time.AddMilliseconds(power * Constants.posStatTimeMultiplier)
                    monster.strength += power * Constants.posStatMultiplier
                    If monster.strength > monster.baseStrength.value * Constants.maxStatBoostMultiplier Then monster.strength = monster.baseStrength.value * Constants.maxStatBoostMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on itself and raised strength by " & power & "!")
                Case "agility"
                    monster.baseAgility.time = Date.Now
                    monster.baseAgility.time = monster.baseAgility.time.AddMilliseconds(power * Constants.posStatTimeMultiplier)
                    monster.agility += power * Constants.posStatMultiplier
                    If monster.agility > monster.baseAgility.value * Constants.maxStatBoostMultiplier Then monster.agility = monster.baseAgility.value * Constants.maxStatBoostMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on itself and raised agility by " & power & "!")
                Case "toughness"
                    monster.baseToughness.time = Date.Now
                    monster.baseToughness.time = monster.baseToughness.time.AddMilliseconds(power * Constants.posStatTimeMultiplier)
                    monster.toughness += power * Constants.posStatMultiplier
                    If monster.toughness > monster.baseToughness.value * Constants.maxStatBoostMultiplier Then monster.toughness = monster.baseToughness.value * Constants.maxStatBoostMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on itself and raised toughness by " & power & "!")
                Case "intelligence"
                    monster.baseIntelligence.time = Date.Now
                    monster.baseIntelligence.time = monster.baseIntelligence.time.AddMilliseconds(power * Constants.posStatTimeMultiplier)
                    monster.intelligence += power * Constants.posStatMultiplier
                    If monster.intelligence > monster.baseIntelligence.value * Constants.maxStatBoostMultiplier Then monster.intelligence = monster.baseIntelligence.value * Constants.maxStatBoostMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on itself and raised intelligence by " & power & "!")
                Case "health"
                    monster.health += power
                    If monster.health > monster.maxHealth Then monster.health = monster.maxHealth
                    pic.addEvent(monster.name & " used " & action.name & " on itself and increased health by " & power & "!")
            End Select
        End If

        'monster gets supermeter on successful attack
        monster.superMeter += power

    End Sub

    Public Sub attackEnemy(ByRef monster As Kaiju, ByRef action As ActionDefinition)
        Dim target As Kaiju = Nothing
        Dim actualPower As Integer = 0
        If monster.name = monster1.name Then
            target = monster2
        Else
            target = monster1
        End If

        'get the stat we are using to calculate power and then calculate it
        Dim stat As Single = Nothing
        Select Case action.primaryStat
            Case "strength"
                stat = monster.strength
            Case "agility"
                stat = monster.agility
            Case "toughness"
                stat = monster.toughness
            Case "intelligence"
                stat = monster.intelligence
            Case Else
                Console.WriteLine("incorrect primary stat for {0}.", action.name)
        End Select

        Dim power As Single = Math.Round((stat * action.statPower + (monster.strength / 10)) * Constants.attackModifier)
        If action.primaryStat = "strength" Then power = Math.Round(power * 1.3)

        If target.currentState = Constants.State.defending Then
            power = Math.Round(power / Constants.defendingPowerReduction)
        End If

        Dim monsterAccuracy As Single = Constants.baseAccuracy + (2 - action.statPower) * monster.agility + (2 - action.statPower) * rand(monster.intelligence) 'monster.intelligence / 1.6

        'monster gets chance to dodge attack if defending, waiting, or vulnerable
        Dim dodgeChance As Single = 0
        If target.currentState = Constants.State.defending Then
            dodgeChance = (Constants.dodgeAgilityModifier * target.agility + target.intelligence / Constants.dodgeIntelligenceModifier) * Constants.defendingMultiplier
        ElseIf target.currentState = Constants.State.vulnerable Then
            dodgeChance = (Constants.dodgeAgilityModifier * target.agility + target.intelligence / Constants.dodgeIntelligenceModifier) * Constants.vulnerableMultiplier
        Else
            dodgeChance = (Constants.dodgeAgilityModifier * target.agility + target.intelligence / Constants.dodgeIntelligenceModifier) * Constants.waitingMultiplier
        End If

        dodgeChance = dodgeChance + rand(monster.intelligence)               'dodgeChance / Constants.dodgeMultiplier + rand(100)  

        If monsterAccuracy <= dodgeChance And target.currentState <> Constants.State.stunned Then                   'if stunned, cannot dodge
            pic.addEvent(monster.name & " performed " & action.name & " but " & target.name & " was able to dodge!")
            target.superMeter += power          'get supermeter if you dodge
            Exit Sub
        End If

        If action.myType.dmg = "hurt" Then
            Select Case action.myType.affectedStat
                Case "strength"
                    target.baseStrength.time = Date.Now
                    target.baseStrength.time = target.baseStrength.time.AddMilliseconds(power * Constants.negStatTimeMultiplier)
                    target.strength -= power * Constants.negStatMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and lowered strength by " & power & "!")
                Case "agility"
                    target.baseAgility.time = Date.Now
                    target.baseAgility.time = target.baseAgility.time.AddMilliseconds(power * Constants.negStatTimeMultiplier)
                    target.agility -= power * Constants.negStatMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and lowered agility by " & power & "!")
                Case "toughness"
                    target.baseToughness.time = Date.Now
                    target.baseToughness.time = target.baseToughness.time.AddMilliseconds(power * Constants.negStatTimeMultiplier)
                    target.toughness -= power * Constants.negStatMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and lowered toughness by " & power & "!")
                Case "intelligence"
                    target.baseIntelligence.time = Date.Now
                    target.baseIntelligence.time = target.baseIntelligence.time.AddMilliseconds(power * Constants.negStatTimeMultiplier)
                    target.intelligence -= power * Constants.negStatMultiplier
                    pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and lowered intelligence by " & power & "!")
                Case "health"
                    If target.currentState = Constants.State.stunned Then
                        target.health -= Math.Round(power * Constants.stunDamageBonus)
                        target.currentState = Constants.State.waiting
                        target.stunLevel = 0
                        pic.addEvent(monster.name & " used " & action.name & " on a STUNNED " & target.name & " and hit for " & Math.Round(power * Constants.stunDamageBonus) & "!")
                    Else
                        Select Case target.currentState
                            Case Constants.State.defending
                                target.health -= power
                                target.stunLevel += power * Constants.stunAmount
                            Case Constants.State.vulnerable
                                target.health -= power * 1.25
                                target.stunLevel += power * Constants.stunAmount * 1.25
                            Case Else
                                target.health -= power
                                target.stunLevel += power * Constants.stunAmount
                        End Select
                        pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and hit for " & power & "!")
                    End If
                Case "stun"
                    Select Case target.currentState
                        Case Constants.State.defending
                            target.stunLevel += power * Constants.stunAmount * 5 * 0.75
                            pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and increased stun level by " & power * Constants.stunAmount * 5 * 0.75 & "!")
                        Case Constants.State.vulnerable
                            target.stunLevel += power * Constants.stunAmount * 5 * 1.25
                            pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and increased stun level by " & power * Constants.stunAmount * 5 * 1.25 & "!")
                        Case Else
                            target.stunLevel += power * Constants.stunAmount * 5
                            pic.addEvent(monster.name & " used " & action.name & " on " & target.name & " and increased stun level by " & power * Constants.stunAmount * 5 & "!")
                    End Select
            End Select
        End If

        'check for stun
        If target.stunLevel >= target.toughness * Constants.maxStun Then
            target.currentState = Constants.State.stunned
            target.nextState = Constants.State.waiting
            If target.name = monster1.name Then
                m1StateChangeTimer.Interval = Constants.stunReset / target.toughness
                m1StateChangeTimer.Enabled = True
                monster.vulnerableTime = m1StateChangeTimer.Interval
            ElseIf target.name = monster2.name Then
                m2StateChangeTimer.Interval = Constants.stunReset / target.toughness
                m2StateChangeTimer.Enabled = True
                monster.vulnerableTime = m2StateChangeTimer.Interval
            End If
            pic.addEvent(target.name & " is STUNNED for " & Math.Round((Constants.stunReset / target.toughness) / 1000) & " seconds!")
        End If

        'both monsters get supermeter on successful attack
        monster.superMeter += power
        target.superMeter += power

    End Sub

    Private Sub calcInitiative(ByRef m As KaijuBaseInterface)

        Select Case m.currentState
            Case Constants.State.stunned
                m.initiative = 0
            Case Constants.State.vulnerable 'lower if vulnerable (just attacked)
                m.initiative = Math.Round(m.intelligence + (m.agility / 2)) * Constants.vulnerableMultiplier
            Case Constants.State.defending  'higher if defending
                m.initiative = Math.Round(m.intelligence + (m.agility / 2)) * Constants.defendingMultiplier
            Case Constants.State.attacking  'lower if attacking
                m.initiative = Math.Round(m.intelligence + (m.agility / 2)) * Constants.attackingMultiplier
            Case Constants.State.waiting    'base level
                m.initiative = Math.Round(m.intelligence + (m.agility / 2))
        End Select

    End Sub

    Private Function getMonsterTime(ByRef m As KaijuBaseInterface)
        Return Nothing
    End Function

    Private Sub m1StunTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If monster1.currentState <> Constants.State.stunned And monster1.stunLevel > 0 Then
            monster1.stunLevel -= monster1.toughness * Constants.stunMultiplier
            If monster1.stunLevel < 0 Then monster1.stunLevel = 0
            pic.needsUpdate = True
        End If
    End Sub

    Private Sub m2StunTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If monster2.currentState <> Constants.State.stunned And monster2.stunLevel > 0 Then
            monster2.stunLevel -= monster2.toughness * Constants.stunMultiplier
            If monster2.stunLevel < 0 Then monster2.stunLevel = 0
            pic.needsUpdate = True
        End If
    End Sub

    Private Sub m1StateChangeTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If monster1.nextState <> Nothing Then
            monster1.currentState = monster1.nextState
            monster1.nextState = Nothing
        End If
        m1StateChangeTimer.Enabled = False
    End Sub

    Private Sub m2StateChangeTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If monster2.nextState <> Nothing Then
            monster2.currentState = monster2.nextState
            monster2.nextState = Nothing
        End If
        m2StateChangeTimer.Enabled = False
    End Sub

    Private Sub m1ActionTimerElapsed(ByVal sender As Object, ByVal e As ActionEventArgs)

    End Sub
End Class
