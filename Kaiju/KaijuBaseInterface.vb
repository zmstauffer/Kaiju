Public Interface KaijuBaseInterface
    'Event tookDamage()
    'methods
    'Function action1() As Integer
    'Function action2() As Integer
    'Function action3() As Integer
    'Function ultimate() As Integer
    Function validateStats() As Boolean
    Sub calcStats()
    Function action(ByVal enemy As KaijuBaseInterface) As Decision

    'properties
    Property strength As Integer
    Property agility As Integer
    Property intelligence As Integer
    Property toughness As Integer
    Property health As Integer
    Property name As String
    Property teamName As String
    Property initiative As Integer
    Property currentState As Constants.State
    Property stunLevel As Integer
    Property superMeter As Integer

End Interface
