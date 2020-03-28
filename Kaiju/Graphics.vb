Imports System.IO

Public Class Graphics

    Public needsUpdate As Boolean
    Public eventArray(10) As String
    Public startTime As Date
    Public name1 As String
    Public name2 As String

    Public Sub New()
        'set window width and height for consistency
        Console.SetWindowSize(120, 30)
        needsUpdate = False
        For Each s In eventArray
            s = ""
        Next

        startTime = Date.Now

    End Sub

    Public Sub drawMainMenu(ByRef monsterList As List(Of Kaiju))

        Dim textOffset = 2
        Dim vertOffset = 1
        Dim overallBoxWidth As Integer = Console.WindowWidth / 2
        Dim boxHeight As Integer = monsterList.Count + 2

        drawBox(1, 1, overallBoxWidth, boxHeight, ConsoleColor.Green)
        drawInlineText(2, 1, "Monsters", overallBoxWidth - 4, ConsoleColor.Green)
        For i As Integer = 1 To monsterList.Count
            drawText(i & ". " & monsterList.Item(i - 1).name, textOffset, vertOffset + i,)
            Dim winText As String
            If monsterList.Item(i - 1).numWins <> 1 Then winText = " WINS" Else winText = " WIN"
            drawText(monsterList.Item(i - 1).numWins & winText, textOffset + 40, vertOffset + i,)
        Next

        drawText("0: Exit Program", textOffset, boxHeight)
        Console.SetCursorPosition(1, boxHeight + 2)
        Console.WriteLine("Enter Combatants separated by a space: ")
        Console.SetCursorPosition(1, boxHeight + 3)

    End Sub

    Public Sub drawMonsterCard(ByRef m As Kaiju, ByVal offset As Integer)

        Dim textOffset = offset + 3
        Dim vertOffset = 3
        'all around box
        drawBox(offset, 1, 42, 18, ConsoleColor.Blue)

        'stat box
        drawBox(offset + 1, 2, 40, 10, ConsoleColor.Blue)
        drawTeamName(offset + 2, m.teamName)
        drawText("       Monster: " & m.name, textOffset, vertOffset,)
        drawText("      Action 1: " & m.action1.name, textOffset, vertOffset + 1,)
        drawText("      Action 2: " & m.action2.name, textOffset, vertOffset + 2,)
        drawText("      Action 3: " & m.action3.name, textOffset, vertOffset + 3,)
        drawText("      Ultimate: " & m.ultimate.name, textOffset, vertOffset + 4,)
        drawText("        Health: ", textOffset, vertOffset + 5,)
        drawText("    Stun Meter: ", textOffset, vertOffset + 6,)
        drawText("Ultimate Meter: ", textOffset, vertOffset + 7,)

        'set all bars to 20 or 0, will be updated in updateMonsterCard Sub
        drawBar(20, textOffset + 16, vertOffset + 5, ConsoleColor.Red)
        drawBar(0, textOffset + 16, vertOffset + 6, ConsoleColor.DarkMagenta)
        drawBar(0, textOffset + 16, vertOffset + 7, ConsoleColor.DarkGreen)

        'decision box
        drawBox(offset + 1, vertOffset + 10, 40, 5, ConsoleColor.Blue)
        drawText("Current State: " & m.currentState.ToString(), textOffset, vertOffset + 11)
        drawText("Last Action: ", textOffset, vertOffset + 12)
        drawText("Vulnerable Time: ", textOffset, vertOffset + 13)
    End Sub

    Public Sub updateMonsterCard(ByRef m As Kaiju, ByVal offset As Integer)

        Dim textOffset = offset + 3
        Dim vertOffset = 3

        'normalized function: normalize x into range(a,b), x = (x-a/b-a), then multiply by scaling factor(20 in this case for max 20 increments for bar graphic) 
        Dim healthLength As Integer = Math.Min(Math.Round((m.health / m.maxHealth) * 20), 20)
        Dim healthBuffer As Integer = 20 - healthLength
        Dim stunLength As Integer = Math.Min(Math.Round((m.stunLevel / m.maxStun) * 20), 20)
        Dim stunBuffer As Integer = 20 - stunLength
        Dim superLength As Integer = Math.Min(Math.Round((m.superMeter / 100) * 20), 20)
        Dim superBuffer As Integer = 20 - superLength
        Dim stateLength As Integer = Len("Current State: " & m.currentState.ToString())
        Dim actionLength As Integer = Len("Last Action: " & m.lastAction)
        Dim vulnerableLength As Integer = Len("Vulnerable Time: " & m.vulnerableTime)

        'first draw colored bar then draw black bar to "erase" extra bar left over. We aren't clearing the console to avoid flickering so this seemed to be the best way to do it.
        drawBar(healthLength, textOffset + 16, vertOffset + 5, ConsoleColor.Red)
        drawBar(healthBuffer, textOffset + 16 + healthLength, vertOffset + 5, ConsoleColor.Black)

        drawBar(stunLength, textOffset + 16, vertOffset + 6, ConsoleColor.DarkMagenta)
        drawBar(stunBuffer, textOffset + 16 + stunLength, vertOffset + 6, ConsoleColor.Black)

        drawBar(superLength, textOffset + 16, vertOffset + 7, ConsoleColor.DarkGreen)
        drawBar(superBuffer, textOffset + 16 + superLength, vertOffset + 7, ConsoleColor.Black)

        drawText("Current State: " & m.currentState.ToString(), textOffset, vertOffset + 11)
        drawBar(Math.Max(36 - stateLength, 0), textOffset + stateLength, vertOffset + 11, ConsoleColor.Black)

        drawText("Last Action: " & m.lastAction, textOffset, vertOffset + 12)
        drawBar(Math.Max(36 - actionLength, 0), textOffset + actionLength, vertOffset + 12, ConsoleColor.Black)

        drawText("Vulnerable Time: " & m.vulnerableTime, textOffset, vertOffset + 13)
        drawBar(Math.Max(36 - vulnerableLength, 0), textOffset + vulnerableLength, vertOffset + 13, ConsoleColor.Black)

        drawText("Decisions: " & m.numDecisions, textOffset, vertOffset + 14)
    End Sub

    Public Sub outputEventList()
        color(ConsoleColor.Green)
        For i As Integer = 0 To eventArray.Count - 1 Step 1
            WriteAt(eventArray(i), 1, 20 + i)
        Next
        color(ConsoleColor.White)
    End Sub

    Public Sub addEvent(ByVal addString As String)
        'output to log
        Dim filePath As String = String.Format("C:\TEMP\FightLog - {0} vs. {1} @ {2}.txt", name1, name2, startTime.ToString("dd-MMM-yyyy"))
        Using writer As New StreamWriter(filePath, True)
            writer.WriteLine(addString)
        End Using

        'pad addString as necessary
        If Len(addString) < 120 Then
            addString = addString & StrDup(120 - Len(addString), " ")
        End If
        Dim index = Array.IndexOf(eventArray, "")
        If index <> -1 Then
            eventArray(index) = addString
        Else
            'no empty strings
            For i As Integer = 1 To eventArray.Count - 1 Step 1
                eventArray(i - 1) = eventArray(i)
            Next
            eventArray(9) = addString
        End If
    End Sub

    Public Sub updateStun(ByRef m As Kaiju, ByVal offset As Integer)
        Dim textOffset = offset + 3
        Dim vertOffset = 3
        Dim clearText As String = StrDup(20, "█")
        color(ConsoleColor.Black)
        WriteAt(clearText, textOffset + 16, vertOffset + 5)
    End Sub

    Public Sub WriteAt(ByVal s As String, ByVal x As Integer, ByVal y As Integer)
        Try
            Console.SetCursorPosition(x, y)
            Console.Write(s)
            Console.SetCursorPosition(0, 29)
        Catch

        End Try
    End Sub

    Public Sub drawTeamName(ByVal offset As Integer, ByVal name As String)
        Dim barLength As Integer = (36 - name.Length) / 2
        Dim barString = StrDup(barLength, "═")
        drawText(barString, offset, 2, ConsoleColor.Blue)
        drawText(" " & name & " ", offset + barLength, 2, ConsoleColor.White)
        drawText(barString, offset + barLength + name.Length + 1, 2, ConsoleColor.Blue)
    End Sub

    Public Sub drawInlineText(ByVal horzOffset As Integer, ByVal vertOffset As Integer, ByVal text As String, ByVal lineLength As Integer, Optional ByVal col As ConsoleColor = ConsoleColor.White)
        Dim barLength As Integer = (lineLength - text.Length) / 2
        Dim barString = StrDup(barLength, "═")
        drawText(barString, horzOffset, vertOffset, col)
        drawText(" " & text & " ", horzOffset + barLength, vertOffset, ConsoleColor.White)
        drawText(barString, horzOffset + barLength + text.Length + 2, vertOffset, col)
    End Sub

    Public Sub drawText(ByVal str As String, ByVal startX As Integer, ByVal startY As Integer, Optional ByVal col As ConsoleColor = ConsoleColor.White)
        color(col)
        WriteAt(str, startX, startY)
    End Sub

    Public Sub drawBox(ByVal startX As Integer, ByVal startY As Integer, ByVal width As Integer, ByVal height As Integer, ByVal col As ConsoleColor)
        Dim topString As String = "╔" & StrDup(width - 2, "═") & "╗"
        Dim bottomString As String = "╚" & StrDup(width - 2, "═") & "╝"
        Dim sides As String = "║" & StrDup(width - 2, " ") & "║"

        color(col)
        WriteAt(topString, startX, startY)
        For i As Integer = startY + 1 To startY + height - 1 Step 1
            WriteAt("║", startX, i)
            WriteAt("║", startX + width - 1, i)
        Next
        WriteAt(bottomString, startX, startY + height)
        color(ConsoleColor.White)
    End Sub

    Public Sub drawBar(ByVal len As Integer, ByVal x As Integer, ByVal y As Integer, Optional ByVal col As ConsoleColor = ConsoleColor.Red)
        'len = string length
        'x = horizontal offset
        'y = vertical offset

        If len <= 0 Then Exit Sub
        Dim str As String = StrDup(len, "█") '■
        color(col)
        WriteAt(str, x, y)
        color(ConsoleColor.White)
    End Sub

    Public Sub color(ByVal color As ConsoleColor)
        Console.ForegroundColor = color
    End Sub

    Public Sub drawVS()
        Dim offset = 50
        Dim vertStart = 8
        color(ConsoleColor.Red)
        WriteAt(" _   __   ____", offset, vertStart)
        WriteAt("| | / /  / __/", offset, vertStart + 1)
        WriteAt("| |/ /  _\ \  ", offset, vertStart + 2)
        WriteAt("|___/  /___/  ", offset, vertStart + 3)
        color(ConsoleColor.White)
    End Sub

End Class