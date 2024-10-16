# powerplant-coding-challenge

## Build and setup

This solution was created and developed using Visual Studio 2022, .net 8.0
And separated in 3 project 

  - PowerPlant.Api:
    Contains the logic for The Api and swagger
  - PowerPlant.Application:
    Contains the business logic we need
  - PowerPlant.Application.UnitTest:
    Contains the unit test to cover the business logic

## Launch and using

Simply build the solution and launch the PowerPlant.Api
Open a browser https://localhost:8888/swagger/index.html

you can now post on https://localhost:8888/ProductionPlan

### Payload

 - load: The load is the amount of energy (MWh) that need to be generated during one hour.
 - fuels: based on the cost of the fuels of each powerplant, the merit-order can be determined which is the starting point for deciding which powerplants should be switched on and how much power they will deliver.  Wind-turbine are either switched-on, and in that case generate a certain amount of energy depending on the % of wind, or can be switched off. 
   - gas(euro/MWh): the price of gas per MWh. Thus if gas is at 6 euro/MWh and if the efficiency of the powerplant is 50% (i.e. 2 units of gas will generate one unit of electricity), the cost of generating 1 MWh is 12 euro.
   - kerosine(euro/Mwh): the price of kerosine per MWh.
   - co2(euro/ton): the price of emission allowances (optionally to be taken into account).
   - wind(%): percentage of wind. Example: if there is on average 25% wind during an hour, a wind-turbine with a Pmax of 4 MW will generate 1MWh of energy.
 - powerplants: describes the powerplants at disposal to generate the demanded load. For each powerplant is specified:
   - name:
   - type: gasfired, turbojet or windturbine.
   - efficiency: the efficiency at which they convert a MWh of fuel into a MWh of electrical energy. Wind-turbines do not consume 'fuel' and thus are considered to generate power at zero price.
   - pmax: the maximum amount of power the powerplant can generate.
   - pmin: the minimum amount of power the powerplant generates when switched on. 

### response

The response should be a json as in `example_payloads/response3.json`, which is the expected answer for `example_payloads/payload3.json`, specifying for each powerplant how much power each powerplant should deliver. The power produced by each powerplant has to be a multiple of 0.1 Mw and the sum of the power produced by all the powerplants together should equal the load.

  - name: the name of the powerplant
  - p: the production needed

### example

[First example](example_payloads/payload1.json)
[Second example](example_payloads/payload2.json)
[Third example](example_payloads/payload3.json)

## Improvement & open question

  - What to do when the power plan can't deliver the load requested ? 
    Currently, we respond with the best matching scenario

