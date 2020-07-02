


When you launch the application using IIS Express,  you should be able to view teh swagger implementation by browsing the below URL

https://localhost:44357/swagger/index.html

when browsed, after succesfully running the application, system launches a swagger implementation for the API.
System prompts for entryTime, exitTime parameters are mandatory and need to be a valid datetime values.

system responds with a price tier and total cost associated with the entry and exit times.

Application is broadly divided into 

1) Tests
  Unit testing suite.
  \CarParkRateCalc.API.Tests.Controllers\TestBase.cs - contains few tests. This needs to be expanded. However, constrained myself to limited tests for this sample.
2) Common
       common utilties - mapper and DI
3) API
      Predominantly for exposing contracts and necessary bootstraping for exposing the API
4) Services
      actual implementation of the service is present in this section.

# CarParkRateCalc
repository for api which calculates the car park fees

The problem: We need an API that does a rate calculation engine for a carpark
The inputs for this engine are:
1. Car Entry Date and Time
2. Car Exit Date and Time

Based on these 2 inputs the engine program should calculate the correct rate for the customer and
display the name of the rate along with the total price to the customer using the following rates:
Name of the Rate Early Bird
Type Flat Rate
Total Price $13.00
Entry Condition Enter between 6:00 AM to 9:00 AM
Exit Condition Exit between 3:30 PM to 11:30 PM
Name of the Rate Night Rate
Type Flat Rate
Total Price $6.50
Entry Condition Enter between 6:00 PM to midnight (weekdays)
Exit Condition Exit between 3:30 PM to 11:30 PM
Name of the Rate Weekend Rate
Type Flat Rate
Total Price $10.00
Entry Condition Enter anytime past midnight on Friday to Sunday
Exit Condition Exit any time before midnight of Sunday
Note: If a customer enters the carpark before midnight on Friday and if they qualify for Night rate on
a Saturday morning, then the program should charge the night rate instead of weekend rate.
For any other entry and exit times the program should refer the following table for calculating the
total price.
Name of the Rate Standard Rate
Type Hourly Rate
0-1 Hours $5.00
1-2 Hours $10.00
2-3 Hours $15.00
3+ Hours $20.00 flat rate for each calendar day of parking
Note: The customer should get the cheapest deal based on the rules which apply to the time period.
