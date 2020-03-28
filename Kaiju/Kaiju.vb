Imports System.IO

Public MustInherit Class Kaiju
    Implements KaijuBaseInterface

    Private _strength As Integer
    Private _agility As Integer
    Private _intelligence As Integer
    Private _toughness As Integer
    Private _health As Integer
    Private _name As String
    Private _teamName As String
    Private _initiative As Integer
    Private _currentState As Constants.State
    Private _stunLevel As Integer
    Private _superMeter As Integer

    Public numWins As Integer = 0
    Public lastAction As String
    Public nextState As Constants.State
    Public vulnerableTime As Single
    Public numDecisions As Integer
    Public baseStrength As New tempStat
    Public baseAgility As New tempStat
    Public baseIntelligence As New tempStat
    Public baseToughness As New tempStat
    Public superReady As New Boolean
    Public maxHealth As Integer
    Public maxStun As Integer

    Public action1 As ActionDefinition
    Public action2 As ActionDefinition
    Public action3 As ActionDefinition
    Public ultimate As UltimateActionDefinition

    Public myDecision As Decision

#Region "Properties"
    Public Overridable Property strength As Integer Implements KaijuBaseInterface.strength
        Get
            Return _strength
        End Get
        Set(value As Integer)
            _strength = value
        End Set
    End Property

    Public Overridable Property agility As Integer Implements KaijuBaseInterface.agility
        Get
            Return _agility
        End Get
        Set(value As Integer)
            _agility = value
        End Set
    End Property

    Public Overridable Property intelligence As Integer Implements KaijuBaseInterface.intelligence
        Get
            Return _intelligence
        End Get
        Set(value As Integer)
            _intelligence = value
        End Set
    End Property

    Public Overridable Property toughness As Integer Implements KaijuBaseInterface.toughness
        Get
            Return _toughness
        End Get
        Set(value As Integer)
            _toughness = value
        End Set
    End Property

    Public Overridable Property health As Integer Implements KaijuBaseInterface.health
        Get
            Return _health
        End Get
        Set(value As Integer)
            _health = value
        End Set
    End Property

    Public Overridable Property name As String Implements KaijuBaseInterface.name
        Get
            Return _name
        End Get
        Set(value As String)
            If value <> "" Then
                _name = value
            End If
        End Set
    End Property

    Public Overridable Property teamName As String Implements KaijuBaseInterface.teamName
        Get
            Return _teamName
        End Get
        Set(value As String)
            If value <> "" Then
                _teamName = value
            End If
        End Set
    End Property

    Public Overridable Property initiative As Integer Implements KaijuBaseInterface.initiative
        Get
            Return _initiative
        End Get
        Set(value As Integer)
            _initiative = value
        End Set
    End Property

    Public Overridable Property currentState As Constants.State Implements KaijuBaseInterface.currentState
        Get
            Return _currentState
        End Get
        Set(value As Constants.State)
            _currentState = value
        End Set
    End Property

    Public Overridable Property stunLevel As Integer Implements KaijuBaseInterface.stunLevel
        Get
            Return _stunLevel
        End Get
        Set(value As Integer)
            _stunLevel = value
        End Set
    End Property

    Public Overridable Property superMeter As Integer Implements KaijuBaseInterface.superMeter
        Get
            Return _superMeter
        End Get
        Set(value As Integer)
            _superMeter = value
        End Set
    End Property

#End Region

    Public Sub New()
        'must override

    End Sub


    Public Function validateStats() As Boolean Implements KaijuBaseInterface.validateStats
        If _strength + _agility + _intelligence + _toughness <= Constants.maxAttributes Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub calcStats() Implements KaijuBaseInterface.calcStats
        _initiative = Math.Round(agility + (intelligence * 0.5))
        _currentState = Constants.State.waiting
        _stunLevel = 0
        _superMeter = 0
        _health = ((_toughness * 100) / ((_toughness + 16) + 10)) * Constants.healthModifier                '(x*a)/(x+b)+c, larger a gives larger overall numbers, larger b brings numbers closer together, larger c flattens the curve
        maxHealth = _health
        maxStun = _toughness * Constants.maxStun
        numDecisions = 0

        'set base stats
        baseStrength.value = _strength
        baseAgility.value = _agility
        baseToughness.value = _toughness
        baseIntelligence.value = _intelligence

    End Sub

    Public MustOverride Function action(ByVal enemy As KaijuBaseInterface) As Decision Implements KaijuBaseInterface.action

    Public Sub resetBaseValues()
        On Error Resume Next
        If baseAgility.time < Date.Now Then
            _agility = baseAgility.value
        End If
        If baseIntelligence.time < Date.Now Then
            _intelligence = baseIntelligence.value
        End If
        If baseStrength.time < Date.Now Then
            _strength = baseStrength.value
        End If
        If baseToughness.time < Date.Now Then
            _toughness = baseToughness.value
        End If
    End Sub

    Public Sub reset()
        _initiative = Math.Round(agility + (intelligence * 0.5))
        _currentState = Constants.State.waiting
        _stunLevel = 0
        _superMeter = 0
        _health = ((_toughness * 100) / ((_toughness + 16) + 10)) * Constants.healthModifier                '(x*a)/(x+b)+c, larger a gives larger overall numbers, larger b brings numbers closer together, larger c flattens the curve
        numDecisions = 0

        'make sure any buffs are worn off
        _agility = baseAgility.value
        _intelligence = baseIntelligence.value
        _toughness = baseToughness.value
        _strength = baseStrength.value

    End Sub

End Class

Public Class tempStat
    Public value As Integer
    Public time As DateTime
End Class
