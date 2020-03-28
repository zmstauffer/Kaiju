Public Class Constants

    Public Enum State
        waiting = 1
        attacking = 2
        defending = 3
        vulnerable = 4
        stunned = 5
    End Enum

    Public Const maxAttributes As Integer = 100

    Public r As Random = New Random

    Public Const attackingMultiplier As Single = 0.9
    Public Const defendingMultiplier As Single = 2
    Public Const vulnerableMultiplier As Single = 0.85
    Public Const waitingMultiplier As Single = 1

    Public Const stunMultiplier As Single = 0.2                 'base amount stun goes down
    Public Const stunTimer As Integer = 5000                    'timer interval
    Public Const stunReset As Integer = 100000                  'amount of time before monster stun wears off
    Public Const maxStun As Single = 10                         'have to exceed toughness*maxStun to stun monster
    Public Const healthModifier As Integer = 2.5

    Public Const attackModifier As Single = 0.25                 'affects how hard an attack hits
    Public Const dodgeMultiplier As Single = 10                 'how easy is it to dodge

    Public Const negStatMultiplier As Single = 1                'how much a negative stat move affects the enemy (total strength of effect)
    Public Const negStatTimeMultiplier As Single = 300          'how much time negative stat move affects enemy

    Public Const posStatMultiplier As Single = 1
    Public Const posStatTimeMultiplier As Single = 300

    Public Const stunAmount As Single = 7 'was 10                       'how much stun do you get per point of damage
    Public Const stunDamageBonus As Single = 1.25               'damage bonus from hitting a stunned opponent
    Public Const defendingPowerReduction As Single = 2          'how much damage we save if defending, such as power/defendingPowerReduction

    Public Const vulnerableTimeMultiplier = 1200               'multiplied by stat strength for actions to determine time monster is vulnerable after attack

    Public Const timerMultiplier As Single = 0.5

    Public Const dodgeAgilityModifier As Single = 2.15          'agility*dodgeAgilityModifier helps monster dodge
    Public Const dodgeIntelligenceModifier As Single = 1        'intelligence/dodgeIntelligenceModifier helps monster dodge

    Public Const timeIntelligenceModifier As Single = 5         'the higher this is, the less time you are vulnerable per intelligence point

    Public Const maxStatBoostMultiplier As Single = 1.2         'what percentage your stats can be boosted

    Public Const baseAccuracy As Integer = 75
End Class


