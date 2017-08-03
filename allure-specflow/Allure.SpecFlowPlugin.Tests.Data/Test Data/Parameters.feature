Feature: Parameters

 
Scenario: Scenario with parametrized steps
	
	Given Step with table
		| name         | surname | gender | age |
		| John         | Smith   | male   | 30  |
		| "Mary","Ann" | Jane;   | female | 25  |
		|              |         |        |     |
		| Eric         | Cartman | ,      | ,,  |
