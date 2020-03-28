Imports System.Threading
Imports System.Timers

Module Main

    Dim rando As Random = New Random

    Sub Main()

        Dim pic As New Graphics

        'load all the monsters
        Dim monsterList As New List(Of Kaiju)

        'for testing
        Dim strMonster As Kaiju = New strengthMonster
        Dim agiMonster As Kaiju = New agilityMonster
        Dim tuffMonster As Kaiju = New toughnessMonster
        Dim intMonster As Kaiju = New intelligenceMonster
        Dim frosty As Kaiju = New AbominableSnowmen
        Dim sealyboi As Kaiju = New SxSealTeam6
        Dim ofe As Kaiju = New OFE
        Dim piggie As Kaiju = New TheSilverAssassinPig
        Dim zoomzoom As Kaiju = New Zoomy
        Dim bomber As Kaiju = New Bombers

        agiMonster.calcStats()
        tuffMonster.calcStats()
        intMonster.calcStats()
        frosty.calcStats()
        sealyboi.calcStats()
        ofe.calcStats()
        piggie.calcStats()
        zoomzoom.calcStats()
        bomber.calcStats()

        'add all the monsters to the main list
        monsterList.Add(agiMonster)
        monsterList.Add(tuffMonster)
        monsterList.Add(intMonster)
        monsterList.Add(frosty)
        monsterList.Add(sealyboi)
        monsterList.Add(zoomzoom)
        monsterList.Add(bomber)
        monsterList.Add(ofe)
        monsterList.Add(piggie)

        'validate monsters
        For Each monster In monsterList
            validateMonster(monster)
            Thread.Sleep(100)
        Next

        'output that all monsters are validated
        Console.ForegroundColor = ConsoleColor.Green
        Console.WriteLine("All Monsters Validated")
        Console.ForegroundColor = ConsoleColor.White
        Thread.Sleep(1500)
        Console.Clear()

        pic.drawMainMenu(monsterList)

        Dim entries As String = ""
        entries = Console.ReadLine

        While entries <> "0"
            'decipher entry. no error checking...
            Dim leftString = Left(entries, 1)
            Dim rightString = Right(entries, 1)

            pic.name1 = monsterList.Item(leftString - 1).name
            pic.name2 = monsterList.Item(rightString - 1).name

            Dim fightController As New FightController(monsterList.Item(leftString - 1), monsterList.Item(rightString - 1), pic)
            fightController.fight()

            pic.addEvent("THE FIGHT IS OVER!")
            pic.outputEventList()
            Thread.Sleep(2000)
            Console.Clear()

            'add winner screen
            If monsterList.Item(leftString - 1).health > monsterList.Item(rightString - 1).health Then
                pic.drawText(monsterList.Item(leftString - 1).name & " WINS! It had " & monsterList.Item(leftString - 1).health & " health left.", 1, 1)
                monsterList.Item(leftString - 1).numWins += 1
            ElseIf monsterList.Item(leftString - 1).health < monsterList.Item(rightString - 1).health Then
                pic.drawText(monsterList.Item(rightString - 1).name & " WINS! It had " & monsterList.Item(rightString - 1).health & " health left.", 1, 1)
                monsterList.Item(rightString - 1).numWins += 1
            Else
                pic.drawText("IT WAS A DRAW!", 1, 1)
            End If

            'reset monsters
            monsterList.Item(leftString - 1).reset()
            monsterList.Item(rightString - 1).reset()

            Thread.Sleep(5000)

            'redraw main menu
            entries = ""
            Console.Clear()
            pic.drawMainMenu(monsterList)
            entries = Console.ReadLine()
        End While

        'all done, quit gracefully
        shutdown()
    End Sub

    Public Sub testTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        Console.WriteLine("testTimer elapsed")
    End Sub

    Sub shutdown()
        Console.WriteLine("Press any key to exit.")
        Console.ReadKey()
        End
    End Sub

    Function validateMonster(ByRef monster As KaijuBaseInterface)
        If monster.name <> "" Then
            Console.WriteLine("Validating {0}{1}", monster.name, vbCrLf)
        Else
            Console.WriteLine("Validating monster..." & vbCrLf)
        End If

        Thread.Sleep(500)

        'check stats
        If monster.validateStats() = False Then
            Console.WriteLine(monster.name & "'s stats are greater than 100!")
            shutdown()
        Else
            Console.WriteLine(monster.name & " has passed stat validation.")
        End If

        'check strings
        If monster.name = "" Or monster.teamName = "" Then
            Console.WriteLine("Monster doesn't have either a name or a team name.")
            shutdown()
        Else
            Console.WriteLine(monster.name & " has passed string validation.")
        End If

        Console.WriteLine(vbCr)

        Return True

    End Function

    Function rand(ByVal maxNum As Integer)
        Return rando.Next(1, maxNum)
    End Function

    Function rand(ByVal minNum As Integer, ByVal maxNum As Integer)
        Return rando.Next(minNum, maxNum)
    End Function

End Module
