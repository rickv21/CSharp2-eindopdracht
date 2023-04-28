# Startdocument voor de C#-2 eindopdracht 

Startdocument van **Rick Vinke**, **Jesse Vugteveen**, **Jesse Oost**, **Max Appeldorn** en **Cedric Smit**.

## Beschrijving

## In- en uitvoer

In deze sectie wordt de in- en uitvoer van de application beschreven.

#### Invoer

In de onderstaande tabel wordt alle invoer (die de gebruiker moet invoeren om de toepassing te laten werken) beschreven.

| Case        | Type   | Conditions                           |
|-------------|--------|--------------------------------------|
| Player name | String | Kan niet null zijn.                  |
| Ip-adres    | String | Moet een geldig IP of hostname zijn. |

#### Uitvoer

In de onderstaande tabel wordt de verschillende uitvoer van de applicatie beschreven.

| Case                                                  | Type    | Conditions                                                |
|-------------------------------------------------------|---------|-----------------------------------------------------------|
| Aantal beurten in Memory.                             | int     | Kan nooit minder zijn dan het aantal memory kaarten / 2 . |
| Naam van de speler die aan de beurt is.               | String  | Kan niet null zijn.                                       |
| Naam van de speler die een spel heeft gewonnen.       | String  | Kan niet null zijn.                                       |
| De index van de ingevoerde steen van vier op een rij. | int     |                                                           |
| Of de geselecteerde Memory kaarten hetzelfde zijn.    | boolean |                                                           |
| Het aantal dam stenen van een speler.                 | int     |                                                           |
| De positie van de huidige dam steen.                  | int     |                                                           |


#### Calculaties
In de onderstaande tabel worden de calculaties van de applicatie beschreven.

| Case                              | Calculatie                                                                   |
|-----------------------------------|------------------------------------------------------------------------------|
| Memory kaart vergelijking.        | Calculeren of de twee geselecteerde Memory kaarten hetzelfde zijn.           |
| Memory win status.                | Calculeren of de Memory game is gewonnen.                                    |
| Vier op een rij steen calculatie. | Calculeren of de vier op een rij steen kan vallen en hoe ver die kan vallen. |
| Vier op een rij win calculatie.   | Calculeren of er vier op een rij is.                                         |
| Dammen mogelijke zet calculatie.  | Calculeren welke zetten de speler kan maken met dammen.                      |
| Dammen zet actie calculatie       | Calculeren of de actie van de speler de steen van de andere speler pakt.     |
| Dammen win status                 | Calculeren of een van de spelers dammen heeft gewonnen.                      |
| Netwerk gegevens uitwisselen      | Stuur gegevens tussen de spelers zodat de gegevens gelijk zijn.              |


## Klassendiagram
Deze volgt nog voor week 4.

## Wireframes

![Game1](game1.png "Game1")

## Planning

## Testplan