
# Technical Challenge

##Setup
Requires 
  Visual Studio 2022
  Northwind DB setup and running locally
  
## Developer Notes
I followed roughly the following steps for this technical test.

- Initial setup of the Northwind DB 
- Clone and run the repo
- Some manual testing of the API using swagger.
- An initial parse of the code to understand the purpose, layout, etc.
- Create a draft plan for the intended works
- Create the initial OrderClient and DbClient interfaces and implementations
- Use these in the controller
- Second round of manual testing, including the bugs found in the first round
- Fix any bugs found in the manual testing
- Add in simple unit tests for the OrderClient


## Future Improvements

- Improve the DBClient to make better use of mappers and confirm behaviour in some cases. 
- Expand the current unit tests and add more for the DBClient and the controller.

